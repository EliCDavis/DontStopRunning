using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Linq; // used for Sum of array

/// <summary>
/// This generates a city by generating a random perlin noise map.
/// The map is then rounded off to only have the center to show.
/// The map is then sectioned off and evaluated to see if theres enough 
/// 'civilization' to build a building in that area.
/// 
/// USAGE:
/// Attatch to a gameobject in the scene.
/// I encourage playing around with the parameters to generate the city!
/// Click play to see results
/// </summary>
public class AreaGenerator : MonoBehaviour {

	// How wide and long a single section is when building the city.
	// A city is made of multiple sections
	public int dimensionOfSection = 30;


	/// <summary>
	/// The percentage needed to consider an area 'civilized'
	/// Number between 0 - 100
	/// </summary>
	public int civilizationNeededToBuild = 50;

	
	/// <summary>
	/// The percentage needed to consider an area to be split up
	/// and evaluated on it's parts.
	/// Number between 0 - 100
	/// Should be kept less than 'civilizationNeededToBuild'
	/// </summary>
	public int civilizationNeededToReconsider = 15;


	// Texture painted when the area is 'civilized' 
	public Texture2D civArea;


	// Basic texture painted when nothing specials going on
	public Texture2D uncivArea;


	/// <summary>
	/// Things such as trees or bushes that will spawn where no civilization lies.
	/// </summary>
	public GameObject[] uncivilizedObjects = null;


	/// <summary>
	/// When placing uncivilized objects, how close they should be together on average.
	/// Value from 0 to 1.
	/// </summary>
	public float uncivilizedObjectDensity = 0.5f;


	/// <summary>
	/// The integer area that is not occupied by any structure. used for path finding.
	/// </summary>
	List<Vector3> freeArea = new List<Vector3>();


