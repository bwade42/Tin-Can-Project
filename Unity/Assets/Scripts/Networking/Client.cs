using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections;
using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text; 


public delegate void ConnectedHandler(IClient a);
public delegate void ClientMessageReceivedHandler(IClient a, string msg);
public delegate void ClientMessageSubmittedHandler(IClient a, bool close);

public sealed class Client : MonoBehaviour,IClient
{
	private const ushort Port = 80;
	private string ip = "10.0.0.134";

	private System.Net.IPAddress ipAddress;

	private Socket listener;
	private bool close;

	private readonly ManualResetEvent connected = new ManualResetEvent(false); // ManualResetEvent instances signal completion. 
	private readonly ManualResetEvent sent = new ManualResetEvent(false); // signal message sent
	private readonly ManualResetEvent received = new ManualResetEvent(false); // signal message recieved

	public event ConnectedHandler Connected; 

	public event ClientMessageReceivedHandler MessageReceived;

	public event ClientMessageSubmittedHandler MessageSubmitted;

	public Client()
	{
		StartClient ();
	}

	/// <summary>
	/// Starts the client.
	/// </summary>
	public void StartClient()
	{
		try
		{
			ipAddress = System.Net.IPAddress.Parse(ip);
			System.Net.IPEndPoint remoteEndPoint = new IPEndPoint (ipAddress,Port);
			this.listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);  

			this.listener.BeginConnect( remoteEndPoint,new AsyncCallback(ConnectCallback), this.listener);  
			ConnectedHandler connectedHandler = this.Connected;

			if (connectedHandler != null)
			{
				connectedHandler(this);
			}
		}
		catch (SocketException)
		{
			// TODO:
		}
	}

	#region Server Conenction Handling

	/// <summary>
	/// Determines whether this instance is connected.
	/// </summary>
	/// <returns><c>true</c> if this instance is connected; otherwise, <c>false</c>.</returns>
	public bool IsConnected()
	{
		return !(this.listener.Poll(1000, SelectMode.SelectRead) && this.listener.Available == 0);
	}

	/// <summary>
	/// Connects the callback.
	/// </summary>
	/// <param name="result">Result.</param>
	private void ConnectCallback(IAsyncResult result)
	{
		Socket server = (Socket)result.AsyncState;
		try
		{
			server.EndConnect(result);
			this.connected.Set();
		}
		catch (SocketException)
		{
		}
	}
	#endregion


	#region Recieve data

	/// <summary>
	/// Receive a message from the server
	/// </summary>
	public void Receive()
	{
		var state = new StateObject(this.listener);

		state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
	}

	/// <summary>
	/// Callback used for recieving messages
	/// </summary>
	/// <param name="result">Result.</param>
	private void ReceiveCallback(IAsyncResult result)
	{
		var state = (IStateObject)result.AsyncState;
		var receive = state.Listener.EndReceive(result);

		if (receive > 0)
		{
			state.Append(Encoding.UTF8.GetString(state.Buffer, 0, receive));
			print ("Data = " + state.Text);

		}

		if (receive == state.BufferSize)
		{
			state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
		}
		else
		{
			ClientMessageReceivedHandler messageReceived = this.MessageReceived;

			if (messageReceived != null)
			{
				messageReceived(this, state.Text);
				print ("Data = " + state.Text);
			}

			state.Reset();
			this.received.Set();
		}
	}
	#endregion

	#region Send data
	/// <summary>
	/// Send the specified msg and close.
	/// </summary>
	/// <param name="msg">Message.</param>
	/// <param name="close">If set to <c>true</c> close.</param>
	public void Send(string msg, bool close)
	{
		if (!this.IsConnected())
		{
			throw new Exception("Destination socket is not connected.");
		}

		var response = Encoding.UTF8.GetBytes(msg);

		this.close = close;
		this.listener.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, this.listener);
	}
	/// <summary>
	/// Sends the callback.
	/// </summary>
	/// <param name="result">Result.</param>
	private void SendCallback(IAsyncResult result)
	{
		try
		{
			var receiver = (Socket)result.AsyncState;

			receiver.EndSend(result);
		}
		catch (SocketException)
		{
			// TODO:
		}
		catch (ObjectDisposedException)
		{
			// TODO;
		}

		var messageSubmitted = this.MessageSubmitted;

		if (messageSubmitted != null)
		{
			messageSubmitted(this, this.close);
		}

		this.sent.Set();
	}
	#endregion

	/// <summary>
	/// Close a socket.
	/// </summary>
	private void Close()
	{
		try
		{
			if (!this.IsConnected())
			{
				return;
			}

			this.listener.Shutdown(SocketShutdown.Both);
			this.listener.Close();
		}
		catch (SocketException)
		{
			// TODO:
		}
	}
	/// <summary>
	/// Releases all resource used by the <see cref="Client"/> object.
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Client"/>. The <see cref="Dispose"/> method
	/// leaves the <see cref="Client"/> in an unusable state. After calling <see cref="Dispose"/>, you must release all
	/// references to the <see cref="Client"/> so the garbage collector can reclaim the memory that the
	/// <see cref="Client"/> was occupying.</remarks>
	public void Dispose()
	{
		this.connected.Close();
		this.sent.Close();
		this.received.Close();
		this.Close();
	}

}
