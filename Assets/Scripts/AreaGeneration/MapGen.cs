using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

class MapGen{
	
	
	static MapGen instance = null;
	
	public static MapGen GetInstance(){
		if(	instance == null){
			instance = new MapGen();
		} 
		
		return instance;
	}

	/// <summary>
	/// Because the civ map leaves no room for a border, we're adding it in. 
	/// </summary>
	/// <returns>The height from civ map.</returns>
	/// <param name="civMap">Civ map.</param>
	/// <param name="bumpyness">Bumpyness.</param>
	/// <param name="xSections">X sections.</param>
	/// <param name="ySections">Y sections.</param>
	/// <param name="border">Border.</param>
	public Texture2D genHeightFromCivMap (Texture2D civMap, float bumpyness,int xSections,int ySections, int border)
	{

		scale = bumpyness;
		Texture2D noise = CreateMap (civMap.width,civMap.height,Random.Range(0,100),Random.Range(0,100));

		//Generate the smaller civ map in order to fit border in.
		Texture2D shrunkCivMap = new Texture2D (civMap.width, civMap.height);

		shrunkCivMap.SetPixels (civMap.GetPixels ());
		shrunkCivMap.Apply ();



		shrunkCivMap = scaleTexture(shrunkCivMap,(int)(civMap.width*((double)xSections/(double)(xSections+border+border))),
		             			  (int)(civMap.height*((double)ySections/(double)(ySections+border+border))));

		//return shrunkCivMap;

		Color[] civPix = civMap.GetPixels ();
		//clear larger  civ map and paste in the shrunken right in the middle.
		for (int i = 0; i < civPix.Length; i ++) {
			civPix[i] = new Color(1,1,1,1);
		}

		Texture2D newCivMap = new Texture2D (civMap.width, civMap.height);

		newCivMap.SetPixels (civPix);

		int xBorderEnds = (int)(newCivMap.width * ((double)border / (xSections + border + border)));
		int yBorderEnds = (int)(newCivMap.height * ((double)border / (ySections + border + border)));
		newCivMap.SetPixels (xBorderEnds, yBorderEnds, shrunkCivMap.width, shrunkCivMap.height, shrunkCivMap.GetPixels ());
		newCivMap.Apply ();


		//create noise and height map and apply noise in height map where needed based on civ map
		Color[] heightPix = newCivMap.GetPixels ();

		Texture2D newNoiseMap = CreateMap (newCivMap.width,newCivMap.height,Random.Range(0,100),Random.Range(0,100));
		Color[] noisePix = newNoiseMap.GetPixels ();

		for (int i = 0; i < heightPix.Length; i ++) {
			
			if(heightPix[i].a > .8f){//this is not civilized, lets add some terrain.
				
				heightPix[i] = noisePix[i];
				
			} else {
				
				heightPix[i] = new Color(.5f,.5f,.5f,.5f);
				
			}
			
			
		}

		Texture2D heightMap = new Texture2D (newNoiseMap.width,newNoiseMap.height);



		heightMap.SetPixels (heightPix);
		heightMap.Apply ();
		return heightMap;
	}


	//http://stackoverflow.com/questions/356035/algorithm-for-detecting-clusters-of-dots
	/*
	public Texture2D structureDensityMap(int width, int height, Structure[] structures){

		//load the image
		PImage sample;
		sample = loadImage("test.png");
		size(sample.width, sample.height);
		image(sample, 0, 0);
		int[][] heat = new int[width][height];
		int dotQ = 0;
		int[][] dots = new int[width*height][2];
		int X = 0;
		int Y = 1;
		
		
		//parameters
		int resolution = 1; //distance between points in the grid
		int distance = 20; //distance at wich two points are considered near
		float threshold = 0.6;
		int level = 240; //minimum brightness to detect the dots
		int sensitivity = 1; //how much does each dot matters
		
		//detect all dots in the sample
		loadPixels();
		for(int x=0; x<width; x++){
			for(int y=0; y<height; y++){
				color c = pixels[y*sample.width+x];
				if(brightness(c)<level) {
					dots[dotQ][X] += x;
					dots[dotQ++][Y] += y;
				}
			}
		}
		
		//calculate heat
		for(int x=0; x<width; x+=resolution){
			for(int y=0; y<height; y+=resolution){
				for(int d=0; d<dotQ; d++){
					if(dist(x,y,dots[d][X],dots[d][Y]) < distance)
						heat[x][y]+=sensitivity;
				}
			}
		}
		
		//render the output
		for(int a=0; a<width; ++a){
			for(int b=0; b<height; ++b){
				pixels[b*sample.width+a] = color(heat[a][b],0,0);
			}
		}
		updatePixels();
		


	}
	*/

