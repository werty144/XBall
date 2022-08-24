using System.Collections.Generic;


using MessagePack;


[MessagePackObject(keyAsPropertyName: true)]
public class ApiGameInfo
{
  public string path;
  public GameInfo body;
}

[MessagePackObject(keyAsPropertyName: true)]
public class ApiPrepareGame
{
	public string path;
	public PrepareGameBody body;
}

[MessagePackObject(keyAsPropertyName: true)]
public class PrepareGameBody
{
	public string side;
	public GameInfo game;
}

[MessagePackObject(keyAsPropertyName: true)]
public class ApiServerReady
{
	public string path;
	public int port;
}

[MessagePackObject(keyAsPropertyName: true)]
public class GameInfo
{
	public GameState state;
	public string score;
	public long time;
	public string status;
}

[MessagePackObject(keyAsPropertyName: true)]
public class GameProperties 
{
	public int playersNumber;
	public string speed;
}

[MessagePackObject(keyAsPropertyName: true)]
public class GameState
{
	public List<Player> players;
	public BallState ballState;
}

[MessagePackObject(keyAsPropertyName: true)]
public class Point
{
	public float x;
	public float y;
}

[MessagePackObject(keyAsPropertyName: true)]
public class Player
{
	public int id;
	public string side;
	public PlayerState state;
}

[MessagePackObject(keyAsPropertyName: true)]
public class PlayerState
{
	public float x;
	public float y;
	public float z;
	public float rotationAngle;
	}

[MessagePackObject(keyAsPropertyName: true)]
public class BallState
{
	public int? ownerId;
	public float x;
	public float y;
	public float z;
}

[MessagePackObject(keyAsPropertyName: true)]
public class Vector
{
	public float x;
	public float y;
}

public class Side
{
	public static string LEFT = "LEFT";
	public static string RIGHT = "RIGHT";
}