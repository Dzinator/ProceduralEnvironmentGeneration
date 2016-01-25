using UnityEngine;
using System.Collections;

public class Steerable : MonoBehaviour {
	/*
	 * Base class providing steering behaviour methods and necessary fields
	 *
	 */

	//velocity of the object
	public Vector3 velocity;
	protected float maxVelocity = 2f;

	//cumulative force applied to the velocity of the object
	protected Vector3 currentSteering;
	
	protected float speed = 10f;
	
	public float mass = 1f;
	
	protected bool stopped = false;
	
	protected float slowingRadius = 10f;
	
	//for seek and flee behaviours
	public Transform target;
	public Vector3 point;
	public Transform enemy;

	public Steerable leader;

	//time related fields
	protected float nextCue;
	protected float cueStep = 2f;


	protected int pathIndex = 0; //used in path following method


	/*Steering methods*/

	protected Vector3 wander(){
		//wander using circle ahead method
		
		float circleDist = 1f;
		
		Vector3 circleCenter = velocity;
		//circleCenter = Vector3.Lerp (transform.position, velocity, 0.5f);
		circleCenter = Vector3.ClampMagnitude(circleCenter, circleDist);
		/*Debug ray drawing*/
		Debug.DrawRay (transform.position, circleCenter, Color.blue);
		Debug.DrawRay (transform.position, velocity, Color.green);
	

		Vector3 rand = Random.onUnitSphere;
		//rand = rand.normalized;
		Vector3 displacement = new Vector3 (rand.x, rand.y, rand.z);
		displacement = circleCenter + displacement;
		
//		Debug.DrawRay (transform.position, displacement, Color.black);
//		displacement = Vector3.ClampMagnitude (displacement, 0.2f);
//		displacement = displacement / mass;
		
		return displacement;
		
	}

	protected Vector3 seek(Vector3 t, bool hasArrival){
		/*seek towards a vector3D point in space, seeking steering behaviour
		 * @param t the point to seek towards
		 * @param hasArrival true if the seek should show arrival behaviour
		 * @return seeking force
		 */
		
		
		//arrival behaviour
		Vector3 desiredVelocity = transform.position - t;
		float dist = Vector3.Distance (transform.position, t);
		if(dist < slowingRadius && hasArrival) {
			desiredVelocity = Vector3.Normalize (t - transform.position) * maxVelocity * (dist / slowingRadius);
		}else{
			desiredVelocity = Vector3.Normalize (t - transform.position) * maxVelocity;
		}
		
		Vector3 steering = desiredVelocity - velocity;
		//clamp within some influence factor
//		steering = Vector3.ClampMagnitude (steering, 0.2f);
//		steering = steering / mass;
		
		
		Debug.DrawRay (transform.position, velocity, Color.black, Time.deltaTime);
		Debug.DrawRay (transform.position, desiredVelocity, Color.blue, Time.deltaTime);
		return steering;
	}
	
	protected Vector3 flee(Vector3 t){
		/*flee from a vector3D position in space, fleeing steering behaviour
		 * @param t the point to flee from
		 * @return fleeing force
		 */ 
		
		//seek steering behaviour
		
		Vector3 desiredVelocity = Vector3.Normalize (transform.position - t) * maxVelocity;
		Vector3 steering = desiredVelocity - velocity;
		//clamp within some influence factor
//		steering = Vector3.ClampMagnitude (steering, 0.05f);
//		steering = steering / mass;
		
		
		Debug.DrawRay (transform.position, velocity, Color.black, Time.deltaTime);
		Debug.DrawRay (transform.position, desiredVelocity, Color.blue, Time.deltaTime);
		return steering;
		
	}

	protected Vector3 seekAndOrbit(Vector3 t) {
		/*Seek alternative that orbits around target point when it's close enough
		 * * @param t the point to seek towards
		 * @return seeking force
		 */

		Vector3 desiredVelocity = Vector3.Normalize (this.transform.position - t) * maxVelocity;
		Vector3 steering = velocity - desiredVelocity;


	
		//clamp within some influence factor
//		steering = Vector3.ClampMagnitude (steering, 0.05f);
//		steering = steering / mass;

		return steering;

	}



