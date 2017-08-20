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
        float testLocationRadius = 6.0f;
        float testSightRadius = 2.0f;
        Vector3 loc1Offset = new Vector3(-testLocationRadius - (2 * testSightRadius), 0, 0);
        Vector3 loc2Offset = new Vector3(0, 0, -testLocationRadius - (2 * testSightRadius));
        GameObject loc1 = T_PlaceLocation(loc1Offset, testLocationRadius);
        LocationObjectScript locScript = loc1.GetComponent<LocationObjectScript>();
        LocationSubject locSub = locScript.Subject as LocationSubject;
        Vector3[] locs = locSub.GetAreaWaypoints(testSightRadius, 1);
        if (locs.Length > 0)
        {
            for (int i = 0; i <= locs.Length - 1; i++)
            {
                //Debug.Log(i + " : " + locs[i].x + "," + locs[i].y + "," + locs[i].z + "\n");
                T_PlaceLocation(locs[i], testSightRadius);
            }
        }
        else
        {
            Debug.Log("  -- loc1 !  -- No waypoints generated.");
        }

        GameObject loc2 = T_PlaceLocation(loc2Offset, testLocationRadius);
        LocationObjectScript loc2Script = loc2.GetComponent<LocationObjectScript>();
        LocationSubject loc2Sub = loc2Script.Subject as LocationSubject;
        Vector3[] locs2 = loc2Sub.GetAreaWaypoints(testSightRadius);
        if (locs2.Length > 0)
        {
            for (int i = 0; i <= locs2.Length - 1; i++)
            {
                //Debug.Log(i + " : " + locs[i].x + "," + locs[i].y + "," + locs[i].z + "\n");
                T_PlaceLocation(locs2[i], testSightRadius);
            }
        }
        else
        {
            Debug.Log(" -- loc2 !  -- No waypoints generated.");
        }

    }

    public void TestSet3()
    {
        CreateLocation(new Vector3(3.0f, 0, 3.0f), 2.0f);
        CreateLocation(new Vector3(0, 0, -2.0f), 1.6f);
        CreateLocation(new Vector3(-3.0f, 0, -4.5f), 1.7f);
        CreateLocation(new Vector3(3.0f, 0, -4.0f), 1.6f);
        CreateLocation(new Vector3(-3.0f, 0, 3.0f), 4.0f);

        SpawnObject(DbIds.Bush, new Vector3(-4.0f, 0, -4.0f));
        T_SpawnMobPacks();
    }

    public void TestSet4()
    {
        T_SpawnMobPacks();
    }

    public void TestSet5()
    {

    }

    public void TestSet6()
    {
        // make the selected target hungry.
        if (lastSelector != null)
        {
            SubjectObjectScript objScript = lastSelector.GetComponentInParent<SubjectObjectScript>();
            if (objScript != null)
            {
                if (objScript.GetType() == typeof(AnimalObjectScript))
                {
                    AnimalObjectScript animal = objScript as AnimalObjectScript;
                    animal.T_Npc.T_SetValues(newFood: animal.T_Npc.Definition.FoodHungry);
                }
            }
        }
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
        Subject newSubject = MasterSubjectList.GetSubject(DbIds.Plinkett);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        sPlinkett.InitializeFromSubject(newSubject);
        // When: Plinkett is attacked by Gobber.
        sPlinkett.Damage(MasterSubjectList.GetSubject(DbIds.Gobber), 10);
        // Then: Plinkett's memory of Gobber has negative safety.
        SubjectMemory subMem = sPlinkett.T_Npc.T_GetMemory(DbIds.Gobber);
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
        Subject newSubject = MasterSubjectList.GetSubject(DbIds.Plinkett);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        sPlinkett.InitializeFromSubject(newSubject);
        sPlinkett.T_Npc.T_Memories.Add(new SubjectMemory(DbIds.Gobber, -1, 0));

        int startingSafety = sPlinkett.GetSafety();

        Subject gobSubject = MasterSubjectList.GetSubject(DbIds.Gobber);
        GameObject testGob = Instantiate(gobSubject.Prefab, new Vector3(0, 0, 2), Quaternion.identity);
        testGob.GetComponent<AnimalObjectScript>().InitializeFromSubject(gobSubject);

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
        LocationSubject newLocation = new LocationSubject(MasterSubjectList.GetSubject(DbIds.Location) as LocationSubject);
        newLocation.Name = "Location " + Time.time;
        newLocation.Description = "New Location " + Time.time;
        newLocation.Radius = newRadius;
        newLocation.Coordinates = newPosition;
        newLocation.Icon = null;
        newLocation.Layer = 1;
        //add the next id available
        newLocation.SubjectID = MasterSubjectList.GetNextID();

        GameObject location1 = Instantiate(newLocation.Prefab, newPosition, Quaternion.identity);
        LocationObjectScript locScript = location1.GetComponent<LocationObjectScript>();
        locScript.InitializeFromSubject(newLocation);
        locScript.Relocate(newLocation);

        if (!MasterSubjectList.AddSubject(newLocation)) Debug.Log("FAIL ADD");
        return location1;
    }

    private void T_SpawnMobPacks()
    {
        SpawnObject(DbIds.Plinkett, new Vector3(3.5f, 0, -4.0f));
        SpawnObject(DbIds.Plinkett, new Vector3(2.5f, 0, -4.5f));
        SpawnObject(DbIds.Plinkett, new Vector3(3.0f, 0, -3.5f));
        SpawnObject(DbIds.Plinkett, new Vector3(4.5f, 0, -5.0f));
        SpawnObject(DbIds.Plinkett, new Vector3(-3.5f, 0, -4.0f));
        SpawnObject(DbIds.Plinkett, new Vector3(-2.5f, 0, -4.0f));
        AnimalObjectScript gob = SpawnObject(DbIds.Gobber, new Vector3(3.5f, 0, 3.5f)).GetComponent<AnimalObjectScript>();
        gob.T_Npc.T_SetValues(newFood: 50);
        AnimalObjectScript gob2 = SpawnObject(DbIds.Gobber, new Vector3(3.0f, 0, 2.5f)).GetComponent<AnimalObjectScript>();
        gob2.T_Npc.T_SetValues(newFood: 50);
    }
}

