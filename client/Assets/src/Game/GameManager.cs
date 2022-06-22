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


public class GameInfo
{
	public GameState state;
	public string score;
	public long time;
	public string status;
}

public class GameProperties 
{
	public int playersNumber;
	public string speed;
}

public class GameState
{
	public List<Player> players;
	public BallState ballState;
}

public class Point
{
	public float x;
	public float y;
}

public class Player
{
	public int id;
	public Side side;
	public PlayerState state;
}

public class PlayerState
{
	public float x;
	public float y;
	public float z;
	public float rotationAngle;
	}

public class BallState
{
	public int? ownerId;
	public float x;
	public float y;
	public float z;
}

public class Vector
{
	public float x;
	public float y;
}

public enum Side
{
	LEFT,
	RIGHT
}