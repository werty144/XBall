using System.Collections;
using System.Collections.Generic;


using UnityEngine;
using Steamworks;
using log4net;


public class SteamFriendsManager : MonoBehaviour
{
    private static Dictionary<ulong, Texture2D> usersImages = new Dictionary<ulong, Texture2D>();
    private static Dictionary<ulong, string> usersNames = new Dictionary<ulong, string>();
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
        usersImages[pCallback.m_steamID.m_SteamID] = image;
    }
    
    public static Texture2D getAvatar(ulong userID)
    {
        
        if (usersImages.ContainsKey(userID))
        {
            return usersImages[userID];
        }

        int imageID = SteamFriends.GetLargeFriendAvatar(new CSteamID(userID));
        if (imageID == 0)
        {
            // no avatar for user
            return null;
        }

        if (imageID == -1)
        {
            // not loaded yet
            return null;
        }
        
        var image = Utils.GetSteamImageAsTexture2D(imageID);
        usersImages[userID] = image;
        return image;
    }

    public static string getName(ulong userID)
    {
        
        if (usersNames.ContainsKey(userID))
        {
            return usersNames[userID];
        }

        var name = SteamFriends.GetFriendPersonaName(new CSteamID(userID));
        usersNames[userID] = name;

        return name;
    }

    public static List<ulong> getFriends()
    {
        var friendsIDs = new List<ulong>();
        int nFriends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        for (int i = 0; i < nFriends; i++)
        {
            var friendID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            friendsIDs.Add(friendID.m_SteamID);
        }
        return friendsIDs;
    }

    public static List<ulong> getFriendsOnline()
    {
        var friendsIDs = new List<ulong>();
        foreach (ulong userID in getFriends())
        {
            if (SteamFriends.GetFriendPersonaState(new CSteamID(userID)) == EPersonaState.k_EPersonaStateOnline)
            {
                friendsIDs.Add(userID);
            }
        }
        return friendsIDs;
    }
}
