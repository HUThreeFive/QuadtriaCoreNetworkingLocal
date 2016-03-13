using UnityEngine;
using System.Collections;

public class PieceMovement : MonoBehaviour {

	public Material[] materials;

	public bool itemSelected = false;
	private Rigidbody rb;
	private CapsuleCollider cc;
	private GameObject sphere;
	Vector3 newPosition;
	private Color originalColor;
	public GameManager _GameManager;
	public ChatManager _ChatManager;
    public NetworkManager _NetworkManager;
	public bool isActive;
	public AudioSource robotActivate;


	private void Start()
	{
		newPosition = transform.position;
		originalColor = GetComponent<Renderer> ().materials [0].color;

		GameObject gameManager = GameObject.Find ("Game Manager");
		_GameManager = gameManager.GetComponent<GameManager> ();

		GameObject chatManager = GameObject.Find ("Network Manager");
		_ChatManager = chatManager.GetComponent<ChatManager> ();

        GameObject netManager = GameObject.Find ("Network Manager");
        _NetworkManager = netManager.GetComponent<NetworkManager> ();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_GameManager == null) {
			GameObject gameManager = GameObject.Find ("Game Manager");
			_GameManager = gameManager.GetComponent<GameManager> ();
		}
		if (_GameManager.isPlayerTurn) {
			//if(_GameManager.CheckAvailableMoves())
			//{
				if (Input.GetMouseButtonDown(1))
				{
					OnRightClick();
				}
				if (Input.GetMouseButtonDown(0))
				{
					if (itemSelected == true)
					{
						RaycastHit hit;
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						if (Physics.Raycast(ray, out hit))
						{
							if (Physics.Raycast(ray, out hit))
							{
								if (hit.collider.tag == "circle" && hit.collider.tag != "player1Piece" &&
									hit.collider.tag != "player2Piece")
								{
									string objectName = hit.transform.ToString ();

									if(_GameManager.ValidateMove(objectName, transform.position))
                                    {
										newPosition = hit.collider.transform.position;
										transform.position = newPosition;
										GetComponent<Renderer> ().materials [0].color = originalColor;
										itemSelected = false;
										_GameManager.isPlayerTurn = false;
										_GameManager.CheckForWins (_GameManager.isPlayerTurn);
										_GameManager.moveCount++;
										_GameManager.moveCountText.text = (_GameManager.moveLimit - _GameManager.moveCount).ToString();
										
										_ChatManager.SendMove (BuildString ());
									}
									else
									{
										Debug.Log ("Player tried to make an invalid move.");
									}						
								}
							}
						}
					}
				}
		//}
			
		}
	}

	private void OnRightClick()
	{
		if (_GameManager.isPlayerTurn) {
			if (itemSelected == true)
			{
				GetComponent<Renderer> ().materials [0].color = originalColor;

				itemSelected = false;
			}
		}


	}

	private void OnMouseOver()
	{
        if (!_GameManager.isNetworkGame) // if AI game
        {
            if (_GameManager.isPlayerTurn)
            {
                if (itemSelected == false)
                {
                    if (Input.GetMouseButtonDown(0) && tag == "player1Piece")
                    {
                        robotActivate.Play();
                        GetComponent<Renderer>().materials[0].color = Color.yellow;
                        itemSelected = true;
                    }

                }
                else if (itemSelected == true)
                {
                    foreach (GameObject piece in _GameManager.m_playerPieces)
                    {
                        if (piece.GetComponent<Renderer>().materials[0].color == Color.yellow &&
                            piece.transform.position != transform.position)
                        {
                            OnRightClick();
                        }
                    }
                }
            }
        }
        else //if Network Game
        {
            if (_GameManager.isPlayerTurn)
            {
                if (itemSelected == false)
                {
                    if (Input.GetMouseButtonDown(0) && tag == "player1Piece" && _NetworkManager.isServer)
                    {
                        robotActivate.Play();
                        GetComponent<Renderer>().materials[0].color = Color.yellow;
                        itemSelected = true;
                    }
                    else if (Input.GetMouseButtonDown(0) && tag == "player2Piece" && !_NetworkManager.isServer)
                    {
                        robotActivate.Play();
                        GetComponent<Renderer>().materials[0].color = Color.yellow;
                        itemSelected = true;
                    }

                }
                else if (itemSelected == true)
                {
                    foreach (GameObject piece in _GameManager.m_playerPieces)
                    {
                        if (piece.GetComponent<Renderer>().materials[0].color == Color.yellow &&
                            piece.transform.position != transform.position)
                        {
                            OnRightClick();
                        }
                    }
                }
            }
        }
	}

	private string BuildString()
	{
		string positionString = "*";
        GameObject[] currentPositions;
        if (_NetworkManager.isServer) 
        {
            currentPositions = _GameManager.m_playerPieces;
        } 
        else 
        {
            currentPositions = _GameManager.m_opponentPieces;
        }
		foreach (GameObject o in currentPositions) {

            positionString += o.transform.position.x + "|"+ o.transform.position.y + "|" + o.transform.position.z + "+";
		}

		return positionString;
	}
}
