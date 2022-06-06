using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using static MainMenu;


public class LobbyViewContoller : MonoBehaviour
{
    [SerializeField] private GameObject memberBarPrefab;
    private bool ready = false;
    private Color32 green = new Color32(72, 236, 70, 225);
    private Color32 red = new Color32(224, 0, 26, 225);
    private GameObject infoText;
    private GameObject content;


    void Start()
    {
        infoText = GameObject.Find("InfoText");
        content = GameObject.Find("Scroll View/Viewport/Content");
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
        MainMenu.leaveLobby();
    }

    public void OnReadyButton()
    {
        ready = !ready;
        MainMenu.setLobbyReady(ready);
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
