using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager 
{
    public static GameState state;
    public static long time;
    public static string score;
	public static Side side;


    public static void setGameInfo(GameInfo gameInfo)
    {
        state = gameInfo.state;
        time = gameInfo.time;
        score = gameInfo.score;
    }

    public static void prepareGame(GameState state_, Side side_)
    {
		side = side_;
		state = state_;
    }
}
