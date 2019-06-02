using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class RemoveUnderwaterTrees
{
	[MenuItem("SWTG/Remove Underwater Trees")]
	public static void Execute() 
	{
		var terrain = GameObject.FindObjectOfType<Terrain>();
		Debug.Log("Removing trees below water level on " + terrain.name);

		// get the width and depth of the terrain
		var terrainSize = terrain.terrainData.size;
		//Debug.Log( "terrainSize : " + terrainSize );

		// get the tree data from the terrain data
		var treeInstances = terrain.terrainData.treeInstances;
		Debug.Log("Old : Total Trees = " + treeInstances.Length);

		// create a list to store the modified information
		var newTreeInstances = new List<TreeInstance>();

		// calculate the normalized Water Level
		var normalizedWaterLevel = 0.001f;

		// cycle through each tree
		for (var t = 0; t < treeInstances.Length; t++ )
             {
			// check if the tree Y is lower than the water level
			if (treeInstances[t].position.y > normalizedWaterLevel) {
				// if not, add tree to newTreeInstances List
				newTreeInstances.Add(treeInstances[t]);
			}
		}

		// apply newTreeInstances List to terrain data
		terrain.terrainData.treeInstances = new TreeInstance[newTreeInstances.Count];
		terrain.terrainData.treeInstances = newTreeInstances.ToArray();
		Debug.Log("New : Total Trees = " + terrain.terrainData.treeInstances.Length);
	}
}
