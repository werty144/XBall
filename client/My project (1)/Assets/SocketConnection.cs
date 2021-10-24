using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using NativeWebSocket;

public class SocketConnection : MonoBehaviour
{
  WebSocket websocket;
  CoordsGetter coordsGetter;
  public int x = 0;

  // Start is called before the first frame update
  async void Start()
  {

    coordsGetter = GameObject.FindWithTag("coords").GetComponent<CoordsGetter>();

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
      Debug.Log("OnMessage!");
      Debug.Log(bytes);

      // getting the message as a string
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      // var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
      try {
        var json = JsonConvert.DeserializeObject<ApiMessage>(message);
        // var reader = new JsonReader();
        // dynamic json = reader.Read(src.ToString());
        x = json.body.state;
      }catch {

      }
      // Debug.Log("OnMessage! " + message);
    };

    // Keep sending messages at every 0.3s
    InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

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
      // Sending bytes
      // await websocket.Send(new byte[] { 10, 20, 30 });

      // Sending plain text
      if (coordsGetter.messages.Count > 0) {
        await websocket.SendText(coordsGetter.messages.Dequeue());
      }
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }

}

public class ApiMessage {
  public string path;
  public Game body;
}

public class Game {
  public int id;
  public KeyValuePair<int, int> players;
  public int state;
}