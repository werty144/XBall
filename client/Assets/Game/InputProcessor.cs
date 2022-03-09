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

        if (!wantThrow) 
        {
            removeHighlightIfPresent();

            selectedPlayer = null;
        }

        if (wantThrow)
        {
            requestCreator.throwRequest(selectedPlayer, point2D);
            wantThrow = false;
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
        wantThrow = true;
    }

    public void attack()
    {
        if (selectedPlayer == null) 
        {
            return;
        }

        requestCreator.attackRequest(selectedPlayer);
    }
}
