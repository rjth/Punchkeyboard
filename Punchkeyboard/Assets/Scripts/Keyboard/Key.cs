using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
	public delegate void OnKeyPressed();
	public static OnKeyPressed keyPressed;
	public string KeyCapChar;
	public string AlterateKeyCapChar;
	public Rigidbody Rigidbody;
	public bool KeyPressed = false;
	public Color PressedKeycapColor;
	public Color KeycapColor;
	public Color InitialKeycapColor;

	protected Transform initialPosition;
	private KeycodeAdder keycodeAdder;
	private Text keyCapText;
	private Vector3 initialLocalPosition;
	private Quaternion initialLocalRotation;
	private Vector3 constrainedPosition;
	private Quaternion constrainedRotation;
	private bool uppercaseSwitch = true;
	private bool symbolSwitch = false;
	private bool checkForButton = true;
	private const float DistanceToBePressed = 0.01f;
	private const float KeyBounceBackMultiplier = 1500f;
	private KeySoundController keySoundController;
	private float currentDistance = -1;

	void Start()
	{
		keycodeAdder = this.gameObject.GetComponent<KeycodeAdder> ();

		keyCapText = this.gameObject.GetComponentInChildren<Text> ();
		KeycapColor = this.gameObject.GetComponent<Renderer> ().material.color;
		InitialKeycapColor = KeycapColor;

		initialPosition = new GameObject(string.Format("[{0}] initialPosition", this.gameObject.name)).transform;
		initialPosition.parent = this.transform.parent;
		initialPosition.localPosition = Vector3.zero;
		initialPosition.localRotation = Quaternion.identity;

		if(Rigidbody == null)
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		initialLocalPosition = this.transform.localPosition;
		initialLocalRotation = this.transform.localRotation;

		constrainedPosition = initialLocalPosition;
		constrainedRotation = initialLocalRotation;

		keySoundController = transform.parent.root.gameObject.GetComponent<KeySoundController> ();

		SwitchKeycapCharCase ();
	}

	void FixedUpdate()
	{
		ConstrainPosition ();
		currentDistance = Vector3.Distance(this.transform.position, initialPosition.position);

		Vector3 PositionDelta = initialPosition.position - this.transform.position;
		this.Rigidbody.velocity = PositionDelta * KeyBounceBackMultiplier * Time.deltaTime;
	}

	void Update()
	{
		if (checkForButton)
		{
			if (currentDistance > DistanceToBePressed)
			{
				KeyPressed = true;
				keyPressed ();
				if (symbolSwitch)
				{
					keycodeAdder.SimulateAlternateKeyPress ();
				}
				else
				{
					keycodeAdder.SimulateKeyPress ();
				}
				keySoundController.StartKeySound (this.gameObject.transform);
				checkForButton = false;
			}
		} else if (!checkForButton)
		{
			if (currentDistance < DistanceToBePressed)
			{
				KeyPressed = false;
				checkForButton = true;
			}
		}

		ChangeKeyColorOnPress ();
	}

	void LateUpdate()
	{
		ConstrainPosition ();
	}

	void ChangeKeyColorOnPress()
	{
		if (KeyPressed)
		{
			gameObject.GetComponent<Renderer> ().material.color = PressedKeycapColor;
		}
		else
		{
			gameObject.GetComponent<Renderer> ().material.color = KeycapColor;
		}
	}

	void ConstrainPosition()
	{
		constrainedPosition.y = this.transform.localPosition.y;
		if (this.transform.localPosition.y > initialLocalPosition.y)
		{
			constrainedPosition.y = initialLocalPosition.y;
		}
		this.transform.localPosition = constrainedPosition;
		this.transform.localRotation = constrainedRotation;
	}

	public void SwitchKeycapCharCase()
	{
		if (uppercaseSwitch)
		{
			keyCapText.text = KeyCapChar.ToLower ();
			uppercaseSwitch = false;
		}
		else
		{
			keyCapText.text = KeyCapChar.ToUpper ();
			uppercaseSwitch = true;
		}
	}

	public void SwitchToSymbols()
	{
		if (!symbolSwitch)
		{
			keyCapText.text = AlterateKeyCapChar;
			symbolSwitch = true;
		}
		else
		{
			keyCapText.text = KeyCapChar;
			keyCapText.text = KeyCapChar.ToLower ();
			symbolSwitch = false;
		}
	}
}