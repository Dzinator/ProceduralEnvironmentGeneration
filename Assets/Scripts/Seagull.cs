using UnityEngine;
using System.Collections;

public class Seagull : Steerable {
//	Vector3 velocity;
//	float maxVelocity = 1f;
//	Vector3 desiredVelocity;
//
//	Vector3 steering;
//
//	float acceleration;
//	float speed = 10f;
//
//	public float mass = 1f;
//
//	public Transform target;
//	public Transform enemy;


	// Use this for initialization
	void Start () {
		//velocity = new Vector3 (100f, 0f, 100f);
		//velocity = this.transform.forward;

	
	}

	// Update is called once per frame
	void Update () {

		//call base class method to orbit
		currentSteering = seekAndOrbit (target.position);

		//clamp within some influence factor
		currentSteering = Vector3.ClampMagnitude (currentSteering, 0.05f);
		currentSteering = currentSteering / mass;

		velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);

		transform.Translate (velocity * speed * Time.deltaTime);

	}
	
	// Update is called once per frame
	void FunnyUpdate () {
		//seek
		//desiredVelocity = Vector3.Normalize (target.position - this.transform.position) * maxVelocity;
		//steering = desiredVelocity - velocity;

		Vector3 desiredVelocity = Vector3.Normalize (this.transform.position - target.position) * maxVelocity;
		Vector3 steering = velocity - desiredVelocity;

		
		//flee
		//desiredVelocity = Vector3.Normalize (this.transform.position - enemy.position) * maxVelocity;
		//Vector3 steering2 = desiredVelocity - velocity;

		//clamp within some influence factor
		steering = Vector3.ClampMagnitude (steering, 0.05f);
		steering = steering / mass;

//		steering2 = Vector3.ClampMagnitude (steering2, 0.05f);
//		steering2 = steering2 / mass;

		Vector3 steering2 = -steering; //flee

		//flee if enemy is within a certain distance
		if(Vector3.Distance(enemy.position, this.transform.position) < 10.0f)  
			velocity = Vector3.ClampMagnitude((velocity + steering2), maxVelocity);
		else
			velocity = Vector3.ClampMagnitude((velocity + steering), maxVelocity);

		Debug.DrawRay (transform.position, velocity, Color.green, Time.deltaTime);
		Debug.DrawRay (transform.position, -desiredVelocity, Color.blue, Time.deltaTime);
//		Debug.DrawRay (velocity, steering, Color.green, Time.deltaTime);
//		Debug.DrawRay (velocity, steering2, Color.red, Time.deltaTime);
		//euler integration
		//Vector3 newPos = transform.position + velocity;
		transform.Translate (velocity * speed * Time.deltaTime);
	
	}
}
