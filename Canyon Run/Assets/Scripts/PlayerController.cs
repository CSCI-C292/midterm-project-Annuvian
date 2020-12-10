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
    // Movement
    float stickInputY;
    float pitchRate = 100f;
    float stickInputX;
    float rollRate = 220f;
    float yawInput;
    float dampingCE = 5f;
    // Engines
    float throttlePosition = 80;
    float easyCAS = 80;
    // Flight
    float groundSpeed;
    float altitude;
    float rateOfClimb;
    float heading;
    float angleOfAttack;
    float liftCoefficient;
    bool airBrakeDeployed;
    Vector3 direction;
    // Navigation
    float headingToCurrentWaypoint;
    float distanceToCurrentWaypoint;
    float timeToCurrentWaypoint;
    public int currentWaypointIndex = 0;
    // Timers
    DateTime currentTime = DateTime.Now;
    float runTime;
    public int runSeconds = 0;
    public int runMinutes = 0;
    public int runHour = 0;
    float outOfBoundsTime = 5f;

    // References
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
    public Transform currentWaypoint;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Text ddiWaypointText;
    // IFEI
    [SerializeField] Text timeText;
    [Header("Weapons")]
    [SerializeField] AGM_65F weapon1;
    [SerializeField] AGM_65F weapon2;
    [SerializeField] AGM_65F weapon3;
    [SerializeField] AGM_65F weapon4;
    AGM_65F[] weaponArray;
    public GameObject target;
    int selectedWeaponIndex = 0;
    [SerializeField] int ammoRemaining = 4;
    [Header("Audio")]
    [SerializeField] AudioSource agmRifle;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!easyPhysics)
        {
            rb.velocity = new Vector3(0f, 0f, 100);
        }
        currentWaypoint = waypoints[currentWaypointIndex];
        ddiWaypointText.text = currentWaypointIndex.ToString();
        if (easyPhysics)
        {
            rb.useGravity = false;
        }
        if (!easyPhysics)
        {
            easyPhysics = true;
            rb.velocity = new Vector3(0, 0, 0);
        }
        weaponArray = new AGM_65F[] { weapon1, weapon2, weapon3, weapon4 };

    }

    void Update()
    {
        if (currentWaypoint == null)
        {
            gameController.Win();
        }
        IncrementTime();
        UpdateHUD();
        UpdateNavComputer();
        if (outOfBounds)
        {
            outOfBoundsTime -= Time.deltaTime;
            if (outOfBoundsTime <= 0)
            {
                gameController.OutOfBoundsTooLong();
            }
        }
        if (easyPhysics)
        {
            AdjustSpeed();
            transform.Translate(Vector3.forward * easyCAS * Time.deltaTime);
        }
        if (Input.GetButtonDown("Increase Throttle"))
        {
            IncreaseThrottle();
        }
        if (Input.GetButtonDown("Decrease Throttle"))
        {
            DecreaseThrottle();
        }
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
        if (health <= 0)
        {
            gameController.Killed();
        }
        if (Input.GetButtonDown("Fire1") && target != null)
        {
            if (selectedWeaponIndex < 4)
            {
                weaponArray[selectedWeaponIndex].Launch(target);
                agmRifle.Play();
                ammoRemaining--;
                if (selectedWeaponIndex < 3)
                {
                    selectedWeaponIndex++;
                }
            }
            if (ammoRemaining <= 0 && currentWaypoint != null)
            {
                gameController.OutOfAmmo();
            }
        }
    }

    private void FixedUpdate()
    {
        rb.inertiaTensor = Vector3.one;
        transform.Rotate(Input.GetAxis("Vertical") * pitchRate * Time.deltaTime, 0.0f, -Input.GetAxis("Horizontal") * rollRate * Time.deltaTime);
        UpdateAirCraftData();
        
        if (!easyPhysics)
        {
            rb.AddForce(transform.forward * 158000);
            ApplyLift();
            ApplyDrag();
        }
    }

    void UpdateHUD()
    {
        if (!easyPhysics)
        {
            airSpeedText.text = Conversions.Speed(groundSpeed).ToString();
            machText.text = Conversions.Mach(groundSpeed).ToString();
            groundSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "G";
            trueSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "T";
        }
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

    void CalculateAoA()
    {
        Vector3 localVelocityDir = transform.InverseTransformDirection(rb.velocity).normalized;
        //angleOfAttack = Mathf.Atan2(transform.forward.y - localVelocityDir.y, transform.forward.z - localVelocityDir.z);
        angleOfAttack = Vector3.SignedAngle(transform.forward, rb.velocity.normalized, Vector3.right);
        Debug.DrawRay(transform.position, transform.forward * 500, Color.green);
        Debug.DrawRay(transform.position, rb.velocity.normalized * 500, Color.red);

    }

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
            waypointDistanceNameText.text = distanceToCurrentWaypoint.ToString() + " " + currentWaypoint.name;
        }
    }

    public void CycleToNextWaypoint()
    {
        currentWaypointIndex += 1;
        currentWaypoint = waypoints[currentWaypointIndex];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            gameController.Crashed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bounds")
        {
            outOfBounds = true;
            outOfBoundsWarning.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bounds")
        {
            outOfBounds = false;
            outOfBoundsTime = 10f;
            outOfBoundsWarning.enabled = false;
        }
    }

    private void IncreaseThrottle()
    {
        if (throttlePosition != 100)
        {
            throttlePosition += 5;
        }
    }

    private void DecreaseThrottle()
    {
        if (throttlePosition != 80)
        {
            throttlePosition -= 5;
        }
    }

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