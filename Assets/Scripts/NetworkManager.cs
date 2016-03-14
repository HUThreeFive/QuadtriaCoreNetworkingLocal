/*
	All scripts are writen in really primitive way. As a networking beginner you should understand all
	things in this code and therefore you should not have any problems with converting/editing this code 
	for your own needs...

	OneManGames 2014 - Moopey
	email : omgdevelopersgroup@gmail.com
*/

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[RequireComponent (typeof (NetworkView))]
public class NetworkManager : MonoBehaviour
{
	public string connectionIP = "localhost";	//Direct connect IP
	public int connectionPort = 25001;		//Network Port

    public int broadcastPort = 25035;
	public string playerName;
	[HideInInspector]
	public string tempName;							// Temp player name, prevent to access Host/Join menu before a player name will be typed 
	public string networkMessage;

	public bool isConnected;
	public bool isServer;
    public string tempIP = "";

    public UdpClient sender;
    public UdpClient receiver;
    private string[] receivedData;

    private GameManager _GameManager;

    private IPAddress hostIP;
    bool tryConnect = true;

	ChatManager mChat;										// Access to the chat script

	void Start()
	{
		DontDestroyOnLoad(this);
		mChat = GetComponent<ChatManager>();
		connectionIP = Network.player.ipAddress;

        GameObject gameManager = GameObject.Find ("Game Manager");
        _GameManager = gameManager.GetComponent<GameManager> ();
	}

	void Update()
	{
		NetworkMessageCheck();						// This will check if we are connected, disconnected, client or server and display a message depending on this

        if (tryConnect)
        {
            if (receivedData != null)
            {
                DirectConnect(receivedData[1].Trim());
                tryConnect = false;
            }
        }

        if (isServer)
        {
            if (Network.connections.Length > 0)
            {
                CancelInvoke("SendData");
                sender.Close();
            }
        }
    }

    void OnGUI()
	{
		GUI.Box(new Rect(5, 5, 250, 25), "");
		GUI.Label(new Rect(10, 10, 250, 25), networkMessage);		// Used to display text message from NetworkMessageCheck

		if(playerName == "")				// If playerName is none then show "enter player name" gui
		{
			GUI.BeginGroup(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 25, 200, 50), "");
			tempName = GUI.TextField(new Rect(0, 0, 190, 25), tempName);
			if(GUI.Button(new Rect(75, 25, 50, 25), "Next"))
				playerName = tempName;
			GUI.EndGroup();
		}
		else
		{
			if(!isConnected)				// If we are not connect to the server, show 2 options to choose from (create server or direct connect to one)
			{
				GUI.BeginGroup(new Rect((Screen.width / 2) - 100, (Screen.height / 2) - 25, 200, 50), "");
                tempIP = GUI.TextField(new Rect(0, 27, 190, 25), tempIP);
				if(GUI.Button(new Rect(0, 0, 95, 25), "Host"))
					CreateServer();
                if (GUI.Button(new Rect(100, 0, 95, 25), "Join"))
                {
                    if (tempIP != "")
                    {
                        connectionIP = tempIP;
                        DirectConnect(connectionIP);
                    }
                    else
                    {
                        StartReceivingIP();
                    }
                }
				GUI.EndGroup();
			}
			else
			{
				if(GUI.Button(new Rect(260, 5, 100, 25), "Disconnect"))
				{
					mChat.SendMessage("C*" + playerName + "*disconnected!");
					Network.Disconnect();		// Disconnect from the network
				}
			}
		}
	}
	void NetworkMessageCheck()
	{
        NetworkPeerType type = Network.peerType;
		if(type == NetworkPeerType.Disconnected)
		{
			networkMessage = "Status: Disconnected from the server";
			isConnected = false;

//            if (receivedData != null)
//            {
//                DirectConnect(receivedData[1]);
//            }
		}
		else if(Network.peerType == NetworkPeerType.Connecting)
		{
			networkMessage = "Status: Connecting the server...";
		}
		else if(Network.peerType == NetworkPeerType.Client)
		{
			networkMessage = "Status: Client - " + connectionIP;

			isConnected = true;
            _GameManager.hasGameStarted = true;
            _GameManager.isPlayerFirst = false;
		}
		else if(Network.peerType == NetworkPeerType.Server)
		{
			networkMessage = "Status: Server - " + connectionIP;
			isConnected = true;
            _GameManager.hasGameStarted = true;
            _GameManager.isPlayerFirst = true;
		}
	}

	void OnConnectedToServer()
	{
		mChat.SendMessage("C*" + playerName + "*joined!");		// Add message to the chat when player connect
	}

	void OnServerInitialized()
	{
        mChat.SendMessage("C*" + playerName + "*joined!");		// Add message to the chat when player connect
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		mChat.messages.Clear();												// Clear messages history
        receivedData = null;

        if (isServer)
        {
            Network.CloseConnection(Network.connections[0], false);
            Debug.Log("Local server connection disconnected");
        }
        else
        {
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
        }

        _GameManager.NewGame();
	}

    void OnPlayerConnected(NetworkPlayer player)
    {
        CancelInvoke("SendData");
        sender.Close();
    }

    void OnPlayDisconnected(NetworkPlayer player)
    {
        Network.RemoveRPCs(player);
        Network.Disconnect();
    }

	public void CreateServer()																// Create simple local server
	{
        Network.maxConnections = 1;
		Network.InitializeServer(1, connectionPort, false);
		isServer = true;
        _GameManager.isPlayerFirst = true;

        sender = new UdpClient (broadcastPort, AddressFamily.InterNetwork);
        IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, broadcastPort);
        sender.Connect (groupEP);

        //SendData ();
        InvokeRepeating("SendData",0,5f);

	}

    public void DirectConnect(string ip)															// Connect to the local server
	{
		Network.Connect(ip, connectionPort);
		isServer = false;
        _GameManager.isPlayerFirst = false;
	}

    public void SendData ()
    {
        string customMessage = playerName+"*"+connectionIP;

        if (customMessage != "") {
            sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
        }
    }

    public void StartReceivingIP ()
    {

        try {
            if (receiver == null) {
                receiver = new UdpClient (broadcastPort);
                receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
            }
        } catch (SocketException e) {
            Debug.Log (e.Message);
        }
    }

    private void ReceiveData (IAsyncResult result)
    {
        IPEndPoint receiveIPGroup = new IPEndPoint (IPAddress.Any, broadcastPort);
        byte[] received;
        if (receiver != null) 
        {
            received = receiver.EndReceive (result, ref receiveIPGroup);
        } 
        else 
        {
            return;
        }
        //receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
        string receivedString = Encoding.ASCII.GetString (received);

        if (!string.IsNullOrEmpty(receivedString))
        {
            receivedData = receivedString.Split('*');
            //hostIP = IPAddress.Parse(receivedData[1]);

        }
    }
}