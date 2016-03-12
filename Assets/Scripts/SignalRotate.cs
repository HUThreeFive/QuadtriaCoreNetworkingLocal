using UnityEngine;
using System.Collections;


public class SignalRotate : MonoBehaviour {

	// Use this for initialization
	private bool m_QuadEmpty = false;
	public AudioSource activeElectricity;
	void Update()
	{
		if (m_QuadEmpty == false)
			transform.Rotate (new Vector3 (15, 45, 15) * Time.deltaTime);
		else
			transform.Rotate (new Vector3 (90, 180, 90)* Time.deltaTime);
	}

    public void SetEmpty()
	{
		m_QuadEmpty = true;
	}
}

