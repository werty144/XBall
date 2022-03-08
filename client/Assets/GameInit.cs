using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SocketConnection;

public class GameInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SocketConnection.state.players.Count; i++) 
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.tag = "Player";
            player.AddComponent<PlayerScript>();
            player.GetComponent<PlayerScript>().id = SocketConnection.state.players[i].id;
        }

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
