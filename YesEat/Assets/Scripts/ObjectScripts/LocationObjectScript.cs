using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocationObjectScript : SubjectObjectScript
{
    #region Private members
    private bool isChanged = false;
    private int count;
    #endregion
   
    
    // Use this for initialization
    void Start()
    {
        thisSubject = new LocationSubject();
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        count += 1;

        if (isChanged)
        {
            //->> to do: Publish to Masterlist
            //Relocate(thisSubject as LocationSubject);
            isChanged = false;
        }

        if (count > 100)
        {
            UpdateRelatedInLocation();
            count = 0;
        }
        
    }

    /// <summary>
    /// Relocate and resize using location subject card
    /// </summary>
    /// <param name="newLocation"></param>
    public void Relocate(LocationSubject newLocation)
    {
        //resize and reposition if we have a valid location subject
        if (newLocation != null)
        {
            gameObject.transform.localScale = new Vector3((float)newLocation.Radius, 0.1f, (float)newLocation.Radius);
            gameObject.transform.position = newLocation.Coordinates;
        }
    }

    /// <summary>
    /// UpdateRelatedInLocation finds objects inside the location and adds them to the relatedSubjects locally
    /// </summary>
    public void UpdateRelatedInLocation()
    {
        LocationSubject locationSubject = thisSubject as LocationSubject;
        if (locationSubject == null) return;

        //grab collides within the area of our location and create a list of objects
        Collider[] hitColliders = Physics.OverlapSphere(locationSubject.Coordinates, locationSubject.Radius);

        
        List<int> foundSubjectIDs = new List<int>();
        for (int i = 0; i < hitColliders.Length; i++)
        {
            //this tries to grab the base script class for our objects, move along if null
            SubjectObjectScript script = hitColliders[i].GetComponent<SubjectObjectScript>() as SubjectObjectScript;
            if (script == null) continue;

            //record each subjectID found if not already recorded
            int foundID = script.ThisSubject.SubjectID;
            if(!foundSubjectIDs.Contains(foundID))
            {
                foundSubjectIDs.Add(foundID);
            }
        }

        //Compare to old so that we may not need to publish to master
        int[] newRelated = new int[foundSubjectIDs.Count];
        newRelated = foundSubjectIDs.ToArray();

        if (newRelated.Length != thisSubject.RelatedSubjects.Length)
            isChanged = true;
        else 
        {
            for (int i = 0; i < newRelated.Length; i++)
            {
                if (newRelated[i] != thisSubject.RelatedSubjects[i])
                {
                    isChanged = true;
                    break;
                }
            }
        }

        //Store output in local subject copy
        thisSubject.RelatedSubjects = foundSubjectIDs.ToArray();
    }
}
