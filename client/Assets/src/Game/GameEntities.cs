using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameManager;
using static GameConstants;


public class GameEntities : MonoBehaviour
{
    public List<GameObject> myPlayers = new List<GameObject>();
    public GameObject ball;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameManager.state.players.Count; i++) 
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            player.transform.localScale = new Vector3(2 * GameConstants.playerRadius, 1F, 2 * GameConstants.playerRadius);
            player.tag = "Player";
            player.AddComponent<PlayerController>();
            player.GetComponent<PlayerController>().id = GameManager.state.players[i].id;
            player.GetComponent<PlayerController>().userId = GameManager.state.players[i].userId;
            if (player.GetComponent<PlayerController>().userId == 0)
            {
                myPlayers.Add(player);
                player.GetComponent<Renderer>().material.color = Color.blue;
            } else
            {
                player.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<BallController>();
        ball.transform.localScale = new Vector3(GameConstants.ballRadius * 2, GameConstants.ballRadius * 2, GameConstants.ballRadius * 2);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
