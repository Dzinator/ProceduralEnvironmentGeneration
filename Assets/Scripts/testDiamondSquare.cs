using UnityEngine;
using System.Collections;
using System.IO;


//tests the diamondsquare noise generating algorithm
public static class testDiamondSquare {

	// Use this for initialization
	public static void testDiamondSquareAlg (int size) 
	{
		int size_height_map = size;
		Debug.Log("in start");
		//initialize heightMap
		float[,] heightMap = new float[size_height_map , size_height_map];


		for ( int featureSize = size_height_map / 8; featureSize < size_height_map; featureSize *= 2)
		{
			//seed heightMpa
			for ( int i = 0; i < featureSize; i++)
			{
				for ( int j = 0; j < featureSize; j++)
				{
					heightMap[i,j] = Random.Range(-1f, 1f);
				}
			}

			//use diamond square Algorithm
			heightMap = new DiamondSquare().generateHeightMap(heightMap, size_height_map, size_height_map, featureSize, 1);

			//write texture to disk
			generateTexture(heightMap, featureSize, size_height_map);

			//reset values of heightMap
			heightMap = new float[size_height_map, size_height_map];
		}

		//print to log photos generated
		Debug.Log ("path to file " + Application.dataPath );
	}
	
	//generates a png file to disk of the heightMap
	static void generateTexture(float[,] heightMap, int featureSize, int size_height_map) 
	{
		//create new texture with detail specified by user
		Texture2D texture = new Texture2D(size_height_map, size_height_map);

		//iterate over all pixels 
		for ( int i = 0; i < size_height_map; i++)
		{
			for ( int j = 0; j < size_height_map; j++)
			{
				texture.SetPixel(i, j, Color.Lerp(Color.white, Color.black, heightMap[i,j]));
			}
		}

		//write to disk file
		var bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../SavedScreen " + "featureSize" + featureSize + ".png", bytes);
	}
}