	protected Vector3 separation(float personalSpace){
		/*Separates an object from other objects in the vicinity that are tagged "Follower"
		 * * @param personalSpace distance within which we start separating from other objects
		 * @return separation force
		 */
		Vector3 force = Vector3.zero;
		
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, personalSpace);
		int i = 0;
		while (i < hitColliders.Length) {
			if(hitColliders[i].tag == "Follower") {
				//flee in proportion to closeness of neighbor
				force += flee(hitColliders[i].transform.position) * (Vector3.Distance(transform.position, hitColliders[i].transform.position)/personalSpace);
			}
			i++;
		}
		
		//		if (neighborCount != 0) {
		//			force /= neighborCount;
		//			force = -force;
		//		}
		
		//Vector3.Normalize (force);
		//force = force * 2f; // max separation 
		
		//force = Vector3.ClampMagnitude (force, 0.019f);
		//force = force / mass;
		
		Debug.DrawRay (transform.position, force, Color.yellow);
		
		return force;
		
	}
	/*Evade and pursuit?*/

	protected Vector3 followPath(Transform[] waypoints, float arrivalDist) {
		/*Produces a steering towards waypoints of a path
		 * * @param waypoints Vector3 points composing the path to follow
		 * * @param arrivalDist distance to cosider that we reached a waypoint
		 * @return following force
		 */
		if (waypoints.Length == 0)
			return Vector3.zero;


		if (Vector3.Distance (transform.position, waypoints [pathIndex].position) < arrivalDist) {
			//check if we arrived at next waypoint, if so update path index
			if(pathIndex + 1 == waypoints.Length) {
				pathIndex = 0;
				//Debug.Log ("Following "+pathIndex);
			}
			else {
				pathIndex++;
				//Debug.Log ("Following "+pathIndex);
			}
		}

		Vector3 nextWypt = waypoints [pathIndex].position;

		Vector3 steering = seek (nextWypt, true);

//		steering = Vector3.ClampMagnitude (steering, 0.019f);
//		steering = steering / mass;

		return steering;

	}

	protected Vector3 avoidTerrain(float threshold) {
		/*
		 * Scans towards the velocity direction to see if there is terrain, produces fleeing force
		 * 
		 */

		Vector3 steering = Vector3.zero;

		RaycastHit hit;
		if (Physics.Raycast (transform.position, velocity, out hit)) {
			//check for terrain
			if(hit.collider.tag == "Terrain") {
				if(hit.distance < threshold) {
					steering = flee(hit.point);
				}
			}
		}
		return steering;
		
	}

	protected float findTerrainHeight(){
		//Raycast up or down to find terrain and return the object diference towards the terrain height
		float height = 0f;

		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			//check downwards for terrain
			if(hit.collider.tag == "Terrain") {
				height = hit.point.y; 
			}
		} else {
			if(Physics.Raycast (transform.position, Vector3.up, out hit)){
			//check upwards for terrain
				if(hit.collider.tag == "Terrain") {
					height = hit.point.y; 
				}
			}
			else height = 0;
		}

		return height;
	}

	protected Vector3 followLeader(Steerable leader, float followingDist){
		/*Produces a steering towards poit just behind a leader
		 * * @param leader object to follow as the leader
		 * * @param followingDist the distance to which we follow the leader 
		 * @return following force
		 */

		Vector3 tv = -leader.velocity;
		tv = Vector3.Normalize (tv) * followingDist;
		Vector3 behindPos = leader.transform.position + tv;
		
		
		Vector3 steering = seek (behindPos, true);
		//steering = steering + separation (1f);

//		steering = Vector3.ClampMagnitude (steering, 0.02f);
//		steering = steering / mass;
		
		return steering;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