	//As opposed to createMap(), this takes a civilization map and makes it take in account for terrain size.
	public Texture2D civilizationMapInTerrain(Texture2D civMap,int xSections,int ySections, int border){


		
		//Generate the smaller civ map in order to fit border in.
		Texture2D shrunkCivMap = new Texture2D (civMap.width, civMap.height);
		
		shrunkCivMap.SetPixels (civMap.GetPixels ());
		shrunkCivMap.Apply ();
		
		
		
		shrunkCivMap = scaleTexture(shrunkCivMap,(int)(civMap.width*((double)xSections/(double)(xSections+border+border))),
		                            (int)(civMap.height*((double)ySections/(double)(ySections+border+border))));

		
		Color[] civPix = civMap.GetPixels ();
		//clear larger  civ map and paste in the shrunken right in the middle.
		for (int i = 0; i < civPix.Length; i ++) {
			civPix[i] = new Color(1,1,1,1);
		}
		
		Texture2D newCivMap = new Texture2D (civMap.width, civMap.height);
		
		newCivMap.SetPixels (civPix);
		
		int xBorderEnds = (int)(newCivMap.width * ((double)border / (xSections + border + border)));
		int yBorderEnds = (int)(newCivMap.height * ((double)border / (ySections + border + border)));
		newCivMap.SetPixels (xBorderEnds, yBorderEnds, shrunkCivMap.width, shrunkCivMap.height, shrunkCivMap.GetPixels ());
		newCivMap.Apply ();

		
		//create noise and height map and apply noise in height map where needed based on civ map
		Color[] heightPix = newCivMap.GetPixels ();
		
		for (int i = 0; i < heightPix.Length; i ++) {
			
			if(heightPix[i].a == 1){//this is not civilized, lets add some terrain.
				heightPix[i] = new Color(0,0,0,0);
			} else {
				heightPix[i] = new Color(1,1,1,1);
			}
			
		}
		
		Texture2D heightMap = new Texture2D (newCivMap.width,newCivMap.height);

		heightMap.SetPixels (heightPix);
		heightMap.Apply ();
		return heightMap;
	}

	public Texture2D createHeightMap(int width, int height ,int iterationsOfNoise, float startingScale){

		Texture2D baseMap = CreateMap(width,height,Random.Range(0,10000), Random.Range(0,10000), startingScale);
		Color[] basePix = baseMap.GetPixels ();

		for (int i = 0; i < iterationsOfNoise; i++) {

			float newScale = startingScale*i;

			Texture2D texToBlend =CreateMap(width,height,Random.Range(0,10000), Random.Range(0,10000), newScale);
			Color[] blendPix = texToBlend.GetPixels();

			for(int pixIndex = 0; pixIndex < basePix.Length; pixIndex++){


				/* THINGS TO THINK ABOUT
				 * the greater the r,b,g, or a value, the more the blend pixel takes effect..
				 * each iteration does decreasing amount of change to the texture
				 */
				Color pix = basePix[pixIndex];
				pix = new Color( ( (pix.r*3) + blendPix[pixIndex].r )/ 4,  
				                 ( (pix.g*3) + blendPix[pixIndex].g )/ 4,
				                 ( (pix.b*3) + blendPix[pixIndex].b )/ 4,
				                 ( (pix.a*3) + blendPix[pixIndex].a )/ 4);

				basePix[pixIndex] = pix;
			}


		}

		baseMap.SetPixels (basePix);
		baseMap.Apply();
		return baseMap;

	}

	private Texture2D scaleTexture(Texture2D source,int targetWidth,int targetHeight) {
		Texture2D result=new Texture2D(targetWidth,targetHeight,source.format,true);
		Color[] rpixels=result.GetPixels(0);
		float incX=((float)1/source.width)*((float)source.width/targetWidth);
		float incY=((float)1/source.height)*((float)source.height/targetHeight);
		for(int px=0; px<rpixels.Length; px++) {
			rpixels[px] = source.GetPixelBilinear(incX*((float)px%targetWidth),
			                                      incY*((float)Mathf.Floor(px/targetWidth)));
		}
		result.SetPixels(rpixels,0);
		result.Apply();
		return result;
	}
	
	
	
