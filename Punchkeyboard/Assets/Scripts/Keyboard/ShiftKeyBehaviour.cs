using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsInput;

public class ShiftKeyBehaviour : MonoBehaviour
{
	public GameObject Housing;
	private Renderer keyRenderer;
	private BoxCollider keyCollider;
	private GameObject keyCap;

	private Key shiftKeyController; 
	private GameObject[] keys;
	private Key[] keyControllers;
	private bool shiftToggle = true;

	void Start()
	{
		Key.keyPressed += ShiftKeyPressed;

		shiftKeyController = this.gameObject.GetComponent<Key> ();
		keys = GameObject.FindGameObjectsWithTag ("Key");
		keyControllers = new Key[keys.Length];
		for (int i = 0; i < keys.Length; i++)
		{
			keyControllers [i] = keys [i].GetComponent<Key> ();
		}

		keyRenderer = this.gameObject.GetComponent<Renderer> ();
		keyCollider = this.gameObject.GetComponent<BoxCollider> ();
		keyCap = this.gameObject.transform.GetChild(0).gameObject	;
	}

	void ShiftKeyPressed()
	{
		if (shiftKeyController.KeyPressed)
		{
			for (int i = 0; i < keyControllers.Length; i++)
			{
				keyControllers [i].SwitchKeycapCharCase ();
			}
			if (shiftToggle)
			{
				shiftKeyController.KeycapColor = shiftKeyController.PressedKeycapColor;
				InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
				shiftToggle = false;
			}
			else if (!shiftToggle)
			{
				shiftKeyController.KeycapColor = shiftKeyController.InitialKeycapColor;
				InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
				shiftToggle = true;
			}
		}
	}
		
	public void ShiftVisibilityToggle(bool state)
	{
		keyRenderer.enabled = state;
		keyCollider.enabled = state;
		keyCap.SetActive(state);
		Housing.SetActive(state);
		shiftKeyController.KeycapColor = shiftKeyController.InitialKeycapColor;
	}

	void OnDisable()
	{
		Key.keyPressed -= ShiftKeyPressed;
		if (!shiftToggle)
		{
			InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
		}
	}
}
