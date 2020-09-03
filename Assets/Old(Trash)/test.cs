using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

		void Start () {
			
			Terrain terrain = GetComponent<Terrain>();
			
			// Get a reference to the terrain
			TerrainData terrainData = terrain.terrainData;
			
			// Populate an array with current height data
			float[,] orgHeightData = terrainData.GetHeights(0,0,terrainData.heightmapResolution, terrainData.heightmapResolution);
			
			// Initialise a new array of same size to hold rotated data
			float[,] mirroredHeightData = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
			
			for (int y = 0; y < terrainData.heightmapResolution; y++)
			{
				for (int x = 0; x < terrainData.heightmapResolution; x++)
				{
					
					// Rotate each element clockwise
					mirroredHeightData[y,x] = orgHeightData[terrainData.heightmapResolution - y - 1, x];
				}
			}
			
			// Finally assign the new heightmap to the terrainData:
			terrainData.SetHeights(0, 0, mirroredHeightData);
		}
	}