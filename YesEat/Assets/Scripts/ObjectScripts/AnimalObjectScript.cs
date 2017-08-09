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
    private float AiCoreTickCounter;
    private float AiTickRate;
    private float MetabolizeTickCounter;
    private GameObject[] seenLocationObjects;
    private LocationSubject destination;
    private NpcCore npcCharacter;
    private bool isCarcass;
    private float decaytime;
    private Inventory inventory;
    private SubjectObjectScript chaseTarget;
    #endregion

    public NpcCore T_Npc { get { return npcCharacter; } set { npcCharacter = value; } }

    internal Inventory Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }

    // Use this for initialization
    void Start()
    {

    }

    public override void InitializeFromSubject(MasterSubjectList _masterSubjectList, Subject newSubject)
    {
        AiTickRate = 1.0f;
        isCarcass = false;
        subject = newSubject as AnimalSubject;
        masterSubjectList = _masterSubjectList;
        npcCharacter = new NpcCore(this, masterSubjectList, subject);
        Inventory = new Inventory((subject as AnimalSubject).InventorySize, masterSubjectList);
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
    /// Begin path finding to a new location.
    /// </summary>
    /// <param name="newLocation">The location to move to.</param>
    internal void MoveToNewLocation(LocationSubject newLocation)
    {
        if (destination != null)
        {
            if (newLocation == null) Debug.Log("null location");
            // if we are not already at the location set it as the destination
            else if (destination.SubjectID != newLocation.SubjectID)
            {
                // remove chase target if we're moving to a location
                chaseTarget = null;
                destination = newLocation;
            }
        }
        else
        {
            chaseTarget = null;
            destination = newLocation;
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
                float distance = Vector3.Distance(destination.Coordinates, transform.position);
                if (distance > (npcCharacter.SightRangeNear)) MoveTowardsPoint(destination.Coordinates, npcCharacter.MoveSpeed);
                else if (distance > 0.5) MoveTowardsPoint(destination.Coordinates, npcCharacter.MoveSpeed / 2);
                else destination = null;
            }
            else if (chaseTarget != null) // chase the target
            {
                float distance = Vector3.Distance(chaseTarget.transform.position, transform.position);
                if (distance > 1.0) MoveTowardsPoint(chaseTarget.transform.position, npcCharacter.MoveSpeed);
                else chaseTarget = null;
            }
        }
        else // this animal is dead
        {
            if (!isCarcass) //newly dead
            {
                Inventory.Add(new InventoryItem(5, 1));
                isCarcass = true;
                decaytime = 20.0f;
            }
            decaytime -= Time.deltaTime;
            UpdateDeadnessColor();
            if (decaytime < 0) Destroy(this.gameObject);
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
        if (isCarcass)
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
        //first, lets turn in the direction we need to go
        float step = moveSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
        //then we can move forward
        transform.position += (transform.forward * step);
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

        // if our position is within 1 unit of a location center add the location to our memory.
        foreach (GameObject locationObject in seenLocationObjects)
        {
            if (Vector3.Distance(locationObject.transform.position, transform.position) <= 1)
            {
                npcCharacter.Inspect(locationObject);
            }
        }

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
