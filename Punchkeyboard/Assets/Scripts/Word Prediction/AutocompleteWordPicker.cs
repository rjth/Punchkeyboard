using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using System;

public class AutocompleteWordPicker : MonoBehaviour
{
	public InputField TextField;
	public NGramGenerator WordPredictor;

	void Start()
	{
		
	}
	public void ReplaceWord(string correctWord)
	{
		List<string> inputText = new List<string>();
		StringBuilder builder = new StringBuilder();
		string input = TextField.text;
		string[] parts = input.Split(' ');
		parts = parts.Take(parts.Length - 1).ToArray();

		for(int i = 0; i < parts.Length; i++)
		{
			inputText.Add(parts[i]);
		}

		inputText.Add(correctWord);

		foreach (string w in inputText)
		{
			builder.Append(w).Append(" ");
		}
		TextField.text = builder.ToString();
		TextField.ActivateInputField();

		WordPredictor.PredictNextWords(correctWord);
	}
	/*
	public void ReplaceWord(string correctWord)
	{
		List<string> inputText = new List<string>();
		StringBuilder builder = new StringBuilder();
		string input = TextField.text;
		input = ReverseString (input);
		input = input.Substring (input.IndexOf (' ') + 1);
		input = ReverseString (input);
		Debug.Log (input);

		string[] parts = input.Split(' ');
		parts = parts.Take(parts.Length - 1).ToArray();

		for(int i = 0; i < parts.Length; i++)
		{
			inputText.Add(parts[i]);
		}

		inputText.Add(correctWord);

		foreach (string w in inputText)
		{
			builder.Append(w).Append(" ");
		}
		TextField.text = builder.ToString();
		TextField.ActivateInputField();

		WordPredictor.PredictNextWords(correctWord);
	}
	*/
	public static string ReverseString(string s)
	{
		char[] charArray = s.ToCharArray ();
		Array.Reverse (charArray);
		return new string (charArray);
	}
}