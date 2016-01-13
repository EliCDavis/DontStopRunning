using UnityEngine;
using System.Collections;
using EliDavis.Characters.Enemy;

namespace EliDavis.Prototyping {

	public class GameInit : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
			EnemyFactory.CreateTurret (new Vector3(0,0, -25));
			EnemyFactory.CreateDrone (new Vector3(-1.8f, 9.9f, 31f), new Rect (-50f, -50f, 100f, 100f), 20f );

		}
		
		// Update is called once per frame
		void Update () {
		
		}

	}

}