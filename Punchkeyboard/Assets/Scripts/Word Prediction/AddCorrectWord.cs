using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCorrectWord : MonoBehaviour
{
	private AutocompleteWordPicker wordPicker;

	void Start()
	{
		wordPicker = gameObject.GetComponentInParent<AutocompleteWordPicker>();
	}

	public void WordChosen()
	{
		wordPicker.ReplaceWord(gameObject.GetComponentInChildren<Text>().text);
	}
}