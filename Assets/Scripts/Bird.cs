using UnityEngine;
using System.Collections;

public class Bird : Steerable {

	Transform airplane;

	// Use this for initialization
	void Start () {
		target = new GameObject ().transform;
		airplane = GameObject.Find ("Airplane").GetComponent<Transform>();
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//call base class method to orbit
		currentSteering = seekAndOrbit (point);
		
		//clamp within some influence factor
		currentSteering = Vector3.ClampMagnitude (currentSteering, 0.05f);
		//randomize orbiting
		currentSteering = currentSteering + Vector3.ClampMagnitude (wander (), 0.04f);

		currentSteering = currentSteering / mass;

		if (Vector3.Distance (airplane.position, transform.position) < 50.0f) { 
			//follow plane if it gets close
			//Debug.Log("seeking plane");
			currentSteering = Vector3.zero;
			currentSteering = seekAndOrbit(airplane.position);
			currentSteering = Vector3.ClampMagnitude (currentSteering, 0.5f);
			currentSteering = currentSteering / mass;
			//velocity = Vector3.ClampMagnitude ((velocity + currentSteering), maxVelocity);
		}

		
		velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);
		
		transform.Translate (velocity * speed * Time.deltaTime);
	
	}
}
