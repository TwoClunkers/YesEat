using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalObjectScript : SubjectObjectScript
{
    #region Private members
    public int sightNear;
    public int sightFar;
    public int count;
    public float speed;
    private GameObject[] nearObjects;
    private GameObject[] farObjects;
    
    #endregion

    // Use this for initialization
    void Start()
    {
        sightFar = 5;
        sightNear = 2;
        speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if(location != null)
        {
            float distance = Vector3.Distance(location.Coordinates, transform.position);
            if(distance > 5) MoveTowardsPoint(location.Coordinates, speed);
            else if(distance > 1) MoveTowardsPoint(location.Coordinates, speed/3);
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

    void Observe()
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

    }
}
