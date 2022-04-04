using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;

public class SteamTest : MonoBehaviour {
    protected Callback<GetAuthSessionTicketResponse_t> ticketResponse;

	private void OnEnable() {
		if (SteamManager.Initialized) {
			ticketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnTicketResponse);
		}
	}

	private void OnTicketResponse(GetAuthSessionTicketResponse_t pCallback)
    {
        Debug.Log(pCallback.m_hAuthTicket);

        Debug.Log(pCallback.m_eResult);
	}
    
	void Start() {
		if(SteamManager.Initialized) {
            byte[] ticket = new byte[1024];
            UInt32 size;

			UInt32 id = (UInt32) SteamUser.GetAuthSessionTicket(ticket, 1024, out size);
			Debug.Log(id);
		}
	}
}