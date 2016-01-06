using UnityEngine;
using System.Collections;

namespace PlayerInGameControl {

	/// <summary>
	/// This dictates the behavior of all the powers that the player has at 
	/// their disposal
	/// 
	/// Powers:
	/// Teleport - Teleports the player to the marker location indicated.
	/// 
	/// Force Push - Pushes game objects that have rigid bodies attatched off
	/// in the direction othe player is looking at.
	/// </summary>
	public class PlayerPowerBehavior : MonoBehaviour {


		/// <summary>
		/// Reference to the main player behavoir that keeps up
		/// with things such as UI and health and other central themes.
		/// </summary>
		private PlayerBehavior playerBehavoir = null;


	    //How far the player is allowed to teleport
	    int teleportDistance = 25;


	    //The different states the player could be in
	    enum PlayerState  {
	          Playing,
	          Teleporting  
	    }


	    //The current state the player is in.
	    PlayerState currentPlayerState = PlayerState.Playing;


	    void Start()
	    {
	        desiredFOV = (int)Camera.main.fieldOfView;

			playerBehavoir = gameObject.GetComponent<PlayerBehavior> ();

	    }


	    // Update is called once per frame
	    void Update () {

	        //Make sure to keep the cursor in the center of the screen
	        Cursor.lockState = CursorLockMode.Locked;

	        //Update appropriataly based on the current state.
	        switch (currentPlayerState)
	        {
	            case PlayerState.Playing:
	                playingUpdate();
	                break;

	            case PlayerState.Teleporting:
	                teleportUpdate();
	                break;
	        }

	        //Keep the field of view up to date
	        fovUpdate();

	    }

	    //called once per frame for every collider/rigidbody that is touching rigidbody/collider.
	    void OnCollisionStay()
	    {

	        //Update appropriately based on the current state
	        switch (currentPlayerState)
	        {
	            case PlayerState.Teleporting:
	                teleportOnCollisionStay();
	                break;
	        }

	    }


	    //Reference to the teleport marker that indicates where we're going to be teleporting
	    GameObject teleportMarker = null;


	    /// <summary>
	    /// The update function for when we're in the playing state (Player has free control of the player)
	    /// </summary>
	    void playingUpdate()
	    {

	        //This will update our teleport marker
			if (Input.GetButton("Power1"))
	        {

	            //If we don't have a telport marker to show where we're going create one
	            if (teleportMarker == null)
	            {
	                //Create the base market
	                teleportMarker = new GameObject("Teleport Marker");
	                
	                //Create the effects that mimick dishonered's
	                GameObject effects = Resources.Load("TeleportMarkerEffect") as GameObject;
	                effects = Instantiate(effects);
	                effects.transform.parent = teleportMarker.transform;
	                effects.transform.position = Vector3.zero;
	            }

	            //Create a ray from where the mouse is on the screen.
	            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	            RaycastHit hit;

	            //Cast ray to see where we are going to teleport too.
	            if (Physics.Raycast(ray, out hit, teleportDistance))
	            {
	                //We don't want the marker to be stuck inside whatever we're teleporting too so let's back it up some
	                teleportMarker.transform.position = ray.origin + (ray.direction * hit.distance) - ray.direction.normalized;
	            }
	            else
	            {
	                //We're just going to be teleporting into the air.
	                teleportMarker.transform.position = ray.origin + (ray.direction * teleportDistance);
	            }

	        }

	        //Check whether or not their trying to use a power
			if (Input.GetButtonUp("Power1"))
	        {

	            //Don't need the indicator to where we're going to update now.
	            if(teleportMarker != null)
	            {
	                Destroy(teleportMarker);
	            }

				// If we have enough boost power to execute the power
				if(playerBehavoir.requestBoostPower(30f)){

					//Create a ray from where the mouse is on the screen.
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					
					//Cast ray to see where we are going to teleport too.
					if (Physics.Raycast(ray, out hit, teleportDistance))
					{
						//Teleport if we hit something within our teleport distance
						teleportTo(hit.point);
					} 
					else
					{
						//Just teleport into the air where ever we where aiming
						teleportTo(ray.origin + (ray.direction*teleportDistance));
					}

				} else {
					Audio.SoundEffects.Play (Audio.SoundEffectType.NotEnoughBoostError);
				}

	        }

	        //Power that "force pushes" Objects that have rigid bodies
			if (Input.GetButtonUp("Power2"))
	        {
				if(playerBehavoir.requestBoostPower(10f)){
					forcePush();
				} else {
					Audio.SoundEffects.Play (Audio.SoundEffectType.NotEnoughBoostError);
				}

	        }

	    }


