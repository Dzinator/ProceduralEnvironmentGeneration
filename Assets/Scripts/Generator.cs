using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;
using System.IO;

public class Generator : MonoBehaviour {
	
	#region public declarations
	//size of initial generated mesh ( sizeX by sizeZ) (only power's of 2 are allowed)
	public int sizeX;
	public int sizeZ;

	//initial heightmap seed distance between points
	public int featureSize;

	//size of mesh (only power's of 2 are allowed)
	public int meshSideLength;

	//how many units (meters) are spaced between vectors
	public int step;

	//difference in elevation needing clamping to smooth jagged edges
	public float thresholdClampHeight;

	//number of iterations of diamondSquare
	public int nNoiseIterations;

	//texture will have textureDetail ^ 2 pixels as texture
	public int textureDetail;

	//maximum range of displacement for an iteration of the noise value
	public float randomDisplacement;

	//for external seeding
	public bool readHeightMapSeedFile;
	public TextAsset heightMapSeedFile;

	//components needed to generate meshes
	public GameObject meshHolderPrefab;
	public Material meshMaterial;

	//thresholds of height when deciding on texture terrain
	public float[] textureHeightThresholds; // ( same size as the number of texture posted below, must be in increasing order)
	
	//textures for terrain
	public Texture2D[] textureHeightValues;
	public Color defaultTextureColor;

	//script for updating terrain meshes dependent upon a game object
	public UpdateTerrain updateTerrain;
	#endregion

	#region private members
	//height map for starting mesh
	public float [,] heightMap;

	//number of meshes needed to generate the terrain
	int nMeshes = 1;
	
	Dictionary<string, Mesh> meshes;
	Vector3[] edgeVector_x;
	Vector3[] edgeVector_z;
	#endregion

	// Use this for initialization
	void Awake () 
	{
		//testing stuff
		thresholdClampHeight = randomDisplacement / 20f;

		//check if we need more than 1 mesh to generate terrain
		if ( sizeX > meshSideLength && sizeZ > meshSideLength)
		{
			//calcualte number of meshes required
			nMeshes = (sizeX / meshSideLength);
		}
		
		//initialize heightMap
		heightMap = new float[sizeX, sizeZ];
		
		//generate heightMap from noise produced by diamonSquareAlgorithm
		diamondSquareHeightMapGeneration();

		//clamp featurePoints
		clampHeightVertices(heightMap);

		//create and generate meshes
		createInitialMeshes();

		//initialize parameters for UpdateTerrain
		//updateTerrainScript.initialize(); // not fully implemented
	}
	
	//updates the HeightMap values according to the a random seeding or file read and the number of iterations specified 
	void diamondSquareHeightMapGeneration()
	{
		//parse input seed file
		if ( readHeightMapSeedFile)
		{
			try
			{
				parse(heightMapSeedFile.text);
			}
			catch ( System.Exception e)
			{
				//error, seed randomly
				Debug.Log ("could not read file, seeding heightMap randomly");
				seedHeightMapRandomly();
			}
		}
		else
			seedHeightMapRandomly(); //seed randomly

		//update height map using diamond square algorithm with the specified number of iterations
		for ( int i = 1; i <= nNoiseIterations; i++)
			heightMap = new DiamondSquare().generateHeightMap(heightMap, sizeX, sizeZ, featureSize, step * randomDisplacement); //24f
	}

