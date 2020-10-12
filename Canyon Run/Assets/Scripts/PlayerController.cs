using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Variables
    // Movement
    float stickInputY;
    float pitchRate = 50f;
    float stickInputX;
    float rollRate = 120f;
    float yawInput;
    float dampingCE = 5f;
    // Flight
    float groundSpeed;
    float altitude;
    float rateOfClimb;
    float heading;
    float angleOfAttack;
    Vector3 direction;
    // Navigation
    float headingToCurrentWaypoint;
    float distanceToCurrentWaypoint;
    float timeToCurrentWaypoint;
    // Timers
    DateTime currentTime = DateTime.Now;

    // References
    Rigidbody rb;
    // HUD
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
    [SerializeField] Text groundSpeedText;
    [SerializeField] Text trueSpeedText;
    [SerializeField] Text ddiHeadingDistanceText;
    [SerializeField] Text ddiTTTText;
    [SerializeField] Text ddiTimeText;
    [SerializeField] Transform currentWaypoint;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Text ddiWaypointText;
    // IFEI
    [SerializeField] Text timeText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0f, 0f, 100);
    }

    void Update()
    {
        UpdateHUD();
        /*stickInputY = Input.GetAxis("Vertical") * pitchRate * Time.deltaTime;
        stickInputX = Input.GetAxis("Horizontal") * rollRate * Time.deltaTime;
        transform.Rotate(stickInputY, 0f, -stickInputX, Space.Self);*/
    }

    private void FixedUpdate()
    {
        rb.inertiaTensor = Vector3.one;
        transform.Rotate(Input.GetAxis("Vertical"), 0.0f, -Input.GetAxis("Horizontal"));
        rb.AddTorque(DampenMovement());
        UpdateAirCraftData();
        rb.AddForce(transform.forward * 158000);
        ApplyLift();
    }

    void UpdateHUD()
    {
        airSpeedText.text = Conversions.Speed(groundSpeed).ToString();
        altitudeText.text = Conversions.Altitude(altitude).ToString();
        machText.text = Conversions.Mach(groundSpeed).ToString();
        groundSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "G";
        trueSpeedText.text = Conversions.Speed(groundSpeed).ToString() + "T";
        rateOfClimbText.text = Conversions.Speed(rateOfClimb).ToString();
        headingText.text = heading.ToString();
        aoaText.text = Math.Round(angleOfAttack, 1).ToString();
        UpdateClock();
    }

    void UpdateAirCraftData()
    {
        //groundSpeed = rb.velocity.z;
        groundSpeed = Mathf.Max(0f, rb.velocity.magnitude);
        altitude = transform.position.y;
        rateOfClimb = rb.velocity.y;
        heading = transform.rotation.eulerAngles.y;
        direction = Vector3.Normalize(rb.velocity) * 360;
        angleOfAttack = Vector3.Angle(transform.forward, direction);
        //CalculateAoA();
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

    void ApplyLift()
    {
        float lift = 0.5f * 1.225f * (float)Math.Pow(groundSpeed, 2) * 38f * angleOfAttack;
        rb.AddForce(Vector3.up * lift);
    }

    void CalculateAoA()
    {
        Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
        angleOfAttack = Mathf.Atan2(velocity.y, velocity.z);
        //angleOfAttack = Vector3.Dot(transform.forward, rb.velocity.normalized);
    }

    Vector3 DampenMovement()
    {
        Vector3 localVelocityAngle = rb.angularVelocity;
        Vector3 damping = localVelocityAngle * -dampingCE;
        return damping;
    }
    /* Vector3 dir = target.position - player.position;
     * float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
     * this.transform.localEulerAngles = new Vector3(0, 0, angle);
    */
}