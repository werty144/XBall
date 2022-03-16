using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using static MainMenu;

public class InviteManager : MonoBehaviour
{
    int playersNumber = 3;
    int? invitedId = null;
    string speed = "FAST";

    public void processPlayerNumberSelection(int newOption)
    {
        Debug.Log("Entered player number");
        if (newOption == 0)
        {
            playersNumber = 3;
        } else 
        {
            playersNumber = newOption;
        }
    }

    public void processIdInput(string id)
    {
        invitedId = Int32.Parse(id);
    }

    public void processSpeedSelection(int newOption)
    {
        switch (newOption)
        {
            case 0:
                speed = "FAST";
                break;
            case 1:
                speed = "SLOW";
                break;
            case 2:
                speed = "NORMAL";
                break;
            default:
                break;
        }
    }

    public void sendInvite()
    {
        if (invitedId == null)
        {
            return;
        }

        MainMenu.sendInvite((int) invitedId, speed, playersNumber);
    }
}