	void createInitialMeshes()
	{
		//generate meshes
		for(int mx = 0; mx < nMeshes; mx++)
		{
			for(int mz = 0; mz < nMeshes; mz++)
			{
				//create mesh and its needed attributes
				Mesh mesh = new Mesh();
				Vector3[] vertices = new Vector3[meshSideLength * meshSideLength]; //create vertex array
				int[] triangles = new int[ 6 * (meshSideLength - 1) * (meshSideLength - 1)]; //generate triangles array
				Vector2[] textUV = new Vector2[vertices.Length]; //texture array
				Vector3[] normals = new Vector3[vertices.Length]; //create normals array

				//fill vertices array
				for(int i = 0; i < meshSideLength; i++)
					for(int j = 0; j < meshSideLength; j++)
						vertices[ j + i * meshSideLength] = new Vector3(j * step, heightMap[j + mx * meshSideLength, i + mz * meshSideLength], i * step); 

				//fill triangles array
				int index = 1;
				for ( int i = 0; i < triangles.Length; i += 6) //iterate through triangle array two triangles (a square) at a time (6 vertices)
				{
					//asign first triangle in square
					triangles[i] = index;
					triangles[i + 1] = index -1;
					triangles[i + 2] = (index - 1) + meshSideLength ;
					
					//assign second triagnle in square
					triangles[i + 3] = index;
					triangles[i + 4] = (index - 1) + meshSideLength ;
					triangles[i + 5] = index + meshSideLength ;
					
					//update index
					if( index % meshSideLength == (meshSideLength - 1))
						index += 2;
					else
						index++;
				}
			
				//set texture coordinates and normals
				for ( int z = 0; z < meshSideLength; z++)
				{
					for ( int x = 0 ; x < meshSideLength; x++)
					{
						//calculate index
						int ind = x + z * meshSideLength;
						
						//split all vertices on the texture evenly apart with step  1 / meshSideLength
						textUV[ind] = new Vector2(x / ((float) meshSideLength), z / ((float) meshSideLength));
						
						//assign normal array
						normals[ind] = Vector3.up; //new Vector3(Random.value, Random.value, Random.value);//Vector3.up;
					}
				}
			
				//generate texture for given vertices
				Texture2D texture = generateTexture(vertices, meshSideLength / 8, step * randomDisplacement / 24f);

				//assign mesh variables
				mesh.vertices = vertices;
				mesh.triangles = triangles;
				mesh.uv = textUV;
				mesh.normals = normals;
				mesh.name = "mesh " + mx + "," + mz;

				//create child object of parent generator to render mesh
				GameObject meshHolder = (GameObject) Instantiate(meshHolderPrefab) as GameObject;
				meshHolder.name = "mesh " + mx + "," + mz;

				//assign newly created mesh
				meshHolder.GetComponent<MeshFilter>().mesh = mesh;

				//get material from newly created object
				meshHolder.GetComponent<MeshRenderer>().material = meshMaterial;
				meshHolder.GetComponent<MeshRenderer>().material.mainTexture = texture;
				meshHolder.GetComponent<MeshCollider>().sharedMesh = mesh;
				//make generator parent of mesh renderer object
				meshHolder.transform.parent = transform;

				//place mesh in the world
				meshHolder.transform.position = new Vector3(mx * meshSideLength * step - step * mx - 1, 0f, mz * meshSideLength * step - step * mz - 1);

				//add mesh to upDateTerrain generation script
				//updateTerrain.meshesInScene.Add(mx + "," + mz, mesh);
			}
		}
	}

	//clamps the height of the vertices if a vertex exceeds the heightClampthreshold
	void clampHeightVertices(float[,] heightMap)
	{
		for ( int x = 0; x < meshSideLength; x+= featureSize)
		{
			for ( int z = 0; z < meshSideLength; z+= featureSize)
			{
				//create array for adjacent vertices
				ArrayList adjacentVertices = new ArrayList(4); //max of 4 adjacent vertices
				
				//try and access all adjcent vertices
				try { adjacentVertices.Add(heightMap[(x - 1), z ]);} catch ( System.Exception e) {} //left vertex
				try { adjacentVertices.Add(heightMap[(x + 1), z ]);} catch ( System.Exception e) {} //right vertex
				try { adjacentVertices.Add(heightMap[ x,  z - 1]);} catch ( System.Exception e) {} //lower vertex
				try { adjacentVertices.Add(heightMap[ x,  z + 1]);} catch ( System.Exception e) {} //upper vertex

				//counter for number of vertices that given vertex (x,z) exceeds threshold
				int counter = 0;

				//max height value for adjacent nodes
				float max = float.MinValue;

				//check if vertex (x,z) is above threshold when compared to all neighbours
				foreach (float adjHeight in adjacentVertices)
				{
					//check threshold
					if ( Mathf.Abs(heightMap[x, z] - adjHeight) > thresholdClampHeight)
						counter++;
					//update max height
					if ( adjHeight > max)
						max = adjHeight;
				}
				
				//check if given vertex (x,z) exceeds threshold on all its neighbours
				if ( counter == adjacentVertices.Count)
					heightMap[x, z] = max; // + thresholdClampHeight / 4f;
			}
		}
	}

