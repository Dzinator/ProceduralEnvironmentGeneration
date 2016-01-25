using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour {
	public Vector3 velocity;
	float maxVelocity = 1.5f;

	float speed = 10f;

	public float mass = 1f;

	bool stopped = false;

	float slowingRadius = 10f;


	public Transform target;
	public Transform enemy;

	float nextCue;
	float cueStep = 2f;

	
	public Vector3 currentSteering;

	// Use this for initialization
	void Start () {
		velocity = new Vector3 (0f, 0f, 0f);

		currentSteering = Vector3.zero;
		nextCue = Time.time + cueStep;
	}

	
	// Update is called once per frame
	void Update () {

//		if(Vector3.Distance(enemy.position, transform.position) < 10.0f)  
//			currentSteering = flee (enemy.position);
//		else
		//currentSteering = seek (target.position, true);
		currentSteering = wander ();

		if(Vector3.Distance(enemy.position, transform.position) < 10.0f)  
						currentSteering = currentSteering + flee (enemy.position);

	
		velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);
		
//		transform.Translate (velocity * speed * Time.deltaTime);
//		Vector3 temp = transform.position;
//		temp.y = 0;
//		transform.position = temp;
		
		if (Random.value < 0.0 && !stopped){
			stopped = true;
			nextCue = Time.time + cueStep;
			
		}
		if (Time.time <= nextCue && stopped) {
			//stop for a while

		} else if (Time.time > nextCue && stopped) {
			stopped = false;
		} else {
			transform.Translate (velocity * speed * Time.deltaTime);
			Vector3 temp = transform.position;
			temp.y = 0;
			transform.position = temp;
		}


	}

	Vector3 wander(){
		//wander using circle ahead method

		float circleDist = 1f;

		Vector3 circleCenter = velocity;
		//circleCenter = Vector3.Lerp (transform.position, velocity, 0.5f);
		circleCenter = Vector3.ClampMagnitude(circleCenter, circleDist);
		Debug.DrawRay (transform.position, circleCenter, Color.blue);
		Debug.DrawRay (transform.position, velocity, Color.green);
		//Vector3.Normalize (circleCenter);

		//Vector3 displacement;// = circleCenter + velocity;
		//displacement = Vector3.Normalize (displacement);

		//displacement =  Quaternion.AngleAxis ((Random.value * Mathf.PI * 2f), Vector3.up) * displacement;
		Vector3 rand = Random.onUnitSphere;
		//rand = rand.normalized;
		Vector3 displacement = new Vector3 (rand.x, 0, rand.z);
		displacement = circleCenter + displacement;

		Debug.DrawRay (transform.position, displacement, Color.black);
		displacement = Vector3.ClampMagnitude (displacement, 0.2f);
		displacement = displacement / mass;

		return displacement;




	}

	Vector3 badWander() {
		if (Time.time > nextCue) {
			//new wandering direction
			Vector3 t = new Vector3 (Random.Range (-100f, 100f), 0, Random.Range (-100f, 100f));
			nextCue = Time.time + cueStep;
			return (seek (t, false));


		} else
			return currentSteering;
	}

	Vector3 badSeek(Vector3 t){
		//seek

		Vector3 steering;
		
		Vector3 desiredVelocity = Vector3.Normalize (this.transform.position - t) * maxVelocity;
		steering = velocity - desiredVelocity;
		//clamp within some influence factor
		steering = Vector3.ClampMagnitude (steering, 0.02f);
		steering = steering / mass;

		
		Debug.DrawRay (transform.position, velocity, Color.black, Time.deltaTime);
		Debug.DrawRay (transform.position, desiredVelocity, Color.blue, Time.deltaTime);
		//		Debug.DrawRay (velocity, steering, Color.green, Time.deltaTime);
		//		Debug.DrawRay (velocity, steering2, Color.red, Time.deltaTime);
		//euler integration
		//Vector3 newPos = transform.position + velocity;
		//transform.Translate (velocity * speed * Time.deltaTime);
		return steering;
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
