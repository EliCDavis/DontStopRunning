using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class StructureFactory  {

	public static Vector2 structureDimensions(StructureType structure){
		switch (structure) {
			
		case StructureType.Fort:
			return new Vector2(25,10);

		case StructureType.Building:
			return new Vector2(10,10);

		case StructureType.Hut:
			return new Vector2(5,5);

		case StructureType.Substantial:
			return new Vector2(30,30);

		default:
			Debug.LogError("Structure Dimensions Have Not Been Defined!");
			return Vector2.zero;
		}
	}

	public static Structure createStructure(StructureType structure, Area area, Vector3 position, float civValue){

		GameObject obj = buildStructure (structureDimensions (structure), area.getFlatSeed() + area.getStructuresInArea().Length, area.getBiome (), civValue);
		obj.transform.position = position;

		return new Structure(structure, obj, area);

		GameObject sturctReference;

		string themeFolderDirectoryName;
		switch (area.getBiome()) {
			
		case AreaBiome.Test:
			themeFolderDirectoryName = "Test";
			break;
			
		case AreaBiome.Forest:
			themeFolderDirectoryName = "Forest";
			break;
			
			
		case AreaBiome.Desert:
			themeFolderDirectoryName = " Desert";
			break;
			
		default:
			Debug.LogError("Area biome not defined!");
			return null;
		}


		switch (structure) {
			
			case StructureType.Fort:
			sturctReference = Resources.Load("Structures/Civilization/"+themeFolderDirectoryName+"/Fort")as GameObject;
				break;
				
			case StructureType.Building:
			sturctReference = Resources.Load("Structures/Civilization/"+themeFolderDirectoryName+"/Building")as GameObject;
				break;
				
				
			case StructureType.Hut:
			sturctReference = Resources.Load("Structures/Civilization/"+themeFolderDirectoryName+"/Hut")as GameObject;
				break;
				
			default:
				Debug.LogError("Structure Model Has Not Been Defined!");
				return null;
		}

		GameObject structureObject = Object.Instantiate (sturctReference, position, Quaternion.identity)as GameObject;

		return new Structure(structure,structureObject,area);

	}

	/// <summary>
	/// Builds a list of all the structures that will fit within the area assigned.  
	/// Hopefully will make it so that it sorts all structures for smallest to biggest.(But not garunteed).
	/// </summary>
	/// <returns>The stucture fit in plot.</returns>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public static StructureType[] whatStuctureFitInPlot(int width, int height){
		List<StructureType> typesThatFit = new List<StructureType>();


		if (structureDimensions (StructureType.Hut).x <= width && structureDimensions (StructureType.Hut).y <= height) {
			typesThatFit.Add(StructureType.Hut);
		}

		
		if (structureDimensions (StructureType.Building).x <= width && structureDimensions (StructureType.Building).y <= height) {
			typesThatFit.Add(StructureType.Building);
		}

		if (structureDimensions (StructureType.Fort).x <= width && structureDimensions (StructureType.Fort).y <= height) {
			typesThatFit.Add(StructureType.Fort);
		}
		if (structureDimensions (StructureType.Substantial).x <= width && structureDimensions (StructureType.Substantial).y <= height) {
			typesThatFit.Add(StructureType.Substantial);
		}


		return typesThatFit.ToArray ();

	}


	/// <summary>
	/// This will build a structure that fits within the dimensions
	/// based on the seed passed in.
	/// </summary>
	/// <returns>The structure.</returns>
	/// <param name="dimensions">Dimensions.</param>
	/// <param name="seed">Seed.</param>
	/// <param name="theme">Theme.</param>
	public static GameObject buildStructure(Vector2 dimensions, int seed, AreaBiome theme, float civValue){


		// First we begin by determining how many blocks we can fit in the x and y dimensions
		// plotDimensions is how many modules you can fit in the structure.
		float blockDimension = 3.0f;
		Vector2 plotDimensions = new Vector2( Mathf.FloorToInt( dimensions.x / blockDimension ), Mathf.FloorToInt( dimensions.y / blockDimension ));


		// Create random blueprints
		int[][] blueprints = generateStructureBlueprints (plotDimensions, seed, civValue);

		// If there are no blue prints don't go any further
		if(blueprints == null){
			return null;
		}


		// Build from the blueprints
		GameObject structure = new GameObject ("Structure: "+seed+", "+dimensions.ToString());

		for(int x = 0; x < blueprints.Length; x ++){
			
			for(int y = 0; y < blueprints[x].Length; y ++){
			
				// Get the number of floors wanted from this module's position.
				int floors = blueprints[x][y];
			
				// Move to the next portion if there's nothin to build here.
				if(floors <= 0){
					continue;
				}

				// Build a module for each floor
				for(int f = 0; f < floors; f ++){

					// Just use a cube for now
					GameObject module = null;

					int numOfWalls = numOfModulesTouching(blueprints, x, y);

					if(numOfWalls == 1 && f == floors-1){
					
						module = Resources.Load("Structures/Civilization/Basic/1STop") as GameObject;
						module = GameObject.Instantiate(module);
					
					} else {

						module = GameObject.CreatePrimitive(PrimitiveType.Cube);
						module.transform.localScale = Vector3.one*blockDimension;
					
					}

					// Make sure the cube is apart of the parent
					module.transform.SetParent(structure.transform);

					// Set the appropriate position nd scale
					module.transform.localPosition = (new Vector3(x,f,y))*blockDimension;


				}
			
			}
			
		}

		return structure;
	}


	static int[][] generateStructureBlueprints(Vector2 plotDimensions, int seed, float civValue){

		if( civValue > 1f || civValue < 0f){
			Debug.LogError("The civ value you passed in was rediculous, val: "+civValue);
			return null;
		}

		// Set up our seed so we get same results with same seed every time
		Random.seed = seed;

		// Create blueprints which represent how many floors high the module will be.
		int[][] blueprints = new int[(int)plotDimensions.x][];

		for (int x = 0; x < blueprints.Length; x ++) {
			blueprints[x] = new int[(int)plotDimensions.y];
		}
		
		// Initialize the blueprints to 0 to represent no modules.
		for(int x = 0; x < blueprints.Length; x ++){
			
			for(int y = 0; y < blueprints[x].Length; y ++){
				blueprints[x][y] = 0;
			}
			
		}

		// Determine the height of the base, determined by the civ value and the plot.
		int heightOfBase = 0;

		float magOfPlot = plotDimensions.magnitude;

		heightOfBase = Mathf.CeilToInt(Mathf.Sqrt (magOfPlot));

		heightOfBase = Mathf.RoundToInt(heightOfBase*civValue*2.6f);

		if(heightOfBase <= 0){
			return null;
		}

		// Generate base of the structure
		for(int x = 0; x < blueprints.Length; x ++){
			
			for(int y = 0; y < blueprints[x].Length; y ++){

				// Make it more likely to generate base modules towards the center of the plot

				// Distance from middle
				Vector2 curPos = new Vector2(x,y);
				Vector2 middle = new Vector2(blueprints.Length / 2f, blueprints[x].Length/2f);
				float distance = Vector2.Distance(curPos, middle);

				// Percentage of how close we are to the middle
				float closeness = 1.0f - (distance / middle.magnitude);

				// If we're close enough to the center to be considered part of the base.
				// Random is thrown in to add more of a variation, bases are not all the same.
				if(0.5f + Random.Range(0f,0.5f) < closeness + Mathf.Sqrt(closeness*civValue*.1f) ){
					blueprints[x][y] = heightOfBase;
				}

			}
			
		}

		// Go through the blue prints and find holes or stray modules
		for(int x = 0; x < blueprints.Length; x ++){
			
			for(int y = 0; y < blueprints[x].Length; y ++){

				int numofWalls = numOfModulesTouching (blueprints, x, y);
				
				// If we're not a valid position then flatten this module out
				if(numofWalls == 0){
					blueprints[x][y] = 0;
				}
				
				
				// If we know this is a hole then let's fill it in!
				if(numofWalls == 4){
					blueprints[x][y] = heightOfBase;
				}

			}

		}

		// If the height is one there's no need to add any apendages
		if(heightOfBase <= 1){
			return blueprints;
		}

		// Add appendages appropriately
		for (int x = 0; x < blueprints.Length; x ++) {
			
			for (int y = 0; y < blueprints[x].Length; y ++) {

				if(blueprints[x][y] == 0){

					// Can we even add an appendage?
					if(numOfModulesTouching(blueprints, x, y) != 0){

						// Likelyhood of adding a new appendage
						if(Random.Range(0f,1f) < civValue){

							// Get height of appendage..
							int heightOfAppendage = 1+ (int)(Random.Range(0,heightOfBase)*civValue);

							// Set the appendage 
							blueprints[x][y] = heightOfAppendage;

						}

					}

				}

			}

		}

		// Go through and fill in holes again.
		for(int x = 0; x < blueprints.Length; x ++){
			
			for(int y = 0; y < blueprints[x].Length; y ++){

				if(blueprints[x][y] == 0){
					int numofWalls = numOfModulesTouching (blueprints, x, y);
					
					
					
					// If we know this is a hole then let's fill it in!
					if(numofWalls == 4){
						
						// Get height of appendage..
						int heightOfAppendage = 1+ (int)(Random.Range(0,heightOfBase)*civValue);
						
						blueprints[x][y] = heightOfAppendage;
					}
				}


				
			}
			
		}


		return blueprints;

	}

	/// <summary>
	/// The number of modules
	/// </summary>
	/// <returns>The of modules touching.</returns>
	/// <param name="blueprints">Blueprints.</param>
	/// <param name="xCord">X cord.</param>
	/// <param name="yCord">Y cord.</param>
	static int numOfModulesTouching(int[][] blueprints, int xCord, int yCord){

		int numOfWalls = 0;

		// Build a list of valid positions to check.
		Vector2[] posToCheck = new Vector2[4];
		
		if(xCord + 1 < blueprints.Length){
			posToCheck[0] = new Vector2(xCord+1, yCord);
		}
		
		if(xCord - 1 >= 0){
			posToCheck[1] = new Vector2(xCord-1, yCord);
		}
		
		if(yCord + 1 < blueprints[xCord].Length){
			posToCheck[2] = new Vector2(xCord, yCord+1);
		}
		
		if(yCord - 1 >= 0){
			posToCheck[3] = new Vector2(xCord, yCord-1);
		}

		
		for(int p = 0; p < posToCheck.Length; p ++){
			
			// Is there a valid position on a certain side?
			if(posToCheck[p] != null){
				
				if( blueprints[(int)posToCheck[p].x][(int)posToCheck[p].y] != 0){
					
					numOfWalls ++;
					
				}
				
			}

			
		}
				
				

		return numOfWalls;

	}

}
