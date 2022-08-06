using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsListViewController : MonoBehaviour
{
    [SerializeField] private GameObject friendBarPrefab;
    private Transform content;

    void Start()
    {
        content = this.gameObject.transform.Find("Scroll View/Viewport/Content");
    }
    
    public void addFriend(ulong userID)
    {
        var friendBar = Instantiate(friendBarPrefab);
        friendBar.GetComponent<FriendBarController>().friendSteamID = userID;
        friendBar.transform.SetParent(content, false);
    }
}
