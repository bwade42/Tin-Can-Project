using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class IPAdressField : MonoBehaviour
{

	private InputField ipField;

	private string IPAddress = "";

	private bool inputReady = false;

	// Use this for initialization
	void Start ()
	{
		ipField = GetComponent<InputField> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Adds a listener that invokes the "GetIPAddress" method when the player finishes editing the main input field.
		//Passes the main input field into the method when "LockInput" is invoked
		ipField.onEndEdit.AddListener(delegate {AddIPAddress(ipField); });
	}

	// Checks if there is anything entered into the input field.
	public void AddIPAddress(InputField input)
	{
		inputReady = true;
		SetIPAddress (input.text);
	}

	public string GetIPAddress()
	{
		return IPAddress;
	}

	public void SetIPAddress(string ip)
	{
		IPAddress = ip;
	}

	public bool IsInputReady()
	{
		return inputReady;
	}
}

