using UnityEngine;
using System.Collections;

using System;
using System.Net.Sockets;
using System.ComponentModel;



/// <summary>
/// Socket listner. IDisposable interface allows us to proper release
/// sockets.
/// </summary>
public interface IClient : IDisposable
{
	event ConnectedHandler Connected;

	event ClientMessageReceivedHandler MessageReceived;

	event ClientMessageSubmittedHandler MessageSubmitted;

	void StartClient();

	bool IsConnected();

	void Receive();

	void Send(string msg, bool close);
}
