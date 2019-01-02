using UnityEngine;
using System.Collections;
using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text; 
using System.Collections.Generic;


/// <summary>
/// Client Manager. Manages the client thats already connected to a server. 
/// </summary>
public  class ClientManager : MonoBehaviour
{
	Client client;

	public void Start()
	{
		client = new Client (); 
	}

	/// <summary>
	/// Determines whether this instance is client available.
	/// </summary>
	/// <returns><c>true</c> if this instance is client available; otherwise, <c>false</c>.</returns>
	public bool IsClientAvailable()
	{
		if (client.IsConnected ()) {
			return true;
		}

		return false;
	}

	/// <summary>
	/// Sends a message to the server
	/// </summary>
	/// <param name="msg">Message.</param>
	public void SendMessage(String msg)
	{
		client.Send (msg,true);
		//client.Receive ();
	}

	/// <summary>
	/// Gets a messages from the server
	/// </summary>
	public void GetMessage()
	{
		client.Receive ();
	}
}

