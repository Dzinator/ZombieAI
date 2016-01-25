using UnityEngine;
using System.Collections;

public class InitializerScript : MonoBehaviour {
	//Game parameters as specified in assignment
	public int n; //nmber of zombies
	public float p; //probability of zombie spawn
	public float r; //easy zombies/ hard zombies
	//speed
	public float v;

	public static ArrayList zombies; //to hold zombies references
	//nb of zombies by difficulty
	//int easyZombies;
	int hardZombies;

	//prefabs
	public ClassicZombie classicPrefab;
	public ShamblerZombie shamblerPrefab;
	public ModernZombie modernPrefab;
	public PhoneZombie phonePrefab;

	//lanes corners
	public static Transform[] innerPoints;
	public static Transform[] middlePoints;
	public static Transform[] outerPoints;


	void spawnZombies() {
		hardZombies = (int) (n * r / (1 + r));

		int[] dir = new int[3];

		//iterate to spawn n zombies 
		for(int  i=0; i < n; i++) {
			//starting and ending waypoints on lane
			int startingWpt = 0;
			int endWpt = 0;
			//choose random lane
			int lane = Random.Range(0,3);
			//set direction
			if(dir[lane] == 0) {
				dir[lane] = Random.Range(1,3);
			}

			startingWpt = Random.Range(0,4);//choose random starting point
			if(dir[lane] == 1) {
				//left direction in lane

				//make sure no zombie spawns in survivor area at first
				startingWpt = Random.Range(0,4);
				while(startingWpt == 1) startingWpt = Random.Range(0,4);

				switch(startingWpt) {
				case 0:
					endWpt = 3;
					break;
				case 1:
					endWpt = 0;
					break;
				case 2:
					endWpt = 1;
					break;
				case 3:
					endWpt = 2;
					break;

				}


			}
			else if(dir[lane] == 2) {
				//right direction in lane
				//make sure no zombie spawns in survivor area at first
				startingWpt = Random.Range(0,4);
				while(startingWpt == 0) startingWpt = Random.Range(0,4);
				
				switch(startingWpt) {
				case 0:
					endWpt = 1;
					break;
				case 1:
					endWpt = 2;
					break;
				case 2:
					endWpt = 3;
					break;
				case 3:
					endWpt = 0;
					break;
					
				}

			}
			/*initialize zombies*/
			Vector3 s = Vector3.zero;
			Vector3 e = Vector3.zero;

			switch(lane) {
			case 0:
				s = innerPoints[startingWpt].position;
				e = innerPoints[endWpt].position;;
				break;
			case 1:
				s = middlePoints[startingWpt].position;
				e = middlePoints[endWpt].position;;
				break;
			case 2:
				s = outerPoints[startingWpt].position;
				e = outerPoints[endWpt].position;;
				break;
				
			}

			s = Vector3.Lerp(s, e, Random.value);

			//Check if there is a zombie already
			if(Physics.CheckSphere(s, 0.49f)) {
				i--;
				continue;
			}


			if(i < hardZombies) {
				//spawn a hard zombie
				if(Random.value > 0.5f) {
					//modern
					ModernZombie z = (ModernZombie) Instantiate(modernPrefab) as ModernZombie;
					z.Initialize(s, e, startingWpt, endWpt, lane, v*2.0f); //speed = 2v
					zombies.Add(z);
					z.transform.position = s;
					z.isEasy = false;
					z.transform.tag = "Zombie";
					z.name = "ModernZombie";

				}
				else {
					//phone
					PhoneZombie z = (PhoneZombie) Instantiate(phonePrefab) as PhoneZombie;
					z.Initialize(s, e, startingWpt, endWpt, lane, v*2.0f); //speed = 2v
					zombies.Add(z);
					z.transform.position = s;
					z.isEasy = false;
					z.transform.tag = "Zombie";
					z.name = "PhoneZombie";
				}


			}
			else {
				//spawn easy
				if(Random.value > 0.5f) {
					//classic zombie instanciation
					ClassicZombie z = (ClassicZombie) Instantiate(classicPrefab) as ClassicZombie;
					z.Initialize(s, e, startingWpt, endWpt, lane, v);
					zombies.Add(z);
					z.transform.position = s;
					z.isEasy = true;
					z.transform.tag = "Zombie";
					z.name = "ClassicZombie";
					
				}
				else {
					//shambler
					ShamblerZombie z = (ShamblerZombie) Instantiate(shamblerPrefab) as ShamblerZombie;
					z.Initialize(s, e, startingWpt, endWpt, lane, v/2.0f); //speed = v/2
					zombies.Add(z);
					z.transform.position = s;
					z.isEasy = true;
					z.transform.tag = "Zombie";
					z.name = "ShamblerZombie";
					
				}

			}


		}
	}