		/// <summary>
		/// Forces the push.
		/// </summary>
		void forcePush(){

			//Grabs all rigid bodies in scene
			Rigidbody[] rigidBodiesInScene = GameObject.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
			
			//Iterate through all rigid bodies in the scene
			for (int i = 0; i < rigidBodiesInScene.Length; i++)
			{
				
				//Distance from player
				float distance = Vector3.Distance(rigidBodiesInScene[i].transform.position, transform.position);
				
				//How far our power effects objects in the scene
				float powerMaxDistance = 10f;
				
				//ifi the object is close enough for our power to have an effect
				if (distance < powerMaxDistance)
				{
					
					//Determine how much force we want to add to the object ( The closer the stronger )
					float force = ((powerMaxDistance - distance)/ powerMaxDistance) * 80f;
					
					//Add that force in the direction the player is looking
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					rigidBodiesInScene[i].AddForce(ray.direction* force,ForceMode.Impulse);
				}
				
			}

		}


	    /// <summary>
	    /// Changes the player state to teleporting and set's up the effects to take place as they are teleporting
	    /// </summary>
	    /// <param name="teleportPlayerToPosition">The position we want the player to end in once the teleport is finnished</param>
	    void teleportTo(Vector3 teleportPlayerToPosition)
	    {

	        //slow down time 
	        Time.timeScale = .5f;
	        
	        //Set the position we want our teleport update to try moving the player too
	        positionTeleportingTo = teleportPlayerToPosition;

	        //Make sure the rigidbody isn't using gravity pulling the player down as they try teleporting upwards
	        transform.GetComponent<Rigidbody>().useGravity = false;
	        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

	        //Change state to us teleporting
	        currentPlayerState = PlayerState.Teleporting;

	        //Set a new desired fov to add "tunnel vision" to the teleport effect
	        originalFOV = Camera.main.fieldOfView;
	        desiredFOV = (int)(originalFOV * (7/3));

	    }


	    //The point where we want the player ot be once the teleport is finnished
	    Vector3 positionTeleportingTo = Vector3.zero;

	    //Temp variable used to hold original FOV before we started teleporting
	    float originalFOV = 0f;

	    /// <summary>
	    /// Our Teleport Update that Lerps the player from Point A to Point B
	    /// </summary>
	    void teleportUpdate()
	    {
	        
	        //If we're not teleporting then we need to back out
	        if (currentPlayerState != PlayerState.Teleporting)
	        {
	            return;
	        }

	        //If we're half a unit to our desitnation no need to get any closer
	        if ( (transform.position - positionTeleportingTo).magnitude < .5f )
	        {
	            stopTeleporting();
	        }

	        //Arbitrary speed for how fast we want the player to move
	        float teleportSpeed = 65f;

	        //Lerp the position to the desired position for teleportation
	        transform.position = Vector3.Lerp(transform.position, positionTeleportingTo, teleportSpeed*Time.deltaTime*Time.timeScale);
	        
	    }

	    /// <summary>
	    /// Turns off any effects of teleporting and goes back to play state.
	    /// </summary>
	    void stopTeleporting()
	    {
	        //Turn gravity back on for the player
	        transform.GetComponent<Rigidbody>().useGravity = true;

	        //Change the time back to normal
	        Time.timeScale = 1f;

	        //Give player back movement
	        currentPlayerState = PlayerState.Playing;

	        //Change the FOV back to being more appropriate
	        desiredFOV = (int)originalFOV;
	    }

	    /// <summary>
	    /// If we're teleporting and we hit something we should stop teleporting 
	    /// </summary>
	    void teleportOnCollisionStay()
	    {
	        stopTeleporting();
	    }


	    //The field of view we want the player to have
	    int desiredFOV = 60;

	    /// <summary>
	    /// Smoothly changes field of fiew.
	    /// Updates only when the desiredFOV is not equal to the actual FOV, then begins Lerping values
	    /// </summary>
	    void fovUpdate()
	    {
	        //Stop moving if we're already at the desired fov
	        if(Camera.main.fieldOfView == desiredFOV)
	        {
	            return;
	        }

	        //Move the camera toward the desired field of view
	        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFOV,10*Time.deltaTime);
	    }

	}

}
