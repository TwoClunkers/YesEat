
using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Location Subject extends the Subject Class to describe locations for AI
/// </summary>
public class LocationSubject : Subject
{
    #region Private members
    private Vector3 coordinates;
    private float radius;
    private int layer;
    #endregion

    /// <summary>
    /// Create new LocationSubject object.
    /// </summary>
    public LocationSubject() : base()
    {
        name = "Location";
        description = "This is the Location Description";
        icon = new Sprite();

        coordinates = new Vector3(1, 1, 1);
        radius = 1.0f;
        layer = 1;
    }

    /// <summary>
    /// Create new LocationSubject object based on an existing one.
    /// </summary>
    public LocationSubject(LocationSubject newLocation) : base()
    {
        name = newLocation.Name;
        description = newLocation.Description;
        icon = newLocation.Icon;
        coordinates = newLocation.Coordinates;
        radius = newLocation.Radius;
        prefab = newLocation.Prefab;
        layer = newLocation.Layer;
        relatedSubjects = newLocation.RelatedSubjects;
    }

    /// <summary>
    /// Copy an existing LocationSubject.
    /// </summary>
    public override Subject Copy()
    {
        LocationSubject newLocationSubject = new LocationSubject();
        newLocationSubject.subjectID = subjectID;
        newLocationSubject.name = name;
        newLocationSubject.description = description;
        newLocationSubject.icon = icon;
        newLocationSubject.prefab = prefab;
        newLocationSubject.relatedSubjects = relatedSubjects;

        newLocationSubject.coordinates = coordinates;
        newLocationSubject.radius = radius;
        newLocationSubject.layer = layer;
        return newLocationSubject;
    }

    /// <summary>
    /// The center of the location.
    /// </summary>
    public Vector3 Coordinates
    {
        get { return new Vector3(coordinates.x, coordinates.y, coordinates.z); }
        set { coordinates = value; }
    }

    /// <summary>
    /// The radius of the location. (around the y axis)
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    /// <summary>
    /// The layer should allow for smaller divisions to overlap larger defined areas
    /// </summary>
    public int Layer
    {
        get { return layer; }
        set { layer = value; }
    }

    /// <summary>
    /// Add this subject to the npcCharacter's memories.
    /// </summary>
    /// <param name="npcCharacter">The NPC to add memories to.</param>
    public override void TeachNpc(NpcCore npcCharacter)
    {
        if (!npcCharacter.Definition.Memories.Exists(o => o.SubjectID == this.subjectID))
        {
            npcCharacter.Definition.Memories.Add(new LocationMemory(subjectID, 0, 0));
        }
    }

    /// <summary>
    /// Generate waypoints around a location that will allow a mob with sightRadius 
    /// to see the entire area if each waypoint is travelled to.
    /// </summary>
    /// <param name="sightRadius">The sight radius of the mob that will explore the area.</param>
    /// <returns>An array of waypoints.</returns>
    public Vector3[] GetAreaWaypoints(float sightRadius, int generatorType = 0)
    {
        List<Vector3> waypoints = new List<Vector3>();
        float lappedSightRadius;
        float sightDiameter;
        float loopRadius;
        float currentLoopStepDegrees;
        int maxLoops;
        int loopIndex;
        int maxPointsThisLoop;
        float randomAngle = UnityEngine.Random.Range(0.1f, 89.1f);
        switch (generatorType)
        {
            default:
                lappedSightRadius = (sightRadius * 0.85f);
                if (lappedSightRadius < radius)
                {
                    sightDiameter = lappedSightRadius * 2;
                    maxLoops = (int)Mathf.Ceil(radius / sightDiameter);
                    loopIndex = maxLoops;
                    while (loopIndex > 0)
                    {
                        loopRadius = radius - ((float)loopIndex * sightDiameter) + lappedSightRadius;
                        if (loopRadius < lappedSightRadius)
                        {
                            if ((radius - ((loopIndex - 1) * sightDiameter)) > lappedSightRadius)
                            {
                                currentLoopStepDegrees = 120.0f;
                                loopRadius = lappedSightRadius;
                            }
                            else
                            {
                                currentLoopStepDegrees = 360.0f;
                                loopRadius = 0;
                            }
                        }
                        else
                        {
                            currentLoopStepDegrees = (Mathf.Asin(lappedSightRadius / loopRadius) * Mathf.Rad2Deg) * 2;
                        }
                        maxPointsThisLoop = (int)Mathf.Ceil(360 / currentLoopStepDegrees);
                        currentLoopStepDegrees = 360 / (float)maxPointsThisLoop;
                        for (int i = 0; i < maxPointsThisLoop; i++)
                        {
                            float degree = (currentLoopStepDegrees * i) + randomAngle;
                            Vector3 newPoint = new Vector3(
                                Mathf.Cos(degree * Mathf.Deg2Rad) * loopRadius, 0,
                                Mathf.Sin(degree * Mathf.Deg2Rad) * loopRadius);
                            waypoints.Add(newPoint + coordinates);

                        }
                        loopIndex -= 1;
                    }
                }
                break;
            case 1:
                lappedSightRadius = sightRadius;
                if (lappedSightRadius < radius)
                {
                    sightDiameter = lappedSightRadius * 2;
                    loopRadius = radius;
                    if (loopRadius < lappedSightRadius)
                    {
                        if (radius > lappedSightRadius)
                        {
                            currentLoopStepDegrees = 120.0f;
                            loopRadius = lappedSightRadius;
                        }
                        else
                        {
                            currentLoopStepDegrees = 360.0f;
                            loopRadius = 0;
                        }
                    }
                    else
                    {
                        currentLoopStepDegrees = (Mathf.Asin(lappedSightRadius / loopRadius) * Mathf.Rad2Deg) * 2;
                    }
                    maxPointsThisLoop = (int)Mathf.Ceil(360 / currentLoopStepDegrees);
                    currentLoopStepDegrees = 360 / (float)maxPointsThisLoop;
                    for (int i = 0; i < maxPointsThisLoop; i++)
                    {
                        float degree = (currentLoopStepDegrees * i) + randomAngle;
                        Vector3 newPoint = new Vector3(
                            Mathf.Cos(degree * Mathf.Deg2Rad) * loopRadius, 0,
                            Mathf.Sin(degree * Mathf.Deg2Rad) * loopRadius);
                        waypoints.Add(newPoint + coordinates);

                    }
                }
                break;
        }
        // always return at least the center point of the location
        if (waypoints.Count < 1)
        {
            waypoints.Add(coordinates);
        }

        return waypoints.ToArray();

    }
}


