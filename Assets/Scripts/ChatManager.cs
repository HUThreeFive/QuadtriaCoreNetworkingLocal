/*
	All scripts are writen in really primitive way. As a networking beginner you should understand all
	things in this code and therefore you should not have any problems with converting/editing this code 
	for your own needs...

	OneManGames 2014 - Moopey
	email : omgdevelopersgroup@gmail.com
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;				// You need to import this to be able to use List<> in C#

[RequireComponent (typeof (NetworkView))]
public class ChatManager : MonoBehaviour
{
    public Rect chatPosition = new Rect(5f, 35f, 400f, 200f);

	public string message;
	public List<string> messages;	//List of all messages

	NetworkManager mNetwork;			//NetworkScript

	public GameManager _GameManager;

	void Start()
	{
		mNetwork = GetComponent<NetworkManager>();

		GameObject gameManager = GameObject.Find ("Game Manager");
		_GameManager = gameManager.GetComponent<GameManager> ();
	}

	void Update()
	{
		if(messages.Count > (int)chatPosition.height / 25)		// Check how many messages you can fit in your chat window depending on chatPosition.height and divide by 25
		{
			messages.RemoveAt(0);										// This will remove the oldest message if new one is received and there is no more space in chat
		}
	}

	void OnGUI()
	{
		if(mNetwork.isConnected)											// Show chat if we are connected to the server
		{
            GUI.BeginGroup(new Rect(5f, 35f, 400f, 200f), "");
            chatPosition = new Rect(5f, 35f, 400f, 200f);
			GUI.Box(new Rect(0, 0, chatPosition.width, chatPosition.height), "Chat");

			for (var i = 0; i < messages.Count; i++)
				GUI.Label(new Rect(5, 30+(i*15), chatPosition.width - 10, 30), messages[i]);		// Display all messages in messages list (for every message in messages, display GUI.Label)

			message = GUI.TextField(new Rect(5, chatPosition.height - 30, chatPosition.width - 110, 25), message);
			if(GUI.Button(new Rect(chatPosition.width - 100, chatPosition.height - 30, 90, 25), "Send"))
			{
				if(message != "")
				{
					string tempMessage = "C*" + mNetwork.playerName + "*" + message;					// This will conver player name from SimpleNetwork script and current message into one string
					SendMessage(tempMessage);																// Player Name + message string converted into one will be sended to the SendMessage
				}
			}
			GUI.EndGroup();
		}
	}

	public new void SendMessage(string msg)
	{
		GetComponent<NetworkView>().RPC ("ReciveMessage", RPCMode.All, msg);								// RPC Call to add message to the messages list

		message = "";                                                                                               // Detele message from TextField
	}

	public void SendMove(string msg)
	{
        if (mNetwork.isServer)
        {
            msg = "M*Server" + msg;
        }
        else
        {
            msg = "M*Client" + msg;
        }
		GetComponent<NetworkView>().RPC ("ReciveMessage", RPCMode.All, msg);								// RPC Call to add message to the messages list

		message = "";
	}


	[RPC]
	public void ReciveMessage(string msg)
	{
		string[] pieces = msg.Split('*');
		if (pieces[0] == "M")        //its a move
		{
			MakeMove (pieces);
		}
		else if(pieces[0] == "C")
		{
			string message = pieces[1] + " : " + pieces[2];
			messages.Add(message);                                                                                      // RPC call for message so everyone currently connected to the server can see it in chat
		}

	}

	private void MakeMove(string[] pieces)
	{
        string[] positions = pieces [2].Split('+');
        int x = 0;
        GameObject[] currentPositions;

		if (pieces [1] == "Server") 
        {
            currentPositions = _GameManager.m_playerPieces;
		} 
        else 
        {
            currentPositions = _GameManager.m_opponentPieces;
		}

        foreach (GameObject go in currentPositions) 
        {
            string[] xAndz = positions[x].Split('|');

            if (go.transform.position.x.ToString() != xAndz[0] || go.transform.position.z.ToString() != xAndz[1])      
            {
                // this is what changed
                Vector3 temp = new Vector3(Convert.ToSingle(xAndz[0]), go.transform.position.y, Convert.ToSingle(xAndz[1]));
                go.transform.position = temp;

                _GameManager.moveCount++;
                _GameManager.moveCountText.text = (_GameManager.moveLimit - _GameManager.moveCount).ToString();

                _GameManager.CheckForWins (_GameManager.isPlayerTurn);

                if (pieces [1] == mNetwork.playerName) 
                {
                    _GameManager.isPlayerTurn = false;
                } 
                else 
                {
                    _GameManager.isPlayerTurn = true;
                }


            }

            x++;
        }
	}
}