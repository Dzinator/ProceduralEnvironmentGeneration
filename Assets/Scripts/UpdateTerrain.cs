using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateTerrain : MonoBehaviour {


	#region public members
	//target being looked after to check if we generate extra random terrain
	public GameObject plane;

	//reference to start generator script
	public Generator generator;

	//distance from target to start generating distant meshes
	public float thresholdDistanceToStartGeneration;

	public float thresholdDistanceToDeleteMesh;

	//dictionary for meshes in scene
	public Dictionary<string, Mesh> meshesInScene =  new Dictionary<string, Mesh>(50);
	#endregion

	#region private members

	enum Direction { N, S, E, W, NE, SE, SW, NW}; //standard 8 cardinal points

	/* Notes on direction

		In our game, 
				positive z direction  --> North	
				negative z direction  --> South

				positive x direction --> East
				negative x direction --> West


			z ( North)

			^
			|
			|
			*------>   x (East)
	 */

	public ArrayList meshesCurrentlyGenerating = new ArrayList(10);
	int nMeshesCrossedForGeneration;
	int nMeshesCrossedDiagonally;
	#endregion

	void Start() { 

		nMeshesCrossedForGeneration = Mathf.CeilToInt( thresholdDistanceToStartGeneration/ generator.meshSideLength );
		nMeshesCrossedDiagonally = Mathf.CeilToInt( thresholdDistanceToStartGeneration / Mathf.Sqrt( 2 * Mathf.Pow(generator.meshSideLength * generator.step, 2)));
	
	}


	// Update is called once per frame
	void Update () 
	{
		//get plane direction and position
		//Vector3 direction = plane.rigidbody.velocity.normalized;
		Vector3 position = plane.transform.position;

		//get indices of mesh underneath plane
		int index_x =  (int) (position.x / (generator.meshSideLength * generator.step));
		int index_z =  (int) (position.z / (generator.meshSideLength * generator.step));

		//check all meshes within distance of plane if they are generated
		//updateMissingMeshes(index_x, index_z, generator.meshSideLength);

		/*foreach(string s in meshesCurrentlyGenerating)
		{
			Debug.Log ("missing meshes are " + s);
		}*/

		//generate missing meshes
		//generateMissingMeshes();

		//




		//string key = index_x + "," + index_z;
		//Mesh currentMesh = meshesInScene[key];
	}

	//iterates over all meshes in the array
	void generateMissingMeshes()
	{
		Debug.Log ("meshes in currently generating : ");

		//iterate over all generating meshes
		foreach ( string meshName in meshesCurrentlyGenerating)
		{
			Debug.Log("strint key " + meshName);
			//get discrete mesh position
			string[] param = meshName.Split(',');
			int x = int.Parse(param[0]), z = int.Parse(param[1]);
			Debug.Log("in generate missing meshes parsed x : " + x + " z " + z);
			int seed_x_start = 0, seed_z_start = 0, seed_x_end = generator.meshSideLength, seed_z_end = generator.meshSideLength;

			//check if we share any sides with current meshes in scene, if so get their heigths on there shared border
			float[,] heigthMap = new float[generator.meshSideLength, generator.meshSideLength];
			
			/*Debug.Log ("printing heightmap BEFORE SEEDING ");
			for(int i = 0; i < generator.meshSideLength; i++)
			{
				string s= "";
				for ( int j = 0; j < generator.meshSideLength; j++)
				{
					s += heigthMap[i, j];
				}
				Debug.Log (s);
				s = "";
			}*/


			#region update HeightMap with shared borders
			ArrayList borders = new ArrayList(4);
			//north face shared
			try
			{
				//try and get mesh
				Mesh sharedBorder = meshesInScene[(x+1) + "," + z];
				Debug.Log ("north values ");
				string s = "";
				//iterate over all heights of current existing mesh
				for ( int xx = 0; xx < generator.meshSideLength; xx++)
				{
					//Debug.Log ("index in loop " + (xx * generator.meshSideLength));
					//heigthMap[xx, generator.meshSideLength - 1 ] = sharedBorder.vertices[xx * generator.meshSideLength].y;
					heigthMap[generator.meshSideLength - 1, xx] = sharedBorder.vertices[xx * generator.meshSideLength].y;
					s += heigthMap[generator.meshSideLength - 1, xx] + " ";
				}
				Debug.Log (s);
				s = "";
				seed_x_end -= generator.featureSize;
				borders.Add(0);
			}
			catch ( System.Exception e) {}
			//south face shared
			try
			{
				//try and get mesh
				Mesh sharedBorder = meshesInScene[(x-1) + "," + z];

				//iterate over all heights of current existing mesh
				for ( int xx = 0; xx < generator.meshSideLength; xx++)
				{
					heigthMap[0 , xx] = sharedBorder.vertices[(generator.meshSideLength - 1) + xx * generator.meshSideLength].y;
				}

				seed_x_start += generator.featureSize;
				borders.Add(1);
			}
			catch ( System.Exception e) {}
			//east face shared
			try
			{
				//try and get mesh
				Mesh sharedBorder = meshesInScene[(x) + "," + (z + 1)];
				
				//iterate over all heights of current existing mesh
				for ( int zz = 0; zz < generator.meshSideLength; zz++)
				{
					heigthMap[zz , generator.meshSideLength - 1] = sharedBorder.vertices[zz].y;
				}

				seed_z_start += generator.featureSize;
				borders.Add(2);
			}
			catch ( System.Exception e) {}
			//west face shared
			try
			{
				//try and get mesh
				Mesh sharedBorder = meshesInScene[(x) + "," + (z - 1)];
				
				//iterate over all heights of current existing mesh
				for ( int zz = 0; zz < generator.meshSideLength; zz++)
				{
					heigthMap[zz, 0] = sharedBorder.vertices[ (generator.meshSideLength - 1) * generator.meshSideLength + zz].y;
				}
				seed_z_end -= generator.featureSize;
				borders.Add(3);
			}
			catch ( System.Exception e) {}
			#endregion


			Debug.Log ("x start " + seed_x_start + " x end " + seed_x_end + " z start " + seed_z_start + " z end " + seed_z_end);


			//randomly seed the terrain
			for ( int i = seed_x_start; i < seed_x_end; i += generator.featureSize)
			{
				for ( int j = seed_z_start; j < seed_z_end; j += generator.featureSize)
				{
					heigthMap[i, j] = Random.Range(-1f, 1f) * generator.randomDisplacement * generator.step;
				}
			}



			Debug.Log ("printing heightmap after seeding");
			for(int i = 0; i < generator.meshSideLength; i++)
			{
				string s= "";
				for ( int j = 0; j < generator.meshSideLength; j++)
				{
					s += heigthMap[i, j] + " ";
				}
				Debug.Log (s);
				s = "";
			}
	
			//generate noise on heigthMap
			//for ( int c = 1; c <= generator.nNoiseIterations; c++) //((int[]) borders.ToArray(typeof(int)))
			Debug.Log ("sizex = size _z = " + generator.meshSideLength);
				heigthMap = new DiamondSquare().generateHeightMap(heigthMap, generator.meshSideLength, generator.meshSideLength, /*generator.sizeX /*/ generator.featureSize/** generator.meshSideLength*/, generator.randomDisplacement * generator.step / (6 /** c*/));

			/*Debug.Log ("printing heightmap after noise");
			for(int i = 0; i < generator.meshSideLength; i++)
			{
				string s= "";
				for ( int j = 0; j < generator.meshSideLength; j++)
				{
					s += heigthMap[i, j] + " ";
				}
				Debug.Log (s);
				s = "";
			}*/

			//generate mesh based on heigthMap
			generator.generateMesh(x, z, heigthMap);
		}

		//delete meshesCurrentlyBeingGenerated
		meshesCurrentlyGenerating = new ArrayList(10);
	}










	void updateMissingMeshes(int x, int z, int meshSideLength)
	{
		Debug.Log ("treshold for mesh creation " + thresholdDistanceToStartGeneration + " nMeshesCrossedForGeneration " + nMeshesCrossedForGeneration + " nMeshesCrossedDiagonally " + nMeshesCrossedDiagonally);

		//check messhes in North direction
		for (int i = 1; i <= nMeshesCrossedForGeneration; i++)
		{
			//Debug.Log ("North i : " + i);
			//check if mesh does not exist in scene
			if (!meshesInScene.ContainsKey((x + i) + "," + z))
			{
				//check if not already generating
				if ( !meshesCurrentlyGenerating.Contains((x + i) + "," + z))
					meshesCurrentlyGenerating.Add((x + i) + "," + z);
				break;
			}
		}

		//check meshes in South direction
		for (int i = 1; i <= nMeshesCrossedForGeneration; i++)
		{
			//Debug.Log ("South i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey((x - i) + "," + z))
			{
				//check if not already generating
				if ( !meshesCurrentlyGenerating.Contains((x - i) + "," + z))
					meshesCurrentlyGenerating.Add((x - i) + "," + z);
				break;
			}
		}

		//check meshes in East direction
		for (int i = 1; i <= nMeshesCrossedForGeneration; i++)
		{
			//Debug.Log ("East i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( x + "," + (z + i)))
			{
				//check if not already generating
				if ( !meshesCurrentlyGenerating.Contains(x + "," + (z + i)))
				    meshesCurrentlyGenerating.Add(x + "," + (z + i));
				break;
			}
		}

		//check meshes in West direction
		for (int i = 1; i <= nMeshesCrossedForGeneration; i++)
		{
			//Debug.Log ("West i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( x + "," + (z - i)))
			{
				//check if not already generating
				if ( !meshesCurrentlyGenerating.Contains(x + "," + (z - i)))
					meshesCurrentlyGenerating.Add(x + "," + (z - i));
				break;
			}
		}

		//check meshes in North-East direction
		for (int i = 1; i <= nMeshesCrossedDiagonally; i++)
		{
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( (x + i) + "," + (z + i)))
			{
				//Debug.Log ("NorthEast i : " + i);
				//check if we have the 2 required side meshes to generate the diagonal mesh
				if ( !meshesCurrentlyGenerating.Contains( (x) +  "," + (z + i)) 
				    &&  !meshesCurrentlyGenerating.Contains( (x + i) +  "," + (z) ) 
				    &&  !meshesCurrentlyGenerating.Contains((x + i) + "," + (z + i)))
				{
					meshesCurrentlyGenerating.Add((x + i) + "," + (z + i));
				}
				break;


				//CAREFUL HERE, MAYBE NEED THE SIDES TO GENERATE
				//generate mesh
				//add to generatingMesh arrylist
			}
		}

		//check meshes in North-West direction
		for (int i = 1; i <= nMeshesCrossedDiagonally; i++)
		{
			//Debug.Log ("NorthWest i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( (x + i) + "," + (z - i)))
			{
				//check if we have the 2 required side meshes to generate the diagonal mesh
				if ( !meshesCurrentlyGenerating.Contains( (x) +  "," + (z - i)) 
				    &&  !meshesCurrentlyGenerating.Contains( (x + i) +  "," + (z) ) 
				    &&  !meshesCurrentlyGenerating.Contains((x + i) + "," + (z - i)))
				{
					meshesCurrentlyGenerating.Add((x + i) + "," + (z - i));
				}
				break;
			}
		}

		//check meshes in South-West direction
		for (int i = 1; i <= nMeshesCrossedDiagonally; i++)
		{
			//Debug.Log ("SoutWest i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( (x - i) + "," + (z - i)))
			{
				//check if we have the 2 required side meshes to generate the diagonal mesh
				if ( !meshesCurrentlyGenerating.Contains( (x) +  "," + (z - i)) 
				    &&  !meshesCurrentlyGenerating.Contains( (x - i) +  "," + (z) ) 
				    &&  !meshesCurrentlyGenerating.Contains((x - i) + "," + (z - i)))
				{
					meshesCurrentlyGenerating.Add((x - i) + "," + (z - i));
				}
				break;
			}
		}

		//check meshes in South-East direction
		for (int i = 1; i <= nMeshesCrossedDiagonally; i++)
		{
			//Debug.Log ("SouthEast i : " + i);
			//check if mesh does not exist
			if (!meshesInScene.ContainsKey( (x - i) + "," + (z + i)))
			{
				//check if we have the 2 required side meshes to generate the diagonal mesh
				if ( !meshesCurrentlyGenerating.Contains( (x) +  "," + (z + i)) 
				    &&  !meshesCurrentlyGenerating.Contains( (x - i) +  "," + (z) ) 
				    &&  !meshesCurrentlyGenerating.Contains((x - i) + "," + (z + i)))
				{
					meshesCurrentlyGenerating.Add((x - i) + "," + (z + i));
				}
				break;
			}
		}
	}

















}
