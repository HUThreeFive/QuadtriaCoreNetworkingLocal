using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private Vector3 targetCameraPos, startCameraPos;
	private Quaternion targetCameraRot, startCameraRot;
	private Camera[] allCameras;
	public Camera gameCamera;
	private int targetPosX,targetPosY,targetPosZ,targetRotX,targetRotY,targetRotZ;
	private float progress = 1f;
	private GameObject gameCanvas;
	public GameManager gameManager;
	public AudioSource buttonClick;
    public NetworkManager _nManager;
    public ChatManager _mChat;



	void Start(){
		allCameras = Camera.allCameras;
		foreach (Camera c in allCameras){
			if (c.tag.ToString () == "gameCamera")
				gameCamera = c;

            GameObject networkManager = GameObject.Find ("Network Manager");
            _nManager = networkManager.GetComponent<NetworkManager> ();
		}

		gameCanvas = GameObject.FindGameObjectWithTag ("gameCanvas");
	}

	private void newGameClick(){
		targetPosX = 0;
		targetPosY = 5;
		targetPosZ = -20;
		targetRotX = 20;
		targetRotY = 0;
		targetRotZ = 0;
		Debug.Log ("newGameButton click!");

		buttonClick.Play ();

		targetCameraPos = new Vector3 (targetPosX, targetPosY, targetPosZ);
		targetCameraRot = Quaternion.Euler (targetRotX, targetRotY, targetRotZ);
		startCameraPos = gameCamera.transform.position;
		startCameraRot = gameCamera.transform.rotation;
		progress = 0f;	
		gameCanvas.gameObject.SetActive (false);
		gameManager.NewGame ();
	}

	public void multiplayerClick(){
		targetPosX = 20;
		targetPosY = 5;
		targetPosZ = 0;
		targetRotX = 20;
		targetRotY = 270;
		targetRotZ = 0;
		Debug.Log ("multiplayerButton click!");

		buttonClick.Play ();

		targetCameraPos = new Vector3 (targetPosX, targetPosY, targetPosZ);
		targetCameraRot = Quaternion.Euler (targetRotX, targetRotY, targetRotZ);
		startCameraPos = gameCamera.transform.position;
		startCameraRot = gameCamera.transform.rotation;
		progress = 0f;	
		gameCanvas.gameObject.SetActive (false);
	}

	public void optionsClick(){
		targetPosX = -20;
		targetPosY = 5;
		targetPosZ = 0;
		targetRotX = 20;
		targetRotY = 90;
		targetRotZ = 0;
		Debug.Log ("optionsButton click!");

		buttonClick.Play ();

		targetCameraPos = new Vector3 (targetPosX, targetPosY, targetPosZ);
		targetCameraRot = Quaternion.Euler (targetRotX, targetRotY, targetRotZ);
		startCameraPos = gameCamera.transform.position;
		startCameraRot = gameCamera.transform.rotation;
		progress = 0f;	
		gameCanvas.gameObject.SetActive (false);
	}

	public void OnMouseDown(){
		if (this.tag == "returnMain") {

			targetPosX = 0;
			targetPosY = 55;
			targetPosZ = 0;
			targetRotX = 90;
			targetRotY = 0;
			targetRotZ = 0;

			targetCameraPos = new Vector3 (targetPosX, targetPosY, targetPosZ);
			targetCameraRot = Quaternion.Euler (targetRotX, targetRotY, targetRotZ);
			startCameraPos = gameCamera.transform.position;
			startCameraRot = gameCamera.transform.rotation;
			progress = 0f;	

			gameCanvas.SetActive (true);
		}

		if (this.tag == "newGame") 
		{
			targetPosX = 0;
			targetPosY = 55;
			targetPosZ = 0;
			targetRotX = 90;
			targetRotY = 0;
			targetRotZ = 0;

			//buttonClick.Play ();

			targetCameraPos = new Vector3 (targetPosX, targetPosY, targetPosZ);
			targetCameraRot = Quaternion.Euler (targetRotX, targetRotY, targetRotZ);
			startCameraPos = gameCamera.transform.position;
			startCameraRot = gameCamera.transform.rotation;
			progress = 0f;	

	
		}
	}

	void Update () {

		if (progress < 1) {
			progress += Time.deltaTime;

			gameCamera.transform.rotation = Quaternion.Lerp(startCameraRot, targetCameraRot, progress);
			gameCamera.transform.position = Vector3.Lerp(startCameraPos, targetCameraPos, progress);
		}
	}
}
