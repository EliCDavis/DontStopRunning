using UnityEngine;
using System.Collections;
using System.Linq;

namespace Enemy {


	/// <summary>
	/// This is the AI for a classic drone that can patrol areas and attack it's target from the sky.
	/// </summary>
	public class DroneBehavior : MonoBehaviour {

		enum DroneState {

			/// <summary>
			/// This is when the drone is surveying a given area out on patrol
			/// looking for it's target
			/// </summary>
			Patrol,

			/// <summary>
			/// This is when the drone has found it's target and is currentely
			/// trying to eliminate it
			/// </summary>
			Persuing,

			/// <summary>
			/// This is when the drone has come out of contact with it's target
			/// and it trying to find where it went so it may persue it further
			/// </summary>
			Searching
		}


		/// <summary>
		/// The current state the drone is in which dictates it's behvaior
		/// </summary>
		private DroneState currentState = DroneState.Patrol;


		/// <summary>
		/// How high we want the drone to be above the ground.
		/// </summary>
		private float desiredHeighFromGround = 10f;


		/// <summary>
		/// We can't have autopilot be perfect, and there will be a certain
		/// range of error that it will try to stay within when obtaining a 
		/// certain altitude
		/// </summary>
		private float acceptableRange = 0.7f;


		/// <summary>
		/// Reference to the rigidbody of the drone.
		/// </summary>
		private Rigidbody body = null;


		/// <summary>
		/// The field of view that the drone has when looking for it's target.
		/// </summary>
		private float fov = 40;


		/// <summary>
		/// How far out the drone can see.
		/// </summary>
		private float eyeSight = 100;


		/// <summary>
		/// The airspace the drone is allowed to be on patrol searching for a target.
		/// </summary>
		private Rect patrolAirspace; 


		/// <summary>
		/// When the drone is in patrol state these are the waypoints it trys to hit
		/// </summary>
		private Vector2[][] waypointsToPatrol;


		/// <summary>
		/// The waypoint the drone is currentely persuing
		/// </summary>
		private Vector2 currentWayPointMovingToPostion;


		/// <summary>
		/// Sets up the patrols airspace and creates waypoints to make sure to completely
		/// examine that airspace
		/// </summary>
		/// <param name="airspace">Airspace the drone will patrol</param>
		/// <param name="altitudeToPatrol">Altitude from the ground the drone will hover.</param>
		public void setPatrolAirspace(Rect airspace, float altitudeToPatrol){

			patrolAirspace = airspace;

			// Sanatize Data
			desiredHeighFromGround = Mathf.Clamp(altitudeToPatrol, 0, eyeSight);

			// Create waypoints based on the airspace and how high we patrol at //

			// Determine how much area we can see based on our field of view and altitude
			float distanceAcrossGround = Mathf.Tan(fov*Mathf.Deg2Rad)*desiredHeighFromGround*2;

			// Determine how much sections the airspace must be broken into to be patrolled
			int sectionsX = Mathf.CeilToInt (airspace.width / distanceAcrossGround);
			int sectionsY = Mathf.CeilToInt (airspace.height / distanceAcrossGround);


			// Build waypoints based on sections
			Vector2[][] waypoints = new Vector2[sectionsX][];



			for(int x = 0; x < sectionsX; x ++){

				waypoints[x] = new Vector2[sectionsY];

				for(int y = 0; y < sectionsY; y ++){
					waypoints[x][y] = new Vector2(patrolAirspace.x + ((float)(patrolAirspace.width/sectionsX)*(x+.5f)), 
					                              patrolAirspace.y + ((float)(patrolAirspace.height/sectionsY)*(y+.5f)));

					// Debugging purposes to see where the waypoints are
					GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					Destroy(sphere.GetComponent<Collider>());
					sphere.transform.position = new Vector3(waypoints[x][y].x, altitudeToPatrol, waypoints[x][y].y);
				}

			}

			waypointsToPatrol = waypoints;

			currentWayPointMovingToPostion = getClosestPatrolWaypoint ();

		}

