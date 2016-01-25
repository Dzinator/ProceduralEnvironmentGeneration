using UnityEngine;
using System.Collections;

public class FollowingFish : Steerable {

	Transform airplane;

	// Use this for initialization
	void Start () {
		maxVelocity = 2;
		airplane = GameObject.Find ("Airplane").GetComponent<Transform>();
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 followSteering = followLeader (leader, 0.5f);
		followSteering = Vector3.ClampMagnitude (followSteering, 0.02f);
		followSteering = followSteering / mass;

		Vector3 separationSteering = currentSteering + separation (2f);
		separationSteering = Vector3.ClampMagnitude (separationSteering, 0.018f);
		separationSteering = separationSteering / mass;

		//velocity = Vector3.ClampMagnitude((velocity + followSteering + separationSteering), maxVelocity);

		if(Vector3.Distance(airplane.position, transform.position) < 20.0f)  
			velocity = Vector3.ClampMagnitude((velocity + flee(airplane.position)), maxVelocity);
		else
			velocity = Vector3.ClampMagnitude((velocity + followSteering + separationSteering), maxVelocity);

		

		transform.Translate (velocity * speed * Time.deltaTime);
		Vector3 temp = transform.position;
		temp.y = -2;
		transform.position = temp;
	
	}
}
