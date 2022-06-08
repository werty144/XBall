using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NativeWebSocket;
using System.Threading.Tasks;


using static GameInfo;
using static GameManager;
using static Side;
using static Constants;


public class SocketConnection : MonoBehaviour
{
	WebSocket websocket;
	bool firstMessageSent = false;
	public static Queue<string> messages = new Queue<string>();
	string password;

	void Awake()
	{
		websocket = new WebSocket("ws://" + Constants.serverURL);
	}

	public async Task StartConnection(string password_)
	{
		password = password_;
		websocket.OnOpen += () =>
		{
			Debug.Log("Connection open!");
		};

		websocket.OnError += (e) =>
		{
			Debug.Log("Error! " + e);
		};

		websocket.OnClose += (e) =>
		{
			Debug.Log("Connection closed!");
		};

		websocket.OnMessage += (bytes) => ProcessMessage(bytes);

		InvokeRepeating("SendWebSocketMessage", 0.0f, 0.05f);

		await websocket.Connect();
	}

	public bool isOpen()
	{
		return websocket.State == WebSocketState.Open;
	}

	void Update()
	{
		#if !UNITY_WEBGL || UNITY_EDITOR
			websocket.DispatchMessageQueue();
		#endif
	}

	async void SendWebSocketMessage()
	{
		if (websocket.State == WebSocketState.Open)
		{
			// Separate manager?..
			if (!firstMessageSent)
			{
				await websocket.SendText(password);
				firstMessageSent = true;
				return;
			}

			while (messages.Count > 0)
			{
				await websocket.SendText(messages.Dequeue());
			}
		}
	}

	private async void OnApplicationQuit()
	{
		await websocket.Close();
	}

	private void ProcessMessage(byte[] bytes)
	{
		{
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			try
			{
				// If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
				dynamic json = JsonConvert.DeserializeObject(message);
				switch ((string) json.path)
				{
					case "game":
						var gameInfo = JsonConvert.DeserializeObject<ApiGameInfo>(message).body;
						GameManager.setGameInfo(gameInfo);
						break;
					case "prepareGame":
						var body = JsonConvert.DeserializeObject<ApiPrepareGame>(message).body;
						MainMenu.prepareGame(body.game.state, body.side);
						break;
					default:
						break;
				}
			} catch (Exception e) 
			{
				Debug.Log("Error when parsing! " + e);
			}
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
