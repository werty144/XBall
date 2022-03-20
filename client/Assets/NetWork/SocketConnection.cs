using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

using NativeWebSocket;


public class SocketConnection : MonoBehaviour
{
  WebSocket websocket;
  public static GameState state;
  bool firstMessageSent;
  bool gameStarted;
  public static Queue<string> messages = new Queue<string>();

  // Start is called before the first frame update
  async void Start()
  {
    Debug.Log("Connection started!");

    firstMessageSent = false;

    websocket = new WebSocket("ws://localhost:8080");

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
      try {
        // If compiler not working on this line you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings
        dynamic json = JsonConvert.DeserializeObject(message);
        if (json.path == "invite") {
          var invite = JsonConvert.DeserializeObject<ApiInvite>(message).body;
          var replyObject = JsonConvert.SerializeObject(new {path = "acceptInvite", body = new {inviteId = invite.inviteId}});
          messages.Enqueue((string)replyObject);
        } else if (json.path == "game") {
          // This code should definetely be elsewhere. Also the state variable is not the variable of the socket connection. 
          state = JsonConvert.DeserializeObject<ApiGameInfo>(message).body.state;
          if (!gameStarted) {
            startGame();
            gameStarted = true;
          }
        }
      } catch (Exception e) {
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
      if (!firstMessageSent) {
        await websocket.SendText("0_salt");
        firstMessageSent = true;
        return;
      }
      // Sending bytes
      // await websocket.Send(new byte[] { 10, 20, 30 });

      // Sending plain text
      while (messages.Count > 0) {
        await websocket.SendText(messages.Dequeue());
      }
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }

  void startGame() {
    SceneManager.LoadScene("GameScene");
  }
}


public class ApiInvite {
  public string path;
  public Invite body;
}

public class Invite {
  public int inviteId;
  public int inviterId;
  public int invitedId;
  public GameProperties gameProperties;
}

public class ApiGameInfo {
  public string path;
  public GameInfo body;
}

public class GameInfo {
  public GameState state;
  public string score;
  public long time;
  public string status;
}

public class GameProperties {
  public int playersNumber;
  public string speed;
}

public class GameState {
  public List<Player> players;
  public BallState ballState;
}

public class Point {
  public float x;
  public float y;
}

public class Player {
  public int id;
  public int userId;
  public PlayerState state;
}

public class PlayerState {
  public float x;
  public float y;
  public float z;
  public float rotationAngle;
}

public class BallState {
  public int? ownerId;
  public float x;
  public float y;
  public float z;
}

public class Vector {
  public float x;
  public float y;
}
