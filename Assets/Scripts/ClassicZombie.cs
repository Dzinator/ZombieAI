using UnityEngine;
using System.Collections;

public class ClassicZombie : Zombie {
	InitializerScript init;

	// Use this for initialization
	void Start () {
		init = (InitializerScript)GameObject.Find ("Initializer").GetComponent<InitializerScript>();

		isVisible = false;

	}

	void FixedUpdate() {
		//reset visibility halo
		if (isVisible) {
			halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
		}
		else {
			halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
		}

		//handle zombies by changing state
		float d = distZombieAhead ();
		if (d > slowingDist) {
			currState = State.Walking;
		} else if (d > stopDist) {
			currState = State.Decelerating;
		} else {
			currState = State.Halted;
		}
		
		//execute appropriate state
		if (currState == State.Walking) {
			walking();
		} else if (currState == State.Decelerating) {
			decelerating(d);
		} else {
			halted();
		}


		//handle rare weird cases where zombie leaves map for no apparent reason
		if ((transform.position.x > 33 || transform.position.x < 0) || (transform.position.z > 21 || transform.position.z < 0)) {
			InitializerScript.zombies.Remove(this);

			bool isClockwise = false;
			if((iStart == 0 && iEnd == 3) || (iStart == iEnd + 1)){
				isClockwise = false;//counter clockwise
			}
			else{
				isClockwise = true; //clockwise
			}

			init.spawnOneZombie(isEasy, isClockwise);
			Destroy(this.gameObject);
		}

	}

	// Update is called once per frame
	void Update () {

	
	}
}
