using System;
using System.Collections.Generic;
using UnityEngine;

public partial class NpcCore
{
    private void AiCoreSubprocessExplore()
    {
        // explore unknown locations
        if (unexploredLocations.Count > 0)
        {
            objectScript.MoveToNewLocation(unexploredLocations[0]);
        }
        else
        {
            // explore known locations for new stuff
            // populate locations to re-explore
            if (reExploreLocations.Count == 0)
            {
                reExploreLocations = GetAllKnownLocations(objectScript.transform.position);
            }
            objectScript.MoveToNewLocation(reExploreLocations[0]);
        }
    }

    public Vector3[] GetAreaWaypoints(Vector3 center, float searchRadius, float sightRadius)
    {
        List<Vector3> waypoints = new List<Vector3>();

        int step = 1;
        float sightDiameter = sightRadius * 2;
        float stepFactor = Mathf.PI * ((step * 2) - 1);
        //step circumferance = PI*sightRadius*(step*4-2)
        //wp count = step circumferance / sightRadius*2
        //so: wp count = stepFactor
        while(searchRadius > (stepFactor*sightDiameter+sightDiameter))
        {
            for (int i = 0; i < Mathf.Ceil(stepFactor); i++)
            {
                Vector2 tempPoint = new Vector2();
                tempPoint = DegreeToVector2((360 / stepFactor) * i);
                Vector3 newPoint = new Vector3(tempPoint.x * sightDiameter, 0, tempPoint.y * sightDiameter);
                waypoints.Add(newPoint);
            }

            step += 1;
        }

        return waypoints.ToArray();
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}