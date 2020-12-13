using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Variables
    bool outOfBounds = false;
    public bool easyPhysics = true;
    public int health = 100;
    float delayTime = 1f;
    // Movement
    [Header("Movement")]
    float stickInputY;
    float pitchRate = 100f;
    float stickInputX;
    float rollRate = 220f;
    float yawInput;
    float dampingCE = 5f;
    // Engines
    [Header("Engines")]
    float throttlePosition = 80;
    float easyCAS = 200;
    // Flight
    [Header("Flight Info")]
    float groundSpeed;
    float altitude;
    float rateOfClimb;
    float heading;
    float angleOfAttack;
    float liftCoefficient;
    bool airBrakeDeployed;
    Vector3 direction;
    // Navigation
    [Header("Navigation Computer")]
    float headingToCurrentWaypoint;
    float distanceToCurrentWaypoint;
    float timeToCurrentWaypoint;
    public int currentWaypointIndex = 0;
    // Timers
    [Header("Timers")]
    DateTime currentTime = DateTime.Now;
    float runTime;
    public int runSeconds = 0;
    public int runMinutes = 0;
    public int runHour = 0;
    float outOfBoundsTime = 5f;

    // References
    [Header("Game Control")]
    Rigidbody rb;
    [SerializeField] GameController gameController;
    [SerializeField] Text outOfBoundsWarning;
    // HUD
    [Header("HUD Elements")]
    [SerializeField] Text headingText;
    [SerializeField] Text rateOfClimbText;
    [SerializeField] Text airSpeedText;
    [SerializeField] Text altitudeText;
    [SerializeField] Text aoaText;
    [SerializeField] Text machText;
    [SerializeField] Text gText;
    [SerializeField] Text peakGText;
    [SerializeField] Text waypointDistanceNameText;
    [SerializeField] Text hourText;
    [SerializeField] Text minuteText;
    [SerializeField] Text secondText;
    // Right DDI
    [Header("Right DDI Elements")]
    [SerializeField] Text groundSpeedText;
    [SerializeField] Text trueSpeedText;
    [SerializeField] Text ddiHeadingDistanceText;
    [SerializeField] Text ddiTTTText;
    [SerializeField] Text ddiTimeText;
    // Waypoints
    public Transform currentWaypoint;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Text ddiWaypointText;
    // IFEI
    [SerializeField] Text timeText;
    // Weapons
    [Header("Weapons")]
    [SerializeField] AGM_65F weapon1;
    [SerializeField] AGM_65F weapon2;
    [SerializeField] AGM_65F weapon3;
    [SerializeField] AGM_65F weapon4;
    AGM_65F[] weaponArray;
    public GameObject target;
    int targetIndex;
    GameObject[] detectedTargets;
    int selectedWeaponIndex = 0;
    [SerializeField] int ammoRemaining = 4;
    [Header("Audio")]
    [SerializeField] AudioSource agmRifle;

    void Start()
    {
        // Grabs the Rigidboy componnent attached to this game object
        rb = GetComponent<Rigidbody>();

        // If easyPhysics is off, uses the rigidbody to apply forward starting thrust
        if (!easyPhysics)
        {
            rb.velocity = new Vector3(0f, 0f, 100);
        }

        // Sets the current waypoint to be the first waypoint in the array of waypoints
        currentWaypoint = waypoints[currentWaypointIndex];

        // Updates the waypoint text on the DDI to display the correct waypoint number
        ddiWaypointText.text = currentWaypointIndex.ToString();

        // If easy physics is on, turns off gravity for this game object
        if (easyPhysics)
        {
            rb.useGravity = false;
        }

        // If easy physics is off, forces easy physics to turn on (just for prototyping purposes I want it to always be in easy physics mode as normal physics mode isn't fully functional)
        if (!easyPhysics)
        {
            easyPhysics = true;
            rb.velocity = new Vector3(0, 0, 0);
        }

        // Registers all the loaded weapons into the weapon computer
        weaponArray = new AGM_65F[] { weapon1, weapon2, weapon3, weapon4 };

        // Searches for all enemies in the scene and stores them in the targets array
        detectedTargets = GameObject.FindGameObjectsWithTag("Enemy");

        // Sets the target to the first enemy detected
        target = detectedTargets[0];

        // Sets target index to 0
        targetIndex = 0;
    }

    void Update()
    {
        // If out of ammo, there are enemies remaining, and there are no player weapons in the scene, triggers game end from out of ammo after a short delay
        if (ammoRemaining <= 0 && GameObject.FindGameObjectsWithTag("Enemy").Length >= 1 && GameObject.FindGameObjectsWithTag("Player Weapon").Length == 0)
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0)
            {
                gameController.OutOfAmmo();
            }
        }

        // Switching targets
        if (Input.GetButtonDown("Switch Target"))
        {
            // Updates the targets array with existing enemies in the scene
            detectedTargets = GameObject.FindGameObjectsWithTag("Enemy");

            // If the last enemy isn't selected, switch to the next one, if the last one is selected, switch back to the first one
            if (targetIndex < detectedTargets.Length - 1)
            {
                targetIndex++;
            }
            else
            {
                targetIndex = 0;
            }

            // Sets the target to be the target in the target array of the specified index
            target = detectedTargets[targetIndex];

            // Sets the current waypoint to be equal to the chosen target
            currentWaypoint = target.gameObject.transform;
        }

        // If there are no enemies remaining on the map, ends the game with the win condition
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            gameController.Win();
        }

        // Updates the mission timer on the DDI and clock on the IFEI
        IncrementTime();

        // Updates all HUD elements
        UpdateHUD();

        // Updates the Navigation Computer
        UpdateNavComputer();

        // If player is out of bounds, triggers the out of bounds timer and ends the game when it reaches 0 with the out of bounds condition
        if (outOfBounds)
        {
            outOfBoundsTime -= Time.deltaTime;
            if (outOfBoundsTime <= 0)
            {
                gameController.OutOfBoundsTooLong();
            }
        }

        // If easy physics is on, pushes the aircraft forward based on easyCAS amount
        if (easyPhysics)
        {
            AdjustSpeed();
            transform.Translate(Vector3.forward * easyCAS * Time.deltaTime);
        }

        // Increases throttle when button is pressed
        if (Input.GetButtonDown("Increase Throttle"))
        {
            IncreaseThrottle();
        }

        // Decreases throttle when button is pressed
        if (Input.GetButtonDown("Decrease Throttle"))
        {
            DecreaseThrottle();
        }

        // Toggles the airbrake when the button is pressed (currently not functional)
        if (Input.GetButtonDown("Toggle Airbrake"))
        {
            if (airBrakeDeployed)
            {
                airBrakeDeployed = false;
            }
            else
            {
                airBrakeDeployed = true;
            }
        }

        // Anytime health reaches 0, game ends with player killed condition
        if (health <= 0)
        {
            gameController.Killed();
        }

        // If fire button is depressed and the player has a target, fires selected weapon
        if (Input.GetButtonDown("Fire1") && target != null)
        {
            // As long as a valid index is chosen, fires a missile
            if (selectedWeaponIndex < 4)
            {
                // Launches the selected weapon
                weaponArray[selectedWeaponIndex].Launch(target);

                // Plays the audio clip for firing of an AGM-65F
                agmRifle.Play();

                // Decreases the amount of total ammo
                ammoRemaining--;

                // Increases the index as long as the last missile wasn't the one selected
                if (selectedWeaponIndex < 3)
                {
                    selectedWeaponIndex++;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb.inertiaTensor = Vector3.one;

        // Rotates the aircraft based on the control surface settings
        transform.Rotate(Input.GetAxis("Vertical") * pitchRate * Time.deltaTime, 0.0f, -Input.GetAxis("Horizontal") * rollRate * Time.deltaTime);

        // Updates all aircraft data
        UpdateAirCraftData();
        
        // If easy physics are off, applies thrust from the engines to the plane and applies lift and drag forces
        if (!easyPhysics)
        {
            rb.AddForce(transform.forward * 158000);
            ApplyLift();
            ApplyDrag();
        }
    }

    // Updates all HUD elements
    void UpdateHUD()
    {
        // Updates based on rigidbody info
        if (!easyPhysics)
        {
            airSpeedText.text = Conversions.Speed(groundSpeed).ToString();
            machText.text = Conversions.Mach(groundSpeed).ToString();
            groundSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "G";
            trueSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "T";
        }
        // Updates based on transform info
        else
        {
            airSpeedText.text = Conversions.Speed(easyCAS).ToString();
            machText.text = Conversions.Mach(easyCAS).ToString();
            groundSpeedText.text = Conversions.Speed(easyCAS).ToString() + "G";
            trueSpeedText.text = Conversions.Speed(easyCAS).ToString() + "T";
        }
        altitudeText.text = Conversions.Altitude(altitude).ToString();
        rateOfClimbText.text = Conversions.Speed(rateOfClimb).ToString();
        headingText.text = heading.ToString();
        aoaText.text = Math.Round(angleOfAttack, 1).ToString();
        secondText.text = runSeconds.ToString();
        minuteText.text = runMinutes.ToString();
        hourText.text = runHour.ToString();
        ddiTimeText.text = runHour.ToString() + ":" + runMinutes.ToString() + ":" + runSeconds.ToString() + "EI";
        UpdateClock();
    }

    // Updates many physics based data
    void UpdateAirCraftData()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        groundSpeed = localVelocity.z;
        altitude = transform.position.y;
        rateOfClimb = rb.velocity.y;
        heading = (float)Math.Round(transform.rotation.eulerAngles.y);
        direction = Vector3.Normalize(rb.velocity)/* 360*/;
        CalculateAoA();
    }

    // Updates the clock
    void UpdateClock()
    {
        currentTime = DateTime.Now;
        string hour;
        string minute;
        string second;
        if (currentTime.Hour < 10)
        {
            hour = "0" + currentTime.Hour.ToString();
        }
        else
        {
            hour = currentTime.Hour.ToString();
        }
        if (currentTime.Minute < 10)
        {
            minute = "0" + currentTime.Minute.ToString();
        }
        else
        {
            minute = currentTime.Minute.ToString();
        }
        if (currentTime.Second < 10)
        {
            second = "0" + currentTime.Second.ToString();
        }
        else
        {
            second = currentTime.Second.ToString();
        }
        timeText.text = hour + ":" + minute + ":" + second;
    }

    // Updates the timer
    void IncrementTime()
    {
        runTime += Time.deltaTime;
        if (runTime >= 1f)
        {
            runSeconds += 1;
            if (runSeconds == 60)
            {
                runMinutes += 1;
                runSeconds = 0;
                if (runMinutes == 60)
                {
                    runHour += 1;
                    runMinutes = 0;
                }
            }
            runTime = 0;
        }
    }

    // Calculates and applies lift forces
    void ApplyLift()
    {
        // Using formula for lift: L = (1/2) * d * v^2 * s * CL
        // L = Newtons of lift
        // d = Density of the air in kg/m^3 (1.225 at sea level and 15C and dry air)
        // v = Velocity in m/s of the object traveling through the air
        // s = Wing area of the plane in square meters
        // CL = Lift Coefficient (based on AoA and specific wing of the plane)
        float lift = 0.5f * 1.225f * (float)Math.Pow(groundSpeed, 2) * 38f * CalculateLiftCoefficient(angleOfAttack);
        rb.AddForce(transform.up * lift);
    }

    // Calculates angle of attack
    void CalculateAoA()
    {
        Vector3 localVelocityDir = transform.InverseTransformDirection(rb.velocity).normalized;
        //angleOfAttack = Mathf.Atan2(transform.forward.y - localVelocityDir.y, transform.forward.z - localVelocityDir.z);
        angleOfAttack = Vector3.SignedAngle(transform.forward, rb.velocity.normalized, Vector3.right);
        Debug.DrawRay(transform.position, transform.forward * 500, Color.green);
        Debug.DrawRay(transform.position, rb.velocity.normalized * 500, Color.red);

    }

    // Calculates and applies drag forces
    void ApplyDrag()
    {
        // Using the formula for drag: D = (1/2) * p * v^2 * CD * A
        // D = Drag force in Newtons
        // p = Density of the air in kg/m^3 (1.225 at sea level at 15C and dry air)
        // v = Velocity of the object traveling through the air
        // CD = Drag Coefficient
        // A = Cross sectional area of the object traveling through the air
        // NOTE: NEED TO ADJUST THE A TO CHANGE WITH PITCH OF THE AIRCRAFT DUE TO INCREASE EXPOSED SURFACE AREA OF THE WING TO THE ONCOMING AIR
        float drag = 0.5f * 1.225f * (float)Math.Pow(groundSpeed, 2) * 15f;
        rb.AddForce(-rb.velocity.normalized * drag);
    }

    // Calculates the lift coefficient based on the angle of attack
    float CalculateLiftCoefficient(float aoaRad)
    {
        float aoa = aoaRad/* * Mathf.Rad2Deg*/;
        if (aoa <= -5f)
        {
            return 0f;
        }
        else if (aoa > -5f && aoa < -3f)
        {
            return 0.25f;
        }
        else if (aoa >= -3f && aoa < 0f)
        {
            return 0.5f;
        }
        else if (aoa >= 0f && aoa < 2f)
        {
            return 0.75f;
        }
        else if (aoa >= 2f && aoa < 5f)
        {
            return 1f;
        }
        else if (aoa >= 5f && aoa < 7f)
        {
            return 1.25f;
        }
        else if (aoa >= 7f && aoa < 10f)
        {
            return 1.5f;
        }
        else if (aoa >= 10f && aoa < 12f)
        {
            return 1.6f;
        }
        else if (aoa >= 12f && aoa < 19f)
        {
            return 1.7f;
        }
        else if (aoa >= 19f && aoa < 23f)
        {
            return 1.5f;
        }
        else
        {
            return 1.0f;
        }
    }

    // Updates the navigation computer
    void UpdateNavComputer()
    {
        if (currentWaypoint != null)
        {
            // Get heading to waypoint
            headingToCurrentWaypoint = (float)((System.Math.Atan2((transform.position.x - currentWaypoint.position.x), (transform.position.z - currentWaypoint.position.z)) / System.Math.PI) * 180f);
            if (headingToCurrentWaypoint < 0)
            {
                headingToCurrentWaypoint += 360f;
            }
            // Get distance to waypoint
            distanceToCurrentWaypoint = (float)Conversions.MeterstoNauticalMiles(Vector3.Distance(transform.position, currentWaypoint.transform.position));

            // Get ToT
            // TODO FORMULA HERE FOR TOT TO WAYPOINT HERE
            ddiWaypointText.text = currentWaypointIndex.ToString();
            ddiHeadingDistanceText.text = headingToCurrentWaypoint.ToString() + "°/ " + distanceToCurrentWaypoint.ToString();
            waypointDistanceNameText.text = distanceToCurrentWaypoint.ToString() + " " + currentWaypoint.gameObject.name;
        }
    }

    // Cycles to the next waypoint in the array
    public void CycleToNextWaypoint()
    {
        currentWaypointIndex += 1;
        currentWaypoint = waypoints[currentWaypointIndex];
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If player hits the ground, triggers game end with crashed condition
        if (collision.gameObject.tag == "Terrain")
        {
            gameController.Crashed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If player exits map boundaries, triggers out of bounds timer
        if (other.gameObject.tag == "Bounds")
        {
            outOfBounds = true;
            outOfBoundsWarning.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player enters map area of operations, resets out of bounds timer
        if (other.gameObject.tag == "Bounds")
        {
            outOfBounds = false;
            outOfBoundsTime = 10f;
            outOfBoundsWarning.enabled = false;
        }
    }

    // Increase throttle position
    private void IncreaseThrottle()
    {
        if (throttlePosition != 100)
        {
            throttlePosition += 5;
        }
    }

    // Decrease throttle position
    private void DecreaseThrottle()
    {
        if (throttlePosition != 80)
        {
            throttlePosition -= 5;
        }
    }

    // Adjusts the aircraft speed based on throttle position
    void AdjustSpeed()
    {
        if (throttlePosition == 80)
        {
            if (easyCAS >= 80)
            {
                LoseSpeed();
            }
        }
        else if (throttlePosition == 85)
        {
            if (easyCAS > 100)
            {
                LoseSpeed();
            }
            else
            {
                GainSpeed();
            }
        }
        else if (throttlePosition == 90)
        {
            if (easyCAS > 231)
            {
                LoseSpeed();
            }
            else
            {
                GainSpeed();
            }
        }
        else if (throttlePosition == 95)
        {
            if (easyCAS > 257)
            {
                LoseSpeed();
            }
            else
            {
                GainSpeed();
            }
        }
        else if (throttlePosition == 100)
        {
            if (easyCAS > 308)
            {
                LoseSpeed();
            }
            else
            {
                GainSpeed();
            }
        }
    }

    // Increases speed
    void GainSpeed()
    {
        if (!airBrakeDeployed)
        {
            easyCAS += 5f * Time.deltaTime;
        }
        else
        {
            easyCAS += 2.5f * Time.deltaTime;
        }
    }

    // Decreases speed
    void LoseSpeed()
    {
        if (!airBrakeDeployed)
        {
            easyCAS -= 2.5f * Time.deltaTime;
        }
        else
        {
            easyCAS -= 5f * Time.deltaTime;
        }
    }
    /* Vector3 dir = target.position - player.position;
     * float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
     * this.transform.localEulerAngles = new Vector3(0, 0, angle);
    */
}