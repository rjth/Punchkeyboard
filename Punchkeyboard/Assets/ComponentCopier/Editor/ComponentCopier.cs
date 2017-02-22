using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

class ComponentCopier : EditorWindow {
    
	Vector2 scrollPos;
	static bool[] foldout;
	static bool[] selectAll;
	static Component[] components;
	static Type[] t;
	static bool[] enabled;
	static List<FieldInfo>[] fields;
	static List<PropertyInfo>[] properties;
	static object[][] fieldVals;
	static object[][] propertyVals;

	static bool[] selectedComponents;
	static bool[][] selectedFields;
	static bool[][] selectedProperties;
	
	static Dictionary<Type, int> copyTypeCount;
	static Dictionary<Type, int> pasteTypeCount;
	static List<FieldInfo> fieldsToRemove;
	static List<PropertyInfo> propertiesToRemove;
	
	static int typeIndex;
    
    [MenuItem ("Edit/Copy Components %&c")]
	[MenuItem ("CONTEXT/Transform/Copy Components %&c")]
    static void Copy () {
    	if (!Selection.activeGameObject)
		{
			EditorUtility.DisplayDialog("Nothing selected", "Select a gameobject first.", "oops");
    		return;
		}
		
		components = Selection.activeGameObject.GetComponents<Component>();
		
		selectedComponents = new bool[components.Length];
		t = new Type[components.Length];
		enabled = new bool[components.Length];
		
		fields = new List<FieldInfo>[components.Length];
		properties = new List<PropertyInfo>[components.Length];
	
		fieldVals = new object[components.Length][];
		propertyVals = new object[components.Length][];
		
		selectedFields = new bool[components.Length][];
		selectedProperties = new bool[components.Length][];
		foldout = new bool[components.Length];
		selectAll = new bool[components.Length];
		
		copyTypeCount = new Dictionary<Type, int>();
		
		fieldsToRemove = new List<FieldInfo>();
		propertiesToRemove = new List<PropertyInfo>();
		
		for(int i = 0; i < components.Length; i++)
		{
			t[i] = components[i].GetType();
			
			if(t[i].IsSubclassOf(typeof(Behaviour)))
				enabled[i] = (components[i] as Behaviour).enabled;
			else if(t[i].IsSubclassOf(typeof(Component)) && t[i].GetProperty("enabled") != null)
				enabled[i] = (bool) t[i].GetProperty("enabled").GetValue(components[i], null);
			else
				enabled[i] = true;
			
			fields[i] = new List<FieldInfo>();
			properties[i] = new List<PropertyInfo>();
			
			fields[i].AddRange(t[i].GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default));
			properties[i].AddRange(t[i].GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default));
			
