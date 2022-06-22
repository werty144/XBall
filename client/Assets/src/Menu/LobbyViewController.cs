using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;


using static LobbyManager;


public class LobbyViewController : MonoBehaviour
{
    [SerializeField] private GameObject memberBarPrefab;
    // In the future can use script on prefab if needed
    private Color32 green = new Color32(72, 236, 70, 225);
    private Color32 red = new Color32(224, 0, 26, 225);
    private GameObject infoText;
    private GameObject content;
    private int playersNumber = 3;
    private string speed = "FAST";


    void Start()
    {
        infoText = GameObject.Find("InfoText");
        content = GameObject.Find("Scroll View/Viewport/Content");

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

    public void OnLobbyCreate()
    {
        LobbyManager.createLobby(getMetaData());
    }

    public void OnInvite()
    {
        LobbyManager.inviteToLobby();
    }

    public void processPlayerNumberSelection(int newOption)
    {
        if (newOption == 0)
        {
            playersNumber = 3;
        } else 
        {
            playersNumber = newOption;
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
        clean();

        infoText.GetComponent<Text>().text = string.Format("Speed: {0}\nPlayers number: {1}", lobbyData.metaData.speed, lobbyData.metaData.playersNumber);

        foreach (var member in lobbyData.membersData)
        {
            var memberBar = Instantiate(memberBarPrefab);
            memberBar.transform.Find("Name").GetComponent<Text>().text = member.name;
            if (member.isReady)
            {
                memberBar.transform.Find("ReadyPanel").GetComponent<Image>().color = green;
            } else 
            {
                memberBar.transform.Find("ReadyPanel").GetComponent<Image>().color = red;
            }

            memberBar.transform.SetParent(content.transform);
        }
    }

    public void leaveLobby()
    {
        clean();
        LobbyManager.leaveLobby();
    }

    public void OnReadyButton()
    {
        LobbyManager.lobbyChangeReady();
    }

    private void clean()
    {
        infoText.GetComponent<Text>().text = "Info:";

        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
