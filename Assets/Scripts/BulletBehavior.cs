using UnityEngine;
using System.Collections;
using PlayerInGameControl;

/// <summary>
/// Bullet behavior we want to have happen for a bullet that is launched from a Turret
/// Explodes on contact
/// </summary>
public class BulletBehavior : MonoBehaviour {


	/// <summary>
	/// The explosion effect we want to have happen when
	/// the bullet collides with something
	/// </summary>
    GameObject explosionPrefab;


	/// <summary>
	/// The max distance the explosive bullet can deal damage to characters
	/// </summary>
	float range = 5f;


	/// <summary>
	/// The max damage a bullet can do at the closest distance to the player
	/// </summary>
	float damage = 25f;


	/// <summary>
	/// The lifetime the bullet had before self detonating
	/// </summary>
	float lifetime = 5f;


	/// <summary>
	/// Inflicts all damage on characters within a specified range.
	/// </summary>
	void damageCharacters(){

		PlayerInGameControl.PlayerBehavior[] players = GameObject.FindObjectsOfType(typeof(PlayerInGameControl.PlayerBehavior)) as PlayerInGameControl.PlayerBehavior[];

		for (int i = 0; i < players.Length; i ++) {

			float distance = Vector3.Distance(players[i].transform.position, transform.position);

			if(distance < range){
				players[i].damage( (1 - (distance/range)) * damage );
			}

		}

	}


	// Use this for initialization
	void Start () {

        //Set the explosion effect we want to have happen when this object is created.
        explosionPrefab = Resources.Load("ExplosionPrefab") as GameObject;

		Destroy (gameObject, 5f);

    }


    /// <summary>
    /// MonoBehaviour override that is called when our rigidbody detects a collision
    /// </summary>
    void OnCollisionEnter()
    {
		detonate ();
    }


	/// <summary>
	/// Explodes the bullet and deals appropriate damage to characters
	/// </summary>
	void detonate(){
		//Instantiate an explosion
		Instantiate(explosionPrefab,transform.position,transform.rotation);
		
		// Damage Anyone near
		damageCharacters ();
		
		//Delete ourselves from the scene because we exploded
		Destroy(gameObject);
	}


}