			foreach(FieldInfo field in fields[i])
			{
//				foreach(Attribute attr in field.GetCustomAttributes(typeof(SerializeField), false))
//					Debug.Log(field.Name);
				if(field.DeclaringType == typeof(Behaviour) || field.DeclaringType == typeof(Component) || field.DeclaringType == typeof(UnityEngine.Object)
					|| (!(field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), false).Length > 0))
				   )
					fieldsToRemove.Add(field);
			}
			
			foreach(PropertyInfo property in properties[i])
			{
				if(property.DeclaringType == typeof(Behaviour) || property.DeclaringType == typeof(Component) || property.DeclaringType == typeof(UnityEngine.Object))
					propertiesToRemove.Add(property);
			}
			
			foreach(FieldInfo field in fieldsToRemove)
				fields[i].Remove(field);
			
			foreach(PropertyInfo property in propertiesToRemove)
				properties[i].Remove(property);
			
			fieldsToRemove.Clear();
			propertiesToRemove.Clear();
			
			fieldVals[i] = new object[fields[i].Count];
			propertyVals[i] = new object[properties[i].Count];
			
			selectedFields[i] = new bool[fields[i].Count];
			selectedProperties[i] = new bool[properties[i].Count];
			
			foldout[i] = false;
		}
		
		EditorWindow.GetWindow(typeof(ComponentCopier), true, "Which components would you like to copy?", true);
    }
	
	void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			for (int i = 0; i < components.Length; i++)
			{

				selectedComponents[i] = EditorGUILayout.BeginToggleGroup(t[i].Name, selectedComponents[i]);
					
				if(fields[i].Count > 0 || properties[i].Count > 0)
				{
					foldout[i] = EditorGUILayout.Foldout(foldout[i], t[i].Name + " fields and properties");
					if(foldout[i])
					{
						if((fields[i].Count + properties[i].Count) > 1)
						{
							EditorGUI.BeginChangeCheck();
							selectAll[i] = EditorGUILayout.Toggle("Select All", selectAll[i]);
							if(EditorGUI.EndChangeCheck())
								SelectDeselectAll(i);
						}
						
						if(fields[i].Count > 0)
						{
							EditorGUILayout.LabelField("Fields:", "");
							for(int j = 0; j < fields[i].Count; j++)
								selectedFields[i][j] = EditorGUILayout.Toggle(fields[i][j].Name, selectedFields[i][j]);
						}
			
						if(properties[i].Count > 0)
						{
							EditorGUILayout.LabelField("Properties:", "");
				         	for(int j = 0; j < properties[i].Count; j++)
								selectedProperties[i][j] = EditorGUILayout.Toggle(properties[i][j].Name, selectedProperties[i][j]);
						}
					}
				}
				EditorGUILayout.EndToggleGroup();
			}
		

		EditorGUILayout.EndScrollView();
		
		if(GUILayout.Button("Copy", GUILayout.Height(30)))
		{
			CopyData();
			this.Close();
		}
		
	}
	
	static void SelectDeselectAll(int componentIndex)
	{
		if(fields[componentIndex].Count > 0)
		{
			for(int j = 0; j < fields[componentIndex].Count; j++)
				selectedFields[componentIndex][j] = selectAll[componentIndex];
		}

		if(properties[componentIndex].Count > 0)
		{
         	for(int j = 0; j < properties[componentIndex].Count; j++)
				selectedProperties[componentIndex][j] = selectAll[componentIndex];
		}
	}
	
	void CopyData()
	{
		for(int i = 0; i < selectedComponents.Length; i++)
		{
			if(selectedComponents[i])
			{	
				if(copyTypeCount.ContainsKey(t[i]))
					copyTypeCount[t[i]] = copyTypeCount[t[i]] + 1;
				else
					copyTypeCount.Add(t[i], 1);
				
				for(int j = 0; j < selectedFields[i].Length; j++)
				{
					if(selectedFields[i][j])
						fieldVals[i][j] = fields[i][j].GetValue(components[i]);
				}
				
				for(int j = 0; j < selectedProperties[i].Length; j++)
				{
					if(selectedProperties[i][j])
					{
						if(properties[i][j].CanRead && properties[i][j].GetIndexParameters().Length == 0)
							propertyVals[i][j] = properties[i][j].GetValue(components[i], null); 
						else
							Debug.LogWarning(properties[i][j].Name + " could not be copied.");
					}
				}
			}
		}
	}
	
	[MenuItem ("Edit/Paste Components %&v")]
	[MenuItem ("CONTEXT/Transform/Paste Components %&v")]
    static void Paste () {
				
		if (Selection.gameObjects.Length == 0)
    		return;

		// I was having a problem with undo grouping:
		// if an AddComponent and a RecordObject were being grouped together only the AddComponent would get undone...
		// So seperating them out into separate foreaches and incrementing the current group seemed like the best solution
		foreach (GameObject obj in Selection.gameObjects) {
			
			for(int i = 0; i < selectedComponents.Length; i++)
			{
				if(selectedComponents[i])
				{
					Component[] cArray = obj.GetComponents(t[i]);
					List<Component> c = new List<Component>();
					c.AddRange(cArray);
					
					int amountToAdd = copyTypeCount[t[i]] - c.Count;
					
					if(amountToAdd > 0)
					{
						for(int j = 0; j < amountToAdd; j++)
							c.Add(Undo.AddComponent(obj, t[i]));
					}
				}
			}
		}

		Undo.IncrementCurrentGroup();
					
		foreach (GameObject obj in Selection.gameObjects) {
			
			pasteTypeCount = new Dictionary<Type, int>();
			
	    	for(int i = 0; i < selectedComponents.Length; i++)
			{
				if(selectedComponents[i])
				{
					Component[] cArray = obj.GetComponents(t[i]);
					List<Component> c = new List<Component>();
					c.AddRange(cArray);
					
					if(pasteTypeCount.ContainsKey(t[i]))
					{
						pasteTypeCount[t[i]] = pasteTypeCount[t[i]] + 1;
						typeIndex = pasteTypeCount[t[i]];
					}
					else
					{
						typeIndex = 0;
						pasteTypeCount.Add(t[i], typeIndex);
					}

					Undo.RecordObject(c[typeIndex], "Paste component values");
					
					if(t[i].IsSubclassOf(typeof(Behaviour)))
						(c[typeIndex] as Behaviour).enabled = enabled[i];
					else if(t[i].IsSubclassOf(typeof(Component)) && t[i].GetProperty("enabled") != null)
						t[i].GetProperty("enabled").SetValue(c[typeIndex], enabled[i], null);
					
					for(int j = 0; j < selectedFields[i].Length; j++)
					{
						if(selectedFields[i][j])
							fields[i][j].SetValue(c[typeIndex], fieldVals[i][j]);
					}
					
					for(int j = 0; j < selectedProperties[i].Length; j++)
					{
						if(selectedProperties[i][j])
						{
							if(properties[i][j].CanWrite)
								properties[i][j].SetValue(c[typeIndex], propertyVals[i][j], null);
						}
					}
				}
			}
    	}
    }
}
