using System;
using System.Collections.Generic;

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
        maxID = 1; // == Plinkett
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        NpcDefinition plinkettNpcDefinition = new NpcDefinition()
        {
            Attitudes = new List<SubjectAttitude>(),
            FoodHungry = 50,
            FoodMax = 100,
            FoodMetabolizeRate = 1,
            HealthDanger = 40,
            HealthMax = 100,
            HealthRegen = 1,
            LocationMemories = new List<NpcLocationMemory>(),
            MetabolizeInterval = 10,
            MoveSpeed = 5,
            Nest = null,
            SafetyDeadly = -10,
            SafetyHigh = 10,
            SightRangeFar = 5,
            SightRangeNear = 2,
            StarvingDamage = 1,
            Traits = new NpcCharacterTraits(NpcTraits.NestMaker & NpcTraits.Herbivore)
        };
        AnimalSubject plinkett = new AnimalSubject()
        {
            Definition = plinkettNpcDefinition,
            Description = "A herbivore.",
            GrowthTime = 5,
            Icon = new UnityEngine.Sprite(),
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Plinkett",
            RelatedSubjects = new int[0],
            SubjectID = maxID
        };

        masterSubjectList.Add(plinkett);
        
        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = 2; // == Location
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        //TODO: load all subjects into the list.
        LocationSubject NewLocationOne = new LocationSubject()
        {
            Coordinates = new UnityEngine.Vector3(1, 1, 1),
            Description = "A very positional kind of location",
            Icon = new UnityEngine.Sprite(),
            Layer = 1,
            Name = "Location",
            Radius = 2,
            RelatedSubjects = new int[0],
            SubjectID = maxID
        };
        masterSubjectList.Add(NewLocationOne);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = 3; // == Bush
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        //TODO: load all subjects into the list.
        PlantSubject Bush = new PlantSubject()
        {
            SubjectID = maxID,
            Name = "Bush",
            Description = "A Berry Bush",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            ProduceID = 4,
            ProduceTime = 10,
            MaxGrowth = 20,
            GrowthTime = 10,
            MatureGrowth = 20,
            InventorySize = 3
        };
        masterSubjectList.Add(Bush);

        //`-.,.-'-.,.-'-.,.-'-.,.-'-.,.-'
        maxID = 4; // == Berry
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        //TODO: load all subjects into the list.
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
        maxID = 5; // == Meat
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        //TODO: load all subjects into the list.
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
        maxID = 6; // == Gobber
        //,.-`-.,.-`-.,.-`-.,.-`-.,.-`-.,
        NpcDefinition gobberNpcDefinition = new NpcDefinition()
        {
            Attitudes = new List<SubjectAttitude>(),
            FoodHungry = 50,
            FoodMax = 100,
            FoodMetabolizeRate = 1,
            HealthDanger = 40,
            HealthMax = 100,
            HealthRegen = 1,
            LocationMemories = new List<NpcLocationMemory>(),
            MetabolizeInterval = 10,
            MoveSpeed = 5,
            Nest = null,
            SafetyDeadly = -10,
            SafetyHigh = 10,
            SightRangeFar = 5,
            SightRangeNear = 2,
            StarvingDamage = 1,
            Traits = new NpcCharacterTraits(NpcTraits.Carnivore)
        };
        AnimalSubject gobber = new AnimalSubject()
        {
            Definition = gobberNpcDefinition,
            Description = "A carnivore.",
            GrowthTime = 5,
            Icon = new UnityEngine.Sprite(),
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Gobber",
            RelatedSubjects = new int[0],
            SubjectID = maxID
        };

        masterSubjectList.Add(gobber);
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
            if (subjectType != null)
            {
                if (tempSubject.GetType() == subjectType)
                    return tempSubject;
                else
                    return null;
            }
            else
            {
                return tempSubject;
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
