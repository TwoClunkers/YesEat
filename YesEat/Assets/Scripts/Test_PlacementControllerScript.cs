using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlacementControllerScript : MonoBehaviour
{
    public void TestSet1()
    {
        Debug.Log("T_PlinkettSafetyLearning: " + T_PlinkettSafetyLearning());
    }

    public bool T_PlinkettSafetyLearning()
    {
        // Given: A Gobber is hungry and is within range of a Plinkett.
        Subject newSubject = masterSubjectList.GetSubject(1);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        // When: Plinkett is attacked by Gobber.
        sPlinkett.Damage(masterSubjectList.GetSubject(6), 10);
        // Then: Plinkett's memory of Gobber has negative safety.
        SubjectMemory subMem = sPlinkett.T_Npc.T_GetMemory(6);
        if (subMem != null)
            return (sPlinkett.T_Npc.T_GetMemory(6).Safety < 0);
        else
            return false;
    }

    public bool T_PlinkettEvasionTest()
    {
        throw new NotImplementedException();
        // Given: A Plinkett memory has a negative safety for Gobbers.
        Subject newSubject = masterSubjectList.GetSubject(1);
        GameObject testPlinkett = Instantiate(newSubject.Prefab, new Vector3(0, 0, 1), Quaternion.identity);
        AnimalObjectScript sPlinkett = testPlinkett.GetComponent<AnimalObjectScript>();
        
        // When: Plinkett sees a Gobber.
        sPlinkett.Damage(masterSubjectList.GetSubject(6), 10);
        
        // Then: Plinkett's current safety is reduced.
        SubjectMemory subMem = sPlinkett.T_Npc.T_GetMemory(6);
        if (subMem != null)
            return (sPlinkett.T_Npc.T_GetMemory(6).Safety < 0);
        else
            return false;
        
    }

}

