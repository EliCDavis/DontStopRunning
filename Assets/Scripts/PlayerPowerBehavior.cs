using UnityEngine;
using System.Collections;

public class PlayerPowerBehavior : MonoBehaviour {

    public Collider coll;
    void Start()
    {
        //coll = GetComponent<Collider>();
        
    }

    // Update is called once per frame
    void Update () {

        Cursor.lockState = CursorLockMode.Locked;

        //if (Input.GetButton("Fire1"))
        if (Input.GetButton("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.Log("Clicked!");
            if (coll.Raycast(ray, out hit, 100.0F))
            {
                Debug.Log("Hit");
                transform.position = hit.point;
            }

        }
    }
}
