using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

	public GameObject[] m_QuadrantA;
	public GameObject[] m_QuadrantB;
	public GameObject[] m_QuadrantC;
	public GameObject[] m_QuadrantD;
	public GameObject playerPrefab;
	public GameObject opponentPrefab;
	public SignalRotate playerSignal;
	public SignalRotate opponentSignal;
	public GameObject[] m_playerPieces;
	public GameObject[] m_opponentPieces;
	private GameObject m_SelectedPiece;
	public bool isPlayerTurn;
	public bool isPlayerFirst;
	public bool isGameOver;
	public bool isNetworkGame;
	public AIManager aiManager;
	private GameManagerBoardController gameBoard;
    private NetworkManager _nManager;

	public bool isplayerSignalFlipped;
	public bool isopponentSignalFlipped;
	public bool hasGameStarted;
    private bool isFirstMove;
	public int moveCount;
	public int moveLimit;
	public Text moveCountText;
	public GameObject startingHelpPic;
	private GameObject gameCanvas;
	public Text gameOverText;
	public GameObject menuCanvas;
	public GameboardManager _GameboardManager;

	// Use this for initialization
	void Start () {
		hasGameStarted = true;

        moveLimit = 120;
        isFirstMove = true;

        aiManager = GetComponent<AIManager> ();
        gameBoard = GetComponent<GameManagerBoardController> ();

        GameObject networkManager = GameObject.Find ("Network Manager");
        _nManager = networkManager.GetComponent<NetworkManager> ();

        startingHelpPic.SetActive (false);
        //menuCanvas.SetActive (true);

        InitiateGame ();

        //startingHelpPic.SetActive (false);

		//gameCanvas = GameObject.FindGameObjectWithTag ("gameCanvas");

	}

	void InitiateGame()
	{
		gameOverText.text = "";
		moveCountText.text = moveLimit.ToString ();

		isPlayerTurn = false;
		isGameOver = false;
		isNetworkGame = true;
        isPlayerFirst = true;

		isplayerSignalFlipped = false;
		isopponentSignalFlipped = false;

		m_playerPieces = new GameObject[5];
		m_opponentPieces = new GameObject[5];

		SpawnAllPieces (m_QuadrantD, m_playerPieces, playerPrefab);
		SpawnAllPieces (m_QuadrantA, m_opponentPieces, opponentPrefab);

		if (isPlayerFirst) {
			isPlayerTurn = true;
		}

		moveCount = 0;
		hasGameStarted = false;

	}


	void Update ()
    {
        if (hasGameStarted)
        {
            if (moveCount >= moveLimit)
            {
                gameOverText.text = "It's a tie!";
                hasGameStarted = false;
                menuCanvas.SetActive(true);
            }
            else if (CheckForWins(true))
            {
                gameOverText.text = "You lose";
                hasGameStarted = false;
                menuCanvas.SetActive(true);
            }
            else if (CheckForWins(false))
            {
                gameOverText.text = "You win!";
                hasGameStarted = false;
                menuCanvas.SetActive(true);
            }

            if (_nManager.isConnected) //network game
            {
                if (isFirstMove) 
                {
                    if(isPlayerFirst)
                    {   
                        isPlayerTurn = true;
                    }
                    else
                    {
                        isPlayerTurn = false;
                    }

                    isFirstMove = false;
                }
            }
            else if (!isPlayerTurn) //AI game and AI turn
            {
                aiManager.TakeTurn();
            }
            else
            {
                //AI game and player turn
            }
        }
    }


	private void SpawnAllPieces(GameObject[] m_Quadrant, GameObject[] pieces, GameObject prefab)
	{
		for (int i = 0; i < 5; i++) 
		{
			pieces [i] = Instantiate (prefab, m_Quadrant [i].transform.position, m_Quadrant [i].transform.rotation) as GameObject;
		}
	}

	public bool CheckAvailableMoves()
	{
		Vector3[] m_PlayerLocations = new Vector3[5];

		for (int i = 0; i < 5; i++) 
		{
			m_PlayerLocations [i] = m_playerPieces [i].transform.position;
		}

		
		if (!AvailableMoves (m_PlayerLocations))
			return false;
		else
			return true;
	}

	private bool AvailableMoves(Vector3[] m_PlayerLocations)
	{
		string turn;
		string opponent;

		if (isPlayerTurn) 
		{
			turn = "player1Piece";
			opponent = "player2Piece";
		}
		else 
		{
			turn = "player2Piece";
			opponent = "player1Piece";
		}
	

		bool spotAvailable = true;
		GameObject[] playerSpots = new GameObject[5];
		PieceManager pieceManager;

		foreach (GameObject spot in _GameboardManager.spots) 
		{

			Collider[] hitColliders = Physics.OverlapSphere (spot.transform.position, 5f);
			int index = 0;
			int playerIndex = 0;

			while (index < hitColliders.Length) 
			{
				if (hitColliders [index].tag == turn) 
				{
					playerSpots [playerIndex] = spot;
					playerIndex++;
					break;
				}

				index++;
			}
		}


		return spotAvailable;



	}



	private bool WinCondition(GameObject[] m_Quadrant, Vector3[] m_PlayerPieces)//who's pieces are being checked and what quadrant will be the parameters
	{
		bool hasWon = false;
		int pieceIndex = 0;
		GameObject[] spotsFilledByPlayer = new GameObject[5];
		for (int i = 0; i < 5; i++) 
		{
			for (int j = 0; j < 5; j++) 
			{
				if (m_Quadrant [i].transform.position == m_PlayerPieces [j]) 
				{
					spotsFilledByPlayer [pieceIndex] = m_Quadrant [i];
					pieceIndex++;
				}
			}
		}

		if (pieceIndex > 3) 
		{
			//4 or more pieces in quadrant, all possible configurations result in a triangle

			hasWon = true;
		}

		if (pieceIndex == 3) 
		{
			//three pieces, possible triangle, have to check
			if (spotsFilledByPlayer [0] == m_Quadrant [0] && spotsFilledByPlayer [1] == m_Quadrant [2] &&
				spotsFilledByPlayer [2] == m_Quadrant [4]) 
			{
				hasWon = false;
			} 
			else if (spotsFilledByPlayer [0] == m_Quadrant [1] && spotsFilledByPlayer [1] == m_Quadrant [2] &&
				spotsFilledByPlayer [2] == m_Quadrant [3]) 
			{
				hasWon = false;
			} 
			else 
			{

				hasWon = true;
			}

		} 

		return hasWon;
	}

	void PlayerWon(GameObject[] m_Winner)
	{
		Debug.Log ("Player won");
	}

	public bool CheckForWins(bool playerTurn)
	{
		bool isGameOver = false;

		Vector3[] tempPieces = new Vector3[5];

		if (!playerTurn) {
			for (int x=0; x<5; x++)
			{
				tempPieces [x] = m_playerPieces [x].transform.position;
			}
			if (WinCondition (m_QuadrantA, tempPieces) || WinCondition (m_QuadrantB, tempPieces) || WinCondition (m_QuadrantC, tempPieces)) {
				isGameOver = true;
			
				//PlayerWon (m_playerPieces);
			}

			if (isplayerSignalFlipped || HomeQuadrantEmpty(m_QuadrantD,m_playerPieces, playerSignal)) 
			{
				if (WinCondition (m_QuadrantD, tempPieces)) {
					isGameOver = true;

					//PlayerWon (m_playerPieces);
				}
			}

		} 
		else 
		{
			for (int x=0; x<5; x++)
			{
				tempPieces [x] = m_opponentPieces [x].transform.position;
			}
			if (WinCondition (m_QuadrantD, tempPieces) || WinCondition (m_QuadrantB, tempPieces) || WinCondition (m_QuadrantC, tempPieces)) {
				isGameOver = true;

			}

			if (isopponentSignalFlipped || HomeQuadrantEmpty (m_QuadrantA, m_opponentPieces, opponentSignal)) 
			{
				if (WinCondition (m_QuadrantA, tempPieces)) {
					isGameOver = true;

				}
			}
		}
		return isGameOver;
	}

	public bool HomeQuadrantEmpty(GameObject[] m_Quadrant, GameObject[]m_PlayerPieces, SignalRotate m_Signal)
	{
		if (m_Quadrant == m_QuadrantD) 
		{
			if (isplayerSignalFlipped)
				return true;
		}
		if (m_Quadrant == m_QuadrantA) 
		{
			if (isopponentSignalFlipped)
				return true;
		}
		int pieceIndex = 0;
		for (int i = 0; i < m_Quadrant.Length; i++) 
		{
			for (int j = 0; j < m_PlayerPieces.Length; j++) 
			{
				if (m_Quadrant [i].transform.position == m_PlayerPieces [j].transform.position) 
				{
					pieceIndex++;
				}
			}
		}

		if (pieceIndex == 0) {
			//set bool for home signal to true
			m_Signal.SetEmpty();

			if (m_Quadrant == m_QuadrantD) 
			{
				isplayerSignalFlipped = true;
			}
			if (m_QuadrantA == m_Quadrant) 
			{
				isopponentSignalFlipped = true;
			}
			return true;
		} 
		else 
		{
			return false;
		}
	}

	public bool ValidateMove(string newPosition, Vector3 currentPosition)
	{

		if (IsValidMove (newPosition, currentPosition) && !menuCanvas.activeInHierarchy)
			return true;
		else
			return false;
	}

	private bool IsValidMove(string newPosition, Vector3 currentPosition)
	{
		bool moveValidated = false;

		string locationName = newPosition.Substring (0, 2);

		GameObject requestedLocation = GameObject.Find (locationName) as GameObject;



		PieceManager pieceManager = requestedLocation.GetComponent<PieceManager> ();


		foreach (GameObject possibleMove in pieceManager.validMoves) 
		{
			if (possibleMove.transform.position == currentPosition && !CheckForWins (true) && !CheckForWins (false))
				moveValidated = true;
		}

		return moveValidated;

	}

	public void NewGame()
    {
		for (int x = 0; x < 5; x++) 
        {
			Destroy (m_playerPieces[x]);
			Destroy (m_opponentPieces[x]);
		}
            
		InitiateGame ();
		gameOverText.text = "";
	}

}