	//generates a texture depending on the height map for a given mesh
	Texture2D generateTexture(Vector3[] vertices, int featSize, float maxHeightRangeChange) 
	{
		//featSize is the featureSize for the given array
		//maxHeightRangeChange is the maximum height that can be changed in the noise generation

		//create new texture with detail specified by user
		Texture2D texture = new Texture2D(textureDetail, textureDetail);

		//get noise from height of each vertex in mesh
		float[,] noise = new float[meshSideLength, meshSideLength];
	
		//iterate over all points in the mesh
		for ( int z = 0; z < meshSideLength ; z++ )
		{
			for ( int x = 0 ; x < meshSideLength ; x++)
			{
				//get height value of given vertex
				float height = vertices[ (x ) + meshSideLength * (z)].y;

				//update noise height value
				noise[x, z] = height;
			}
		}

		//get noise value for terrain height
		noise = new DiamondSquare().generateHeightMap(noise, meshSideLength, meshSideLength, featSize, maxHeightRangeChange);
		
		//ratio to convert from mesh coordinates to pixel coordinates
		float step_ratio = meshSideLength / ((float) textureDetail);
	
		//iterate over all pixels in texture
		for ( int z = 0; z < textureDetail ; z++)
		{
			for ( int x = 0 ; x < textureDetail ; x++)
			{
				//get height of noise value at given point
				float height = noise[ (int) (step_ratio * x), (int) (step_ratio* z)];

				//boolean to tell if we filled pixel (x,z)
				bool filled = false;

				//iterate over all heights thresholds
				for ( int i = 0 ; i < textureHeightThresholds.Length; i++)
				{
					//check if we satisfay height threshold
					if ( height < textureHeightThresholds[i])
					{
						//set pixxel in texture
						texture.SetPixel(x, z, textureHeightValues[i].GetPixel(Random.Range(0, textureHeightValues[i].width ), Random.Range(0, textureHeightValues[i].height)));

						//update bool
						filled = true;

						//break to prevent multiple pixel overwrites
						break; 
					}
				}

				//check if pixel was filled, if not assign default texture color
				if ( !filled)
					texture.SetPixel(x, z, textureHeightValues[textureHeightValues.Length - 1].GetPixel(Random.Range(0, textureHeightValues[textureHeightValues.Length - 1].width ), Random.Range(0, textureHeightValues[textureHeightValues.Length - 1].height)));
			}
		}

		//set texture parameters
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		
		//apply texture
		texture.Apply();

		//return newly created texture
		return texture;
	}

	void parse(string file_content)
	{
		//Debug.Log ("text contains " + file_content);
		//split lines
		string[] lines = file_content.Split(new char[] {'\n'});
		//Debug.Log ("lines : " + lines.Length);

		//first line feature size
		featureSize = int.Parse(lines[0]);
		//Debug.Log ("feature size " + featureSize);

		//second line is meshSideLength
		meshSideLength = int.Parse(lines[1]);
		//Debug.Log ("mesh side length " + meshSideLength);
		
		//second line size_x and size_z
		sizeX = int.Parse(lines[2]);
		sizeZ = int.Parse(lines[3]);


		//Debug.Log ("sizeX " + sizeX + " sizeZ " + sizeZ);

		//seed heightMap
		for ( int x = 4 ; x < lines.Length ; x++)
		{

			string[] zHeights = lines[x].Split(' '); //new char[] {' '}
			//Debug.Log ("hello + " + zHeights.Length);
			for ( int z = 0; z < zHeights.Length; z++)
			{
				//Debug.Log("value is " + zHeights[z]);
				//if ( float.Parse(zHeights[z]) == 0f || float.Parse(zHeights[z]) == 1f || float.Parse(zHeights[z]) == -1f)
					//heightMap[(x - 3) * featureSize, z * featureSize] = Random.value * randomDisplacement * step; //update this here
				//else
					heightMap[(x - 4) * featureSize, z * featureSize] = float.Parse(zHeights[z]) * step /** randomDisplacement * featureSize*/; //update this here
			}
		} 
	}
	
