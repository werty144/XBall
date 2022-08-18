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
        InvokeRepeating("updateList", 0.0f, 1.0f);
    }
    
    public void addFriend(ulong userID)
    {
        var friendBar = Instantiate(friendBarPrefab);
        friendBar.GetComponent<FriendBarController>().friendSteamID = userID;
        friendBar.transform.SetParent(content, false);
    }

    public void removeFriend(ulong userID)
    {
        foreach (Transform child in content.transform)
        {
            if (child.gameObject.GetComponent<FriendBarController>().friendSteamID == userID)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public bool containsFriend(ulong userID)
    {
        foreach (Transform child in content.transform)
        {
            if (child.gameObject.GetComponent<FriendBarController>().friendSteamID == userID)
            {
                return true;
            }
        }
        return false;
    }

    public void clean()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void updateList()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        var currentOnline = SteamFriendsManager.getFriendsOnline();

        foreach (Transform child in content.transform)
        {
            var userID = child.gameObject.GetComponent<FriendBarController>().friendSteamID;
            if (!currentOnline.Contains(userID))
            {
                removeFriend(userID);
            }
        }

        foreach (ulong userID in currentOnline)
        {
            if (!containsFriend(userID))
            {
                addFriend(userID);
            }
        }
    }
}
