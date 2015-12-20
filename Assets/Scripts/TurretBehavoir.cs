using UnityEngine;
using System.Collections;

/// <summary>
/// Artificial Behvaior given to a turret that seeks out a target and when found fires at it.
/// If it does not have a target it will just stay in idle state.
/// </summary>
public class TurretBehavoir : MonoBehaviour {

    //Defining a list of states our Turret can be in
    enum TurretStates { Idle, AttackingTarget }

    //The current state our turret is in, default to Idle where it's looking for the target
    TurretStates currentState = TurretStates.Idle;

    //How far out the turret can see
    float eyeSight = 25;

    //Field of view of our turret, at what angle can see.
    float fov = 30;

    //Where we ant to fire bullets from
    Transform bulletSpawn;

    //Target of the turret
    GameObject target;

	// Use this for initialization
	void Start () {

        //Get a reference to our bullet spawn location so we can shoot from that location
        bulletSpawn = transform.FindChild("bulletspawn");

        //Define a target for the turret to look for and attack
        target = GameObject.Find("Player");

    }

    
	// Update is called once per frame
	void Update () {

        //If we don't have a target then let's not do anything!
        if(target == null)
        {
            return;
        }

        //Our state machine.  Only update whatever state we're in
        switch (currentState)
        {
            case TurretStates.Idle:
                idleStateUpdate();
                break;

            case TurretStates.AttackingTarget:
                attackingPlayerUpdate();
                break;
        }

        
    }

    /// <summary>
    /// Behvaior the turret will execute when it is in its idle state.
    /// Will look for target
    /// </summary>
    void idleStateUpdate()
    {
        //If we can now see our target
        if (canSeeTarget())
        {
            currentState = TurretStates.AttackingTarget;
            return;
        }

        //Spin around in an attempt to find the target
        transform.Rotate(new Vector3(0, 45*Time.deltaTime*Time.timeScale,0));

        //Draw line to target for debugging purposes
        if(target != null)
        {
            Vector3 toPlayer = target.transform.position - bulletSpawn.transform.position;
            Debug.DrawLine(bulletSpawn.position, toPlayer * eyeSight);
        }

    }

    /// <summary>
    /// Behavior the turret will execute when it is attacking it's target
    /// </summary>
    void attackingPlayerUpdate()
    {
        //If we can't see the target anymore then let's go back to searching for it in idle
        if (!canSeeTarget())
        {
            currentState = TurretStates.Idle;
            return;
        }

        //Draw line to target for debugging purposes
        Vector3 toPlayer = target.transform.position - bulletSpawn.transform.position;
        Debug.DrawLine(bulletSpawn.position, toPlayer * eyeSight, Color.red);

        //Look at the target and fire bullets at it
        transform.LookAt(target.transform.position);
        fireUpdate();

    }


    /// <summary>
    /// Determines whether or not the turret can see it's target.
    /// This is based on it's eye sight, fov, and things obstructing the player
    /// </summary>
    /// <returns>True if the turret can see the player, false if it can't</returns>
    bool canSeeTarget()
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

        //Variable for storing hit results of our raycast
        RaycastHit hit;

        //Direction we want the raycast to go in.
        Vector3 toPlayer = target.transform.position - bulletSpawn.transform.position;

        //Shoot a ray cast aimed at the player to see if it hits.
        //NOTE: A smarter Turret would shoot multiple raycasts at multiple points at the player incase 
        //only part of the player is covered up
        if (Physics.Raycast(bulletSpawn.position, toPlayer, out hit, eyeSight))
        {
            //If what we hit is our target
            if (hit.transform.name == target.name)
            {
                return true;
            }
        }

        //We were unable to see the player
        return false;
    }


    //How many seconds do we want to wait before we launch the next bullet
    int fireSpeed = 3;

    //Have we fired this second?
    bool hasFired = false;

    /// <summary>
    /// Launches a bullet at rate defined by the fireSpeed
    /// If the fireSpeed is 3, then the turret will launch a bullet every 3 seconds
    /// </summary>
    void fireUpdate()
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
    void shootBullet()
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
        int shootingForce = 30;

        //Add a force to the bullet in the correct direction to simulate it launching from the turret
        bulletBody.AddForce(shootingDirection * shootingForce, ForceMode.Impulse);


        //Add Behavoir
        bullet.AddComponent<BulletBehavior>();

    }

}
