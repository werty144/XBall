using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static InviteReceiver;

public class InviteBar : MonoBehaviour
{
    [SerializeField] public int inviteId;


    public void acceptInvite()
    {
        InviteReceiver.acceptInvite(inviteId);
    }

    public void declineInvite()
    {
        InviteReceiver.declineInvite(inviteId);
    }
}
