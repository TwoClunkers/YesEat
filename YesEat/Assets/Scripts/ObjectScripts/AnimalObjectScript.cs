using UnityEngine;
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
    private GameObject[] seenLocationObjects;
    private LocationSubject destination;
    private Vector3[] destinationWayPoints;
    private int currentWaypointIndex;
    private NpcCore npcCharacter;
    private float decaytime;
    private Inventory inventory;
    private SubjectObjectScript chaseTarget;
    private Vector3 targetPosition;
    private bool isCurrentLocationExplored;
    private SubjectObjectScript[] avoidObjects;
    #endregion

    public NpcCore T_Npc { get { return npcCharacter; } set { npcCharacter = value; } }

    internal Inventory Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }

    public bool IsDead { get { return npcCharacter.IsDead; } }

    public bool IsCurrentLocationExplored { get { return isCurrentLocationExplored; } }

    // Don't Use this for Initialization
    void Start()
    {

    }

    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject as AnimalSubject;
        npcCharacter = new NpcCore(this, subject);
        Inventory = new Inventory((subject as AnimalSubject).InventorySize);
        destinationWayPoints = new Vector3[0];
        isCurrentLocationExplored = false;
        decaytime = 20.0f;
        targetPosition = default(Vector3);
        avoidObjects = new SubjectObjectScript[0];

        subject.TeachNpc(npcCharacter);
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

    /// <summary>
    /// Move to target coordinates.
    /// </summary>
    /// <param name="newPosition"></param>
    internal void MoveTo(Vector3 newPosition)
    {
        if (targetPosition == newPosition) return;
        chaseTarget = null;
        destination = null;
        targetPosition = newPosition;
    }

    /// <summary>
    /// Begin path finding to a new location.
    /// </summary>
    /// <param name="newLocation">The location to move to.</param>
    internal void MoveTo(LocationSubject newLocation)
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
            destinationWayPoints = newLocation.GetAreaWaypoints(npcCharacter.RangeSightMid);
            if (destinationWayPoints.Length > 1)
            {
                destinationWayPoints = ShiftToNearestFirst(destinationWayPoints);
            }
        }
        else
        {
            destinationWayPoints = newLocation.GetAreaWaypoints(npcCharacter.RangeSightMid, 1);
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
            if (subject.SubjectID == KbIds.Plinkett) { waypointColor = Color.blue; }
            if (subject.SubjectID == KbIds.Gobber) { waypointColor = Color.yellow; }
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

    public SubjectObjectScript GetChaseTarget()
    {
        return chaseTarget;
    }

    /// <summary>
    /// Chase the target until ChaseStop() is called.
    /// </summary>
    /// <param name="target">The object script of the game object to chase.</param>
    internal void MoveTo(SubjectObjectScript target)
    {
        if (chaseTarget != null)
        {
            // remove destination while chasing a target
            destination = null;
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
        npcCharacter.Update();
        if (npcCharacter.IsDead)
        {
            decaytime -= Time.deltaTime;
            UpdateDeadnessColor();
            if (decaytime < 0) Destroy(this.gameObject);
        }

        // Debug: near vision range
        //DrawDebugCircle(transform.position, npcCharacter.RangeSightNear, 20, new Color(0, 0, 1, 0.5f));
        // Debug: mid vision range
        DrawDebugCircle(transform.position, npcCharacter.RangeSightMid, 20, new Color(0, 1, 0, 0.5f));
        // Debug: far vision range
        DrawDebugCircle(transform.position, npcCharacter.RangeSightFar, 20, new Color(0, 0, 0, 0.3f));
    }

    /// <summary>
    /// Add dropped item to mob inventory.
    /// </summary>
    public void SetDeathDecay(float duration)
    {
        Inventory.Add(new InventoryItem(npcCharacter.Subject.LootID, 5));
        decaytime = duration;
    }

    /// <summary>
    /// Move the GameObject based on destination or chaseTarget.
    /// </summary>
    public void DoMovement()
    {
        if (destination != null) // traveling to a new location
        {
            if (destinationWayPoints.Length != 0)
            {
                float distance = Vector3.Distance(destinationWayPoints[currentWaypointIndex], transform.position);
                if (distance > (npcCharacter.RangeSightNear)) MoveTowardsPoint(destinationWayPoints[currentWaypointIndex], npcCharacter.MoveSpeed);
                else if (distance > npcCharacter.RangeActionClose) MoveTowardsPoint(destinationWayPoints[currentWaypointIndex], npcCharacter.MoveSpeed * 0.85f);
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
            if (distance > npcCharacter.RangeActionClose * 0.9) MoveTowardsPoint(chaseTarget.transform.position, npcCharacter.MoveSpeed);
            else
            {
                Vector3 targetDir = chaseTarget.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(targetDir);
                //chaseTarget = null;
            }
        }
        else if (targetPosition != default(Vector3)) // move to targetPosition
        {
            float distance = Vector3.Distance(targetPosition, transform.position);
            if (distance > npcCharacter.RangeActionClose) MoveTowardsPoint(targetPosition, npcCharacter.MoveSpeed);
            else
            {
                Vector3 targetDir = targetPosition - transform.position;
                transform.rotation = Quaternion.LookRotation(targetDir);
                targetPosition = default(Vector3);
            }
        }
    }

    internal float GetEndurance() { return npcCharacter.Endurance; }

    /// <summary>
    /// Get the current driver for this mob.
    /// </summary>
    /// <returns>The current top driver.</returns>
    internal NpcDrivers GetDriver() { return npcCharacter.GetDriver(); }

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
    public override InventoryItem Harvest(int itemIdToHarvest)
    {
        if (IsDead)
        {
            return Inventory.Take(new InventoryItem(itemIdToHarvest, 1));
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
            Debug.DrawLine(new Vector3(transform.position.x, 0.5f, transform.position.z), targetPosition, (subject.SubjectID == KbIds.Plinkett) ? Color.blue : Color.yellow);
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
        List<GameObject> midRangeList = new List<GameObject>();
        List<GameObject> seenLocationList = new List<GameObject>();

        // only locations
        int layerMask = (1 << 9);
        // scan far sight area for locations to explore later
        seenLocationList = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.RangeSightFar, layerMask).Select(o => o.gameObject).ToList();

        // filter out terrain and locations
        layerMask = ~((1 << 8) | (1 << 9));
        // now let's grab collides in the near area
        midRangeList = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.RangeSightMid, layerMask).Select(o => o.gameObject).ToList();

        // remove this object from the lists
        midRangeList.Remove(this.gameObject);
        seenLocationList.Remove(this.gameObject);

        // store the observations locally
        seenLocationObjects = seenLocationList.ToArray();

        // return a list of observed objects
        midRangeList.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));
        return midRangeList;
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

    /// <summary>
    /// Dig a hole at the current position.
    /// </summary>
    /// <returns>A reference to the hole that was dug.</returns>
    public StructureObjectScript DigHole()
    {
        StructureSubject holeSubject = KnowledgeBase.GetSubject(KbIds.Hole10) as StructureSubject;

        // lift the hole off the playfield
        Vector3 holePosition = new Vector3(transform.position.x, 0.01f, transform.position.z);

        StructureObjectScript hole =
            Instantiate(holeSubject.Prefab, holePosition, Quaternion.identity).GetComponent<StructureObjectScript>();
        hole.InitializeFromSubject(holeSubject);

        return hole;
    }

    /// <summary>
    /// Build a nest using the items contained in holeObjectScript's Inventory.
    /// </summary>
    /// <param name="holeObjectScript">The hole to search for ingredients to build the nest.</param>
    /// <returns>A reference to the nest that was built.</returns>
    internal StructureObjectScript BuildNest(StructureObjectScript holeObjectScript)
    {
        BuildRecipe nestRecipe = npcCharacter.Subject.Nest.Recipe;
        List<InventoryItem> takenItems = new List<InventoryItem>();
        foreach (InventoryItem ingredient in nestRecipe.Ingredients)
        {
            // take out each ingredient
            InventoryItem tempItem = holeObjectScript.Inventory.Take(new InventoryItem(ingredient));
            if (tempItem.Quantity == ingredient.Quantity)
            {
                takenItems.Add(tempItem);
            }
            else
            {
                // not enough of this ingredient, put all ingredients back and cancel the nest build
                foreach (InventoryItem takenIngredient in takenItems)
                {
                    holeObjectScript.Inventory.Add(takenIngredient);
                }
                return null;
            }
        }
        // we have all needed ingredient quantities, build nest
        StructureObjectScript nestObjectScript =
            Instantiate(npcCharacter.Subject.Nest.Prefab, holeObjectScript.transform.position, holeObjectScript.transform.rotation)
            .GetComponent<StructureObjectScript>();

        nestObjectScript.InitializeFromSubject(npcCharacter.Subject.Nest.Copy());

        takenItems.Clear();
        takenItems = holeObjectScript.Inventory.TakeAllItems();
        if (takenItems.Count > 0)
        {
            nestObjectScript.Inventory.Add(takenItems.ToArray());
        }

        // destroy the hole
        Destroy(holeObjectScript.gameObject);

        return nestObjectScript;
    }

    /// <summary>
    /// Add an object to avoid while moving.
    /// </summary>
    /// <param name="objectToAvoid">The object to avoid</param>
    internal void Avoid(SubjectObjectScript objectToAvoid)
    {
        // save this object as something to avoid
        if (!avoidObjects.Contains(objectToAvoid))
        {
            SubjectObjectScript[] newAvoidList = new SubjectObjectScript[avoidObjects.Length + 1];
            Array.Copy(avoidObjects, newAvoidList, avoidObjects.Length);
            avoidObjects = newAvoidList;
            avoidObjects[avoidObjects.GetUpperBound(0)] = objectToAvoid;
        }
    }
}
