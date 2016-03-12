using UnityEngine;
using System.Collections;

public class HelpController : MonoBehaviour {

	public GameObject startPic;
	public GameObject playPic;
	public GameObject homeQuadPic;
	public GameObject tiePic;
	public GameObject winPic;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartToPlay()
	{
		startPic.SetActive (false);
		playPic.SetActive (true);
	}

	public void PlayToHomeQuad()
	{
		playPic.SetActive (false);
		homeQuadPic.SetActive (true);
	}

	public void HomeQuadToTie()
	{
		homeQuadPic.SetActive (false);
		tiePic.SetActive (true);
	}

	public void TieToWin()
	{
		tiePic.SetActive (false);
		winPic.SetActive (true);
	}


	public void WinToTie()
	{
		tiePic.SetActive (true);
		winPic.SetActive (false);
	}

	public void TieToHomeQuad()
	{
		homeQuadPic.SetActive (true);
		tiePic.SetActive (false);
	}

	public void HomeQuadToPlay()
	{
		playPic.SetActive (true);
		homeQuadPic.SetActive (false);
	}

	public void PlayToStart()
	{
		startPic.SetActive (true);
		playPic.SetActive (false);
	}

	public void ExitHelp ()
	{
		startPic.SetActive (false);
		playPic.SetActive (false);
		homeQuadPic.SetActive (false);
		tiePic.SetActive (false);
		winPic.SetActive (false);
	}
		
}
