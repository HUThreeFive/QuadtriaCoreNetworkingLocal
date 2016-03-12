using UnityEngine;
using System.Collections;

public class AIManager : MonoBehaviour {

	[HideInInspector] public GameManager gameManager;
	[HideInInspector] public GameManagerBoardController gameManagerBoardController;
	[HideInInspector] public GameboardManager gameboardManager;
	public int maxAIdepth;
	public int maxAImoves;
	[HideInInspector] public int aiMoveCount;
	[HideInInspector] public Move[] gameboard;
	[HideInInspector] public GameManagerBoardController tempPreferredMove;
	[HideInInspector] public int[] QuadrantA;
	[HideInInspector] public int[] QuadrantB;
	[HideInInspector] public int[] QuadrantC;
	[HideInInspector] public int[] QuadrantD;
	[HideInInspector] public int AIdifficulty;

	public class Move{

		public Vector3 position;
		public Vector3[] possibleMoves;
		public char quadrant;

		public Move(GameObject p, GameObject[] pm, int q){
			position = p.transform.position;

			possibleMoves = new Vector3[pm.Length];
			for (int x = 0; x < pm.Length; x++) {
				possibleMoves[x] = pm[x].transform.position;
			}

			if(q == 0){
				quadrant = 'A';
			}
			else if(q == 1){
				quadrant = 'B';
			}
			else if(q == 2){
				quadrant = 'C';
			}
			else if(q == 3){
				quadrant = 'D';
			}
		}
	}


	// Use this for initialization
	void Awake () {
		maxAIdepth = 6;
		maxAImoves = 2500;
		AIdifficulty = 1;
		gameManager = GetComponent<GameManager> ();
		gameManagerBoardController = GetComponent<GameManagerBoardController> ();

		GameObject board = GameObject.Find ("roughGameBoard") as GameObject;
		gameboardManager = board.GetComponent<GameboardManager> ();

		gameboard = new Move[20];

		for (int x = 0; x < 20; x++) {
			PieceManager pieceManager = gameboardManager.spots[x].GetComponent<PieceManager> ();
			gameboard[x] = new Move(gameboardManager.spots[x], pieceManager.validMoves, x/5);
		}

		QuadrantA = new int[5];
		for (int x = 0; x < 5; x++) {
			QuadrantA [x] = x;
		}
		QuadrantB = new int[5];
		for (int x = 0; x < 5; x++) {
			QuadrantB [x] = x+5;
		}
		QuadrantC = new int[5];
		for (int x = 0; x < 5; x++) {
			QuadrantC [x] = x+10;
		}
		QuadrantD = new int[5];
		for (int x = 0; x < 5; x++) {
			QuadrantD [x] = x+15;
		}

		tempPreferredMove = gameObject.AddComponent<GameManagerBoardController>();
		tempPreferredMove.AIPieces = new int[5];
		tempPreferredMove.HumanPieces = new int[5];
	}

	// Update is called once per frame
	void Update () {
	}
		
	int boardEvaluate(GameManagerBoardController board)
	{
		int boardScore = 0;
		if (AIdifficulty == 1) {

			if (hasWon (board.AIPieces, "AI", board.isAISignalFlipped)) {
				boardScore = 1000 - board.AIdepth;
			} 
			else if (hasWon (board.HumanPieces, "human", board.isHumanSignalFlipped)) {
				boardScore = -1000 + board.AIdepth;
			} 
			else {

				int aCount = 0;
				int bCount = 0;
				int cCount = 0;
				int dCount = 0;

				for (int pieceNum = 0; pieceNum < 5; pieceNum++) {

					if (board.AIPieces [pieceNum] == 'B') {
						++bCount;
					} else if (board.AIPieces [pieceNum] == 'C') {
						++cCount;
					} else if (board.AIPieces [pieceNum] == 'D') {
						++dCount;
					} else if (board.AIPieces [pieceNum] == 'A' && board.isAISignalFlipped) {
						++aCount;
					}

					if (aCount >= 1) {
						boardScore = boardScore + (10 * aCount);
					} else if (bCount >= 1) {
						boardScore = boardScore + (10 * bCount);
					} else if (cCount >= 1) {
						boardScore = boardScore + (10 * cCount);
					} else if (dCount >= 1) {
						boardScore = boardScore + (10 * dCount);
					}
				}
			}
		}
			

//			boardScore = boardScore + (bCount*5);
//			boardScore = boardScore + (cCount*5);
//			boardScore = boardScore + (dCount*5);
//			boardScore = boardScore + (aCount*5);

//			foreach (Vector3 possibleMove in gameboard [board.AIPieces [pieceNum]].possibleMoves) {
//				foreach (int AIPiece in board.AIPieces){
//					if (possibleMove == gameboard[AIPiece].position && gameboard[AIPiece].quadrant != 'A'){
//						boardScore += 8;
//					}
//				}
//				foreach (int HumanPiece in board.HumanPieces){
//					if (possibleMove == gameboard[HumanPiece].position){
//						--boardScore;
//					}
//				}
//			}
//
//			foreach (Vector3 possibleMove in gameboard [board.HumanPieces [pieceNum]].possibleMoves) {
//				foreach (int HumanPiece in board.HumanPieces){
//					if (possibleMove == gameboard[HumanPiece].position && gameboard[HumanPiece].quadrant != 'D'){
//						boardScore -= 8;
//					}
//				}
//				foreach (int AIPiece in board.AIPieces){
//					if (possibleMove == gameboard[AIPiece].position){
//						++boardScore;
//					}
//				}
//			}
//		}
		Destroy(board);
		return boardScore;
	}

