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
    float pitchRate = 100f;
    float stickInputX;
    float rollRate = 220f;
    float yawInput;
    float dampingCE = 5f;
    // Flight
    float groundSpeed;
    float altitude;
    float rateOfClimb;
    float heading;
    float angleOfAttack;
    float liftCoefficient;
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
        transform.Rotate(Input.GetAxis("Vertical") * pitchRate * Time.deltaTime, 0.0f, -Input.GetAxis("Horizontal") * rollRate * Time.deltaTime);
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

    void ApplyLift()
    {
        float lift = 0.5f * 1.225f * (float)Math.Pow(groundSpeed, 2) * 38f * CalculateLiftCoefficient(angleOfAttack);
        rb.AddForce(transform.up * lift);
    }

    void CalculateAoA()
    {
        angleOfAttack = Mathf.Atan2(transform.forward.y, transform.forward.z);
    }

    float CalculateLiftCoefficient(float aoaRad)
    {
        float aoa = aoaRad * Mathf.Rad2Deg;
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

    /* Vector3 dir = target.position - player.position;
     * float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
     * this.transform.localEulerAngles = new Vector3(0, 0, angle);
    */
}