using UnityEngine;
using System.Collections;

public class Survivor : MonoBehaviour {

	bool hasWon = false;
	bool hasLost = false;

	public Transform target;
	public Transform finish;
	public ArrayList coins;
	int coinI = 0;

	public ArrayList alcoves;
	//int alcoveI = 0;
	NavMeshAgent agent;


	InitializerScript init;

	bool isZombieDangerous(Zombie z) {
		bool result = false;

		Vector3 zToS = this.transform.position - z.transform.position;
		Vector3 horizontal = Vector3.Project(zToS, z.direction);
		float vert = (zToS - horizontal).magnitude;
		float hori = horizontal.magnitude;

		float dot = Vector3.Dot(zToS, z.direction);

		float hMax = 0.0f;
		float vMax = 0.0f;


		switch(z.name){
		case "ClassicZombie":
			hMax = 12.0f;
			vMax = 3.0f;
			break;
		case "ShamblerZombie":
			hMax = 12.0f;
			vMax = 4.0f;
			break;
		case "ModernZombie":
			hMax = 12.0f;
			vMax = 4.0f;
			break;
		case "PhoneZombie":
			hMax = 12.0f;
			vMax = 4.0f;
			break;

		}


		if(dot >= 0) {
			//zombie facing survivor
			if(hori < hMax && vert < vMax) {
				result = true;
			}
		}
		else {
			//Zombie behind survivor
			if(z.name == "PhoneZombie"){
				if(hori < hMax && vert < vMax) {
					result = true;
				}
			}
			else{
				if(hori < 3 && vert < 3) {
					result = true;
				}
			}
		}

		
		return result;

	}

	bool lookAround() {
		//Survivor casts ray in direction of all zombies and if it hits them they are set visible

		bool atLeastOne = false; //this is true if a zombie is imminent threat

		foreach (Zombie z in InitializerScript.zombies) {

			try {
				//safety radius of 10 around survivor
//				if(Vector3.Distance(this.transform.position, z.transform.position) < 9.0f /*&& !z.isEasy*/) {
//					atLeastOne = true;
//				}
				//check if zombie is imminent danger
				if(isZombieDangerous(z)) atLeastOne = true;

				//computing zombies' visibility
				RaycastHit hit;
				if (Physics.Raycast(transform.position, z.transform.position - transform.position, out hit, 50.0f)) {
					Debug.DrawRay(transform.position, z.transform.position - transform.position) ;
					if(hit.collider.tag == "Zombie") {
						//Ray saw zombie, set it visible
						z.isVisible = true;
					}
					else if(hit.collider.tag == "Corners") {}
					else {
						z.isVisible = false;
					}
					//float distanceToGround = hit.distance;
				
				}

			} catch (System.Exception ex) {
				Debug.Log(ex);
				//z.isVisible = false;
				continue;
			}
		}
		
		return atLeastOne;

	}

	bool isDead(){
		//This method checks if one of the zombies has seen the survivor

		bool result = false;
		foreach (Zombie z in InitializerScript.zombies) {
			//check if zombie can see survivor
			Vector3 zToS = this.transform.position - z.transform.position;
			Vector3 horizontal = Vector3.Project(zToS, z.direction);
			float vert = (zToS - horizontal).magnitude;
			float hori = horizontal.magnitude;
			//dot product to know zombie direction relative to player
			float dot = Vector3.Dot(zToS, z.direction);

			if(dot >= 0) {
				if(hori < 7.5 && vert < 1.5) {
					result = true;
					break;
				}
			}
			else {
				if(hori < 1.5 && vert < 1.5) {
					result = true;
					break;
				}
			}	
		}

		return result;
	}


	int getClosestCoin() {
		//returns closest coin index
		float minDist = 10000.0f;
		int minCoin = -1;
		for(int i = 0; i < coins.Count; i++) {
			if(((Transform)coins[i]).gameObject.activeInHierarchy){
				Transform t = (Transform) coins[i];
				if (Vector3.Distance(transform.position, t.position) < minDist) {
					minDist = Vector3.Distance(transform.position, t.position);
					minCoin = i;
				}
			}
		}
		//Debug.Log("Smallest is;" + minCoin);
		return minCoin;

	}

	int getClosestAlcove() {
		//returns closest alcove index
		float minDist = 10000.0f;
		int minAlc = 0;
		for(int i = 0; i < alcoves.Count; i++) {
			Transform t = (Transform) alcoves[i];
			if (Vector3.Distance(transform.position, t.position) < minDist) {
				minDist = Vector3.Distance(transform.position, t.position);
				minAlc = i;
			}

		}
		//Debug.Log("Smallest is;" + minCoin);
		return minAlc;
		
	}

	// Use this for initialization
	void Start () {
		//get initializer
		init = (InitializerScript)GameObject.Find ("Initializer").GetComponent<InitializerScript>();
		agent = GetComponent<NavMeshAgent> ();

		//get finish
		finish = GameObject.Find ("FinishPoint").transform;

		//get Coins
		coins = new ArrayList();
		GameObject[] aCoins = GameObject.FindGameObjectsWithTag ("Coin");
		foreach (GameObject t in aCoins) {
			coins.Add(t.transform);
		}

		//get alcoves
		alcoves = new ArrayList();
		GameObject[] alcs = GameObject.FindGameObjectsWithTag ("Alcove");
		foreach (GameObject a in alcs) {
			alcoves.Add(a.transform);
		}

		coinI = getClosestCoin ();
		target = (Transform)coins[coinI];

		//set appropriate speed 1.5v
		agent.speed = 1.5f * init.v;
	}
	
	// Update is called once per frame
	void Update () {

		/* Hard coded Behaviour Tree */

		if (!lookAround ()) {
			//No zombies in sight, go to coins

			//if no more coins go to finish
			if(coinI >= 0)target = (Transform)coins [coinI];
			else target = finish;
			//No zombie in sight we go to the loot
			if (!target.gameObject.activeInHierarchy) {
				coinI = getClosestCoin ();
				if (coinI == -1) {
					target = finish;
				} else {
					target = (Transform)coins [coinI];
				}
			}
		} else if(!hasWon){
			//Go hide from zombie in nearest alcove
			target = (Transform)alcoves[getClosestAlcove()];

		}

		//win conditions
		if (!hasWon && Vector3.Distance(this.transform.position, finish.position) < 0.2f) {
			hasWon = true;
			Debug.Log ("Success");
		}

		//loose conditions
		if (isDead () && !hasLost) {
			hasLost = true;
			Time.timeScale = 0;
			Debug.Log ("Game Over");
		}

		//move towards choosen destination
		agent.SetDestination(target.position);
	
	}
}
