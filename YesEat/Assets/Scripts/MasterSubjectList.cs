using System;
using System.Collections.Generic;
using UnityEngine;

public static class DbIds
{
    // \/   \/   \/  UPDATE LAST INDEX  \/   \/   \/
    public static int Plinkett = 1;          //   \/
    public static int Location = 2;          //   \/
    public static int Bush = 3;              //   \/
    public static int Berry = 4;             //   \/
    public static int Meat = 5;              //   \/
    public static int Gobber = 6;            //   \/
    public static int Branch = 7;            //   \/
    public static int Leaves = 8;            //   \/
    public static int PlinkettNest = 9;      //   \/
    public static int GobberNest = 10;       //   \/
    // ---- UPDATE LAST INDEX --- \/         //   \/
    public static int LastIndex = 10; // <<  <<   <<
    // -------------------------  /\
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
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        NpcDefinition plinkettNpcDefinition = new NpcDefinition()
        {
            Likes = new SubjectAttributes()
            {
                Attributes = SubjectAttributesEnum.SmellsFloral
            },
            Dislikes = new SubjectAttributes()
            {
                Attributes = SubjectAttributesEnum.SmellsPungent
            },
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
            RangeSightFar = 10,
            RangeSightMid = 5.0f,
            RangeSightNear = 2.0f,
            RangeActionClose = 1.0f,
            StarvingDamage = 3,
            Traits = new NpcCharacterTraits(NpcTraits.Herbivore)
        };
        AnimalSubject plinkett = new AnimalSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.SoundsQuiet
                    | SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.LooksBright
                },
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
            SubjectID = DbIds.Plinkett,
            Prefab = Resources.Load("GameObjects/Plinkett") as GameObject
        };
        masterSubjectList.Add(plinkett);

        LocationSubject NewLocationOne = new LocationSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.LooksBright
                    | SubjectAttributesEnum.SoundsQuiet
                },
            Coordinates = new UnityEngine.Vector3(1, 1, 1),
            Description = "A very positional kind of location",
            Icon = new UnityEngine.Sprite(),
            Layer = 1,
            Name = "Location",
            Radius = 2,
            RelatedSubjects = new int[0],
            SubjectID = DbIds.Location,
            Prefab = Resources.Load("GameObjects/LocationMarker") as GameObject
        };
        masterSubjectList.Add(NewLocationOne);

        PlantSubject Bush = new PlantSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.SmellsFloral
                    | SubjectAttributesEnum.LooksDark
                    | SubjectAttributesEnum.FeelsHard
                },
            SubjectID = DbIds.Bush,
            Name = "Bush",
            Description = "A Berry Bush",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = Resources.Load("GameObjects/Bush") as GameObject,

            ProduceID = 4,
            ProduceTime = 2,
            MaxGrowth = 30,
            GrowthTime = 1,
            MatureGrowth = 2,
            InventorySize = 3
        };
        masterSubjectList.Add(Bush);

        FoodSubject Berry = new FoodSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.SmellsFloral
                    | SubjectAttributesEnum.TastesSweet
                    | SubjectAttributesEnum.FeelsSoft
                },
            SubjectID = DbIds.Berry,
            Name = "Berry",
            Description = "A Juicy Berry",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = null,
            Recipe = null,
            MaxStack = 10,
            FoodType = 0,
            FoodValue = 10
        };
        masterSubjectList.Add(Berry);

        FoodSubject Meat = new FoodSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.LooksDark
                    | SubjectAttributesEnum.TastesSalty
                },
            SubjectID = DbIds.Meat,
            Name = "Meat",
            Description = "It was once muscle...",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = null,

            Recipe = null,
            MaxStack = 10,
            FoodType = 1,
            FoodValue = 10
        };
        masterSubjectList.Add(Meat);

        NpcDefinition gobberNpcDefinition = new NpcDefinition()
        {
            Likes = new SubjectAttributes()
            {
                Attributes = SubjectAttributesEnum.FeelsSoft
            },
            Dislikes = new SubjectAttributes()
            {
                Attributes = SubjectAttributesEnum.FeelsHard
            },
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
            RangeSightFar = 10,
            RangeSightMid = 5.0f,
            RangeSightNear = 2,
            RangeActionClose = 1.0f,
            StarvingDamage = 5,
            Traits = new NpcCharacterTraits(NpcTraits.Carnivore)
        };
        AnimalSubject gobber = new AnimalSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.SmellsPungent
                    | SubjectAttributesEnum.SoundsLoud
                    | SubjectAttributesEnum.FeelsHard
                    | SubjectAttributesEnum.LooksDark
                },
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
            SubjectID = DbIds.Gobber,
            Prefab = Resources.Load("GameObjects/Gobber") as GameObject
        };
        masterSubjectList.Add(gobber);

        ItemSubject Branch = new ItemSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsHard
                },
            SubjectID = DbIds.Branch,
            Name = "Branch",
            Description = "A severed part of a large plant or tree.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = null,
            Recipe = null,
            MaxStack = 1,
        };
        masterSubjectList.Add(Branch);

        ItemSubject Leaves = new ItemSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                },
            SubjectID = DbIds.Leaves,
            Name = "Leaves",
            Description = "Leaves from a plant or tree.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Recipe = null,
            MaxStack = 10,
            Prefab = null
        };
        masterSubjectList.Add(Leaves);

        StructureSubject PlinkettNest = new StructureSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.SoundsQuiet
                },
            SubjectID = DbIds.PlinkettNest,
            Name = "Plinkett Nest",
            Description = "A spot that looks comfortable for Plinketts.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            Recipe = new BuildRecipe(
                new Ingredient[]
                {
                    new Ingredient(DbIds.Branch, 3),
                    new Ingredient(DbIds.Leaves, 5)
                }),
            Prefab = null
        };
        masterSubjectList.Add(PlinkettNest);

        StructureSubject GobberNest = new StructureSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.SoundsQuiet
                },
            SubjectID = DbIds.GobberNest,
            Name = "Gobber Nest",
            Description = "A spot that looks comfortable for Gobbers.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            Recipe = new BuildRecipe(
                new Ingredient[]
                {
                    new Ingredient(DbIds.Branch, 3),
                    new Ingredient(DbIds.Leaves, 5)
                }),
            Prefab = null
        };
        masterSubjectList.Add(GobberNest);

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