	bool areaIsFree(int startX, int startY, int width, int height){
		bool free = true;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				
				Vector3 currentPos = new Vector3(startX+x,0,startY+y);

				if(!freeArea.Contains(currentPos)){
					free = false;
				}
				
			}
		}
		return free;
	}


	void removeFreeArea(int startX, int startY, int width, int height){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				Vector3 currentPos = new Vector3(startX+x,0,startY+y);

				if(freeArea.Contains(currentPos)){
					freeArea.Remove(currentPos);
				} else {
					//Debug.LogError("This area is not here to be remo");
				}

			}
		}
	}



	/// <summary>
	/// Averages all the pixels up in one color
	/// </summary>
	/// <returns>The avg color.</returns>
	Color getAvgColor(Texture2D pic){

		return  getAvgColor(pic.GetPixels ());

	}


	/// <summary>
	/// Averages all the pixels up in one color
	/// </summary>
	/// <returns>The avg color.</returns>
	Color getAvgColor(Color[] pic){

		Color[] pix = pic;

		// Totals of all red, blue, green, and alpha
		float totR = 0f;
		float totB= 0f;
		float totG= 0f;
		float totA= 0f;

		// Add them all up
		for (int i = 0; i < pix.Length; i++) {
			totR += pix[i].r;
			totB += pix[i].b;
			totG += pix[i].g;
			totA += pix[i].a;
		}

		// return the average of the totals.
		return new Color (totR/pix.Length,totB/pix.Length,totG/pix.Length,totA/pix.Length);
		
	}



	GameObject currentSectionBeingCreated; //buildStructureOnPlot() and createSection() use this
	void createSection (int xPlot, int yPlot, Texture2D mapOfSection)
	{

		// Populate the free area with all the new land we're going to try building on.
		for (int curXSec = 0; curXSec < dimensionOfSection; curXSec++) {
			for (int curYSec = 0; curYSec < dimensionOfSection; curYSec++) {

				Vector3 newPos = new Vector3( 
				                              ( xPlot *dimensionOfSection )+curXSec, 
				                              0, 
				                              ( yPlot *dimensionOfSection )+curYSec
				                             );

				freeArea.Add(newPos);

			}	
		}

		// Determine civilization level.
		float civLevel = 100 - (getAvgColor (mapOfSection).a * 100);

		// If the civilization level is not enough to be considered then don't bother creation.
		if( civLevel < civilizationNeededToReconsider){
			return;
		}

		// Create a empty object for structures to add themselves to
		currentSectionBeingCreated = new GameObject ("("+xPlot+", "+yPlot+"): Civ Level: "+civLevel);
		currentSectionBeingCreated.transform.position = new Vector3 (xPlot * dimensionOfSection, 0, yPlot * dimensionOfSection);

		// Begin building the structure.
		buildStructureOnPlot (
								xPlot*dimensionOfSection,
								yPlot*dimensionOfSection,
								dimensionOfSection,dimensionOfSection,
								mapOfSection
		                      );

		// Remove empty sections that yeilded no structures
		if(currentSectionBeingCreated.transform.childCount == 0){
			Destroy(currentSectionBeingCreated);
		}


	}

	/// <summary>
	/// Civilization is based on the alpha value of a pixel in a Texture2D.
	/// </summary>
	/// <returns>The civilization value that corresponds to the given alpha, a value betwen 0 and 100</returns>
	/// <param name="alphaValue">Alpha value from the civ map</param>
	float getCivLevel(float alphaValue){
		return 100 - (alphaValue * 100);
	}


	/// <summary>
	/// RECURSIVE METHOD!   Will continue deviding plots into subplots unitil a limit is met or a desired
	/// civilization density is met.
	/// Average the color in the current civ map passed in, and builds civilization if the average is high enough.
	/// If the average is in a toss up then we divide the map into smaller portions and average again.
	/// </summary>
	/// <param name="xStart">X start.</param>
	/// <param name="yStart">Y start.</param>
	/// <param name="widthOfPlot">Width of plot.</param>
	/// <param name="heightofPlot">Heightof plot.</param>
	/// <param name="civMap">Map of civilization in the plot desired to build buildings..</param>
	void buildStructureOnPlot (float xStart ,float yStart, float widthOfPlot, float heightofPlot, Texture2D civMap){

		// Get the amount of civilization the alphamap shows.
		float civLevel = getCivLevel(getAvgColor (civMap).a);

		//because this is a recursive method, and the picture is very curvy, it's possible that this method will keep 
		//subdividing into the plot being .03 width and height.  This is how small it will subdivide.
		int minStructureDimensions = 5;

		// If there's enough area to fit another structure inside of here.
		if (widthOfPlot * heightofPlot >= minStructureDimensions * minStructureDimensions) {


			// The plot is still in a toss up. Undecided what should be here. split up and exam.
			if (civLevel > civilizationNeededToReconsider && civLevel < civilizationNeededToBuild) { 


				// If the plot is still big enough to fit some structure inside of it.
				if(widthOfPlot/2 >= minStructureDimensions && heightofPlot/2 >= minStructureDimensions){

					// Get the width and heights of the picture of a single section
					int xPicSec = (int)civMap.width/2;
					int yPicSec = (int)civMap.height/2;

					// Get the width and height of the sections
					float newPlotSecWidth = widthOfPlot/2;
					float newPlotSecHeight = heightofPlot/2;

					// Generate the bottom left section
					Texture2D bottomLeftCivMap = new Texture2D(xPicSec,yPicSec);
					bottomLeftCivMap.SetPixels(civMap.GetPixels(0,0,xPicSec,yPicSec));
					bottomLeftCivMap.Apply();
					buildStructureOnPlot(xStart,yStart, newPlotSecWidth, newPlotSecHeight, bottomLeftCivMap);

					// Generate the bottom right section
					Texture2D bottomRightCivMap = new Texture2D(xPicSec,yPicSec);
					bottomRightCivMap.SetPixels(civMap.GetPixels(xPicSec,0,xPicSec,yPicSec));
					bottomRightCivMap.Apply();
					buildStructureOnPlot(xStart+newPlotSecWidth,yStart, newPlotSecWidth, newPlotSecHeight, bottomRightCivMap);

					// Generate the top left section
					Texture2D topLeftCivMap = new Texture2D(xPicSec,yPicSec);
					topLeftCivMap.SetPixels(civMap.GetPixels(0,yPicSec,xPicSec,yPicSec));
					topLeftCivMap.Apply();
					buildStructureOnPlot(xStart,yStart+newPlotSecHeight, newPlotSecWidth, newPlotSecHeight, topLeftCivMap);

					// Generate the top right section
					Texture2D topRightCivMap = new Texture2D(xPicSec,yPicSec);
					topRightCivMap.SetPixels(civMap.GetPixels(xPicSec,yPicSec,xPicSec,yPicSec));
					topRightCivMap.Apply();
					buildStructureOnPlot(xStart+newPlotSecWidth,yStart+newPlotSecHeight, newPlotSecWidth, newPlotSecHeight, topRightCivMap);

				}

			}

			// There's enough civilization to build a structure in here.
			if( civLevel >= civilizationNeededToBuild){

				// Grab available options to us to build on.
				StructureType[] buildOptions = StructureFactory.whatStuctureFitInPlot((int)widthOfPlot,(int)heightofPlot);

				// Grab the largest available option (which will always be the last one)
				StructureType selectedBuild = buildOptions[buildOptions.Length-1];

				// Grab the dimensions of the structure we've chosen to build
				Vector2 structDimensions = StructureFactory.structureDimensions(selectedBuild);

				// Randomize the building placement allittle as to not be a strict grid of buildings.
				// TODO Let this be a parameter as to the degree of randomness you want in the buildings placement.
				Vector3 structPos = new Vector3(Random.Range(xStart+(structDimensions.x/2),xStart+widthOfPlot-(structDimensions.x/2)),0,
				                                Random.Range(yStart+(structDimensions.y/2),yStart+heightofPlot- (structDimensions.y/2)));
				                                
				// Create the new structure
				Structure newStructure = StructureFactory.createStructure(selectedBuild, currentArea, structPos, (float)civLevel/100.0f);

				// Set the structure's parent to the section it's being created in
				newStructure.getGameObject().transform.parent = currentSectionBeingCreated.transform;

				// Add the structure to the area object we're building
				currentArea.addStructureToArea(newStructure);

				Vector3 curPos = newStructure.getGameObject().transform.position;

				newStructure.getGameObject().transform.position = new Vector3(curPos.x,1.5f,curPos.z);

			}


		}
		 

	}


	/// <summary>
	/// Spawns a number of uncivilized objects on the map based on 
	/// the density and the dimensions of the sections of the map.
	/// </summary>
	/// <param name="sectionsInXDirection">Sections in X direction.</param>
	/// <param name="sectionsInYDirection">Sections in Y direction.</param>
	/// <param name="civMap">Civ map.</param>
	void spawnUncivilizedObjects (int sectionsInXDirection, int sectionsInYDirection, Texture2D civMap)
	{

		// Can't spawn anything so return
		if (uncivilizedObjects == null) {
			return;
		}

		// Invalid density
		if(uncivilizedObjectDensity < 0f || uncivilizedObjectDensity > 1f){
			Debug.LogError("The object density : "+uncivilizedObjectDensity + " is an invalid value! it must be between 0 and 1");
			return;
		}

		// Determine the amount of items we want to try to spawn.
		int numOfObjectsToSpawn = (int)(dimensionOfSection * sectionsInXDirection * sectionsInYDirection * uncivilizedObjectDensity);

		// Spawn x amount of objects
		for (int i = 0; i < numOfObjectsToSpawn; i ++) {

			// Grab a random spawn point
			Vector3 spawnPoint = new Vector3(
											 Random.Range(0,sectionsInXDirection*dimensionOfSection), 
			                                 0, 
											 Random.Range(0,sectionsInYDirection*dimensionOfSection)
											 );


			// Get the percentages of how far over the random position is.
			float percentOfXDirection = spawnPoint.x / (sectionsInXDirection*dimensionOfSection);
			float percentOfYDirection = spawnPoint.z / (sectionsInYDirection*dimensionOfSection);

			// Determine the pixel based on the percentages.
			int xPixel = (int)(civMap.width * percentOfXDirection);
			int yPixel = (int)(civMap.height * percentOfYDirection);

			// Get the civ value at the position.
			float civValue = getCivLevel(civMap.GetPixel(xPixel,yPixel).a);

			// Determine if the area is uncivilized enough to spawn an object.
			if(civValue > civilizationNeededToReconsider){

				// No good, move onto the next random position
				continue;

			}

			// Get a object we want to spawn.
			GameObject objToSpawn = uncivilizedObjects[Random.Range(0,uncivilizedObjects.Length-1)];

			// Spawn the object
			GameObject.Instantiate(objToSpawn, spawnPoint, Quaternion.identity);

		}

	}

	/// <summary>
	/// Generates terrain and the buildings that occupy the terrain.
	/// </summary>
	/// <param name="sectionsInXDirection">Sections in X direction.</param>
	/// <param name="sectionsInYDirection">Sections in Y direction.</param>
	/// <param name="border">Border of sections passed in.  if you pass in 10*10 sections
	/// and a border of 2, your final result will be a 14*14
	/// </param>
	void CreateArea(int sectionsInXDirection, int sectionsInYDirection, int border)
	{

		// Clear what we have defined as 'free area', array of area that AI can move through
		freeArea.Clear ();

		// Choose the maps dimensions
		int mapHeight = 1025;
		int mapWidth = 1025;


		// Create a seed for our generation, useful for generating the same map over and over if we so choose to
		Vector2 civSeed = new Vector2 (Random.Range (0, 10000), Random.Range (0, 10000));


		// Produce a civilization map, with alpha being civilization
		Texture2D civMap = MapGen.GetInstance ().CreateMap (mapWidth, mapHeight, (int)civSeed.x, (int)civSeed.y);
		civMap = MapGen.GetInstance ().SmoothEdges (civMap);
		guiMapRenderOne = civMap;//for guiPurposes.
	

		// Create our terrain object and position and scale it correctely
		GameObject terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
		terrain.transform.position = new Vector3 ( (dimensionOfSection*(sectionsInXDirection))/2, 0, (dimensionOfSection*(sectionsInYDirection))/2);
		terrain.transform.Rotate (new Vector3 (0, 180, 0));
		terrain.transform.localScale = new Vector3 ( (dimensionOfSection*(sectionsInXDirection))/10, 1, (dimensionOfSection*(sectionsInYDirection))/10);

		// Set the material on the terrain that will make it look like terain
		Material material = new Material(Shader.Find("MyShaders/TerrainTwoLayer"));

		// Set the texture on the terrain that will represent civilization
		material.SetTexture ("_MainTex", civArea);
		material.SetTextureScale ("_MainTex", new Vector2(70,70));

		// Set the texture on the terrain that will represent the wild.
		material.SetTexture ("_PathTex", uncivArea);
		material.SetTextureScale ("_PathTex", new Vector2(70,70));

		// Set the alpha that determines whether or not to render the wild or the civilization texture.
		material.SetTexture ("_PathMask", civMap);

		// Set the material to the terrain
		terrain.GetComponent<Renderer> ().material = material;

		// Create the area object
		currentArea = new Area ("Civ Seed: "+civSeed.ToString(), AreaBiome.Test, civSeed);


		// Create structures by splitting up the civilization map into multiple sections to be generated.
		for (int curXSec = 0; curXSec < sectionsInXDirection; curXSec++) {
			
			for (int curYSec = 0; curYSec < sectionsInYDirection; curYSec++) {
				
				int sectionMapWidth = mapWidth / sectionsInXDirection;
				int sectionMapHeight = mapHeight / sectionsInYDirection;
				
				Texture2D sectionMap = new Texture2D (sectionMapWidth, sectionMapHeight);
				
				sectionMap.SetPixels (civMap.GetPixels (curXSec * sectionMapWidth, 
				                                        curYSec * sectionMapHeight,
				                                        sectionMapWidth, sectionMapHeight));
				
				sectionMap.Apply ();
				
				createSection (curXSec, curYSec, sectionMap);

			}
			
		}

		// Spawn trees to make things look more full
		spawnUncivilizedObjects (sectionsInXDirection, sectionsInYDirection, civMap);

	}


	Area currentArea;
	public bool createAreaOnStart = true;
	void Start () 
	{
		int areaWidthInSections = 30;
		int areaHeightInSections = 30;
		int thincknessOfSurroundedUncivilizedArea = 10;

		if (createAreaOnStart) {

			// Spawn the player because we can
			GameObject player = Resources.Load("Player") as GameObject;
			player = GameObject.Instantiate(player, 
			                                new Vector3(
												((float)areaWidthInSections/2.0f)*dimensionOfSection,
												20f,
												((float)areaHeightInSections/2.0f)*dimensionOfSection
											), 
			                                Quaternion.identity) as GameObject;

			player.transform.name = "Player";

			CreateArea (areaWidthInSections, areaHeightInSections, thincknessOfSurroundedUncivilizedArea);
			

		}

	}

	Texture2D guiMapRenderOne = null;
	Texture2D guiMapRenderTwo = null;
	void OnGUI(){
		if(guiMapRenderOne != null){
			GUI.DrawTexture(new Rect(0,0,200,200),guiMapRenderOne );
					}

		if(guiMapRenderTwo !=null){
			GUI.DrawTexture(new Rect(0,200,200,200),guiMapRenderTwo );

		} 
	}
	

}


