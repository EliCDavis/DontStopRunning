using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

namespace EliDavis.AreaGeneration {


	public class Structure {

		public Structure(StructureType structureType, GameObject structureObject,Area areaStructureLocatedIn){
			type = structureType;
			structureInRealSpace = structureObject;
			areaLocatedIn = areaStructureLocatedIn;
		}

		public void destroySelf ()
		{
			areaLocatedIn.removeStructure (this);
			Object.DestroyImmediate (getGameObject ());
		}
		
		GameObject structureInRealSpace;
		public GameObject getGameObject ()
		{
			return structureInRealSpace;
		}


		Area areaLocatedIn;

		
		StructureType type;


		public StructureType getStructureType(){
			return type;
		}
		
	}

}