	public Texture2D CreateMap(int setx, int sety,int setorgx, int setorgy){
		pixWidth = setx;
		pixHeight = sety;
		xOrg = setorgx;
		yOrg = setorgy;
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		CalcNoise();
		
		return noiseTex;
	}

	public Texture2D CreateMap(int setx, int sety,int setorgx, int setorgy, float setScale){
		pixWidth = setx;
		pixHeight = sety;
		xOrg = setorgx;
		yOrg = setorgy;
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		CalcNoise();
		scale = setScale;

		return noiseTex;
	}

	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	public float scale = 5.0F;
	private Texture2D noiseTex;
	private Color[] pix;
	
	void CalcNoise() {
		float y = 0.0F;
		while (y < noiseTex.height) {
			float x = 0.0F;
			while (x < noiseTex.width) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				pix[Mathf.FloorToInt(y * noiseTex.width + x)] = new Color(sample, sample, sample,sample);
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}
	
	public Texture2D SmoothEdges(Texture2D theMap){
		
		Texture2D mapToSmooth = new Texture2D(theMap.width,theMap.height);
		
		mapToSmooth.SetPixels (theMap.GetPixels ());
		
		float y = 0.0F;
		while (y < mapToSmooth.height) {
			float x = 0.0F;
			while (x < mapToSmooth.width) {
				
				
				float newX = (mapToSmooth.width/(2*(x+1)));//(x+1) is so that your never multiplying by 0
				if(x> mapToSmooth.width/2){
					newX = (mapToSmooth.width/(   (x+ ((x-(mapToSmooth.width/2))*-2)) * 2   ) );
				}
				
				float newY = (mapToSmooth.height/(2*(y+1)));//(x+1) is so that your never multiplying by 0
				if(y> mapToSmooth.height/2){
					newY = (mapToSmooth.height/(   (y+ ((y-(mapToSmooth.height/2))*-2)) * 2   ) );
				}
				
				float sampleFade = ((newX+newY)/2);
				
				float sample = pix[Mathf.FloorToInt(y * mapToSmooth.width + x)].r*sampleFade;
				pix[Mathf.FloorToInt(y * mapToSmooth.width + x)] = new Color(sample, sample, sample,sample);
				x++;
			}
			y++;
		}
		
		mapToSmooth.SetPixels(pix);
		mapToSmooth.Apply();
		
		return mapToSmooth;
	}

	/// <summary>
	/// looks at alpha channel in image and determines if that position is civilized, and adds to list of 
	/// positions occupied by civilization. ranging from 0-1.
	/// The reason for this is because usually, simply calling get pixel()(a majorly convient method if could work)
	/// on an image alot freezes unity (which is extremely inconvenient).
	/// Usually in my code I'm searching for civilization in the image, and would use something like get pixel(x,y)
	/// This provides an alternative by giving me the civilization already by using a safer method Color Arrays!
	/// </summary>
	/// <returns>The positions civilization is taking up image in x and y values between 0-1</returns>
	/// <param name="civTexture"> texture completely blank except white where civilization lies.</param>
	public Vector2[] civPositionsInImage(Texture2D civTexture){
		List<Vector2> positionsOccupiedByCiv = new List<Vector2>();

		Color[] pix = civTexture.GetPixels ();
		for (int i = 0; i< pix.Length; i++) {

			//if we've found some civilized area
			if(pix[i].a == 1){

				//convert the i into x and y coordinates to add to the Vector2 list.
				int y = Mathf.FloorToInt(i/civTexture.width);
				int x = i%civTexture.width;

				//make values between 0 and 1 from image width and height
				float yPic = (float)y/civTexture.height;
				float xPic = (float)x/civTexture.height;

				positionsOccupiedByCiv.Add(new Vector2(xPic, yPic));
			}
		}

		return positionsOccupiedByCiv.ToArray ();
	}
	
	//returns a immage which is flipped on the x and y axis of the picture passed in
	public Texture2D invertPicture(Texture2D picToFlip){
		
		Texture2D pic = new Texture2D(picToFlip.width,picToFlip.height);
		
		Color[] newColors = new Color[picToFlip.GetPixels().Length];
		Color[] oldColors = picToFlip.GetPixels();
		for (int i = 0; i < newColors.Length; i ++) {
			newColors[i] = oldColors[oldColors.Length-1-i];
		}
		
		pic.SetPixels (newColors);
		pic.Apply();
		
		return pic;
	}
	
}
