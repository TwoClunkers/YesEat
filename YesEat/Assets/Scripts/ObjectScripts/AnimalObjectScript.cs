﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// The script attached to the GameObject for controlling movement, collecting world data, and interacting with other game objects.
/// </summary>
public class AnimalObjectScript : SubjectObjectScript
{
    #region Private members
    private float AiCoreTickCounter;
    private float AiTickRate;
    private float MetabolizeTickCounter;
    private GameObject[] seenLocationObjects;
    private LocationSubject destination;
    private Vector3[] destinationWayPoints;
    private int currentWaypointIndex;
    private NpcCore npcCharacter;
    private bool isDead;
    private float decaytime;
    private Inventory inventory;
    private SubjectObjectScript chaseTarget;
    private bool isCurrentLocationExplored;
    #endregion

    public NpcCore T_Npc { get { return npcCharacter; } set { npcCharacter = value; } }

    internal Inventory Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }

    public bool IsDead { get { return isDead; } }

    public bool IsCurrentLocationExplored { get { return isCurrentLocationExplored; } }

    // Don't Use this for Initialization
    void Start()
    {

    }

    public override void InitializeFromSubject(MasterSubjectList _masterSubjectList, Subject newSubject)
    {
        AiTickRate = 0.5f;
        isDead = false;
        subject = newSubject as AnimalSubject;
        masterSubjectList = _masterSubjectList;
        npcCharacter = new NpcCore(this, masterSubjectList, subject);
        Inventory = new Inventory((subject as AnimalSubject).InventorySize, masterSubjectList);
        destinationWayPoints = new Vector3[0];
        isCurrentLocationExplored = false;
    }

    public int GetHealth()
    {
        return npcCharacter.Health;
    }
    public int GetSafety()
    {
        return npcCharacter.Safety;
    }
    public int GetFood()
    {
        return npcCharacter.Food;
    }

    public LocationSubject CurrentDestination { get { return destination; } }

    /// <summary>
    /// Begin path finding to a new location.
    /// </summary>
    /// <param name="newLocation">The location to move to.</param>
    internal void MoveToNewLocation(LocationSubject newLocation)
    {
        if (newLocation == null)
        {
            Debug.Log("AnimalObjectScript::MoveToNewLocation() \n    Error: null location");
            return;
        }

        if (destination != null)
        {
            if (destination.SubjectID == newLocation.SubjectID) return;
        }
        // remove chase target if we're moving to a location
        chaseTarget = null;
        destination = newLocation;
        isCurrentLocationExplored = false;
        // queue up the waypoints for the new location

        // flip a coin on which method of area waypoints to use
        if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
        {
            destinationWayPoints = newLocation.GetAreaWaypoints(npcCharacter.SightRangeNear);
            if (destinationWayPoints.Length > 1)
            {
                destinationWayPoints = ShiftToNearestFirst(destinationWayPoints);
            }
        }
        else
        {
            destinationWayPoints = newLocation.GetAreaWaypoints(npcCharacter.SightRangeNear, 1);
            if (destinationWayPoints.Length > 1)
            {
                destinationWayPoints = destinationWayPoints
                    .OrderBy(o => Vector3.Distance(transform.position, o))
                    .ToArray();
            }
        }
        // debugging- show generated waypoints in editor interface
        for (int i = 0; i < destinationWayPoints.Length; i++)
        {
            Color waypointColor = Color.red;
            if (subject.SubjectID == DbIds.Plinkett) { waypointColor = Color.blue; }
            if (subject.SubjectID == DbIds.Gobber) { waypointColor = Color.yellow; }
            Vector3 wayPtTop =
                new Vector3(destinationWayPoints[i].x,
                0.25f + ((float)i / destinationWayPoints.Length) * 0.5f,
                destinationWayPoints[i].z);
            Debug.DrawLine(destinationWayPoints[i], wayPtTop, waypointColor, 10.0f);
        }
        currentWaypointIndex = 0;
    }

    /// <summary>
    /// Shift the array items so the nearest point is at index 0.
    /// </summary>
    /// <param name="wayPointsToShift">The array to shift.</param>
    /// <returns>The shifted array.</returns>
    private Vector3[] ShiftToNearestFirst(Vector3[] wayPointsToShift)
    {
        if (wayPointsToShift.Length > 1)
        {
            int nearestIndex = 0;
            // check opposite ends of the waypoint ring and start with the closer index
            if (Vector3.Distance(wayPointsToShift[wayPointsToShift.Length / 2], transform.position) <
                Vector3.Distance(wayPointsToShift[0], transform.position))
            {
                nearestIndex = (wayPointsToShift.Length / 2);
            }
            // iterate waypoints and find the nearest
            nearestIndex = GetNearestPoint(transform.position, wayPointsToShift, nearestIndex);
            if (nearestIndex != 0)
            {
                // shift array elements so the nearest is the first.
                Vector3[] newWayPoints = new Vector3[wayPointsToShift.Length];

                Array.Copy(wayPointsToShift, nearestIndex, newWayPoints, 0, wayPointsToShift.Length - nearestIndex);
                Array.Copy(wayPointsToShift, 0, newWayPoints, wayPointsToShift.Length - nearestIndex, nearestIndex);
                return newWayPoints;
            }
            else
            {
                return wayPointsToShift;
            }
        }
        else
        {
            return wayPointsToShift;
        }
    }

    /// <summary>
    /// Recursive method that finds the point nearest to the destinationPoint via Hill Climb searching.
    /// </summary>
    /// <param name="destinationPoint">The point that the wayPoints[] will be compared to.</param>
    /// <param name="wayPoints">The array of ordered points.</param>
    /// <param name="i">The starting index of the search.</param>
    /// <returns>The index of the nearest point.</returns>
    private int GetNearestPoint(Vector3 destinationPoint, Vector3[] wayPoints, int i)
    {
        if (i < 0) i = wayPoints.Length - 1;
        if (i > wayPoints.Length - 1) i = 0;
        int iUp = i + 1;
        int iDown = i - 1;
        if (iDown < 0) iDown = wayPoints.Length - 1;
        if (iUp > wayPoints.Length - 1) iUp = 0;

        float iUpDist = Vector3.Distance(destinationPoint, wayPoints[iUp]);
        float iHere = Vector3.Distance(destinationPoint, wayPoints[i]);
        float iDownDist = Vector3.Distance(destinationPoint, wayPoints[iDown]);

        if (iHere < iUpDist & iHere < iDownDist) return i;
        else
        {
            if (iUpDist < iDownDist)
            { return GetNearestPoint(destinationPoint, wayPoints, iUp); }
            else
            { return GetNearestPoint(destinationPoint, wayPoints, iDown); }
        }
    }

    /// <summary>
    /// Chase the target until ChaseStop() is called.
    /// </summary>
    /// <param name="target">The object scripte of the game object to chase.</param>
    internal void ChaseStart(SubjectObjectScript target)
    {
        // remove destination while chasing a target
        destination = null;
        if (chaseTarget != null)
        {
            if (chaseTarget.GetInstanceID() != target.GetInstanceID())
                chaseTarget = target;
        }
        else chaseTarget = target;
    }

    /// <summary>
    /// Stop chasing a target.
    /// </summary>
    internal void ChaseStop()
    {
        chaseTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!npcCharacter.IsDead)
        {
            MetabolizeTickCounter += Time.deltaTime;
            if (MetabolizeTickCounter >= npcCharacter.Definition.MetabolizeInterval)
            {
                int healthChange = npcCharacter.Metabolize();
                MetabolizeTickCounter -= npcCharacter.Definition.MetabolizeInterval;
                if (healthChange > 0) GameObject.FindGameObjectWithTag("GameController").GetComponent<PlacementControllerScript>().PopMessage(healthChange.ToString(), gameObject.transform.position, 2);
                else if (healthChange < 0) GameObject.FindGameObjectWithTag("GameController").GetComponent<PlacementControllerScript>().PopMessage(healthChange.ToString(), gameObject.transform.position, 0);
            }

            AiCoreTickCounter += Time.deltaTime;
            if (AiCoreTickCounter > AiTickRate)
            {
                npcCharacter.AiCoreProcess();
                AiCoreTickCounter -= AiTickRate;
            }

            // ===  Movement ===
            if (destination != null) // traveling to a new location
            {
                if (destinationWayPoints.Length != 0)
                {
                    float distance = Vector3.Distance(destinationWayPoints[currentWaypointIndex], transform.position);
                    if (distance > (npcCharacter.SightRangeNear)) MoveTowardsPoint(destinationWayPoints[currentWaypointIndex], npcCharacter.MoveSpeed);
                    else if (distance > 0.25) MoveTowardsPoint(destinationWayPoints[currentWaypointIndex], npcCharacter.MoveSpeed * 0.85f);
                    else
                    {
                        if (currentWaypointIndex == destinationWayPoints.GetUpperBound(0))
                        {
                            // we have arrived at the final waypoint for this location
                            isCurrentLocationExplored = true;
                            npcCharacter.AddSearchedLocation(destination.SubjectID);
                            // remember the location now that it is explored fully
                            UnityEngine.Object[] scripts = FindObjectsOfType(typeof(LocationObjectScript));
                            LocationObjectScript inspectLocationScript = scripts.Single(o =>
                                (o as LocationObjectScript).Subject.SubjectID == destination.SubjectID)
                                as LocationObjectScript;
                            inspectLocationScript.TeachNpc(npcCharacter);

                            npcCharacter.Inspect(inspectLocationScript.gameObject);

                            destinationWayPoints = new Vector3[0];
                            destination = null;
                        }
                        else currentWaypointIndex++;
                    }
                }
            }
            else if (chaseTarget != null) // chase the target
            {
                float distance = Vector3.Distance(chaseTarget.transform.position, transform.position);
                if (distance > 0.75) MoveTowardsPoint(chaseTarget.transform.position, npcCharacter.MoveSpeed);
                else
                {
                    Vector3 targetDir = chaseTarget.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(targetDir);
                    chaseTarget = null;
                }
            }

        }
        else // this animal is dead
        {
            if (!isDead) //newly dead
            {
                Inventory.Add(new InventoryItem(DbIds.Meat, 5));
                isDead = true;
                decaytime = 20.0f;
            }
            decaytime -= Time.deltaTime;
            UpdateDeadnessColor();
            if (decaytime < 0) Destroy(this.gameObject);
        }

        // Debug: near vision range
        DrawDebugCircle(transform.position, npcCharacter.SightRangeNear, 20, new Color(0, 0, 1, 0.5f));
        // Debug: far vision range
        DrawDebugCircle(transform.position, npcCharacter.SightRangeFar, 20, new Color(0, 0, 0, 0.3f));
    }

    private void DrawDebugCircle(Vector3 debugCircleCenter, float debugCircleRadius, int debugCircleVertices, Color debugCircleColor)
    {
        Vector3[] visionCircle = new Vector3[debugCircleVertices];
        float degree = (float)360 / debugCircleVertices;
        for (int i = 0; i < debugCircleVertices; i++)
        {
            Vector3 newPoint = new Vector3(
                            (Mathf.Cos((degree * i) * Mathf.Deg2Rad) * debugCircleRadius) + debugCircleCenter.x, 0,
                            (Mathf.Sin((degree * i) * Mathf.Deg2Rad) * debugCircleRadius) + debugCircleCenter.z);
            visionCircle[i] = newPoint;
        }
        Vector3 lastPt = visionCircle[visionCircle.GetUpperBound(0)];
        foreach (Vector3 pt in visionCircle)
        {
            Debug.DrawLine(lastPt, pt, debugCircleColor);
            lastPt = pt;
        }
    }

    /// <summary>
    /// Attempt to harvest an item from this animal. <para/>
    /// Returns: One of this animal's loot if it is dead and has loot in its inventory. <para/>
    /// Returns: NULL if this animal is not dead.
    /// </summary>
    /// <returns>Returns: One of this animal's loot if it is dead and has loot in its inventory. <para/>
    /// Returns: NULL if this animal is not dead.</returns>
    public override InventoryItem Harvest()
    {
        if (IsDead)
        {
            return Inventory.Take(new InventoryItem(5, 1));
        }
        else return null;
    }

    /// <summary>
    /// Move game object towards a world position.
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="moveSpeed"></param>
    private void MoveTowardsPoint(Vector3 targetPosition, float moveSpeed)
    {
        Vector3 targetDir = targetPosition - transform.position;
        float step = (moveSpeed * Time.deltaTime);
        Vector3 newDir;
        if (Vector3.Distance(transform.position, targetPosition) > 1.0)
        { newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F); }
        else
        { newDir = targetDir; }
        // turn in the direction we need to go
        transform.rotation = Quaternion.LookRotation(newDir);
        //then we can move forward
        transform.position += (transform.forward * step);

        // Debug: draw line to current targetPosition
        if (destinationWayPoints.Length > 0)
        {
            Debug.DrawLine(new Vector3(transform.position.x, 0.5f, transform.position.z), targetPosition, (subject.SubjectID == DbIds.Plinkett) ? Color.blue : Color.yellow);
        }
    }

    /// <summary>
    /// Uses Physics.OverlapSphere() to collect and return a list of all within npcCharacter.SightRangeNear, the list 
    /// excludes these game world layers: Terrain(8), Locations(9).
    /// Also populates a list of Locations within npcCharacter.SightRangeFar which can be retrieved using GetObservedLocations(). 
    /// </summary>
    /// <returns></returns>
    public List<GameObject> Observe()
    {
        List<GameObject> nearList = new List<GameObject>();
        List<GameObject> seenLocationList = new List<GameObject>();

        // only locations
        int layerMask = (1 << 9);
        // scan far sight area for locations to explore later
        seenLocationList = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.SightRangeFar, layerMask).Select(o => o.gameObject).ToList();

        // filter out terrain and locations
        layerMask = ~((1 << 8) | (1 << 9));
        // now let's grab collides in the near area
        nearList = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.SightRangeNear, layerMask).Select(o => o.gameObject).ToList();

        // remove this object from the lists
        nearList.Remove(this.gameObject);
        seenLocationList.Remove(this.gameObject);

        // store the observations locally
        seenLocationObjects = seenLocationList.ToArray();

        // return a list of observed objects
        nearList.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));
        return nearList;
    }

    /// <summary>
    /// Return list of all locations observed within npcCharacter.SightRangeFar.
    /// </summary>
    /// <returns></returns>
    internal List<LocationSubject> GetObservedLocations()
    {
        List<GameObject> farObjectList = seenLocationObjects.ToList();
        if (farObjectList.Count() > 0)
        {
            List<LocationSubject> observedLocations = farObjectList.Select(o => o.GetComponent<SubjectObjectScript>().Subject as LocationSubject).ToList();

            Debug.DrawLine(transform.position, new Vector3(transform.position.x, 2.0f, transform.position.z), Color.white, 0.1f);

            return observedLocations;
        }
        else
        {
            return new List<LocationSubject>();
        }
    }

    void UpdateDeadnessColor()
    {
        transform.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.1F, 0.1F, 0.1F, 0.3F), new Color(0.5F, 0.3F, 0.3F, 0.8F), decaytime / 20.0f);
    }

    /// <summary>
    /// Pass-through to damage npc
    /// </summary>
    /// <param name="subjectAttacker"></param>
    /// <param name="damageAmount"></param>
    /// <param name="NpcAttacker"></param>
    /// <returns></returns>
    public bool Damage(Subject subjectAttacker, int damageAmount, NpcCore NpcAttacker = null)
    {
        if (npcCharacter.IsDead) return false; //already dead, so cannot damage
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PlacementControllerScript>().PopMessage((-damageAmount).ToString(), gameObject.transform.position, 0);
        if (npcCharacter.Damage(subjectAttacker, damageAmount, NpcAttacker))
        {
            return true; //it was killed
        }
        else return false;
    }
}
