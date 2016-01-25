using UnityEngine;
using System.Collections;
using System;

//class that computes height values using the diamondSquareAlgorithm
public class invDiamondSquare
{
	float[,] heightMap;
	int size_x;
	int size_z;
	bool ignoreNorth = false, ignoreSouth = false, ignoreEast = false, ignoreWest = false;
	float h;
	bool i = false;
	
	public float[,] generateHeightMap(float[,] seededHeights, int sizeX, int sizeZ, int featureSize, float randomDisplacement /*, int[] ignoreSides*/ ,  bool b)
	{
		//assign class parameters
		heightMap = seededHeights;
		size_x = sizeX;
		size_z = sizeZ;
		i = b;
		h = randomDisplacement;
		//ignoreNorth = b;
		/*if ( ignoreSides != null)
			for ( int i = 0; i < ignoreSides.Length; i++)
		    {
				if ( ignoreSides[i] == 0) ignoreNorth = true;
				else if (ignoreSides[i] == 1) ignoreSouth = true;
				else if (ignoreSides[i] == 2) ignoreEast = true;
				else if (ignoreSides[i] == 3) ignoreWest = true;
			}*/
		
		//assign 
		float scale = 1f;
		int sampleSize = featureSize;
		
		while ( sampleSize > 1)
		{
			diamondSquareAlg(sampleSize, scale);
			
			sampleSize /= 2;
			scale /= 2;
		}
		
		//return newly calculated HeightMap
		return heightMap;
	}
	
	public void diamondSquareAlg(int step_size, float scale)
	{
		int half_step = step_size / 2;
		
		for ( int z = half_step; z < size_z + half_step; z += step_size)
		{
			for ( int x = half_step; x < size_x + half_step; x += step_size)
			{
				setSquareHeight(x, z, step_size, UnityEngine.Random.Range(-1f, 1f) * scale * h );
			}
		}
		
		for ( int z = 0; z < size_z; z += step_size)
		{
			for ( int x = 0; x < size_x; x += step_size)
			{
				setDiamondHeight(x + half_step, z, step_size, UnityEngine.Random.Range(-1f, 1f)  * scale * h ); //HERE, RANDOM VALUE
				setDiamondHeight(x, z + half_step, step_size, UnityEngine.Random.Range(-1f, 1f) * scale * h ); //HERE, RANDOM VALUE
			}
		}
	}
	
	//returns value of height at position (x,z)
	public float getHeight(int x, int z)
	{
		try
		{
			return heightMap[x, z];
		}
		catch ( Exception e) //index out of bounds
		{
			return float.MinValue;
		}
	}
	
	//sets value of height at position (x,z)
	public void setHeight(int x, int z, float value)
	{
		try
		{
			//if ( i && x == 63 || z == 63 || x == 0 || z == 0)
			//return;
			//if ( ignoreNorth && x == 63 || z == 63 || x == 0 || z == 0)
			//return;
			/*if ( ignoreNorth && x == (size_x - 1)) return;
			else if ( ignoreSouth && x == 0) return;
			else if ( ignoreEast && z == 0) return;
			else if ( ignoreWest && z == (size_z - 1)) return;*/
			
			//check if we ignore borders of the heigth map
			//if ( ignoreSides && (x == (size_x - 1) || x == 0 || z ==0 || z == (size_z - 1)))
			//return;
			//else
			
			//else
			heightMap[x, z] = value;
			return;
		}
		catch (Exception e) //index out of bounds
		{
			return;
		}
	}
	
	//sets the height value of mid point of square
	public void setSquareHeight(int x, int z, int size, float value)
	{
		//calculate half step size
		int half_step = size / 2;
		
		// a     b 
		//
		//    x
		//
		// c     d
		
		float 
			a = getHeight(x - half_step, z - half_step),
			b = getHeight(x + half_step, z - half_step),
			c = getHeight(x - half_step, z + half_step),
			d = getHeight(x + half_step, z - half_step);
		
		//check for index out of bounds
		float count = 4f;
		if ( a == float.MinValue) 
		{
			a = 0;
			count--;
		}
		if ( b == float.MinValue) 
		{
			b = 0;
			count--;
		}
		if ( c == float.MinValue)
		{
			c = 0;
			count--;
		}
		if ( d == float.MinValue)
		{
			d = 0;
			count--;
		}
		
		//calcuate height of midpoint as average of 4 corners
		float centerPointHeight = (a + b + c + d) / count + value;
		
		//assign height value to array
		setHeight(x,z, centerPointHeight);
	}
	
	//sets the height value of mid point of diamond
	public void setDiamondHeight(int x, int z, int size, float value)
	{
		//calculate half step size
		int half_step = size / 2;
		
		//   c
		//
		//a  x  b
		//
		//   d
		
		float 
			a = getHeight(x - half_step, z),
			b = getHeight(x + half_step, z),
			c = getHeight(x, z - half_step),
			d = getHeight(x, z + half_step);
		
		//check for index out of bounds
		float count = 4f;
		if ( a == float.MinValue) 
		{
			a = 0;
			count--;
		}
		if ( b == float.MinValue) 
		{
			b = 0;
			count--;
		}
		if ( c == float.MinValue)
		{
			c = 0;
			count--;
		}
		if ( d == float.MinValue)
		{
			d = 0;
			count--;
		}
		
		//calcuate height of midpoint as average of 4 corners
		float centerPointHeight = (a + b + c + d) / count + value;
		
		//assign height value to array
		setHeight(x,z, centerPointHeight);
	}
}