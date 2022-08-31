using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyBarController : MonoBehaviour
{
    public ulong? UserID;
    private bool nameExists = false;
    private bool avatarExists = false;

    void Update()
    {
        if (UserID == null) return;
        if (!nameExists)
        {
            var name = SteamFriendsManager.getName((ulong)UserID);
            setName(name);
        }

        if (!avatarExists)
        {
            var avatar = SteamFriendsManager.getAvatar((ulong)UserID);
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
        if (avatar != null)
        {
            avatarExists = true;
        }
    }
}
