using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


using UnityEngine;
using NativeWebSocket;
using log4net;
using MessagePack;


using static GameInfo;
using static GameManager;
using static Side;
using static Constants;


public class SocketConnection : MonoBehaviour
{
	static WebSocket websocket = null;
	static Queue<byte[]> messagesFromServer = new Queue<byte[]>();
	public static Queue<string> messagesToServer = new Queue<string>();
	string password;
	public static readonly ILog Log = LogManager.GetLogger(typeof(SocketConnection));


	public static async void Connect(int port)
	{
		string url = string.Format("ws://localhost:{0}/", port);
		websocket = new WebSocket(url);


		websocket.OnOpen += () =>
		{
			Log.Info("Socket connection opened");
			ServerManager.OnConnectionOpen();
		};

		websocket.OnError += (e) =>
		{
			Log.Error("Socket connection error: " + e);
		};

		websocket.OnClose += (e) =>
		{
			Log.Info("Socket connection closed");
		};

		websocket.OnMessage += (bytes) => 
		{
			PerformanceTracker.MessagesFromServer += 1;
			messagesFromServer.Enqueue(bytes);
		};

		await websocket.Connect();
	}

	public static bool isOpen()
	{
		if (websocket == null)
		{
			return false;
		}
		return websocket.State == WebSocketState.Open;
	}

	public static bool isClosedOrNull()
	{
		return (websocket == null) || (websocket.State == WebSocketState.Closed);
	}

	public static async void Close()
	{
		if (websocket != null)
		{
			await websocket.Close();
		}
	}

	void Update()
	{
		if (websocket == null) return;

		SendMessagesToServer();

		#if !UNITY_WEBGL || UNITY_EDITOR
			websocket.DispatchMessageQueue();
		#endif

		ProcessMessagesFromServer();
	}

	void ProcessMessagesFromServer()
	{
		var messages = messagesFromServer.Select(b => System.Text.Encoding.UTF8.GetString(b));
		ServerMessageProcessor.processServerMessages(messages);
		messagesFromServer.Clear();
	}

	async void SendMessagesToServer()
	{
		if (isOpen())
		{
			while (messagesToServer.Count > 0)
			{
				await websocket.SendText(messagesToServer.Dequeue());
			}
		}
	}

	private void OnApplicationQuit()
	{
		Close();
	}
}
