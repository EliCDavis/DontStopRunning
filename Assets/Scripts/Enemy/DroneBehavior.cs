using UnityEngine;
using System.Collections;

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
		/// The waypoint's position the drone is currentely persuing
		/// </summary>
		private Vector2 currentWayPointMovingTo;


		/// <summary>
		/// Sets up the patrols airspace and creates waypoints to make sure to completely
		/// examine that airspace
		/// </summary>
		/// <param name="airspace">Airspace the drone will patrol</param>
		/// <param name="altitudeToPatrol">Altitude from the ground the drone will hover.</param>
		public void setPatrolAirspace(Rect airspace, float altitudeToPatrol){

			// Update our airspace
			patrolAirspace = airspace;

			// Sanatize Data
			desiredHeighFromGround = Mathf.Clamp(altitudeToPatrol, 0, eyeSight);

			// Create waypoints based on the airspace and how high we patrol at //

			// Determine how much area we can see based on our field of view and altitude
			float distanceAcrossGround = Mathf.Tan(fov*Mathf.Deg2Rad/2)*desiredHeighFromGround*2;

			// Determine how much sections the airspace must be broken into to be patrolled
			int sectionsX = Mathf.CeilToInt (airspace.width / distanceAcrossGround);
			int sectionsY = Mathf.CeilToInt (airspace.height / distanceAcrossGround);

			// Build waypoints based on sections
			Vector2[][] waypoints = new Vector2[sectionsX][];

			for(int x = 0; x < sectionsX; x ++){

				// Start a new column
				waypoints[x] = new Vector2[sectionsY];

				for(int y = 0; y < sectionsY; y ++){

					waypoints[x][y] = new Vector2(patrolAirspace.x + ((float)(patrolAirspace.width/sectionsX)*(x+.5f)), 
					                              patrolAirspace.y + ((float)(patrolAirspace.height/sectionsY)*(y+.5f)));

					// Debugging purposes to see where the waypoints are
//					GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//					Destroy(sphere.GetComponent<Collider>());
//					sphere.transform.position = new Vector3(waypoints[x][y].x, altitudeToPatrol, waypoints[x][y].y);
				}

			}

			waypointsToPatrol = waypoints;

			currentWayPointMovingTo = getClosestPatrolWaypoint ();

		}

		// Use this for initialization
		void Start () {
		
			body = transform.GetComponent<Rigidbody> ();

			setPatrolAirspace(new Rect (-50f, -50f, 100f, 100f), 20f);

		}


		/// <summary>
		/// Fix update used for any calls to rigidbodies.
		/// </summary>
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


		/// <summary>
		/// The update function for keeping the drone moving to it's different waypoints while looking
		/// for potential targets to persue
		/// </summary>
		private void patrolUpdate(){
		
			// Raycast to ground to determine how high we are
			float groundAltitude = 0;

			Ray ray = new Ray (transform.position, Vector3.down);

			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 10000)) {
			
				groundAltitude = hit.point.y;
			
			} else {
				// No ground underneath us, what do we do with our lives

				// TODO Figure out what to do when theres no ground underneath us
			}

			// TODO: raycast in all directions to make sure we don't hit shit

			// Keep it at the height we'd like
			gyroscopicAutopilotUpdate (groundAltitude + desiredHeighFromGround);

			// Can't patrol shit without waypoints
			if(waypointsToPatrol == null){
				return;
			}

			// If we're not within an acceptable range to be considered
			// at our desired way point
			if (distanceFromCurrentPatrolWaypoint () > .5f) {
				moveTowardsWayPoint ();
			} 
			// we've arived at the waypoint, get a new one
			else {

				currentWayPointMovingTo = getNextWaypointInPatrol();

			}

			// TODO: Look for potential targets

		}


		/// <summary>
		/// whenever the drone needs a new way point to patrol, it will move 
		/// through the indexes of the jagged array to get one.
		/// </summary>
		private Vector2 patrolMovingDirection = new Vector2(1,1);


		/// <summary>
		/// Gets the next waypoint in patrol based on the current one we're at and our moving direction
		/// </summary>
		/// <returns>The next waypoint to move to.</returns>
		private Vector2 getNextWaypointInPatrol(){

			Vector2 newWaypoint = currentWayPointMovingTo;

			int nextX = (int)(newWaypoint.x + patrolMovingDirection.x);
			
			// We've reached the end of one side
			if(nextX == waypointsToPatrol.Length || nextX == -1){
				
				int nextY = (int)(newWaypoint.y + patrolMovingDirection.y);
				
				// We're in the very corner
				if(nextY == waypointsToPatrol[(int)newWaypoint.x].Length || nextY == -1){
					
					// Make the drone turn around completely
					patrolMovingDirection *= -1;
					
				} 
				
				// We're only on the edge of the x coordinate system
				else {
					
					// Change direction heading in the x coordinate plane
					patrolMovingDirection.x *= -1;
					
				}
				
				// Change waypoint
				newWaypoint.y += patrolMovingDirection.y;
				
			} else {
				
				// Change waypoint
				newWaypoint.x += patrolMovingDirection.x;
				
			}
			
			newWaypoint = new Vector2(Mathf.Clamp(newWaypoint.x, 0, waypointsToPatrol.Length-1),
			                          Mathf.Clamp(newWaypoint.y, 0, waypointsToPatrol[0].Length-1));

			return newWaypoint;

		}


		/// <summary>
		/// Moves the drone towards the way point currentely assigned for patrol
		/// </summary>
		private void moveTowardsWayPoint(){

			Vector2 wayPoint = getPositionOfWaypoint(currentWayPointMovingTo);

			// Get the direction the turret needs to move in to get to the waypoint
			Vector3 directionOfWaypoint = new Vector3(wayPoint.x, transform.position.y, wayPoint.y) - transform.position;

			//TODO Implement this functionality using rigidbody forces!

			// If our velocity is not in the same direction
				// add force to cancel out the velocity
				// add force to start heading in the right direction

			transform.position = Vector3.MoveTowards (transform.position, new Vector3(wayPoint.x, transform.position.y, wayPoint.y), 10*Time.deltaTime);

				
		}


		/// <summary>
		/// Makes sure the drone stays at the desired altitude.
		/// </summary>
		/// <param name="altToBe">Altitude to be.</param>
		private void gyroscopicAutopilotUpdate (float altToBe){

			// How fast we want to ascend and descend.
			float speed = 250f;

			// If we're not within an acceptable range of our desired height.
			if (Mathf.Abs (transform.position.y - altToBe) > acceptableRange) {

				// Get direction we're moving in.
				float yVelocity = body.velocity.y;

				// If it's not the right direction
				if( Mathf.Sign(yVelocity) !=  Mathf.Sign( altToBe - transform.position.y ) ){

					// Add force to change direction
					body.AddForce ( speed* Mathf.Sign(altToBe - transform.position.y)*Vector3.up*body.mass, ForceMode.Force);

				}
		
				// If we're not moving fast enough
				if( yVelocity < .1 ){

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
		/// 
		/// Returning 1, 2 does not mean the position 1, 2, but the indexes
		/// in our waypoint jagged array [1][2], which references the actual position
		/// </summary>
		/// <returns>The closest waypoint.</returns>
		private Vector2 getClosestPatrolWaypoint(){

			float clostestDistance = 100000000f;

			Vector2 waypoint = Vector2.zero;

			for (int x = 0; x < waypointsToPatrol.Length; x ++) {

				for (int y = 0; y < waypointsToPatrol[x].Length; y ++) {

					float distance = Vector2.Distance(waypointsToPatrol[x][y], new Vector2(transform.position.x,transform.position.y));
					
					if(distance < clostestDistance){
						waypoint = new Vector2(x,y);
						clostestDistance = distance;
					}

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


		/// <summary>
		/// Returns the distance the drone is from the current
		/// patrol waypoint assigned
		/// WARNING: This ignores alititude!
		/// </summary>
		/// <returns>The from current patrol waypoint.</returns>
		private float distanceFromCurrentPatrolWaypoint(){
			return Vector2.Distance (new Vector2(transform.position.x, transform.position.z), getPositionOfWaypoint(currentWayPointMovingTo));
		}


		/// <summary>
		/// Returns the value that our jagged array of waypoints contains 
		/// </summary>
		/// <returns>The position of waypoint.</returns>
		/// <param name="waypoint">Waypoint.</param>
		private Vector2 getPositionOfWaypoint(Vector2 waypoint){
			return waypointsToPatrol[(int)waypoint.x][(int)waypoint.y];
		}

	}

}