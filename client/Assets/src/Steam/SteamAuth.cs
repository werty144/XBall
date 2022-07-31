using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


using UnityEngine;
using Steamworks;
using Newtonsoft.Json;
using log4net;


public class SteamAuth : MonoBehaviour
{
    protected Callback<GetAuthSessionTicketResponse_t> ticketResponse;
	protected static byte[] ticketBytes = new byte[1024];
	protected static HAuthTicket ticketHandle;
	protected static UInt32 ticketSize;
	protected static ulong SteamID;
	protected static bool SteamIDInitialized = false;
	public static bool ticketReady = false;
	public static string ticket;
	public static readonly ILog Log = LogManager.GetLogger(typeof(SteamAuth));


	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			ticketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnTicketResponse);
		}
	}

	void Start()
	{
		GetSteamID();
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
		if (SteamIDInitialized)
		{
			return SteamID;
		} else
		{
			if (SteamManager.Initialized)
			{
				SteamID = SteamUser.GetSteamID().m_SteamID;
				SteamIDInitialized = true;
				return SteamID;
			} else 
			{
				return null;
			}
		}	
	}

	public static bool isMe(ulong ID)
	{
		return ID == GetSteamID();
	}

	private void OnApplicationQuit()
	{
		if (SteamManager.Initialized) 
		{
			SteamUser.CancelAuthTicket(ticketHandle);
		}
	}
}


