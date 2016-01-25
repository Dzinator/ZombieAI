using UnityEngine;
using System.Collections;

public class ShamblerZombie : Zombie {
	InitializerScript init;

	float laneChangeTime;
	float laneChangeDelay = 1.0f;
	float laneChangeP = 0.1f; // probability of changing lane

	void FixedUpdate() {
		//reset visibility halo
		if (isVisible) {
			halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
		}
		else {
			halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
		}


		//Check if we change lane
		if(Random.value < laneChangeP && Time.time > laneChangeTime) {
			laneChangeTime = Time.time + laneChangeDelay;
			changeLane();
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
		
		
		//handle rare weird cases where zombie leaves map
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

	// Use this for initialization
	void Start () {
		init = (InitializerScript)GameObject.Find ("Initializer").GetComponent<InitializerScript>();

		isVisible = false;

		laneChangeTime = Time.time + laneChangeDelay;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