	void processZombie(Zombie z){
		//called when a zombie reaches a corner

		//Debug.Log ("Processing"+z);
		if (Random.value <= p) {
			//Debug.Log("Deletion");
			//we delete zombie with probability p
			zombies.Remove (z);
			bool easy = z.isEasy;
			bool isClockwise = false;
			if((z.iStart == 0 && z.iEnd == 3) || (z.iStart == z.iEnd + 1)){
				isClockwise = false;//counter clockwise
			}
			else{
				isClockwise = true; //clockwise
			}

			Destroy (z.gameObject);
			spawnOneZombie (easy, isClockwise);
		} else {
			//turn zombie around
			//get appropriate indinces and update start and end of zombie
			if((z.iStart == 0 && z.iEnd == 3) || (z.iStart == z.iEnd + 1)){
				//counter clockwise
				z.iStart = z.iEnd;
				z.iEnd = (z.iStart + 3)% 4;
			}
			else{
				//clockwise
				z.iStart = z.iEnd;
				z.iEnd = (z.iStart + 1)% 4;
			}

			if(z.currLane == Zombie.Lane.Inner) {
				z.start = innerPoints[z.iStart].position;
				z.end = innerPoints[z.iEnd].position;
			}
			else if(z.currLane == Zombie.Lane.Middle) {
				z.start = middlePoints[z.iStart].position;
				z.end = middlePoints[z.iEnd].position;
			}
			else {
				z.start = outerPoints[z.iStart].position;
				z.end = outerPoints[z.iEnd].position;
			}

			z.transform.position = z.start;
			z.direction = (z.end - z.start).normalized;
			//z.resetT = Time.time + 2.0f;
			//z.hasTurned = true;
		}
	}

	public void spawnOneZombie(bool easy, bool clockwiseDirection){
		int nextWpt = 0;
		int wptI = 0;
		int lane = 0;
		Vector3 spawnPos = Vector3.zero;
		Vector3 endWpt = Vector3.zero;

		//find random unnocopied corner to spawn on
		try {
			do {
				wptI = Random.Range (0, 4);
				lane = Random.Range (0, 3); 
				switch (lane) {
				case 0:
					spawnPos = innerPoints [wptI].position;
					break;
				case 1:
					spawnPos = middlePoints [wptI].position;
					break;
				case 2:
					spawnPos = outerPoints [wptI].position;
					break;
				}
			} while(Physics.CheckSphere(spawnPos, 0.49f));
		} catch (System.Exception ex) {
			Debug.Log(ex);
		}

		//choose random direction
		if (!clockwiseDirection) {
			//counter clockwise
			nextWpt = (wptI + 3) % 4;
		} else {
			//clockwise
			nextWpt = (wptI + 1) % 4;
		}

		if (lane == 0) {
			endWpt = innerPoints [nextWpt].position;
		} else if (lane == 1) {
			endWpt = middlePoints [nextWpt].position;
		} else {
			endWpt = outerPoints[nextWpt].position;
		}

		if(!easy) {
			//spawn a hard zombie
			if(Random.value > 0.5f) {
				//modern
				ModernZombie z = (ModernZombie) Instantiate(modernPrefab) as ModernZombie;
				z.Initialize(spawnPos, endWpt, wptI, nextWpt, lane, v*2.0f); //speed = 2v
				zombies.Add(z);
				z.transform.position = spawnPos;
				z.isEasy = false;
				z.transform.tag = "Zombie";
				z.name = "ModernZombie";
				
			}
			else {
				//phone
				PhoneZombie z = (PhoneZombie) Instantiate(phonePrefab) as PhoneZombie;
				z.Initialize(spawnPos, endWpt, wptI, nextWpt, lane, v*2.0f); //speed = 2v
				zombies.Add(z);
				z.transform.position = spawnPos;
				z.isEasy = false;
				z.transform.tag = "Zombie";
				z.name = "PhoneZombie";
			}
			
			
		}
		else {
			//spawn easy
			if(Random.value > 0.5f) {
				//classic zombie instanciation
				ClassicZombie z = (ClassicZombie) Instantiate(classicPrefab) as ClassicZombie;
				z.Initialize(spawnPos, endWpt, wptI, nextWpt, lane, v);
				zombies.Add(z);
				z.transform.position = spawnPos;
				z.isEasy = true;
				z.transform.tag = "Zombie";
				z.name = "ClassicZombie";
				
			}
			else {
				//shambler
				ShamblerZombie z = (ShamblerZombie) Instantiate(shamblerPrefab) as ShamblerZombie;
				z.Initialize(spawnPos, endWpt, wptI, nextWpt, lane, v/2.0f); //speed = v/2
				zombies.Add(z);
				z.transform.position = spawnPos;
				z.isEasy = true;
				z.transform.tag = "Zombie";
				z.name = "ShamblerZombie";
				
			}
			
		}

	}




	// Use this for initialization
	void Start () {
		//initialize lane waypoints
		innerPoints = new Transform[4];
		for (int i =0; i < 4; i++) {
			innerPoints[i] = GameObject.Find("Inner"+i).transform;
		}

		middlePoints = new Transform[4];
		for (int i =0; i < 4; i++) {
			middlePoints[i] = GameObject.Find("Middle"+i).transform;
		}

		outerPoints = new Transform[4];
		for (int i =0; i < 4; i++) {
			outerPoints[i] = GameObject.Find("Outer"+i).transform;
		}

		zombies = new ArrayList (n);

		spawnZombies ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
