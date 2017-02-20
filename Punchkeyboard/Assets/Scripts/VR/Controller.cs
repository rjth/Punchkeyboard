using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	public delegate void OnTrackpadPress(int deviceID, string side);
	public static OnTrackpadPress TrackpadPressed;

	public delegate void OnTriggerPress();
	public static OnTriggerPress TriggerPressed;

	private SteamVR_TrackedObject trackedObject;
	private SteamVR_Controller.Device device;
	private List<Rigidbody> keyRigidbodies = new List<Rigidbody>();

	void Start()
	{
		trackedObject = GetComponent<SteamVR_TrackedObject>();
		GameObject[] keys = GameObject.FindGameObjectsWithTag ("Key");
		for (int i = 0; i < keys.Length; i++)
		{
			keyRigidbodies.Add (keys [i].GetComponent<Rigidbody> ());
		}
	}

	void Update()
	{
		device = SteamVR_Controller.Input ((int)trackedObject.index);
		if (device.GetAxis ().x != 0)
		{
			if (device.GetAxis ().x < -0.33f)
			{
				if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					TrackpadPressed ((int)trackedObject.index, "left");
				}
			}
			else if (device.GetAxis ().x > -0.33f && device.GetAxis ().x < 0.33f)
			{
				if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					TrackpadPressed ((int)trackedObject.index, "center");
				}
			}
			else if (device.GetAxis ().x > 0.33f)
			{
				if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					TrackpadPressed ((int)trackedObject.index, "right");
				}
			}
		}
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger))
		{
			TriggerPressed ();
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Grip) && col.gameObject.tag == "Keyboard")
		{
			foreach (Rigidbody rb in keyRigidbodies)
			{
				rb.isKinematic = true;
			}
			col.transform.SetParent(this.gameObject.transform);
		}
		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Grip) && col.gameObject.tag == "Keyboard")
		{
			foreach (Rigidbody rb in keyRigidbodies)
			{
				rb.isKinematic = false;
			}
			col.transform.SetParent(null);
		}
	}
}