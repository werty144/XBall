using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks;
using System.Linq;
using Newtonsoft.Json;


public class SteamAuth : MonoBehaviour
{
    protected Callback<GetAuthSessionTicketResponse_t> ticketResponse;
	protected static byte[] ticketBytes = new byte[1024];
	protected static HAuthTicket ticketHandle;
	protected static UInt32 ticketSize;
	public static bool ticketReady = false;
	public static string ticket;


	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			ticketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnTicketResponse);
		}
	}

	private static void OnTicketResponse(GetAuthSessionTicketResponse_t pCallback)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			ticketBytes = ticketBytes.Take((int)ticketSize).ToArray();
			ticket = BitConverter.ToString(ticketBytes).Replace("-","");
			ticketReady = true;
		}

	}
    
	private static void getTicket() 
	{
		if (SteamManager.Initialized) 
		{
			ticketHandle = SteamUser.GetAuthSessionTicket(ticketBytes, 1024, out ticketSize);
		}
	}

	public static void Authenticate()
	{
		getTicket();
	}

	public static ulong? GetSteamID()
	{
		if (SteamManager.Initialized) 
		{
			return SteamUser.GetSteamID().m_SteamID;
		}
		return null;
	}

	public static bool isMe(CSteamID ID)
	{
		return ID == SteamUser.GetSteamID();
	}

	public static bool isMe(ulong ID)
	{
		return isMe(new CSteamID(ID));
	}

	private void OnApplicationQuit()
	{
		if (SteamManager.Initialized) 
		{
			SteamUser.CancelAuthTicket(ticketHandle);
		}
	}
}


