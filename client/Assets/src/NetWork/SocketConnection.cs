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
	WebSocket websocket = new WebSocket("ws://localhost:8080");
	// WebSocket websocket = new WebSocket("ws://xball-server.herokuapp.com/");
	bool firstMessageSent = false;
	public static Queue<string> messages = new Queue<string>();
	string password;

	public async void StartConnection(string password_)
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
					case "invite":
						print("Got invite");
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
		}
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
public class Invite
{
  public int inviteId;
  public long inviterId;
  public long invitedId;
  public GameProperties gameProperties;
}