	public void TakeTurn()
	{
		int time = System.DateTime.Now.Minute * 60 +System.DateTime.Now.Second;
		gameManagerBoardController.AIalpha = -1000;
		gameManagerBoardController.AIbeta = 1000;
		gameManagerBoardController.AIdepth = 0;
		gameManagerBoardController.AIscore = 0;

		if (gameManager.moveCount > 1) {
			maxAIdepth = 5;
		}

		if (gameManager == null) {
			gameManager = GetComponent<GameManager> ();
		}

		int[] temp = new int[5];

		for (int x = 0; x < 5; x++) {
			for (int y = 0; y < 20; y++) {
				if (gameManager.m_opponentPieces [x].transform.position == gameboard [y].position) {
					temp [x] = y;
				}
				if (gameManager.m_playerPieces [x].transform.position == gameboard [y].position) {
					gameManagerBoardController.HumanPieces [x] = y;
				}
			}
		}


		gameManagerBoardController.AIPieces [0] = temp [1];
		gameManagerBoardController.AIPieces [1] = temp [3];
		gameManagerBoardController.AIPieces [2] = temp [4];
		gameManagerBoardController.AIPieces [3] = temp [0];
		gameManagerBoardController.AIPieces [4] = temp [2];

		gameManagerBoardController.isAISignalFlipped = gameManager.isopponentSignalFlipped;
		gameManagerBoardController.isHumanSignalFlipped = gameManager.isplayerSignalFlipped;

		aiMoveCount = 0;

		gameManagerBoardController = boardMaxABP (gameManagerBoardController);

		for (int x = 0; x < 5; x++) {
			gameManager.m_opponentPieces [x].transform.position = gameboard [gameManagerBoardController.AIPieces[x]].position;
		}

		Debug.Log (aiMoveCount);

		gameManager.moveCount++;
		gameManager.moveCountText.text = (gameManager.moveLimit - gameManager.moveCount).ToString ();
		Debug.Log (System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second - time);
		gameManager.isPlayerTurn = true;
	}

