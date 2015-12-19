using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Area  {

	/// <summary>
	/// The seed that was used to generate the area.
	/// </summary>
	Vector2 seed;

	public Area(string nameOfArea, AreaBiome setBiome, Vector2 seed){
		name = nameOfArea;
		biome = setBiome;
		structuresInArea = new List<Structure>();
		this.seed = seed;
	}


	string name;
	public string getName(){
		return name;
	}


	AreaBiome biome;
	public AreaBiome getBiome(){
		return biome;
	}

	
	List<Structure> structuresInArea;
	public void addStructureToArea(Structure structureToAdd){
		structuresInArea.Add (structureToAdd);
	}

	public void removeStructure (Structure structure)
	{
		if (structuresInArea.Contains (structure)) {
				structuresInArea.Remove (structure);
		} else {
			Debug.LogError("Structure does not exist in Area to Remove!");
		}
	}

	public Structure[] getStructuresInArea(){
		return structuresInArea.ToArray ();
	}


}