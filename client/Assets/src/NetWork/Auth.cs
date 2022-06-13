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
using System.Security.Cryptography;


using static SteamAuth;
using static SocketConnection;
using static MainMenu;
using static Constants;


public class Auth : MonoBehaviour
{
    static private HttpClient client = new HttpClient();
    static string serverURL = "http://" + Constants.serverURL;

    // Start is called before the first frame update
    async Task Start()
    {
        var vs = GameObject.Find("Global/Connection").GetComponent<SocketConnection>();
        if (!vs.isOpen()) {
            await Authenticate();
        }
    }

    async Task Authenticate()
    {
        SteamAuth.Authenticate();

        int timeSpent = 0;
        while (!SteamAuth.ticketReady)
        {
            await Task.Delay(25);
            timeSpent = timeSpent + 25;

            if (timeSpent > 5000)
            {
                Debug.Log("Can't get steam ticket");
                return;
            }
        }
        var password = await SendTicketGetKey(SteamAuth.ticket);

        if (password != null)
        {
            var connection = GameObject.Find("Connection");
            await connection.GetComponent<SocketConnection>().StartConnection(password);
        } 
    }

    public static async Task<string> SendTicketGetKey(string ticket)
    {
        var credentials = new Credentials();
        credentials.ticket = ticket;

        RSA rsa = RSA.Create();
        string publicKey = rsa.ToXmlString(false);
        credentials.publicKey = publicKey;
        
        string json = JsonConvert.SerializeObject(credentials);
        var response= await client.PostAsync
        (
            serverURL + "/auth", 
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        if (response.StatusCode == HttpStatusCode.OK)
        {
            byte[] encodedPassword = response.Content.ReadAsByteArrayAsync().Result;
            byte[] decodedPassword = rsa.Decrypt(encodedPassword, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(decodedPassword);
        }
        return null;
    }
}


class Credentials
{
    public string ticket;
    public string publicKey;
}
