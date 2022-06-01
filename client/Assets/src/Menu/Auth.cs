using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;


using static SteamAuth;
using static SocketConnection;
using static MainMenu;

public class Auth : MonoBehaviour
{
    static private HttpClient client = new HttpClient();
    static string serverURL = "http://localhost:8080";

    // Start is called before the first frame update
    void Start()
    {
        Authenticate();
    }

    async void Authenticate()
    {
        SteamAuth.Authenticate();

        int timeSpent = 0;
        while (!SteamAuth.ticketReady)
        {
            await Task.Delay(25);
            timeSpent = timeSpent + 25;

            if (timeSpent > 5000)
            {
                Debug.Log("Can't get steam ticekt");
                return;
            }
        }

        var password = SendTicketGetKey(SteamAuth.ticket);

        if (password != null)
        {
            MainMenu.myId = long.Parse(password);
            var connection = GameObject.Find("Connection");
            connection.GetComponent<SocketConnection>().StartConnection(password);
        } 
    }

    public static string SendTicketGetKey(string ticket)
    {
        var credentials = new Credentials();
        credentials.ticket = ticket;
        string json = JsonConvert.SerializeObject(credentials);
        var response = client.PostAsync
        (
            serverURL + "/auth", 
            new StringContent(json, Encoding.UTF8, "application/json")
        ).Result;

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
        return null;
    }
}


class Credentials
{
    public string ticket;
}
