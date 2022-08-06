using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using Steamworks;
using log4net;


public class SteamFriendsManager : MonoBehaviour
{
    private static Dictionary<CSteamID, Texture2D> usersImages = new Dictionary<CSteamID, Texture2D>();
    public static readonly ILog Log = LogManager.GetLogger(typeof(SteamFriendsManager));


    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        }
    }

    private static void OnAvatarImageLoaded(AvatarImageLoaded_t pCallback)
    {
        var image = Utils.GetSteamImageAsTexture2D(pCallback.m_iImage);
        usersImages[pCallback.m_steamID] = image;
    }
    
    public static Texture2D getAvatar(ulong userID_)
    {
        var userID = new CSteamID(userID_);
        if (usersImages.ContainsKey(userID))
        {
            return usersImages[userID];
        }

        int imageID = SteamFriends.GetLargeFriendAvatar(userID);
        if (imageID == 0)
        {
            Log.Debug("no avatar");
            // no avatar for user
            return null;
        }

        if (imageID == -1)
        {
            Log.Debug("not loaded yet");
            // not loaded yet
            return null;
        }
        
        var image = Utils.GetSteamImageAsTexture2D(imageID);
        usersImages[userID] = image;
        return image;
    }

    public static string getName(ulong userID_)
    {
        var userID = new CSteamID(userID_);
        return SteamFriends.GetFriendPersonaName(userID);
    }
}
