using System.Collections;
using System.Collections.Generic;
using System;


using UnityEngine;


using static RequestCreator;
using static GameEntities;


public class InputProcessor : MonoBehaviour
{
    static GameObject selectedPlayer = null; // instead of GameObject, could use custom type like ControllableUnit

    static bool wantThrow = false;
    static bool wantOrient = false;
    static bool wantBend = false;
    static CameraController cameraController = null;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    public static void selectPlayer(GameObject player)
    {
        unselectPlayer();

        selectedPlayer = player;
        selectedPlayer.GetComponent<PlayerController>().SetHighlight();
    }

    public static void selectPlayer(int playerId)
    {
        if (playerId > GameEntities.myPlayers.Count)
        	return;

        unselectPlayer();

        selectedPlayer = GameEntities.myPlayers[playerId - 1];
        selectedPlayer.GetComponent<PlayerController>().SetHighlight();
    }

    public static void fieldLeftClick(Vector3 point)
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        Point point2D = new Point();
        point2D.x = point.x;
        point2D.y = point.z;

        if (!anyIntention()) 
        {
            unselectPlayer();
        }

        if (wantThrow)
        {
            RequestCreator.throwRequest(selectedPlayer, point2D);
            wantThrow = false;
        }

        if (wantOrient)
        {
            RequestCreator.orientationRequest(selectedPlayer, point2D);
            wantOrient = false;
        }

        if (wantBend)
        {
            RequestCreator.bendRequest(selectedPlayer, point2D);
            wantBend = false;
        }
    }

    public static void unselectPlayer()
    {
        if (selectedPlayer != null)
        {
            selectedPlayer.GetComponent<PlayerController>().ResetHighlight();
        }
        selectedPlayer = null;
    }

    public static void fieldRightClick(Vector3 point)
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        Point point2D = new Point();
        point2D.x = point.x;
        point2D.y = point.z;
        RequestCreator.moveRequest(selectedPlayer, point2D);
    }

    public static void grab()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        RequestCreator.grabRequest(selectedPlayer);
    }

    public static void throwIntention()
    {
        cancelIntentions();
        wantThrow = true;
    }

    public static void turnIntention()
    {
        cancelIntentions();
        wantOrient = true;
    }

    public static void bendIntention()
    {
        cancelIntentions();
        wantBend = true;
    }

    public static void cancelIntentions()
    {
        wantThrow = false;
        wantOrient = false;
        wantBend = false;
    }

    public static bool anyIntention()
    {
        return wantBend || wantOrient || wantThrow;
    }

    public static void attack()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        RequestCreator.attackRequest(selectedPlayer);
    }

    public static void stop()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        RequestCreator.stopRequest(selectedPlayer);
    }

    public static void processMousePosition(Vector3 mousePosition)
    {
        if (mousePosition.y >= Screen.height * 0.95)
        {
            cameraController.setMovingDirection(Vector3.forward);
        }
        if (mousePosition.y <= Screen.height * 0.05)
        {
            cameraController.setMovingDirection(Vector3.back);
        }
    }

    public static void switchPlayer()
    {
        if (selectedPlayer != null)
        {
            var currentSelectedPlayerIndex = GameEntities.myPlayers.IndexOf(selectedPlayer);
            var playersNumber = GameEntities.myPlayers.Count;
            var newPlayerToSelect = GameEntities.myPlayers[(currentSelectedPlayerIndex + 1) % playersNumber];
            unselectPlayer();
            selectPlayer(newPlayerToSelect);
        } else
        {
            selectPlayer(GameEntities.myPlayers[0]);
        }
    }
}
