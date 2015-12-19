using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class StructureFactory  {

	public static Vector2 structureDimensions(StructureType structure){
		switch (structure) {
				
			case StructureType.Fort:
				return new Vector2(25,10);
			break;

			case StructureType.Building:
				return new Vector2(10,10);
			break;

			case StructureType.Hut:
				return new Vector2(5,5);
			break;

			default:
				Debug.LogError("Structure Dimensions Have Not Been Defined!");
				return Vector2.zero;
			break;
		}
	}

	public static Structure createStructure(StructureType structure, Area area, Vector3 position){

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
			break;
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
				break;
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

		return typesThatFit.ToArray ();

	}


	static GameObject buildStructure(Vector2 dimensions, int seed, AreaBiome theme){

	}

}
