using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.Text;
using System.Linq;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class NGramGenerator : MonoBehaviour
{
	public Text[] ButtonLabels;
	public List<string> LevenshteinCorpus = new List<string>();

	private Dictionary<string, int> biGramDict = new Dictionary<string, int>();
	private Dictionary<string, int> levenshteinDict = new Dictionary<string, int>();
	private List<string> biGramPredictionCorpus = new List<string>();


	private void Awake()
	{
		// Uncomment this when working in the Unity Editor or with new dictionaries
		/*
		string directoryPath = Application.dataPath + "/Resources/WordPrediction";
		if(!Directory.Exists(directoryPath))
		{    
			Directory.CreateDirectory(directoryPath);
		}

		if(!File.Exists(directoryPath + "/biGramDict.txt") || !File.Exists(directoryPath + "/levenshteinDict.txt"))
		{
			Debug.Log("No dictionaries found, building a new one. This can take a while depending on the corpus size.");
			var loadedCorpus = Resources.Load ("Sample") as TextAsset;
			string stringsCorpus = loadedCorpus.ToString ();

			GenerateBiGrams(stringsCorpus);
			GenerateLevenshteinDict(stringsCorpus);

			LevenshteinCorpus = levenshteinDict.Keys.ToList();
			Debug.Log("Dictionaries were succesfully generated.");
		}
		else
		{
			Debug.Log("Dictionaries found, word prediction is running.");
			biGramDict = LoadDictionary("WordPrediction/biGramDict");
			levenshteinDict = LoadDictionary("WordPrediction/levenshteinDict");

			LevenshteinCorpus = levenshteinDict.Keys.ToList();
		}
		*/

		biGramDict = LoadDictionary("WordPrediction/biGramDict");
		levenshteinDict = LoadDictionary("WordPrediction/levenshteinDict");

		LevenshteinCorpus = levenshteinDict.Keys.ToList();
	}

	private Dictionary<string, int> OrderDictionary(string filePath)
	{
		var loadedDict = Resources.Load (filePath) as TextAsset;
		string stringDict = loadedDict.ToString ();
		var dict = GetDict(stringDict);
		var orderedEnum = from entry in dict orderby entry.Value descending select entry;
		var orderedDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

		return orderedDict;
	}

	private Dictionary<string, int> LoadDictionary(string filePath)
	{
		var loadedDict = Resources.Load (filePath) as TextAsset;
		string stringDict = loadedDict.ToString ();
		var dict = GetDict(stringDict);

		return dict;
	}

	private void GenerateBiGrams(string corpus)
	{
		var nGrams = MakeNgrams(corpus, 2);

		for(int i = 0; i < nGrams.Count(); i++)
		{
			if (biGramDict.ContainsKey(nGrams.ElementAt(i)))
			{
				biGramDict[nGrams.ElementAt(i)] += 1;
			}
			else
			{
				biGramDict.Add(nGrams.ElementAt(i), 1);
			}
		}
		var orderedEnum = from entry in biGramDict orderby entry.Value descending select entry;
		biGramDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

		string s = GetLine(biGramDict);
		File.WriteAllText(Application.dataPath + "/Resources/AutoCorrect/biGramDict.txt", s);

//		#if UNITY_EDITOR
//			AssetDatabase.Refresh();
//		#endif
	}

	private void GenerateLevenshteinDict(string corpus)
	{
		var wordPattern = new Regex("[\\w']+");

		foreach (Match match in wordPattern.Matches(corpus))
		{
			int currentCount=0;
			levenshteinDict.TryGetValue(match.Value, out currentCount);

			currentCount++;
			levenshteinDict[match.Value] = currentCount;
		}
		var orderedEnum = from entry in levenshteinDict orderby entry.Value descending select entry;
		levenshteinDict = orderedEnum.ToDictionary(pair => pair.Key, pair => pair.Value);

		string s = GetLine(levenshteinDict);
		File.WriteAllText(Application.dataPath + "/Resources/AutoCorrect/levenshteinDict.txt", s);

//		#if UNITY_EDITOR
//			AssetDatabase.Refresh();
//		#endif
	}

	public void PredictNextWords(string input)
	{
		foreach(KeyValuePair<string, int> kvp in biGramDict)
		{
			if(kvp.Key.Contains(input.ToLower() + " "))
			{
				biGramPredictionCorpus.Add(kvp.Key.Split(' ')[1]);
			}
		}

		if(biGramPredictionCorpus.Count < ButtonLabels.Length)
		{
			for(int i = 0; i < biGramPredictionCorpus.Count; i++)
			{
				ButtonLabels[i].text = biGramPredictionCorpus[i];
			}
			for(int i = biGramPredictionCorpus.Count; i < ButtonLabels.Length; i++)
			{
				//Don't forget to filter repeating stuff like "to" "to" etc.
				ButtonLabels[i].text = LevenshteinCorpus[i - biGramPredictionCorpus.Count];
			}
		}
		else
		{
			for(int i = 0; i < ButtonLabels.Length; i++)
			{
				ButtonLabels[i].text = biGramPredictionCorpus[i];
			}
		}
		biGramPredictionCorpus.Clear();

	}

	// N-gram creator by Jake Drew bit.ly/N-grams
	public IEnumerable<string> MakeNgrams(string text, byte nGramSize)
	{
		StringBuilder nGram = new StringBuilder();
		Queue<int> wordLengths = new Queue<int>();
		int wordCount = 0;
		int lastWordLen = 0;

		if(text != "" && char.IsLetterOrDigit(text[0]))
		{
			nGram.Append(text[0]);
			lastWordLen++;
		}

		for(int i = 1; i < text.Length - 1; i++)
		{
			char before = text[i - 1];
			char after = text[i + 1];

			if(char.IsLetterOrDigit(text[i]) || (text[i] != ' '
			   && (char.IsSeparator(text[i]) || char.IsPunctuation(text[i]))
				&& (char.IsLetterOrDigit(before) && char.IsLetterOrDigit(after))))
			{
				nGram.Append(text[i]);
				lastWordLen++;
			}
			else if(lastWordLen > 0)
			{
				wordLengths.Enqueue(lastWordLen);
				lastWordLen = 0;
				wordCount++;
			
				if(wordCount >= nGramSize)
				{
					yield return nGram.ToString();
					nGram.Remove(0, wordLengths.Dequeue() + 1);
					wordCount -= 1;
				}

				nGram.Append(" ");
			}
		}
	}

	string GetLine(Dictionary<string, int> d)
	{
		StringBuilder builder = new StringBuilder();
		foreach (KeyValuePair<string, int> pair in d)
		{
			builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
		}
		string result = builder.ToString();
		result = result.TrimEnd(',');
		return result;
	}

	Dictionary<string, int> GetDict(string s)
	{
		Dictionary<string, int> d = new Dictionary<string, int>();
		string[] tokens = s.Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < tokens.Length; i += 2)
		{
			string name = tokens[i];
			string freq = tokens[i + 1];

			int count = int.Parse(freq);
			if (d.ContainsKey(name))
			{
				d[name] += count;
			}
			else
			{
				d.Add(name, count);
			}
		}
		return d;
	}
}