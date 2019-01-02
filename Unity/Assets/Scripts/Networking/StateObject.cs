using UnityEngine;
using System.Collections;
using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text; 

// State object for receiving data from remote device.  
public sealed class StateObject : IStateObject
{
	/* Contains the state information. */

	private const int Buffer_Size = 10000;
	private readonly byte[] buffer = new byte[Buffer_Size];
	private readonly Socket listener;
	private readonly int id;
	private StringBuilder sb;

	public StateObject(Socket listener, int id = -1)
	{
		this.listener = listener;
		this.id = id;
		this.Close = false;
		this.Reset();
	}

	public int Id
	{
		get
		{
			return this.id;
		}
	}

	public bool Close { get; set; }

	public int BufferSize
	{
		get
		{
			return Buffer_Size;
		}
	}

	public byte[] Buffer
	{
		get
		{
			return this.buffer;
		}
	}

	public Socket Listener
	{
		get
		{
			return this.listener;
		}
	}

	public string Text
	{
		get
		{
			return this.sb.ToString();
		}
	}

	public void Append(string text)
	{
		this.sb.Append(text);
	}

	public void Reset()
	{
		this.sb = new StringBuilder();
	}
}