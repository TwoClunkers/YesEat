using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlacementControllerScript : MonoBehaviour
{
    public void TestSet1()
    {
        Debug.Log("Running TestSet1 ...");
        Debug.Log("T_PlinkettSafetyLearning: " + T_PlinkettSafetyLearning());
        Debug.Log("T_PlinkettEvasionTest: " + T_PlinkettEvasionTest());
        Debug.Log("Completed TestSet1.");
    }

    public void TestSet2()
    {
        // testing location waypoint generation
        float testLocationRadius = 7.5f;
        float testSightRadius = 2.0f;
        GameObject loc1 = T_PlaceLocation(new Vector3(-1.0f, 0, -2.0f), testLocationRadius);
        LocationObjectScript locScript = loc1.GetComponent<LocationObjectScript>();
        LocationSubject locSub = locScript.Subject as LocationSubject;
        Vector3[] locs = locSub.GetAreaWaypoints(testSightRadius);
        if (locs.Length > 0)
        {
            for (int i = 0; i <= locs.Length - 1; i++)
            {
                Debug.Log(i + " : " + locs[i].x + "," + locs[i].y + "," + locs[i].z + "\n");
                T_PlaceLocation(locs[i], testSightRadius);
            }
        }
        else
        {
            Debug.Log("!  -- No waypoints generated.");
        }
        //Destroy(loc1);
    }

    public void TestSet3()
    {
        CreateLocation(new Vector3(3.0f, 0, 3.0f), 1.6f);
        CreateLocation(new Vector3(0, 0, 0), 1.6f);
        CreateLocation(new Vector3(-3.0f, 0, -3.0f), 1.6f);
        CreateLocation(new Vector3(3.0f, 0, -3.0f), 1.6f);
        CreateLocation(new Vector3(-3.0f, 0, 3.0f), 2.0f);
        SpawnObject(DbIds.Bush, new Vector3(-4.0f, 0, -4.0f));
        GameObject plink = SpawnObject(DbIds.Plinkett, new Vector3(3.5f, 0, -4.0f));
        AnimalObjectScript plinkScript = plink.GetComponent<AnimalObjectScript>();
        //plinkScript.T_Npc.T_SetValues(newFood: 61);
        GameObject gob = SpawnObject(DbIds.Gobber, new Vector3(3.5f, 0, 3.5f));
        AnimalObjectScript gobScript = gob.GetComponent<AnimalObjectScript>();
    }

    public void TestSet4()
    {

    }

    public void TestSet5()
    {

    }

    public void TestSet6()
    {

    }

    /// <summary>
    /// !!!  FOR TESTING ONLY DO NOT USE !!! <para/>
    /// Given: A Gobber is hungry and is within range of a Plinkett. <para />
    /// When: Plinkett is attacked by Gobber.<para />
    /// Then: Plinkett's memory of Gobber has negative safety.<para />
    /// </summary>
    /// <returns>Success/Fail</returns>
    public bool T_PlinkettSafetyLearning()
    {
        bool testResult = false;
        // Ross -- Tested & working 8/8/17 19:45
        // Given: A Gobber is hungry and is within range of a Plinkett.
        Subject newSubject = masterSubjectList.GetSubject(DbIds.Plinkett);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        sPlinkett.InitializeFromSubject(masterSubjectList, newSubject);
        // When: Plinkett is attacked by Gobber.
        sPlinkett.Damage(masterSubjectList.GetSubject(DbIds.Gobber), 10);
        // Then: Plinkett's memory of Gobber has negative safety.
        SubjectMemory subMem = sPlinkett.T_Npc.T_GetMemory(6);
        List<SubjectMemory> sMem = sPlinkett.T_Npc.T_Memories;
        if (subMem != null)
            testResult = (subMem.Safety < 0);
        else
            testResult = false;

        Destroy(testPlinkett);

        return testResult;
    }

    /// <summary>
    /// !!!  FOR TESTING ONLY DO NOT USE !!! <para/>
    /// Given: A Plinkett memory has a negative safety for Gobbers. <para />
    /// When: Plinkett sees a Gobber. <para />
    /// Then: Plinkett's current safety is reduced. <para />
    /// </summary>
    /// <returns>Success/Fail</returns>
    public bool T_PlinkettEvasionTest()
    {

        GameObject loc1 = T_PlaceLocation(new Vector3(3.0f, 0, 3.0f), 1.6f);
        GameObject loc2 = T_PlaceLocation(new Vector3(0, 0, 0), 1.6f);
        GameObject loc3 = T_PlaceLocation(new Vector3(-3.0f, 0, -3.0f), 1.6f);
        GameObject loc4 = T_PlaceLocation(new Vector3(3.0f, 0, -3.0f), 1.6f);
        GameObject loc5 = T_PlaceLocation(new Vector3(-3.0f, 0, 3.0f), 1.6f);

        // Given: A Plinkett memory has a negative safety for Gobbers.
        Subject newSubject = masterSubjectList.GetSubject(DbIds.Plinkett);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        sPlinkett.InitializeFromSubject(masterSubjectList, newSubject);
        sPlinkett.T_Npc.T_Memories.Add(new SubjectMemory(6, -1, 0));

        int startingSafety = sPlinkett.GetSafety();

        Subject gobSubject = masterSubjectList.GetSubject(DbIds.Gobber);
        GameObject testGob = Instantiate(gobSubject.Prefab, new Vector3(0, 0, 2), Quaternion.identity);
        testGob.GetComponent<AnimalObjectScript>().InitializeFromSubject(masterSubjectList, gobSubject);

        // When: Plinkett sees a Gobber.
        sPlinkett.T_Npc.AiCoreProcess();

        bool testResult = (sPlinkett.GetSafety() < startingSafety);

        Destroy(testPlinkett);
        Destroy(testGob);
        Destroy(loc1);
        Destroy(loc2);
        Destroy(loc3);
        Destroy(loc4);
        Destroy(loc5);
        // Then: Plinkett's current safety is reduced.
        return testResult;

    }

    /// <summary>
    /// !!!  FOR TESTING ONLY DO NOT USE !!! <para/>
    /// Place a new location in the world and add it to the MasterSubjectList.
    /// </summary>
    /// <param name="newPosition">The center point of the new location</param>
    /// <param name="newRadius">The radius of the new location.</param>
    public GameObject T_PlaceLocation(Vector3 newPosition, float newRadius)
    {
        LocationSubject newLocation = new LocationSubject(masterSubjectList.GetSubject(DbIds.Location) as LocationSubject);
        newLocation.Name = "Location " + Time.time;
        newLocation.Description = "New Location " + Time.time;
        newLocation.Radius = newRadius;
        newLocation.Coordinates = newPosition;
        newLocation.Icon = null;
        newLocation.Layer = 1;
        //add the next id available
        newLocation.SubjectID = masterSubjectList.GetNextID();

        GameObject location1 = Instantiate(newLocation.Prefab, newPosition, Quaternion.identity);
        LocationObjectScript locScript = location1.GetComponent<LocationObjectScript>();
        locScript.InitializeFromSubject(masterSubjectList, newLocation);
        locScript.Relocate(newLocation);

        if (!masterSubjectList.AddSubject(newLocation)) Debug.Log("FAIL ADD");
        return location1;
    }

}

