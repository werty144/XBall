using System.Collections;
using System.Collections.Generic;


using UnityEngine;


using static GameManager;
using static GameConstants;


public class GameEntities : MonoBehaviour
{
    public static List<GameObject> myPlayers;
    public GameObject ball;
    public GameObject blueSkin;
    public GameObject redSkin;

    void Start()
    {
        initPlayers();
        initBall();
    }

    void initPlayers()
    {
        myPlayers = new List<GameObject>();
        for (int i = 0; i < GameManager.state.players.Count; i++) 
        {
            GameObject player;
            if (GameManager.state.players[i].side == GameManager.side)
            {
                player = (GameObject) Instantiate(blueSkin, new Vector3(0, 0, 0), Quaternion.identity);
                myPlayers.Add(player);
            } else
            {
                player = (GameObject) Instantiate(redSkin, new Vector3(0, 0, 0), Quaternion.identity);
            }

            player.tag = "Player";
            player.GetComponent<PlayerController>().id = GameManager.state.players[i].id;
        }
    }

    void initBall()
    {
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<BallController>();
        ball.transform.localScale = new Vector3(GameConstants.ballRadius * 2, GameConstants.ballRadius * 2, GameConstants.ballRadius * 2);
    }
}