	//seeds the heightMap by placing random numbers featureSize apart
	void seedHeightMapRandomly()
	{
		//seed feature size points first
		for ( int z = 0 ; z < sizeZ; z += featureSize)
		{
			for ( int x = 0; x < sizeX; x += featureSize)
			{
				heightMap[x,z] = Random.Range(-1f, 1f) * randomDisplacement * step;
			}
		}
	}

	//used in update terrain, not important since update terrain not fully implemented
	public void generateMesh(int x, int  z, float[,] heightMap)
	{
		Debug.Log ("generating mesh " + x + "," + z);
		//create mesh and its needed attributes
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[meshSideLength * meshSideLength]; //create vertex array
		int[] triangles = new int[ 6 * (meshSideLength - 1) * (meshSideLength - 1)]; //generate triangles array
		Vector2[] textUV = new Vector2[vertices.Length]; //texture array
		Vector3[] normals = new Vector3[vertices.Length]; //create normals array
		
		//fill vertices array
		for(int i = 0; i < meshSideLength; i++)
			for(int j = 0; j < meshSideLength; j++)
				vertices[ j + i * meshSideLength] = new Vector3(j * step, heightMap[j, i], i * step); 
		
		//fill triangles array
		int index = 1;
		for ( int i = 0; i < triangles.Length; i += 6) //iterate through triangle array two triangles (a square) at a time (6 vertices)
		{
			//asign first triangle in square
			triangles[i] = index;
			triangles[i + 1] = index -1;
			triangles[i + 2] = (index - 1) + meshSideLength ;
			
			//assign second triagnle in square
			triangles[i + 3] = index;
			triangles[i + 4] = (index - 1) + meshSideLength ;
			triangles[i + 5] = index + meshSideLength ;
			
			//update index
			if( index % meshSideLength == (meshSideLength - 1))
				index += 2;
			else
				index++;
		}
		
		//set texture coordinates and normals
		for ( int zz = 0; zz < meshSideLength; zz++)
		{
			for ( int xx = 0 ; xx < meshSideLength; xx++)
			{
				//calculate index
				int ind = xx + zz * meshSideLength;
				
				//split all vertices on the texture evenly apart with step  1 / meshSideLength
				textUV[ind] = new Vector2(xx / ((float) meshSideLength), zz / ((float) meshSideLength));
				
				//assign normal array
				normals[ind] = Vector3.up; //new Vector3(Random.value, Random.value, Random.value);//Vector3.up;
			}
		}
		
		//generate texture for given vertices
		Texture2D texture = generateTexture(vertices, meshSideLength / 8, step * randomDisplacement / 24f);
	
		
		//assign mesh variables
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = textUV;
		mesh.normals = normals;
		mesh.name = "mesh " + x + "," + z;

		//create child object of parent generator to render mesh
		GameObject meshHolder = (GameObject) Instantiate(meshHolderPrefab) as GameObject;
		meshHolder.name = "mesh " + x + "," + z;
		
		//assign newly created mesh
		meshHolder.GetComponent<MeshFilter>().mesh = mesh;
		
		//get material from newly created object
		meshHolder.GetComponent<MeshRenderer>().material = meshMaterial;
		meshHolder.GetComponent<MeshRenderer>().material.mainTexture = texture;
		
		//make generator parent of mesh renderer object
		meshHolder.transform.parent = transform;
		
		//place mesh in the world
		meshHolder.transform.position = new Vector3(x * meshSideLength * step - x * step, 0f, z * meshSideLength * step - z * step);
		/*Debug.Log ("meshHolder pos : " + meshHolder.transform.position);
		//add mesh to upDateTerrain generation script
		Debug.Log ("key to add to dict is : " + (x + "," + z) + " does it contain it? " + updateTerrain.meshesInScene.ContainsKey((x + "," + z)));
		Debug.Log ("printing entries in dictionary : ");
		foreach ( KeyValuePair<string, Mesh> c in updateTerrain.meshesInScene)
		{
			Debug.Log (c.Key);
		}*/
		updateTerrain.meshesInScene.Add(x + "," + z, mesh);
	}
}