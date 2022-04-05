using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameManager;
using static GameConstants;


public class GameEntities : MonoBehaviour
{
    public List<GameObject> myPlayers = new List<GameObject>();
    public GameObject ball;
    public GameObject blueSkin;
    public GameObject redSkin;
    // Start is called before the first frame update
    void Start()
    {
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

            // player.transform.localScale = new Vector3(20 * GameConstants.playerRadius, 10F, 20 * GameConstants.playerRadius);
            player.tag = "Player";
            player.AddComponent<PlayerController>();
            player.GetComponent<PlayerController>().id = GameManager.state.players[i].id;
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
