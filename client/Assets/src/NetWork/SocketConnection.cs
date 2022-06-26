using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


using UnityEngine;
using Newtonsoft.Json;
using NativeWebSocket;
using log4net;


using static GameInfo;
using static GameManager;
using static Side;
using static Constants;


public class SocketConnection : MonoBehaviour
{
	static WebSocket websocket = null;
	public static Queue<string> messages = new Queue<string>();
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

		websocket.OnMessage += (bytes) => ProcessMessage(bytes);

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

		SendWebSocketMessage();

		#if !UNITY_WEBGL || UNITY_EDITOR
			websocket.DispatchMessageQueue();
		#endif
	}

	async void SendWebSocketMessage()
	{
		if (isOpen())
		{
			while (messages.Count > 0)
			{
				await websocket.SendText(messages.Dequeue());
			}
		}
	}

	private void OnApplicationQuit()
	{
		Close();
	}

	private static void ProcessMessage(byte[] bytes)
	{
		{
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			ServerMessageProcessor.processServerMessage(message);
		}
	}
}


public class ApiGameInfo
{
  public string path;
  public GameInfo body;
}

public class ApiPrepareGame
{
	public string path;
	public PrepareGameBody body;
}

public class PrepareGameBody
{
	public Side side;
	public GameInfo game;
}
