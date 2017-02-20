using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput;

public class KeycodeAdder : MonoBehaviour
{
	public VirtualKeyCode KeyKeycode;
	public VirtualKeyCode KeyKeycodeModifier;
	public VirtualKeyCode[] AlternateKeyKeycode;

	public void SimulateKeyPress()
	{
		InputSimulator.SimulateKeyPress(KeyKeycode);
	}

	public void SimulateAlternateKeyPress()
	{
		InputSimulator.SimulateModifiedKeyStroke (KeyKeycodeModifier, AlternateKeyKeycode);
	}
}