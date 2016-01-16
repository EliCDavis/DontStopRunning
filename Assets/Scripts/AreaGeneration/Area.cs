using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EliDavis.AreaGeneration {

	public class Area  {

		/// <summary>
		/// The seed that was used to generate the area.
		/// </summary>
		Vector2 seed;

		/// <summary>
		/// The name of the area.
		/// </summary>
		string name;

		/// <summary>
		/// The biome of the area
		/// </summary>
		AreaBiome biome;

		/// <summary>
		/// A list of all the structures generated in the area
		/// </summary>
		List<Structure> structuresInArea;

		public Area(string nameOfArea, AreaBiome setBiome, Vector2 seed){
			name = nameOfArea;
			biome = setBiome;
			structuresInArea = new List<Structure>();
			this.seed = seed;
		}


		public string getName(){
			return name;
		}


		public AreaBiome getBiome(){
			return biome;
		}


		public Vector2 getSeed(){
			return seed;
		}

		/// <summary>
		/// Takes the vector2 seed and uses it to create a single integer
		/// to represent the seed.
		/// </summary>
		/// <returns>The flat seed.</returns>
		public int getFlatSeed(){
			return (int)(seed.x - seed.y);
		}

		
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

}