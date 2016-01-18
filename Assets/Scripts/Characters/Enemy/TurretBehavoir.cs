using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EliDavis.Characters.Enemy {

	/// <summary>
	/// Artificial Behvaior given to a turret that seeks out a target and when found fires at it.
	/// If it does not have a target it will just stay in idle state.
	/// </summary>
	public class TurretBehavoir : Enemy {

	    //Defining a list of states our Turret can be in
	    enum TurretStates { 

			/// <summary>
			/// The idle state of the turret where it is looking for a target to fire at
			/// </summary>
			Idle, 

			/// <summary>
			/// The attacking state of the turret where the turret will continually fire
			/// at a target as long as it's visible.
			/// </summary>
			AttackingTarget, 

			/// <summary>
			/// The overriden state of the turret where it await commands externally to 
			/// perform.
			/// </summary>
			Overriden 
		
		}

	    //The current state our turret is in, default to Idle where it's looking for the target
	    private TurretStates currentState = TurretStates.Idle;

	    //How far out the turret can see
		private float eyeSight = 50;

	    //Field of view of our turret, at what angle can see.
		private float fov = 40;

	    //Where we ant to fire bullets from
		private Transform bulletSpawn;


		/// <summary>
		/// When the turret is overridden, this is the position that the turret will continually fire at
		/// </summary>
		private Vector3 overridenPositionToFireAt = Vector3.zero;


		[SerializeField]
		private Slider healthBar;

		// Use this for initialization
		void Start () {

	        //Get a reference to our bullet spawn location so we can shoot from that location
	        bulletSpawn = transform.FindChild("bulletspawn");

			// Let the turret start out with max health.
			currentHealth = MAX_HEALTH;

	    }
	    
		// Update is called once per frame
		void Update () {

	        //Our state machine.  Only update whatever state we're in
	        switch (currentState)
	        {
            case TurretStates.Idle:
                idleStateUpdate();
                break;

            case TurretStates.AttackingTarget:
                attackingTargetUpdate();
                break;

			case TurretStates.Overriden:
				turretOverrideUpdate();
				break;

	        }

	        
	    }


		/// <summary>
		/// Returns the percentage of health the turret has left.
		/// </summary>
		/// <returns>The remaining health percentage.</returns>
		public float getRemainingHealthPercentage(){
			return (float)currentHealth / (float)MAX_HEALTH;
		}


		/// <summary>
		/// Overrides the turret to continually fire at a certain position until
		/// the turret is no longer overriden 
		/// </summary>
		/// <param name="positionToFireAt">Position to fire at.</param>
		public void enterOverride(Vector3 positionToFireAt){
			currentState = TurretStates.Overriden;
			overridenPositionToFireAt = positionToFireAt;
		}


		/// <summary>
		/// Exits the overriden state of the turret and gives back control to the turret's AI
		/// </summary>
		public void exitOverride(){

			if (currentState == TurretStates.Overriden) {
				currentState = TurretStates.Idle;
			}

		}


		/// <summary>
		/// Whenever we take damage update the health bar above our head
		/// </summary>
		protected override void OnDamage ()
		{

			if(healthBar != null){
				healthBar.value = getRemainingHealthPercentage();
			}

		}


		/// <summary>
		/// Create an explosion when we die
		/// </summary>
		protected override void OnDie ()
		{
			// Create a pretty little effect
			Instantiate(Resources.Load("ExplosionPrefab") as GameObject, transform.position, Quaternion.identity);
			
			// Delete the enemy from the scene.
			Destroy(transform.parent.gameObject);
		}


	    /// <summary>
	    /// Behvaior the turret will execute when it is in its idle state.
	    /// Will look for target
	    /// </summary>
	    private void idleStateUpdate()
	    {
	        //If we can now see our target
			if (getClosestVisibleTarget() != null)
	        {
	            currentState = TurretStates.AttackingTarget;
	            return;
	        }

	        //Spin around in an attempt to find the target
	        transform.Rotate(new Vector3(0, 45*Time.deltaTime*Time.timeScale,0));

	    }


	    /// <summary>
	    /// Behavior the turret will execute when it is attacking it's target
	    /// </summary>
		private void attackingTargetUpdate()
	    {

			GameObject closestVisibleTarget = getClosestVisibleTarget ();

	        // If we can't see the target anymore then let's go back to searching for it in idle
			if (closestVisibleTarget == null)
	        {
				transitionIntoIdle();
	            return;
	        }

	        // Draw line to target for debugging purposes
	        //Vector3 toPlayer = target.transform.position - bulletSpawn.transform.position;
	        //Debug.DrawLine(bulletSpawn.position, toPlayer * eyeSight, Color.red);

			Vector3 directionTargetsMovingIn = Vector3.zero;

			// TODO Improve accuracy and target tracking to better hit the target

			// Try grabbing an instance of a rigid body
			Rigidbody targetsBody = closestVisibleTarget.GetComponent<Rigidbody> ();

			// If the object has a rigid body
			if(targetsBody != null){

				// Get the velocity the target is moving
				directionTargetsMovingIn = targetsBody.velocity;

			}

	        // Look at the target's expected position and fire bullets at it
			transform.LookAt(closestVisibleTarget.transform.position + 
			                 (directionTargetsMovingIn*Time.deltaTime*Vector3.Distance(closestVisibleTarget.transform.position,this.transform.position)));

	        fireUpdate();

	    }


		/// <summary>
		/// When the turret is overriden we contually fire at the position given
		/// </summary>
		private void turretOverrideUpdate(){
			transform.LookAt (overridenPositionToFireAt);
			fireUpdate ();
		}


		/// <summary>
		/// Transitions the turret into idle state and resets it's roation
		/// </summary>
		private void transitionIntoIdle(){

			currentState = TurretStates.Idle;

			// Reset rotation of the head of the turret
			Vector3 currentRotation = transform.rotation.eulerAngles;

			currentRotation.x = 0;
			currentRotation.z = 0;

			transform.rotation = Quaternion.Euler(currentRotation);

		}


		/// <summary>
		/// Gets the closest visible target to the turret.
		/// Will return null if no targets exist that fit the criteria
		/// </summary>
		/// <returns>The closest visible target.</returns>
		private GameObject getClosestVisibleTarget(){

			Character[] potentialTargets = GameManagement.GameManager.getInstance ().getFriendlyCharacters ();

			GameObject closestTarget = null;
			float closestDistance = 10000000;

			for (int i = 0; i < potentialTargets.Length; i ++) {

				if(potentialTargets[i] == null){
					continue;
				}

				float ourDistanceFromPotentialTarget = Vector3.Distance(this.transform.position, potentialTargets[i].transform.position);

				// If this object is the closest one we've seen and it is visible by the turret
				if(ourDistanceFromPotentialTarget < closestDistance && canSeeObject(potentialTargets[i].gameObject)) {

					closestTarget = potentialTargets[i].gameObject;
					closestDistance = ourDistanceFromPotentialTarget;

				}

			}

			return closestTarget;

		}


	    /// <summary>
	    /// Determines whether or not the turret can see it's target.
	    /// This is based on it's eye sight, fov, and things obstructing the target
	    /// </summary>
	    /// <returns>True if the turret can see the target, false if it can't</returns>
		private bool canSeeObject(GameObject target)
	    {

	        //We can't see a target if we don't have one!
	        if(target == null)
	        {
	            return false;
	        }

	        //Determine the angle between our target and the direction we're looking in
	        float angle = Vector3.Angle(target.transform.position - transform.position, transform.forward);

	        //If the angle is outside our fov then we can't see the player
	        if (angle > fov)
	        {
	            return false;
	        }

	        

	        //We were unable to see the player
	        return false;

	    }


	    //How many seconds do we want to wait before we launch the next bullet
	    int fireSpeed = 2;

	    //Have we fired this second?
	    bool hasFired = false;

	    /// <summary>
	    /// Launches a bullet at rate defined by the fireSpeed
	    /// If the fireSpeed is 3, then the turret will launch a bullet every 3 seconds
	    /// </summary>
		private void fireUpdate()
	    {
	        //Get the current time since the game has started in seconds
	        int curTime = (int)Time.time;

	        //If we are at a multiple of our fireSpeed  (If this doesn't make sense look into the module (%) operator
	        if (curTime % fireSpeed == 0)
	        {
	            //If we haven't fired this second
	            if (!hasFired)
	            {
	                //Fire a bullet and set hasFired to true
	                shootBullet();
	                hasFired = true;
	            }

	        }
	        else
	        {
	            //We're not firing this second reset for the next multiple of our fireSpeed
	            hasFired = false;
	        }
	    }


	    /// <summary>
	    /// Creates a bullet and gives it physics and direction to simulate it launching from the turret.
	    /// </summary>
		private void shootBullet()
	    {

	        //Creating a basic bitch sphere to work with
	        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	        bullet.transform.name = "Bullet";

	        //move to the correct location
	        bullet.transform.position = bulletSpawn.position;
	        bullet.transform.rotation = bulletSpawn.rotation;


	        //Cutomize//
	        //Shrink
	        bullet.transform.localScale = new Vector3(.2f,.2f,.2f);


	        //Add physics//
	        bullet.AddComponent<Rigidbody>();
	        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
	        bulletBody.useGravity = false;

	        //Shooting Direction
	        Vector3 shootingDirection = transform.TransformDirection(Vector3.forward).normalized;
	        int shootingForce = 60;

	        //Add a force to the bullet in the correct direction to simulate it launching from the turret
	        bulletBody.AddForce(shootingDirection * shootingForce, ForceMode.Impulse);


	        //Add Behavoir
	        bullet.AddComponent<BulletBehavior>();

			// Try getting instance of Audio Source
			AudioSource audio = bulletSpawn.GetComponent<AudioSource> ();

			// If able to get an instance of audio source than play it
			if (audio != null) {
				audio.Play ();
			}

	    }

	}

}
