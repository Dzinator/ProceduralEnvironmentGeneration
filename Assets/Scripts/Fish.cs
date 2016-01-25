using UnityEngine;
using System.Collections;

public class Fish : Steerable {

	//public Transform[] path;

	Transform airplane;

	// Use this for initialization
	void Start () {
//		for (int i = 0; i < 5; i++) {
//			GameObject.Find ();
//		}
		//speed = 4;
		maxVelocity = 2;

		airplane = GameObject.Find ("Airplane").GetComponent<Transform>();
	
	}
	
	// Update is called once per frame
	void Update () {

		currentSteering = wander();
		
		//if(Vector3.Distance(enemy.position, transform.position) < 10.0f)  
		//	currentSteering = currentSteering + flee (enemy.position);
		currentSteering = Vector3.ClampMagnitude (currentSteering, 0.5f);
		currentSteering = currentSteering / mass;

		Vector3 fleeTerrain = avoidTerrain (10f);
		fleeTerrain = Vector3.ClampMagnitude (fleeTerrain, 0.5f);
		fleeTerrain = fleeTerrain / mass;
		//velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);


		if(Vector3.Distance(airplane.position, transform.position) < 20.0f)  
			velocity = Vector3.ClampMagnitude((velocity + flee(airplane.position) + fleeTerrain), maxVelocity);
		else
			velocity = Vector3.ClampMagnitude((velocity + currentSteering + fleeTerrain), maxVelocity);



		transform.Translate (velocity * speed * Time.deltaTime);
		Vector3 temp = transform.position;
		temp.y = -2;
		transform.position = temp;

	
	}
}