	GameManagerBoardController boardMaxABP (GameManagerBoardController game)
	{
		Debug.Log (aiMoveCount);
		GameManagerBoardController temp = gameObject.AddComponent<GameManagerBoardController>();
		temp.AIPieces = new int[5];
		temp.HumanPieces = new int[5];

		GameManagerBoardController preferredMove = gameObject.AddComponent<GameManagerBoardController>();
		preferredMove.AIPieces = new int[5];
		preferredMove.HumanPieces = new int[5];

		for (int x = 0; x < 5; x++) {
			preferredMove.AIPieces [x] = game.AIPieces[x];
			preferredMove.HumanPieces [x] = game.HumanPieces[x];
		}

		bool hasKids = false;
		bool changesScore = false;

		if (game.AIalpha < game.AIbeta)
		{

			for (int pieceNum = 0; pieceNum < 5; pieceNum++)
			{
				if (game.AIalpha < game.AIbeta) {

					foreach (Vector3 possibleMove in gameboard [game.AIPieces [pieceNum]].possibleMoves) {

						bool spotTaken = false;

						foreach (int AIspot in game.AIPieces) {
							if (gameboard[AIspot].position == possibleMove) {
								spotTaken = true;
								break;
							}
						}
						if (!spotTaken) {
							foreach (int HumanSpot in game.HumanPieces) {
								if (gameboard[HumanSpot].position == possibleMove) {
									spotTaken = true;
									break;
								}
							}	
						}
						if (!spotTaken && game.AIalpha < game.AIbeta) {
							aiMoveCount++;
							hasKids = true;

							for (int x = 0; x < 5; x++) {
								temp.AIPieces [x] = game.AIPieces [x];
								temp.HumanPieces [x] = game.HumanPieces [x];
							}

							temp.AIdepth = game.AIdepth + 1;
							temp.AIalpha = game.AIalpha;
							temp.AIbeta = game.AIbeta;

							//makes move
							for (int y = 0; y < 20; y++) {
								if (possibleMove == gameboard [y].position) {
									temp.AIPieces [pieceNum] = y;
									y = 20;
								}
							}
							//temp.AIPieces [pieceNum] = possibleMove; //makeMove

							if (isHomeEmpty ('A', temp.AIPieces)) {
								temp.isAISignalFlipped = true;
							} else {
								temp.isAISignalFlipped = game.isAISignalFlipped;
							}
							if (isHomeEmpty ('D', temp.HumanPieces)) {
								temp.isHumanSignalFlipped = true;
							} else {
								temp.isHumanSignalFlipped = game.isHumanSignalFlipped;
							}

							if (temp.AIdepth < maxAIdepth && !hasWon(temp.AIPieces, "AI", temp.isAISignalFlipped) ){
								if (aiMoveCount > 2000) {
									maxAIdepth = 5;
								}
//								else if (aiMoveCount > 2000) {
//									maxAIdepth = 4;
//								}
//								else if (aiMoveCount > 1500) {
//									maxAIdepth = 6;
//								}
								tempPreferredMove = boardMinABP (temp);

								if (!changesScore || tempPreferredMove.AIscore > game.AIalpha) {
									game.AIalpha = tempPreferredMove.AIscore;
									game.AIscore = tempPreferredMove.AIscore;

									for (int x = 0; x < 5; x++) {
										preferredMove.AIPieces [x] = tempPreferredMove.AIPieces[x];
										preferredMove.HumanPieces [x] = game.HumanPieces[x];
									}
										
									preferredMove.AIalpha = temp.AIalpha;
									preferredMove.AIbeta = game.AIalpha;
									preferredMove.AIscore = tempPreferredMove.AIscore;

									changesScore = true;
								}
							} 
							else {
								int tempScore = boardEvaluate (temp);

								if (tempScore > game.AIalpha) {
									game.AIscore = tempScore;
									game.AIalpha = tempScore;

									for (int x = 0; x < 5; x++) {
										preferredMove.AIPieces [x] = temp.AIPieces [x];	
										preferredMove.HumanPieces [x] = game.HumanPieces[x];
									}

									preferredMove.AIalpha = temp.AIalpha;
									preferredMove.AIbeta = game.AIalpha;
									preferredMove.AIdepth = temp.AIdepth;
									preferredMove.AIscore = tempScore;

									changesScore = true;
								}
							}
						}
					}
				}

			}
		}

		if(!hasKids || !changesScore)
		{
			game.AIscore = boardEvaluate(game);
		}

		Destroy(temp);
		Destroy (preferredMove);
		Destroy (game);
		return preferredMove;
	}

