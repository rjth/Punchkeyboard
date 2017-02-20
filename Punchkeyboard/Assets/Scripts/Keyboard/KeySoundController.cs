using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySoundController : MonoBehaviour
{
	public GameObject KeySoundPlayer;

	public void StartKeySound(Transform keyTransform)
	{
		StartCoroutine ("PlayKeySound", keyTransform);
	}

	private IEnumerator PlayKeySound(Transform keyTransform)
	{
		GameObject player = Instantiate (KeySoundPlayer, keyTransform.position, keyTransform.rotation);
		player.GetComponent<AudioSource> ().Play ();
		yield return new WaitForSeconds (1);
		Destroy (player);
	}
}