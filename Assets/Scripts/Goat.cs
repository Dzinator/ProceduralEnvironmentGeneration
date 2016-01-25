using UnityEngine;
using System.Collections;

public class Goat : Steerable {

	// Use this for initialization
	void Start () {
		maxVelocity = 1;
		speed = 5;
		//velocity = new Vector3 (100f, 0f, 100f);

	
	}
	
	// Update is called once per frame
	void Update () {
		currentSteering = wander ();
		
		//if(Vector3.Distance(enemy.position, transform.position) < 10.0f)  
		//	currentSteering = currentSteering + flee (enemy.position);
		currentSteering = Vector3.ClampMagnitude (currentSteering, 0.5f);
		currentSteering = currentSteering / mass;
		
		velocity = Vector3.ClampMagnitude((velocity + currentSteering), maxVelocity);
		


		if (Random.value < 0.001 && !stopped){
			stopped = true;
			nextCue = Time.time + cueStep;
			
		}
		if (Time.time <= nextCue && stopped) {
			//stop for a while
			
		} else if (Time.time > nextCue && stopped) {
			stopped = false;
		} else {
			if(transform.position.y < 0) velocity = -velocity;
			transform.Translate (velocity * speed * Time.deltaTime);
			Vector3 temp = transform.position;
			temp.y = findTerrainHeight() + 2f;//temp.y - findTerrainHeight();
			transform.position = temp;
		}
	
	}
}
