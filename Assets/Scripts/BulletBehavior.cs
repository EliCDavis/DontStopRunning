using UnityEngine;
using System.Collections;

/// <summary>
/// Bullet behavior we want to have happen for a bullet that is launched from a Turret
/// Explodes on contact
/// </summary>
public class BulletBehavior : MonoBehaviour {

    //The explosion effect we want to have happen
    GameObject explosionPrefab;

	// Use this for initialization
	void Start () {

        //Set the explosion effect we want to have happen when this object is created.
        explosionPrefab = Resources.Load("ExplosionPrefab") as GameObject;

    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    /// <summary>
    /// MonoBehaviour override that is called when our rigidbody detects a collision
    /// </summary>
    void OnCollisionEnter()
    {
        //Instantiate an explosion
        Instantiate(explosionPrefab,transform.position,transform.rotation);

        //Delete ourselves from the scene because we exploded
        Destroy(gameObject);
    }

}
