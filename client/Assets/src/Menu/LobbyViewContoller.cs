using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyViewContoller : MonoBehaviour
{
    [SerializeField] private GameObject userBarPrefab;

    public void OnLobbyUpdate(List<string> usersInLobby)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var name in usersInLobby)
        {
            var userBar = Instantiate(userBarPrefab);
            userBar.GetComponent<Text>().text = name;
            userBar.transform.SetParent(this.transform);
        }
    }
}