		// Use this for initialization
		void Start () {
		
			body = transform.GetComponent<Rigidbody> ();

			setPatrolAirspace(new Rect (-50f, -50f, 100f, 100f), 10f);

		}


		void FixedUpdate (){


			// State machine update
			switch (currentState) {

			case DroneState.Patrol:
				patrolUpdate();
				break;

			case DroneState.Persuing:
				persuingUpdate();
				break;

			case DroneState.Searching:
				searchingUpdate();
				break;

			}

			
		}


		private void persuingUpdate (){
			throw new System.NotImplementedException ();
		}


		private void searchingUpdate (){
			throw new System.NotImplementedException ();
		}


		private Vector2 patrolMovingDirection = new Vector2(1,1);

		private void patrolUpdate(){
		
			gyroscopicAutopilotUpdate (desiredHeighFromGround);


			// If we're not within an acceptable range to be considered
			// at our desired way point
			if (distanceFromCurrentPatrolWaypoint () > .5f) {
				moveTowardsWayPoint ();
			} 
			// we've arived at the waypoint, get a new one
			else {


			}


		}


		private void moveTowardsWayPoint(){

			// Get the direction the turret needs to move in to get to the waypoint
			Vector3 directionOfWaypoint = new Vector3(currentWayPointMovingToPostion.x, transform.position.y, currentWayPointMovingToPostion.y) - transform.position;

			// If our velocity is not in the same direction
				// add force to cancel out the velocity
				// add force to start heading in the right direction

				
		}


		/// <summary>
		/// Makes sure the drone stays at the desired altitude.
		/// </summary>
		/// <param name="altToBe">Altitude to be.</param>
		private void gyroscopicAutopilotUpdate (float altToBe){

			// How fast we want to ascend and descend.
			float speed = 150f;

			// If we're not within an acceptable range of our desired height.
			if (Mathf.Abs (transform.position.y - altToBe) > acceptableRange) {

				// Get direction we're moving in.
				float yVelocity = body.velocity.y;

				// If it's not the right direction
				if( Mathf.Sign(yVelocity) !=  Mathf.Sign( altToBe - transform.position.y ) ){

					// Add force to change direction
					body.AddForce ( speed* Mathf.Sign(altToBe - transform.position.y)*Vector3.up*body.mass, ForceMode.Force);

				}
		
				// If we're not moving at all in terms of altitude
				if( Mathf.Approximately(yVelocity, 0f) ){

					// Start moving us in the appropriate direction
					body.AddForce (Mathf.Sign(altToBe - transform.position.y)*speed*Vector3.up*body.mass, ForceMode.Force);

				}
				
			} 

			// We're moving in a desirable altitude
			else {
				
				// Add force to stop moving
				body.AddForce (body.velocity.y*-1f*Vector3.up*body.mass, ForceMode.Force);

			}

			// Add enough force to keep up at the current height we're at
			body.AddForce ( ( Vector3.up * forceNeededToStayAfloat() ) , ForceMode.Force);

			//TODO: Add force to fix rotation

		}


		/// <summary>
		/// Gets the closest waypoint relative to the drone's position
		/// </summary>
		/// <returns>The closest waypoint.</returns>
		private Vector2 getClosestPatrolWaypoint(){

			Vector2[] flattenedArray = waypointsToPatrol.SelectMany (x=>x).ToArray();

			float clostestDistance = 100000000f;

			Vector2 waypoint = Vector2.zero;

			for (int i = 0; i < flattenedArray.Length; i ++) {

				float distance = Vector2.Distance(flattenedArray[i], new Vector2(transform.position.x,transform.position.y));

				if(distance < clostestDistance){
					waypoint = flattenedArray[i];
				}

			}

			return waypoint;
		}


		/// <summary>
		/// Get's the amount of force needed to keep the drone afloat.
		/// </summary>
		/// <returns>The needed to stay afloat.</returns>
		private float forceNeededToStayAfloat(){

			return Mathf.Abs(body.mass*Physics.gravity.y);

		}



		private float distanceFromCurrentPatrolWaypoint(){
			return Vector3.Distance (new Vector2(transform.position.x, transform.position.y), currentWayPointMovingToPostion);
		}

	}

}