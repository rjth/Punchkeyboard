using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerResponseHandler : MonoBehaviour
{
	public InputField TextInputField;
	public Button SecondarySuggestionBtn;
	public Button PrimarySuggestionBtn;
	public Button TertiarySuggestionBtn;

	public Color HighlightedBtnColor;
	private Color initialBtnColor;

	private SteamVR_Controller.Device device;
	private const int hapticFeedbackStrength = 600;

	void Start ()
	{
		Controller.TrackpadPressed += HandleTrackpad;
		Controller.TriggerPressed += HandleTrigger;

		initialBtnColor = PrimarySuggestionBtn.GetComponent<Image> ().color;
	}

	private void HandleTrackpad(int deviceID, string side)
	{
		device = SteamVR_Controller.Input (deviceID);
		device.TriggerHapticPulse (hapticFeedbackStrength);

		if (side == "left")
		{
			SecondarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", SecondarySuggestionBtn.GetComponent<Image> ());
		}
		else if (side == "center")
		{
			PrimarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", PrimarySuggestionBtn.GetComponent<Image> ());
		}
		else if (side == "right")
		{
			TertiarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", TertiarySuggestionBtn.GetComponent<Image> ());
		}
	}

	private void HandleTrigger()
	{
		TextInputField.ActivateInputField ();
	}

	private IEnumerator HighlightButton (Image img)
	{
		float elapsedTime = 0.0f;
		float totalTime = 0.1f;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			img.color = Color.Lerp (initialBtnColor, HighlightedBtnColor, (elapsedTime / totalTime));
			StartCoroutine ("FadeButton", img);
			yield return null;
		}
	}

	private IEnumerator FadeButton (Image img)
	{
		float elapsedTime = 0.0f;
		float totalTime = 0.05f;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			img.color = Color.Lerp (HighlightedBtnColor, initialBtnColor, (elapsedTime / totalTime));
			yield return null;
		}
	}

	void OnDisable()
	{
		Controller.TrackpadPressed -= HandleTrackpad;
		Controller.TriggerPressed -= HandleTrigger;
	}
}