using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour {
	Vector3 velocity;
	float maxVelocity = 2f;
	
	float speed = 15f;
	
	public float mass = 1f;
	
	//bool stopped = false;
	
	float slowingRadius = 10f;
	
	
	public Wander leader;
	float followingDist = 5f;
	//public Transform enemy;
	Transform airplane;


//	float nextCue;
//	float cueStep = 2f;
	// Use this for initialization
	void Start () {
		airplane = GameObject.Find ("Airplane").GetComponent<Transform>();
		//get leader
		leader = GameObject.FindGameObjectWithTag ("Leader").GetComponent<Wander>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 tv = -leader.velocity;
		tv = Vector3.Normalize (tv) * followingDist;
		Vector3 behindPos = leader.transform.position + tv;


		Vector3 currentSteering = seek (behindPos, false);
		currentSteering = currentSteering + separation ();

		if(Vector3.Distance(airplane.position, this.transform.position) < 20.0f)  
			velocity = Vector3.ClampMagnitude((velocity + flee(airplane.position)), maxVelocity);
		else
			velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);



		transform.Translate (velocity * speed * Time.deltaTime);
		Vector3 temp = transform.position;
		temp.y = 0;
		transform.position = temp;


	
	}

	Vector3 separation(){
		Vector3 force = Vector3.zero;
		//int neighborCount = 0;

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
		int i = 0;
		while (i < hitColliders.Length) {
			if(hitColliders[i].tag == "Follower") {
				//flee in proportion to closeness of neighbor
				force += flee(hitColliders[i].transform.position) * (Vector3.Distance(transform.position, hitColliders[i].transform.position)/1.5f);
			}
			i++;
		}

//		if (neighborCount != 0) {
//			force /= neighborCount;
//			force = -force;
//		}

		//Vector3.Normalize (force);
		//force = force * 2f; // max separation 

		force = Vector3.ClampMagnitude (force, 0.019f);
		force = force / mass;

		Debug.DrawRay (transform.position, force, Color.yellow);

		return force;

	}

	Vector3 seek(Vector3 t, bool hasArrival){
		//seek steering behaviour
		
		
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
		steering = Vector3.ClampMagnitude (steering, 0.02f);
		steering = steering / mass;
		
		
		Debug.DrawRay (transform.position, velocity, Color.black, Time.deltaTime);
		Debug.DrawRay (transform.position, desiredVelocity, Color.blue, Time.deltaTime);
		return steering;
	}
	
	Vector3 flee(Vector3 t){
		//flee stering behaviour
		
		//seek steering behaviour
		
		Vector3 desiredVelocity = Vector3.Normalize (transform.position - t) * maxVelocity;
		Vector3 steering = desiredVelocity - velocity;
		//clamp within some influence factor
		steering = Vector3.ClampMagnitude (steering, 0.05f);
		steering = steering / mass;
		
		
		Debug.DrawRay (transform.position, velocity, Color.black, Time.deltaTime);
		Debug.DrawRay (transform.position, desiredVelocity, Color.blue, Time.deltaTime);
		return steering;
		
	}
}
