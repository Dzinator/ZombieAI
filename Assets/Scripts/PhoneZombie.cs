using UnityEngine;
using System.Collections;

public class PhoneZombie : Zombie {
	InitializerScript init;

	//attributes
	float laneChangeTime;
	float laneChangeDelay = 1.0f;
	float laneChangeP = 0.05f;

	float speedChangeTime;
	float speedChangeDelay = 0.5f;

	float stopTime;
	float stopDelay = 1.0f;
	float stopP = 0.2f;
	float stopDuration = 0.0f;

	float directionChangeTime;
	float directionChangeDelay = 1.0f;
	float directionChangeP = 0.1f;
	
	void FixedUpdate() {
		if (isVisible) {
			halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
		}
		else {
			halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
		}

		//randomly change speed
		if (Time.time > speedChangeTime) {
			//randomly select between 2v and v/2
			currSpeed = Random.Range((maxSpeed/4.0f), maxSpeed);
			speedChangeTime = Time.time + speedChangeDelay;
		}

		//randomly change lane
		if (Random.value < laneChangeP && Time.time > laneChangeTime) {
			laneChangeTime = Time.time + laneChangeDelay;
			changeLane();
		}

		//randomly change direction
		if (Random.value < directionChangeP && Time.time > directionChangeTime) {
			directionChangeTime = Time.time + directionChangeDelay;
			//switching direction
			int t = iEnd;
			iEnd = iStart;
			iStart = t;
			Vector3 v = end;
			end = start;
			start = v;

			direction = -direction;

		}
		
		//handle zombie stopping randomly
		if(Random.value < stopP && Time.time > stopTime) {
			//zombie randomely stops
			stopDuration = Random.Range(0.5f, 3.0f);
			stopTime = Time.time + stopDelay + stopDuration;
			currState = State.Halted;
			halted();
		}
		else if(Time.time < stopTime - stopDelay) {
			//we still in stop delay, we halt
			currState = State.Halted;
			halted();
		}
		else {
			float d = distZombieAhead ();
			if (d > slowingDist) {
				currState = State.Walking;
			} else if (d > stopDist) {
				currState = State.Decelerating;
			} else {
				currState = State.Halted;
			}
			
			//execute appropriate state
			if (Time.time > laneChangeTime) {
				laneChangeTime = Time.time + laneChangeDelay;
				
				if (currState == State.Walking) {
					walking ();
				} else if (currState == State.Decelerating) {
					changeLane();
				} else {
					changeLane();
				}
			} else {
				if (currState == State.Walking) {
					walking ();
				} else if (currState == State.Decelerating) {
					decelerating (d);
				} else {
					halted ();
				}
			}
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
		laneChangeTime = Time.time + laneChangeDelay;
		directionChangeTime = Time.time + directionChangeDelay;
		stopTime = Time.time + stopDelay;
		speedChangeTime = Time.time + speedChangeDelay;

		init = (InitializerScript)GameObject.Find ("Initializer").GetComponent<InitializerScript>();

		isVisible = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}