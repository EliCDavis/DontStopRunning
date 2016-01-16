using UnityEngine;
using System.Collections;
using EliDavis.AreaGeneration;

namespace EliDavis.Prototyping { 

	public class StructurePrototyping : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
			Vector2 plot = new Vector2 (30, 30);


			for(int s = 0; s < 10; s++){
				for (int c = 0; c < 10; c ++) {
					GameObject building = StructureFactory.buildStructure (plot, s*100, AreaBiome.Test, (float)c / 10f);
					if(building != null){
						building.transform.position = new Vector3((plot.x+5)*c, 0, (plot.y+5)*s);
					}

				}
			}

		}
		
		// Update is called once per frame
		void Update () {
		
		}

	}

}