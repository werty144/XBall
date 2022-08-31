using System;
using System.Linq;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;


using static LobbyManager;


public class LobbyViewController : MonoBehaviour
{
    [SerializeField] private GameObject MyInLobbyBarPrefab;
    [SerializeField] private GameObject OpponentsInLobbyBarPrefab;
    [SerializeField] private GameObject gameParamsSwitchers;
    [SerializeField] private GameObject gameParamsJustInfo;
    [SerializeField] private GameObject ReadyButton;
    [SerializeField] private GameObject ReadyLabelImage;
    [SerializeField] private GameObject ReadyStatus;
    [SerializeField] private GameObject playerNumberSwitch;
    [SerializeField] private GameObject speedSwitch;

    private Color32 green = new Color32(72, 236, 70, 225);
    private Color32 red = new Color32(224, 0, 26, 225);
    private Color32 white = new Color32(255, 255, 255, 225);
    private int playersNumber = 3;
    private string speed = "FAST";


    void Start()
    {
        var lobbyData = LobbyManager.getLobbyData();
        if (lobbyData != null)
        {
            OnLobbyUpdate(lobbyData);
        }
    }

    LobbyMetaData getMetaData()
    {
        LobbyMetaData metaData = new LobbyMetaData();
        metaData.speed = speed;
        metaData.playersNumber = playersNumber;
        return metaData;
    }

    public void processPlayerNumberSelection(int newOption)
    {
        switch (newOption)
        {
            case 0:
                playersNumber = 3;
                break;
            case 1:
                playersNumber = 2;
                break;
            case 2:
                playersNumber = 1;
                break;
            default:
                break;
        }
        LobbyManager.setMetaData(getMetaData());
    }

    public void processSpeedSelection(int newOption)
    {
        switch (newOption)
        {
            case 0:
                speed = "FAST";
                break;
            case 1:
                speed = "NORMAL";
                break;
            case 2:
                speed = "SLOW";
                break;
            default:
                break;
        }
        LobbyManager.setMetaData(getMetaData());
    }

    public void OnLobbyUpdate(LobbyData lobbyData)
    {
        manageOpponent(lobbyData);
        manageGameParams(lobbyData);
        manageProfiles(lobbyData);
        manageReady(lobbyData);
    }

    ulong? getOpponentID(LobbyData lobbyData)
    {
        ulong myID = (ulong)SteamAuth.GetSteamID();
        ulong? opponentID = null;
        foreach (ulong id in lobbyData.membersData.Select( m => m.ID))
        {
            if (id != myID)
            {
                opponentID = id;
            }
        }
        return opponentID;
    }

    void manageOpponent(LobbyData lobbyData)
    {
        ulong? opponentID = getOpponentID(lobbyData);
        OpponentsInLobbyBarPrefab.SetActive(opponentID != null);
        ReadyStatus.SetActive(opponentID != null);
    }

    void manageGameParams(LobbyData lobbyData)
    {
        bool iAmOwner = LobbyManager.IAmLobbyOwner();
        gameParamsSwitchers.SetActive(!iAmOwner);
        gameParamsSwitchers.SetActive(iAmOwner);

        setDropdownValue(playerNumberSwitch, lobbyData.metaData.playersNumber.ToString());
        setDropdownValue(speedSwitch, lobbyData.metaData.speed);
    }

    void setDropdownValue(GameObject switch_, string newValue)
    {
        var dropDown = switch_.GetComponent<Dropdown>();
        int desiredIndex = dropDown.options.FindIndex(option => option.text.ToLower() == newValue.ToLower());
        if (dropDown.value != desiredIndex)
        {
            dropDown.value = desiredIndex;
        }
    }

    void manageProfiles(LobbyData lobbyData)
    {
        ulong myID = (ulong)SteamAuth.GetSteamID();
        MyInLobbyBarPrefab.GetComponent<PlayerLobbyBarController>().UserID = myID;

        ulong? opponentID = getOpponentID(lobbyData);

        OpponentsInLobbyBarPrefab.GetComponent<PlayerLobbyBarController>().UserID = opponentID;
    }

    void manageReady(LobbyData lobbyData)
    {
        ulong myID = (ulong)SteamAuth.GetSteamID();
        foreach (LobbyMemberData md in lobbyData.membersData)
        {
            GameObject backGround;
            if (md.ID == myID)
            {
                 backGround = ReadyButton;
            } else
            {
                backGround = ReadyLabelImage;
            }
            if (md.isReady)
            {
                backGround.GetComponent<Image>().color = green;
            } else 
            {
                backGround.GetComponent<Image>().color = white;
            }
        }
    }

    public void leaveLobby()
    {
        LobbyManager.leaveLobby();
    }

    public void OnReadyButton()
    {
        LobbyManager.lobbyChangeReady();
    }

    public void enable()
    {
        this.gameObject.SetActive(true);
    }

    public void disable()
    {
        this.gameObject.SetActive(false);
    }
}
