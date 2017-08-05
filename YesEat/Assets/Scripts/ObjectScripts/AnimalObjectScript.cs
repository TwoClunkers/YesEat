using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AnimalObjectScript : SubjectObjectScript
{
    #region Private members
    private float AiCoreTickTime;
    private GameObject[] nearObjects;
    private GameObject[] seenLocationObjects;
    private LocationSubject destination;
    private NpcCharacter npcCharacter;
    #endregion

    public int AnimalSubjectID = -1;

    // Use this for initialization
    void Start()
    {
        if (AnimalSubjectID == -1) throw new System.Exception(gameObject.name + ": No AnimalSubjectID is assigned for the Prefab.");
        AnimalSubject animalSubject = masterSubjectList.GetSubject(AnimalSubjectID) as AnimalSubject;
        npcCharacter = new NpcCharacter(this, ref masterSubjectList, animalSubject);
    }

    /// <summary>
    /// Begin path finding to a new location.
    /// </summary>
    /// <param name="newLocation">The location to move to.</param>
    internal void MoveToNewLocation(LocationSubject newLocation)
    {
        if (destination != null)
        {
            if (destination.SubjectID != newLocation.SubjectID)
                destination = newLocation;
        }
        else destination = newLocation;
    }

    // Update is called once per frame
    void Update()
    {
        AiCoreTickTime += Time.deltaTime;
        if (AiCoreTickTime > 1.0f)
        {
            npcCharacter.AiCoreProcess();
            AiCoreTickTime -= 1.0f;
        }

        if (destination != null)
        {
            float distance = Vector3.Distance(destination.Coordinates, transform.position);
            if (distance > npcCharacter.SightRangeFar) MoveTowardsPoint(destination.Coordinates, npcCharacter.MoveSpeed);
            else if (distance > 1) MoveTowardsPoint(destination.Coordinates, npcCharacter.MoveSpeed / 3);
            else destination = null;
        }
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
        nearObjects = nearList.ToArray();
        seenLocationObjects = seenLocationList.ToArray();

        //TODO: if our position is within 1 unit of a location center add the location to our memory.
        foreach (GameObject locationObject in seenLocationObjects)
        {
            if(Vector3.Distance(locationObject.transform.position, transform.position) < 1)
            {
                npcCharacter.Inspect(locationObject);
            }
        }

        // return a list of observed objects
        nearList.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));
        return nearList;
    }

    internal List<LocationSubject> GetFarObjects()
    {
        List<GameObject> farObjectList = seenLocationObjects.ToList();
        if (farObjectList.Count() > 0)
            return farObjectList.Select(o => o.GetComponent<SubjectObjectScript>().Location).ToList();
        else
            return new List<LocationSubject>();
    }
}
