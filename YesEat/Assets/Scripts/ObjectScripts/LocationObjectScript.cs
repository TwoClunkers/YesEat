using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocationObjectScript : SubjectObjectScript
{
    #region Private members
    private bool isChanged = false;
    private int count;
    private List<ObjectMemory> localObjects;
    #endregion

    public int testNumber;


    void Awake()
    {
        testNumber = 0;
        subject = new LocationSubject();
        count = 0;
        localObjects = new List<ObjectMemory>();
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
            gameObject.transform.localScale = new Vector3((float)newLocation.Radius * 2, 0.1f, (float)newLocation.Radius * 2);
            gameObject.transform.position = newLocation.Coordinates;
        }
    }

    /// <summary>
    /// UpdateRelatedInLocation finds objects inside the location and adds them to the relatedSubjects locally
    /// </summary>
    public void UpdateRelatedInLocation()
    {
        LocationSubject locationSubject = subject as LocationSubject;
        if (locationSubject == null) return;

        //grab collides within the area of our location and create a list of objects
        int layerMask = ~((1 << 8) | (1 << 9)); // filter out terrain and locations
        Collider[] hitColliders = Physics.OverlapSphere(locationSubject.Coordinates, locationSubject.Radius, layerMask);

        List<int> foundSubjectIDs = new List<int>();
        localObjects.Clear();
        for (int i = 0; i < hitColliders.Length; i++)
        {
            // get the base script class for our objects
            SubjectObjectScript script = hitColliders[i].GetComponent<SubjectObjectScript>() as SubjectObjectScript;
            if (script == null)
            {
                Debug.Log("SubjectObjectScript was not assigned to " + hitColliders[i].name);
                continue;
            }

            //tell each Object that it is within our location
            script.Location = subject as LocationSubject;

            //record each subjectID found if not already recorded
            int foundID = script.Subject.SubjectID;
            if (!foundSubjectIDs.Contains(foundID))
            {
                foundSubjectIDs.Add(foundID);
            }

            //record object quantites for NPC memory
            ObjectMemory existingMemory = localObjects.Find(o => o.SubjectID == script.Subject.SubjectID);
            if (existingMemory != null)
            {
                existingMemory.Quantity++;
            }
            else
            {
                localObjects.Add(new ObjectMemory() { Quantity = 1, SubjectID = script.Subject.SubjectID });
            }
        }

        //Compare to old so that we may not need to publish to master
        int[] newRelated = new int[foundSubjectIDs.Count];
        newRelated = foundSubjectIDs.ToArray();

        if (newRelated.Length != subject.RelatedSubjects.Length)
            isChanged = true;
        else
        {
            for (int i = 0; i < newRelated.Length; i++)
            {
                if (newRelated[i] != subject.RelatedSubjects[i])
                {
                    isChanged = true;
                    break;
                }
            }
        }

        //Store output in local subject copy
        subject.RelatedSubjects = foundSubjectIDs.ToArray();
    }

    /// <summary>
    /// No harvest for a location!
    /// </summary>
    /// <returns></returns>
    public override InventoryItem Harvest()
    {
        return null;
    }

    public override void InitializeFromSubject(Subject newSubject)
    {
        subject = newSubject as LocationSubject;
        count = 0;
    }

    internal void TeachNpc(NpcCore npc)
    {
        subject.TeachNpc(npc);
        LocationMemory locationMemory = npc.Definition.Memories.Find(o => o.SubjectID == Subject.SubjectID) as LocationMemory;
        locationMemory.LastTimeSeen = DateTime.Now;
        locationMemory.ObjectMemories = localObjects;
    }
}
