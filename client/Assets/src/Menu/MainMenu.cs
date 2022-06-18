using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;


using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;


using static SocketConnection;
using static SteamAuth;
using static LobbyViewContoller;
using static GameManager;
using static SteamLobby;
using static SteamP2P;


public class MainMenu : MonoBehaviour
{


    void Start()
    {
    }

    public static void prepareGame(GameState state, Side side)
    {
        OnLeave();
        GameManager.prepareGame(state, side);
    }

    private static void OnLeave()
    {
        LobbyManager.setReadyFalse();
    }

    public static void lobbyReady(ulong lobbyID, int nMembers, string speed, int playersNumber)
    {
        string request = JsonConvert.SerializeObject(
            new 
            {
                path = "lobbyReady",
                body = new 
                {
                    lobbyID = lobbyID,
                    nMembers = nMembers,
                    gameProperties = new
                    {
                        speed = speed,
                        playersNumber = playersNumber
                    }
                }
            }
            );
        SocketConnection.messages.Enqueue(request);
    }

    public static void test()
    {
        foreach (var member in SteamLobby.getOtherMembers())
        {
            SteamP2P.receiveMessages(1);
        }
    }

    public static void log(string log)
    {
        var logger = GameObject.Find("LoggerContent");
        if (logger != null)
        {   
            logger.GetComponent<LoggerViewController>().addLog(log);
        }
    }

    public void OnExit()
    {
        print("Exiting");
        Application.Quit();
    }
}