	GameManagerBoardController boardMinABP (GameManagerBoardController game)
	{
		GameManagerBoardController temp = gameObject.AddComponent<GameManagerBoardController>();
		temp.AIPieces = new int[5];
		temp.HumanPieces = new int[5];

		GameManagerBoardController preferredMove = gameObject.AddComponent<GameManagerBoardController>();
		preferredMove.AIPieces = new int[5];
		preferredMove.HumanPieces = new int[5];

		for (int x = 0; x < 5; x++) {
			preferredMove.AIPieces [x] = game.AIPieces[x];
			preferredMove.HumanPieces [x] = game.HumanPieces[x];
		}

		bool hasKids = false;
		bool changesScore = false;

		if (game.AIalpha < game.AIbeta) {
			for (int pieceNum = 0; pieceNum < 5; pieceNum++) {

				if (game.AIalpha < game.AIbeta) {

					foreach (Vector3 possibleMove in gameboard [game.HumanPieces [pieceNum]].possibleMoves) {

						bool spotTaken = false;

						foreach (int AIspot in game.AIPieces) {
							if (gameboard[AIspot].position == possibleMove) {
								spotTaken = true;
								break;
							}
						}
						if (!spotTaken) {
							foreach (int HumanSpot in game.HumanPieces) {
								if (gameboard[HumanSpot].position == possibleMove) {
									spotTaken = true;
									break;
								}
							}	
						}
						if (!spotTaken && game.AIalpha < game.AIbeta) {
							aiMoveCount++;
							hasKids = true;

							for (int x = 0; x < 5; x++) {
								temp.AIPieces [x] = game.AIPieces [x];
								temp.HumanPieces [x] = game.HumanPieces [x];
							}

							temp.AIdepth = game.AIdepth + 1;
							temp.AIalpha = game.AIalpha;
							temp.AIbeta = game.AIbeta;

							//makes the move

							for (int y = 0; y < 20; y++) {
								if (possibleMove == gameboard [y].position) {
									temp.HumanPieces [pieceNum] = y;
									y = 20;
								}
							}

							if (isHomeEmpty ('A', temp.AIPieces)) {
								temp.isAISignalFlipped = true;
							} else {
								temp.isAISignalFlipped = game.isAISignalFlipped;
							}

							if (isHomeEmpty ('D', temp.HumanPieces)) {
								temp.isHumanSignalFlipped = true;
							} else {
								temp.isHumanSignalFlipped = game.isHumanSignalFlipped;
							}
							//temp.HumanPieces [pieceNum] = possibleMove; //makeMove

							if (temp.AIdepth < maxAIdepth && !hasWon(temp.HumanPieces, "human", temp.isHumanSignalFlipped)) {

								if (aiMoveCount > 2000) {
									maxAIdepth = 4;
								}
//								else if (aiMoveCount > 2000) {
//									maxAIdepth = 4;
//								}
//								else if (aiMoveCount > 1500) {
//									maxAIdepth = 6;
//								}
//
								tempPreferredMove = boardMaxABP (temp);


								if (!changesScore || tempPreferredMove.AIscore < game.AIbeta) {
									game.AIbeta = tempPreferredMove.AIscore;
									game.AIscore = tempPreferredMove.AIscore;

									for (int x = 0; x < 5; x++) {
										preferredMove.HumanPieces [x] = tempPreferredMove.HumanPieces [x];
										preferredMove.AIPieces [x] = game.AIPieces[x];
									}

									preferredMove.AIalpha = game.AIbeta;
									preferredMove.AIbeta = temp.AIbeta;
									preferredMove.AIscore = tempPreferredMove.AIscore;

									changesScore = true;
								} 
							} 
							else {
								int tempScore = boardEvaluate (temp);

								if (tempScore < game.AIbeta) {
									game.AIscore = tempScore;
									game.AIbeta = tempScore;

									for (int x = 0; x < 5; x++) {
										preferredMove.HumanPieces [x] = temp.HumanPieces [x];
										preferredMove.AIPieces [x] = game.AIPieces[x];
									}

									preferredMove.AIalpha = game.AIbeta;
									preferredMove.AIbeta = temp.AIbeta;
									preferredMove.AIdepth = temp.AIdepth;
									preferredMove.AIscore = tempScore;

									changesScore = true;
								}
							}
						}
					}
				}
			}		 
		}
		if (!hasKids || !changesScore) {
			game.AIscore = boardEvaluate (game);
		}

		Destroy (temp);
		Destroy (preferredMove);
		Destroy (game);
		return preferredMove;
	}

	public bool hasWon(int[] playerPieces, string player, bool isSignalFlipped)
	{
		bool hasWon = false;

		if (player == "AI") {
			if (WinCondition (QuadrantD, playerPieces) || WinCondition (QuadrantB, playerPieces) || WinCondition (QuadrantC, playerPieces)) {
				hasWon = true;
			} 
			else if (isSignalFlipped && WinCondition (QuadrantA, playerPieces)) {
				hasWon = true;
			}
		}

		if (player == "human") {
			if (WinCondition (QuadrantA, playerPieces) || WinCondition (QuadrantB, playerPieces) || WinCondition (QuadrantC, playerPieces)) {
				hasWon = true;
			} 
			else if (isSignalFlipped && WinCondition (QuadrantD, playerPieces)) {
				hasWon = true;
			}
		}

		return hasWon;
	}

	private bool WinCondition(int[] quadrant, int[] playerPieces)//who's pieces are being checked and what quadrant will be the parameters
	{
		bool hasWon = false;
		int pieceIndex = 0;

		int[] spotsFilledByPlayer = new int[5];
		for (int i = 0; i < 5; i++) 
		{
			for (int j = 0; j < 5; j++) 
			{
				if (quadrant [i] == playerPieces [j]) 
				{
					spotsFilledByPlayer [pieceIndex] = quadrant [i];
					pieceIndex++;
					j = 5;
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
			if (!(spotsFilledByPlayer [0] == quadrant [0] && spotsFilledByPlayer [1] == quadrant [2] &&
				spotsFilledByPlayer [2] == quadrant [4]) && !(spotsFilledByPlayer [0] == quadrant [1] && spotsFilledByPlayer [1] == quadrant [2] &&
					spotsFilledByPlayer [2] == quadrant [3])) {
				hasWon = true;
			}

		} 
		return hasWon;
	}


	private bool isHomeEmpty (char quadrant, int[] playerPieces)//who's pieces are being checked and what quadrant will be the parameters
	{
		bool isEmpty = true;

		for (int i = 0; i < 5; i++) 
		{
			if (gameboard[playerPieces[i]].quadrant == quadrant)
			{
				isEmpty = false;
				i = 5;
			}
		}
			
		return isEmpty;
	}
}