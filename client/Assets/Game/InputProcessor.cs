using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static SocketConnection;
using static RequestCreator;


public class InputProcessor
{
    GameObject selectedPlayer = null; // instead of GameObject, could use custom type like ControllableUnit
    RequestCreator requestCreator = new RequestCreator();

    bool wantThrow = false;
    bool wantOrient = false;
    CameraController cameraController = Camera.main.GetComponent<CameraController>();

    public void selectPlayer(GameObject player)
    {
        removeHighlightIfPresent();

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

        if (!wantThrow && !wantOrient) 
        {
            removeHighlightIfPresent();

            selectedPlayer = null;
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
    }

    public void removeHighlightIfPresent()
    {
        if (selectedPlayer != null)
        {
            selectedPlayer.GetComponent<PlayerScript>().ResetHighlight();
        }
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

    public void cancelIntentions()
    {
        wantThrow = false;
        wantOrient = false;
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
}
