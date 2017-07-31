using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AnimalObjectScript : SubjectObjectScript
{
    #region Private members
    private int sightNear;
    private int sightFar;
    private int count;
    private float speed;
    private GameObject[] nearObjects;
    private GameObject[] farObjects;
    private LocationSubject destination;

    #endregion

    // Use this for initialization
    void Start()
    {
        sightFar = 5;
        sightNear = 2;
        speed = 4;
    }

    internal void MoveToNewLocation(LocationSubject newLocation)
    {
        if (destination.SubjectID != newLocation.SubjectID) destination = newLocation;
    }

    // Update is called once per frame
    void Update()
    {
        if (destination != null)
        {
            float distance = Vector3.Distance(destination.Coordinates, transform.position);
            if (distance > sightFar) MoveTowardsPoint(destination.Coordinates, speed);
            else if (distance > 1) MoveTowardsPoint(destination.Coordinates, speed / 3);
            else destination = null;
        }
    }

    void MoveTowardsPoint(Vector3 targetPosition, float _speed)
    {
        Vector3 targetDir = targetPosition - transform.position;
        //first, lets turn in the direction we need to go
        float step = _speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        transform.rotation = Quaternion.LookRotation(newDir);
        //then we can move forward
        transform.position += (transform.forward * step);
    }

    public List<GameObject> Observe()
    {
        List<GameObject> nearList = new List<GameObject>();
        List<GameObject> farList = new List<GameObject>();

        //first we can add the contents of far
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, sightFar);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            farList.Add(hitColliders[i].gameObject);

        }
        //now let's grab collides within the near area and remove them from the "far" list
        hitColliders = Physics.OverlapSphere(gameObject.transform.position, sightNear);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            nearList.Add(hitColliders[i].gameObject);
            farList.Remove(hitColliders[i].gameObject);
        }

        //store the observations locally
        nearObjects = nearList.ToArray();
        farObjects = farList.ToArray();

        // return a list of observed objects
        nearList.OrderBy(o => Vector3.Distance(transform.position, o.transform.position));
        return nearList;
    }
}
