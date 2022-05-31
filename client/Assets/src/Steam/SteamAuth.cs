using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using System.Linq;
using Newtonsoft.Json;


using static MainMenu;


public class SteamAuth : MonoBehaviour
{
    protected Callback<GetAuthSessionTicketResponse_t> ticketResponse;
	protected static byte[] ticket = new byte[1024];
	protected static HAuthTicket ticketHandle;
	protected static UInt32 ticketSize;


	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			ticketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnTicketResponse);
		}
	}

	private static void OnTicketResponse(GetAuthSessionTicketResponse_t pCallback)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK) // 1 is OK code
		{
			ticket = ticket.Take((int)ticketSize).ToArray();
			string ticketHexString = BitConverter.ToString(ticket).Replace("-","");
			MainMenu.authenticate(ticketHexString);
		}

	}
    
	private static void getTicket() 
	{
		if(SteamManager.Initialized) 
		{
			ticketHandle = SteamUser.GetAuthSessionTicket(ticket, 1024, out ticketSize);
		}
	}

	public static void authenticate()
	{
		getTicket();
	}

	private void OnApplicationQuit()
	{
		SteamUser.CancelAuthTicket(ticketHandle);
	}
}


