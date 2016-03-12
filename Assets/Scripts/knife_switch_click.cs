using UnityEngine;
using System.Collections;

public class knife_switch_click : MonoBehaviour {

	private Quaternion findRot = Quaternion.Euler (0, 0, 180);
	private Quaternion hostRot = Quaternion.Euler (0, 0, 0);
	private int counter = 1;


	// Use this for initialization
	void Start () {
	
	}

	void OnMouseDown(){
		counter++;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (counter % 2 == 0) {
			this.gameObject.transform.rotation = Quaternion.Lerp (this.transform.rotation, findRot, Time.deltaTime * 2);
		} else {
			this.gameObject.transform.rotation = Quaternion.Lerp (this.transform.rotation, hostRot, Time.deltaTime * 2);
		}
	
	}
}
