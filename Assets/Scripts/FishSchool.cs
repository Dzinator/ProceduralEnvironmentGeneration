using UnityEngine;
using System.Collections;

public class FishSchool : MonoBehaviour {

	public int nbFishes;

	public Fish leaderPrefab;
	public FollowingFish followerPrefab;


	// Use this for initialization
	void Start () {

	
	}

	public void Initialize(Vector3 position, int schoolSize){
		nbFishes = schoolSize;
		//Debug.Log ("Generating " + nbFishes);

		Fish lead = (Fish) (Instantiate (leaderPrefab)) as Fish;
		lead.transform.position = position;


		for (int i = 0; i < nbFishes-1; i++) {
			FollowingFish follow = (FollowingFish) Instantiate (followerPrefab) as FollowingFish;
			follow.transform.position = new Vector3(Random.Range(position.x, position.x+50f), -2f, Random.Range(position.z, position.z+50f));
			follow.leader = lead;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
