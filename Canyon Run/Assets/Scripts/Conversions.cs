using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Conversions
{
    // Converts from meters to feet
    public static double Altitude(float altitudeInMeters)
    {
        return Math.Round(altitudeInMeters * 3.28084f);
    }

    // Converts from meters per second to knots
    public static double Speed(float speedInMetersPerSecond)
    {
        return Math.Round(speedInMetersPerSecond * 1.94384f);
    }

    // Converts from meters per second to mach
    public static double Mach(float speedInMetersPerSecond)
    {
        return Math.Round(speedInMetersPerSecond / 343, 2);
    }

    // Converts from meters per second to feet per second
    public static double MPStoFPS(float speedInMetersPerSecond)
    {
        return Math.Round(speedInMetersPerSecond * 3.28084f);
    }

    // Converts from meters to nautical miles
    public static double MeterstoNauticalMiles(double distanceInMeters)
    {
        return Math.Round(distanceInMeters / 1852, 1);
    }
    // NOTE: RULE OF THUMB IS TO ADD 2% TO THE CAS FOR EVERY 1000 FT OF ALTITUDE

/* void planeLift()
 * {
    speed = rb.velocity.z;
    Vector3 directionVector = Vector3.Normalize(rb.velocity) * 360;
    Vector3 transformVector = transform.rotation.eulerAngles;
    Vector3 angleVector = progradeVector - transformVector;
    angleOfAttack = directionVector.x - transformVector.z;
    lift = angleOfAttack * 1.225f * (Mathf.Pow(speed, 2) / 2) * wingSurfaceArea;
    rb.AddForce(transform.up * lift);
    } */
}