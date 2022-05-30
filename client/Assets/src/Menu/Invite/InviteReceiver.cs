using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static MainMenu;


public class InviteReceiver : MonoBehaviour
{
    [SerializeField] private GameObject inviteBarPrefab;

    private static List<Invite> invites = new List<Invite>();
    private static bool invitesUpdate = false;

    // Update is called once per frame
    void Update()
    {
        if (!invitesUpdate) return;

        invitesUpdate = false;
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Invite invite in invites)
        {
            var inviteBar = Instantiate(inviteBarPrefab);
            InviteBar inviteBarInfo = inviteBar.GetComponent<InviteBar>();
            inviteBarInfo.inviteId = invite.inviteId;
            var text = inviteBarInfo.transform.Find("Text");
            text.GetComponent<Text>().text = "Invite from " + invite.inviterId.ToString();

            inviteBar.transform.SetParent(this.transform);
        }
    }

    public static void receiveInvite(Invite invite)
    {
        invitesUpdate = true;
        invites.Add(invite);
    }

    public static void acceptInvite(int inviteId)
    {
        invites.RemoveAll(invite => invite.inviteId == inviteId);
        invitesUpdate = true;
        MainMenu.acceptInvite(inviteId);
    }

    public static void declineInvite(int inviteId)
    {
        invites.RemoveAll(invite => invite.inviteId == inviteId);
        invitesUpdate = true;
    }
}
