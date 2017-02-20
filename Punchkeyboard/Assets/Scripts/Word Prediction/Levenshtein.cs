using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Levenshtein : MonoBehaviour
{
	public NGramGenerator NGramHandler;
	public Text[] ButtonLabels;

	private const int maxWordLength = 15;
	private const int maxLevenshteinCost = 7;
	private const int minLevenshteinCost = 1;
	private List<string> corpus = new List<string>();
	private bool isUppercase = false;
	private bool isFirstLetterUpper = false;

	public static class LevenshteinDistance
	{
		// Levenshtein distance computation from dotnetpearls.com/levenshtein
		public static int Compute(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			for (int i = 1; i <= n; i++)
			{
				for (int j = 1; j <= m; j++)
				{
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}
	}

	void Start()
	{
		corpus = NGramHandler.LevenshteinCorpus;
		for(int i = 0; i < ButtonLabels.Length; i++)
		{
			ButtonLabels[i].text = corpus[i];
		}
	}

	public void RunAutoComplete(string input)
	{
		if(input.Length > 0)
		{
			char[] lastChar = input.Substring(input.Length - 1).ToCharArray();	
			string lastWord = input.Split(' ').Last ();
			char[] firstCharOfLastWord = lastWord.Substring(0).ToCharArray();
			if (firstCharOfLastWord.Length >= 1)
			{
				if (firstCharOfLastWord[0].ToString().Any(char.IsUpper))
				{
					isFirstLetterUpper = true;
				}
				else
				{
					isFirstLetterUpper = false;
				}

			}
			if(!char.IsWhiteSpace(lastChar[0]))
			{
				if(lastWord.Length < maxWordLength)
				{
					if (input.Length >= 0)
					{
						Dictionary<int, int> dict = new Dictionary<int, int> ();

						for (int i = 0; i < corpus.Count; i++)
						{
							int cost = LevenshteinDistance.Compute (lastWord.ToLower(), corpus[i]);
							if (cost >= minLevenshteinCost && cost <= maxLevenshteinCost)
							{
								dict.Add (i, cost);
							}
						}

						if (lastWord.All (char.IsUpper))
						{
							isUppercase = true;
						}
						if (lastWord.Any (char.IsLower))
						{
							isUppercase = false;
						}

						List<int> distanceOrder = dict.OrderBy (kp => kp.Value).Select (kp => kp.Key).ToList ();

						for (int i = 0; i < distanceOrder.Count; i++)
						{
							if (i < ButtonLabels.Length)
							{
								if (isUppercase)
								{
									ButtonLabels [i].text = corpus [distanceOrder [i]].ToUpper ();
								}
								else if (isFirstLetterUpper && isUppercase == false)
								{
									ButtonLabels [i].text = char.ToUpper(corpus [distanceOrder [i]][0]) + corpus [distanceOrder [i]].Substring(1);
								}
								else if(!isUppercase && isFirstLetterUpper == false)
								{
									ButtonLabels [i].text = corpus [distanceOrder [i]].ToLower ();
								}
							}
						}
					}
				}
			}
		}
	}
}