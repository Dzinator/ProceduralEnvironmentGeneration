using UnityEngine;
using System.Collections;

public class Populator : MonoBehaviour {

	public Generator gen;

	public FishSchool fishes;
	public BirdFlock birds;
	public Goat goat;

	public int nbFishSchools = 10;
	public int nbGoats = 10;
	public int nbFlocks = 10;

	float[,] hm;
	int step;

	// Use this for initialization
	void Start () {

		//fishes.Initialize (new Vector3 (1000, -2, 1000), 10);
		//int fishCount = 5;
		int fishCount = 0;
		int goatCount = 0;
		int birdCount = 0;


		hm = gen.heightMap;
		step = gen.step;

		int meshSide = gen.meshSideLength;
		int featureSize = gen.featureSize;

		// positio in world space veector3 ( x * step, hm[x,z], z*step)

			//goats
		for (int x = 0; x < meshSide; x++) {
			for (int z = 0; z < meshSide; z++) {
				if(hm[x, z] >= 10f && Random.value < 0.01 && goatCount<=nbGoats) {
					//spawn goats
					Goat g = (Goat) (Instantiate (goat)) as Goat;
					g.transform.position = new Vector3 (x*step, hm[x, z]+2f, z*step);
					goatCount++;
				}
			
			}
		}
			//fishes
		while (fishCount <= nbFishSchools) {
			int x = Random.Range(0, meshSide);
			int z = Random.Range(0, meshSide);
			if(hm[x, z] < 0f) {
				//spawn fishes
				fishes.Initialize (new Vector3 (x*step, -2, z*step), 5);
				fishCount++;
			}
		}


		//birds
		for (int x = 0; x < meshSide; x += featureSize) {
			for (int z = 0; z < meshSide; z += featureSize) {

				if(hm[x, z] > step && Random.value < 0.4 && birdCount<=nbFlocks) {
					//spawn  birds
					birds.Initialize (new Vector3 (x*step, hm[x,z] + 10f, z*step), 4);
					birdCount++;
				}
			}
		}

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
