using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SocketConnection;
using static GameConstants;


public class GameInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < SocketConnection.state.players.Count; i++) 
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            player.transform.localScale = new Vector3(2 * GameConstants.playerRadius, 1F, 2 * GameConstants.playerRadius);
            player.tag = "Player";
            player.AddComponent<PlayerScript>();
            player.GetComponent<PlayerScript>().id = SocketConnection.state.players[i].id;
        }

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<BallScript>();
        ball.transform.localScale = new Vector3(GameConstants.ballRadius, GameConstants.ballRadius, GameConstants.ballRadius);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
