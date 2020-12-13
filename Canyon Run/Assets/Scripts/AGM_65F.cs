using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGM_65F : MonoBehaviour
{
    // Fields
    // Engine
    bool hasLaunched = false;
    float primaryThrust = 44557.836f;
    float secondaryThrust = 9674.882f;
    float thrustTime1 = 0.575f;
    float thrustTime2 = 3.495f;

    // References
    Rigidbody rb;
    [SerializeField] GameObject explosion;
    PlayerController player;
    // Targeting
    GameObject target;
    Vector3 targetPosition;
    
    private void Start()
    {
        // Finds player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Accesses the Rigidbody on this game object
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Controls the burn time of the 2 stage engine
        if (hasLaunched)
        {
            if (thrustTime1 > 0)
            {
                thrustTime1 -= Time.deltaTime;
            }
            else if (thrustTime2 > 0)
            {
                thrustTime2 -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        // Keeps missile looking at the target and applies the 2 stage thrust forces
        if (hasLaunched)
        {
            transform.LookAt(targetPosition);
            if (thrustTime1 > 0)
            {
                rb.AddForce(transform.forward * primaryThrust);
            }
            else if (thrustTime2 > 0)
            {
                rb.AddForce(transform.forward * secondaryThrust);
            }
        }
    }

    // Launches the missile
    public void Launch(GameObject target)
    {
        hasLaunched = true;

        // Sets the parent of the missile to null
        transform.SetParent(null, true);

        // Sets the target of this missile to match the target of the player at time of launch
        this.target = target;

        // Sets the target to be the position of the target (so if target is destroyed while missile is in flight, there is no error)
        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        // Face the target before launch
        transform.LookAt(targetPosition);

        // Initial boost to throw missile away from the rack
        rb.AddForce(transform.forward * 308, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When missile contacts something it spawns an explosion
        Instantiate(explosion, transform.position, explosion.transform.rotation);

        // If the missile hits something that isn't the terrain, it destroys that thing instantly
        if(collision.gameObject.tag != "Terrain")
        {
            Destroy(collision.gameObject);
        }

        // Missile is destroyed
        Destroy(gameObject);
    }
}