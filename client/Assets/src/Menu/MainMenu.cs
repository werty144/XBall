using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;


using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using log4net;
using Steamworks;


using static GameManager;
using static ServerManager;
using static LobbyManager;


public class MainMenu : MonoBehaviour
{
    public static readonly ILog Log = LogManager.GetLogger(typeof(MainMenu));
    public static void prepareGame(GameState state, string side)
    {
        OnLeave();
        GameManager.prepareGame(state, side);
        SceneManager.LoadScene("GameScene");
        // RequestCreator.readyRequest();
    }

    private static void OnLeave()
    {
        LobbyManager.setReadyFalse();
    }

    public static void test()
    {
        // var texture = SteamFriendsManager.getAvatar((ulong)SteamAuth.GetSteamID());
        // if (texture != null)
        // {
        //     var image = GameObject.Find("Canvas/TestImage").GetComponent<RawImage>();
        //     image.texture = texture;
        // }
        var friendsList = GameObject.Find("Canvas/MainMenu/FriendsList").GetComponent<FriendsListViewController>();
        friendsList.addFriend((ulong)SteamAuth.GetSteamID());
    }

    public void OnExit()
    {
        Log.Info("Exiting");
        Application.Quit();
    }
}