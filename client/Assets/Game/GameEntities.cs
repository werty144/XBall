using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static SocketConnection;
using static GameConstants;


public class GameEntities : MonoBehaviour
{
    public List<GameObject> myPlayers = new List<GameObject>();
    public GameObject ball;
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
            player.GetComponent<PlayerScript>().userId = SocketConnection.state.players[i].userId;
            if (player.GetComponent<PlayerScript>().userId == 0)
            {
                myPlayers.Add(player);
            }
        }

        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<BallScript>();
        ball.transform.localScale = new Vector3(GameConstants.ballRadius * 2, GameConstants.ballRadius * 2, GameConstants.ballRadius * 2);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
