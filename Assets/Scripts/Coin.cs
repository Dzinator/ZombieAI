using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		//Debug.Log ("lol");
		if(c.tag == "Survivor") {
			gameObject.SetActive(false);
		}

	}
}
