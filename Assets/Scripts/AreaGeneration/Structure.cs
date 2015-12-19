using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

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



	public override bool Equals(System.Object obj){

		// If parameter is null return false.
		if (obj == null){
			return false;
		}
		
		// If parameter cannot be cast to Point return false.
		Structure p = obj as Structure;
		if ((System.Object)p == null){
			return false;
		}
		
		if (p.type == this.type&&p.structureInRealSpace.gameObject.Equals(this.structureInRealSpace)) {
			return true;
		}

		return false;
	}
	
}
