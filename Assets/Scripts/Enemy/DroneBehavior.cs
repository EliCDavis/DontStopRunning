using UnityEngine;
using System.Collections;


namespace Enemy {

	public class DroneBehavior : MonoBehaviour {

		public float desiredHeighFromGround = 10f;

		private Rigidbody body = null;

		// Use this for initialization
		void Start () {
		
			body = transform.GetComponent<Rigidbody> ();

		}


		// Update is called once per frame
		void Update () {
		

		}


		void FixedUpdate (){
			autopilotUpdate ();
			
		}

		/// <summary>
		/// Makes sure the drone stays at the desired altitude
		/// </summary>
		void autopilotUpdate (){


			body.AddForce ( (Vector3.up * forceNeededToStayAfloat()), ForceMode.Force);

		}

		float forceNeededToStayAfloat(){

			return Mathf.Abs(body.mass*Physics.gravity.y);

		}

	}

}