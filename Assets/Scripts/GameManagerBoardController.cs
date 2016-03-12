using UnityEngine;
using System.Collections;

public class GameManagerBoardController : MonoBehaviour {
	public int AIscore;
	public int AIalpha;
	public int AIbeta;
	public int AIdepth;
	public int[] AIPieces;
	public int[] HumanPieces;
	public bool isTie;
	public bool hasPlayer2Won;
	public bool isGameOver;
	public bool isAISignalFlipped;
	public bool isHumanSignalFlipped;


	// Use this for initialization
	void Start () {
		AIPieces = new int[5];
		for (int x = 0; x < 5; x++) {
			AIPieces [x] = 0;
		}
		HumanPieces = new int[5];
		for (int x = 0; x < 5; x++) {
			HumanPieces [x] = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
