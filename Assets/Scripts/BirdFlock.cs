using UnityEngine;
using System.Collections;

public class BirdFlock : MonoBehaviour {

	public int nbBirds;
	
	public Bird birdPrefab;
	
	
	// Use this for initialization
	void Start () {
		
		
	}
	
	public void Initialize(Vector3 position, int flockSize){
		nbBirds = flockSize;
		//Debug.Log ("Generating " + nbFishes);
		
		for (int i = 0; i < nbBirds; i++) {
			Bird b = (Bird) Instantiate (birdPrefab) as Bird;
			b.transform.position = new Vector3(Random.Range(position.x, position.x+50f), position.y, Random.Range(position.z, position.z+50f));

			b.point = position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
