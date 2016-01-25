using UnityEngine;
using System.Collections;

public class Corner : MonoBehaviour {

	InitializerScript init;

	void OnTriggerStay(Collider c) {
		if(c.tag == "Zombie" && Vector3.Distance(c.transform.position, this.transform.position) < 0.2f) {
			Zombie z = c.GetComponent<Zombie>();
			if(z.end == this.transform.position){
				//Debug.Log("sent to processing");
				init.SendMessage("processZombie", z, SendMessageOptions.RequireReceiver);
			}
		}
		
	}

	void OnTriggerEnter(Collider c) {
		if(c.tag == "Zombie" && Vector3.Distance(c.transform.position, this.transform.position) < 0.2f) {
			Zombie z = c.GetComponent<Zombie>();
			if(z.end == this.transform.position){
				//Debug.Log("sent to processing");
				init.SendMessage("processZombie", z, SendMessageOptions.RequireReceiver);
			}
		}
		
	}
	void OnTriggerExit(Collider c) {
		if(c.tag == "Zombie" && Vector3.Distance(c.transform.position, this.transform.position) < 0.2f) {
			Zombie z = c.GetComponent<Zombie>();
			if(z.end == this.transform.position){
				//Debug.Log("sent to processing");
				init.SendMessage("processZombie", z, SendMessageOptions.RequireReceiver);
			}
		}
		
	}


	// Use this for initialization
	void Start () {
		init = (InitializerScript)GameObject.Find ("Initializer").GetComponent<InitializerScript>();
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
