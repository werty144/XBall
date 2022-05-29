using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NativeWebSocket;


using static GameInfo;
using static GameManager;
using static Side;


public class SocketConnection : MonoBehaviour
{
	WebSocket websocket;
	bool firstMessageSent;
	public static Queue<string> messages = new Queue<string>();

	// Start is called before the first frame update
	async void Start()
	{
		Debug.Log("Connection started!");

		firstMessageSent = false;

		websocket = new WebSocket("ws://localhost:8080");
		// websocket = new WebSocket("ws://xball-server.herokuapp.com/");

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

		websocket.OnMessage += (bytes) =>
		{
			// getting the message as a string
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			try
			{
				// If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
				dynamic json = JsonConvert.DeserializeObject(message);
				switch ((string) json.path)
				{
					case "invite":
						var invite = JsonConvert.DeserializeObject<ApiInvite>(message).body;
						MainMenu.receiveInvite(invite);
						break;
					case "game":
						var gameInfo = JsonConvert.DeserializeObject<ApiGameInfo>(message).body;
						GameManager.setGameInfo(gameInfo);
						break;
					case "prepareGame":
						var body = JsonConvert.DeserializeObject<ApiPrepareGame>(message).body;
						GameManager.prepareGame(body.game.state, body.side);
						break;
					default:
						break;
				}
			} catch (Exception e) 
			{
				Debug.Log("Error when parsing! " + e);
			}
		};

		// Keep sending messages at every 0.05s
		InvokeRepeating("SendWebSocketMessage", 0.0f, 0.05f);

		// waiting for messages
		await websocket.Connect();
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
				await websocket.SendText("0_salt");
				firstMessageSent = true;
				return;
			}

			// Sending plain text
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
}


public class ApiInvite
{
  public string path;
  public Invite body;
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


// Should make separate manager
public class Invite {
  public int inviteId;
  public int inviterId;
  public int invitedId;
  public GameProperties gameProperties;
}


