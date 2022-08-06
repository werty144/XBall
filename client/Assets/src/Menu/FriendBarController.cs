using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using UnityEngine.UI;

public class FriendBarController : MonoBehaviour
{
    public ulong friendSteamID;
    private bool nameExists = false;
    private bool avatarExists = false;

    void Update()
    {
        if (!nameExists)
        {
            var name = SteamFriendsManager.getName(friendSteamID);
            setName(name);
        }

        if (!avatarExists)
        {
            var avatar = SteamFriendsManager.getAvatar(friendSteamID);
            setAvatar(avatar);
        }
    }

    void setName(string name)
    {
        var nameText = this.gameObject.transform.Find("Name").GetComponent<Text>();
        nameText.text = name;

        nameExists = true;
    }

    void setAvatar(Texture2D avatar)
    {
        var image = this.gameObject.transform.Find("Avatar").GetComponent<RawImage>();
        image.texture = avatar;
        avatarExists = true;
    }
}
