using System;
using System.Collections.Generic;
using UnityEngine;

public static class DbIds
{
    // \/   \/   \/  UPDATE LAST INDEX  \/   \/   \/
    public static int Plinkett = 1;
    public static int Location = 2;
    public static int Bush = 3;
    public static int Berry = 4;
    public static int Meat = 5;
    public static int Gobber = 6;
    // \/   \/   \/  UPDATE LAST INDEX  \/   \/   \/
    public static int LastIndex = 6;
}

/// <summary>
/// A Character's attitude about a specific subject.
/// </summary>
public class MasterSubjectList
{
    #region Private members
    private List<Subject> masterSubjectList;
    private int maxID;
    #endregion

    public MasterSubjectList()
    {
        masterSubjectList = new List<Subject>();

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Plinkett; // == Plinkett
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        NpcDefinition plinkettNpcDefinition = new NpcDefinition()
        {
            Memories = new List<SubjectMemory>(),
            AttackDamage = 10,
            FoodHungry = 60,
            FoodMax = 100,
            FoodMetabolizeRate = 1,
            HealthDanger = 40,
            HealthMax = 50,
            HealthRegen = 1,
            MetabolizeInterval = 1,
            MoveSpeed = 5,
            Nest = null,
            SafetyDeadly = -10,
            SafetyHigh = 10,
            SightRangeFar = 10,
            SightRangeNear = 2.5f,
            StarvingDamage = 3,
            Traits = new NpcCharacterTraits(NpcTraits.Herbivore)
        };
        AnimalSubject plinkett = new AnimalSubject()
        {
            Definition = plinkettNpcDefinition,
            Description = "A herbivore.",
            GrowthTime = 5,
            Icon = new UnityEngine.Sprite(),
            InventorySize = 1,
            LootID = 5,
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Plinkett",
            RelatedSubjects = new int[0],
            SubjectID = maxID,
            Prefab = Resources.Load("GameObjects/Plinkett") as GameObject
        };

        masterSubjectList.Add(plinkett);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Location; // == Location
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        LocationSubject NewLocationOne = new LocationSubject()
        {
            Coordinates = new UnityEngine.Vector3(1, 1, 1),
            Description = "A very positional kind of location",
            Icon = new UnityEngine.Sprite(),
            Layer = 1,
            Name = "Location",
            Radius = 2,
            RelatedSubjects = new int[0],
            SubjectID = maxID,
            Prefab = Resources.Load("GameObjects/LocationMarker") as GameObject
        };
        masterSubjectList.Add(NewLocationOne);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Bush; // == Bush
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        PlantSubject Bush = new PlantSubject()
        {
            SubjectID = maxID,
            Name = "Bush",
            Description = "A Berry Bush",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = Resources.Load("GameObjects/Bush") as GameObject,

            ProduceID = 4,
            ProduceTime = 2,
            MaxGrowth = 30,
            GrowthRate = 1,
            MatureGrowth = 2,
            InventorySize = 3,
            NodeList = new Vector3[2],
            NodeAttachment = null,
            HeightRatio = 2.5f
        };
        masterSubjectList.Add(Bush);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Berry; // == Berry
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        FoodSubject Berry = new FoodSubject()
        {
            SubjectID = maxID,
            Name = "Berry",
            Description = "A Juicy Berry",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            BuildDirections = null,
            MaxStack = 10,
            FoodType = 0,
            FoodValue = 10
        };
        masterSubjectList.Add(Berry);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Meat; // == Meat
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        FoodSubject Meat = new FoodSubject()
        {
            SubjectID = maxID,
            Name = "Meat",
            Description = "It was once muscle...",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            BuildDirections = null,
            MaxStack = 10,
            FoodType = 1,
            FoodValue = 10
        };
        masterSubjectList.Add(Meat);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = DbIds.Gobber; // == Gobber
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        NpcDefinition gobberNpcDefinition = new NpcDefinition()
        {
            Memories = new List<SubjectMemory>(),
            AttackDamage = 10,
            FoodHungry = 50,
            FoodMax = 100,
            FoodMetabolizeRate = 1,
            HealthDanger = 40,
            HealthMax = 100,
            HealthRegen = 1,
            MetabolizeInterval = 1,
            MoveSpeed = 5,
            Nest = null,
            SafetyDeadly = -10,
            SafetyHigh = 10,
            SightRangeFar = 10,
            SightRangeNear = 2,
            StarvingDamage = 5,
            Traits = new NpcCharacterTraits(NpcTraits.Carnivore)
        };
        AnimalSubject gobber = new AnimalSubject()
        {
            Definition = gobberNpcDefinition,
            Description = "A carnivore.",
            GrowthTime = 5,
            Icon = new UnityEngine.Sprite(),
            InventorySize = 1,
            LootID = 5,
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Gobber",
            RelatedSubjects = new int[0],
            SubjectID = maxID,
            Prefab = Resources.Load("GameObjects/Gobber") as GameObject
        };
        masterSubjectList.Add(gobber);

        // \/ \/ DO NOT CHANGE \/ \/
        maxID = DbIds.LastIndex;
        // /\ /\ DO NOT CHANGE /\ /\
    }

    /// <summary>
    /// Get Subject using subjectID.
    /// </summary>
    /// <param name="subjectID">subjectID of Subject to find.</param>
    /// <param name="subjectType">Required type of subject.</param>
    /// <returns>Returns null if subject is not found or the type of the returned subject does not match subjectType.</returns>
    public Subject GetSubject(int subjectID, Type subjectType = null)
    {
        if (masterSubjectList.Exists(o => o.SubjectID == subjectID))
        {
            Subject tempSubject = masterSubjectList.Find(o => o.SubjectID == subjectID);
            Type tempType = tempSubject.GetType();
            if (subjectType != null)
            {
                if (tempType == subjectType)
                    return tempSubject.Copy();
                else
                    return null;
            }
            else
            {
                return tempSubject.Copy();
            }
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// GetNextID bumps up the ID count to assign to new subjects
    /// </summary>
    /// <returns></returns>
    public int GetNextID()
    {
        maxID += 1;

        return maxID;
    }

    public bool AddSubject(Subject newSubject)
    {
        //if we don't have a valid subject or if the ID is in use return false
        if (newSubject == null) return false;
        if (masterSubjectList.Exists(o => o.SubjectID == newSubject.SubjectID)) return false;

        //otherwise we should be able to add
        masterSubjectList.Add(newSubject);
        return true;
    }
}
