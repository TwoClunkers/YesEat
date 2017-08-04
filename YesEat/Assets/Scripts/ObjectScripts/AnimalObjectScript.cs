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
    private GameObject[] farObjects;
    private LocationSubject destination;
    private NpcCharacter npcCharacter;
    #endregion

    public int AnimalSubjectID = -1;

    // Use this for initialization
    void Start()
    {
        if (AnimalSubjectID == -1) throw new System.Exception(gameObject.name + ": No AnimalSubjectID is assigned.");
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
        if (AiCoreTickTime > 0.5f)
        {
            npcCharacter.AiCoreProcess();
            AiCoreTickTime -= 0.5f;
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
        List<GameObject> farList = new List<GameObject>();

        // only locations
        int layerMask = (1 << 9);

        //first we can add the contents of far
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.SightRangeFar, layerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            farList.Add(hitColliders[i].gameObject);

        }

        // filter out terrain and locations
        layerMask = ~((1 << 8) | (1 << 9));
        //now let's grab collides within the near area and remove them from the "far" list
        hitColliders = Physics.OverlapSphere(gameObject.transform.position, npcCharacter.SightRangeNear, layerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            nearList.Add(hitColliders[i].gameObject);
            farList.Remove(hitColliders[i].gameObject);
        }

        // remove this object itself.
        nearList.Remove(this.gameObject);
        farList.Remove(this.gameObject);

        //store the observations locally
        nearObjects = nearList.ToArray();
        farObjects = farList.ToArray();

        // return a list of observed objects
        nearList.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));
        return nearList;
    }

    internal List<LocationSubject> GetFarObjects()
    {
        List<GameObject> farObjectList = farObjects.ToList();
        if (farObjectList.Count() > 0)
            return farObjectList.Select(o => o.GetComponent<SubjectObjectScript>().Location).ToList();
        else
            return new List<LocationSubject>();
    }
}
