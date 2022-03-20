using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using static SocketConnection;
using static RequestCreator;


public class InputProcessor
{
    GameObject selectedPlayer = null; // instead of GameObject, could use custom type like ControllableUnit
    RequestCreator requestCreator = new RequestCreator();

    bool wantThrow = false;
    bool wantOrient = false;
    bool wantBend = false;
    CameraController cameraController = Camera.main.GetComponent<CameraController>();
    GameEntities gameEntities = GameObject.FindWithTag("GameEntities").GetComponent<GameEntities>();

    public void selectPlayer(GameObject player)
    {
        unselectPlayer();

        selectedPlayer = player;
        selectedPlayer.GetComponent<PlayerScript>().SetHighlight();
    }

    public void fieldLeftClick(Vector3 point)
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        Point point2D = Utils.unityFieldPointToServerPoint(point);

        if (!anyIntention()) 
        {
            unselectPlayer();
        }

        if (wantThrow)
        {
            requestCreator.throwRequest(selectedPlayer, point2D);
            wantThrow = false;
        }

        if (wantOrient)
        {
            requestCreator.orientationRequest(selectedPlayer, point2D);
            wantOrient = false;
        }

        if (wantBend)
        {
            requestCreator.bendRequest(selectedPlayer, point2D);
            wantBend = false;
        }
    }

    public void unselectPlayer()
    {
        if (selectedPlayer != null)
        {
            selectedPlayer.GetComponent<PlayerScript>().ResetHighlight();
        }
        selectedPlayer = null;
    }

    public void fieldRightClick(Vector3 point)
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        
        Point point2D = Utils.unityFieldPointToServerPoint(point);
        requestCreator.moveRequest(selectedPlayer, point2D);
    }

    public void grab()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        requestCreator.grabRequest(selectedPlayer);
    }

    public void throwIntention()
    {
        cancelIntentions();
        wantThrow = true;
    }

    public void turnIntention()
    {
        cancelIntentions();
        wantOrient = true;
    }

    public void bendIntention()
    {
        cancelIntentions();
        wantBend = true;
    }

    public void cancelIntentions()
    {
        wantThrow = false;
        wantOrient = false;
        wantBend = false;
    }

    public bool anyIntention()
    {
        return wantBend || wantOrient || wantThrow;
    }

    public void attack()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        requestCreator.attackRequest(selectedPlayer);
    }

    public void stop()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        requestCreator.stopRequest(selectedPlayer);
    }

    public void processMousePosition(Vector3 mousePosition)
    {
        if (mousePosition.x >= Screen.width * 0.95)
        {
            cameraController.setMovingDirection(Vector3.right);
        }
        if (mousePosition.x <= Screen.width * 0.05)
        {
            cameraController.setMovingDirection(Vector3.left);
        }
    }

    public void switchPlayer()
    {
        if (selectedPlayer != null)
        {
            var currentSelectedPlayerIndex = gameEntities.myPlayers.IndexOf(selectedPlayer);
            var playersNumber = gameEntities.myPlayers.Count;
            var newPlayerToSelect = gameEntities.myPlayers[(currentSelectedPlayerIndex + 1) % playersNumber];
            unselectPlayer();
            selectPlayer(newPlayerToSelect);
        } else
        {
            selectPlayer(gameEntities.myPlayers[0]);
        }
    }
}
