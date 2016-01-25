using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour {
	public Component halo;
	public bool isEasy;
	
	public enum Lane {
		Inner,
		Middle,
		Outer
	}
	public enum State
	{
		Walking,
		Decelerating,
		Halted
	};
	//current attributes
	public Lane currLane;
	public State currState;
	public float currSpeed;
	public float maxSpeed;
	bool isAccelerating;
	//movment attributes
	public Vector3 start;
	public Vector3 end;
	public Vector3 direction;
	public int iStart; //index
	public int iEnd;
	//collision avoidance attributes
	public float collisionDistance;
	protected float slowingDist = 3.0f;
	protected float stopDist = 0.05f;

	public bool isVisible;
	
	public void Initialize(Vector3 s, Vector3 e, int iS, int iE, int laneNb, float speed){
		//initialize zombie as walking
		currState = State.Walking;
		if(laneNb == 0) currLane = Lane.Inner;
		else if(laneNb == 1) currLane = Lane.Middle;
		else currLane = Lane.Outer;
		
		maxSpeed = speed;
		currSpeed = speed;
		start = s;
		end = e;
		iStart = iS;
		iEnd = iE;
		direction = (end - start).normalized;
	}
	
	/*methods corresponding to states (FSM)*/

	protected void walking(){
		if (currSpeed < maxSpeed)
			isAccelerating = true;
		
		if (isAccelerating) {
			//increase speed and make sure it's within range
			currSpeed += 0.3f;
			currSpeed = Mathf.Clamp(currSpeed, 0, maxSpeed);
		}
		
		transform.Translate (direction * currSpeed * Time.deltaTime);
	}
	
	protected void decelerating(float dist){
		//decrease speed
		currSpeed = maxSpeed * Mathf.Sqrt (dist / slowingDist);
		transform.Translate (direction * currSpeed * Time.deltaTime);
	}
	
	protected void halted() {
		currSpeed = 0f;
	}
	
	
	
	/*Helper methods*/
	protected float distZombieAhead() {
		//raycast to front of the zombie if there are other zombies
		RaycastHit hit;
		float dist = Vector3.Distance (transform.position, end);
		if ( Physics.Raycast(transform.position + 0.5f * direction, direction, out hit, dist))
		{
			if ( hit.collider.transform.tag.Equals("Zombie"))
			{
				Debug.DrawRay(transform.position +  0.5f * direction, direction * dist, Color.blue, 10, true);
				return hit.distance;
			}
		}
		return 33.0f; //size of map if no zombie
		
	}
	
	protected Vector3 linesIntersection(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2) {
		//finds intersection of 2 lines (line is set of 2 vectors)
		//need to put lines in the form Ax +By = C
		float A1 = e1.z - s1.z;
		float B1 = s1.x - e1.x;
		float C1 = A1 * s1.x + B1 * s1.z;
		
		float A2 = e2.z - s2.z;
		float B2 = s2.x - e2.x;
		float C2 = A2 * s2.x + B2 * s2.z;
		
		//using determinant
		float det = A1 * B2 - A2 * B1;
		return new Vector3 ((B2*C1 - B1*C2)/det , s1.y, (A1*C2 - A2*C1)/det);
		
	}
	
	protected void changeLane(){
		float laneSpan = 2.0f;
		//perpendicular line to lane
		Vector3 pDirection = new Vector3 (-direction.z, direction.y, direction.x);
		Vector3 pLaneStart = pDirection * laneSpan + transform.position;
		Vector3 pLaneEnd = -pDirection * laneSpan + transform.position;
		
		
		if (Vector3.Distance (transform.position, end) < 1f)
			return;
		
		if (currLane == Lane.Middle) {
			//can change to inner or outer lane
			Vector3 innerNewPos = linesIntersection(InitializerScript.innerPoints[iStart].position,
			                                        InitializerScript.innerPoints[iEnd].position,
			                                        pLaneStart, pLaneEnd);
			Vector3 outerNewPos = linesIntersection(InitializerScript.outerPoints[iStart].position,
			                                        InitializerScript.outerPoints[iEnd].position,
			                                        pLaneStart, pLaneEnd);
			//change to one of the lanes if not occupied with probability .5
			if(Random.value > 0.5f){
				//inner
				if(!Physics.CheckSphere(innerNewPos, 0.49f)) {
					start = InitializerScript.innerPoints[iStart].position;
					end = InitializerScript.innerPoints[iEnd].position;
					currLane = Lane.Inner;
					
					transform.position = innerNewPos;

				}
			}
			else{
				//outer
				if(!Physics.CheckSphere(outerNewPos, 0.49f)) {
					start = InitializerScript.outerPoints[iStart].position;
					end = InitializerScript.outerPoints[iEnd].position;
					currLane = Lane.Outer;
					
					transform.position = outerNewPos;

				}
			}
			
		} else {
			//either in inner or outer -->change to middle lane
			
			Vector3 newPosition = linesIntersection(InitializerScript.middlePoints[iStart].position,
			                                        InitializerScript.middlePoints[iEnd].position,
			                                        pLaneStart, pLaneEnd);
			//check if no zombie on other lane
			if(!Physics.CheckSphere(newPosition, 0.49f)) {
				start = InitializerScript.middlePoints[iStart].position;
				end = InitializerScript.middlePoints[iEnd].position;
				currLane = Lane.Middle;
				
				transform.position = newPosition;
			}
			
		}
		
		
	}

	void setVisible() {
		isVisible = true;

	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
