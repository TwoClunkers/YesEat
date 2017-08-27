using System;
using System.Collections.Generic;
using UnityEngine;

public static class KbIds
{
    public static int Berry;
    public static int Branch;
    public static int Bush;
    public static int Gobber;
    public static int GobberNest;
    public static int Hole10;
    public static int Leaves;
    public static int Location;
    public static int Meat;
    public static int Plinkett;
    public static int PlinkettNest;
}

/// <summary>
/// The main database for all Subjects.
/// </summary>
public class KnowledgeBase
{
    #region Private members
    static private List<Subject> knowledgeBase;
    static private int maxID;
    #endregion

    static KnowledgeBase()
    {
        maxID = 0;
        knowledgeBase = new List<Subject>();
        InitializeDatabase();
    }

    static private void InitializeDatabase()
    {
        KbIds.Branch = GetNextID();
        ItemSubject Branch = new ItemSubject()
        {
            Attributes =
        new SubjectAttributes()
        {
            Attributes = SubjectAttributesEnum.FeelsHard
        },
            SubjectID = KbIds.Branch,
            Name = "Branch",
            Description = "A severed part of a large plant or tree.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = null,
            Recipe = null,
            MaxStack = 10,
        };
        knowledgeBase.Add(Branch);

        KbIds.Leaves = GetNextID();
        ItemSubject Leaves = new ItemSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                },
            SubjectID = KbIds.Leaves,
            Name = "Leaves",
            Description = "Leaves from a plant or tree.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Recipe = null,
            MaxStack = 10,
            Prefab = null
        };
        knowledgeBase.Add(Leaves);

        KbIds.Berry = GetNextID();
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
            SubjectID = KbIds.Berry,
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
        knowledgeBase.Add(Berry);

        KbIds.Meat = GetNextID();
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
            SubjectID = KbIds.Meat,
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
        knowledgeBase.Add(Meat);

        KbIds.PlinkettNest = GetNextID();
        StructureSubject PlinkettNest = new StructureSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.SoundsQuiet
                },
            SubjectID = KbIds.PlinkettNest,
            Name = "Plinkett Nest",
            Description = "A spot that looks comfortable for Plinketts.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            Recipe = new BuildRecipe(
                new List<InventoryItem>()
                {
                    new InventoryItem(KbIds.Branch, 3),
                    new InventoryItem(KbIds.Leaves, 5)
                }),
            InventorySize = 10,
            Prefab = Resources.Load("GameObjects/PlinkettNest") as GameObject
        };
        knowledgeBase.Add(PlinkettNest);

        KbIds.GobberNest = GetNextID();
        StructureSubject GobberNest = new StructureSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes = SubjectAttributesEnum.FeelsSoft
                    | SubjectAttributesEnum.SoundsQuiet
                },
            SubjectID = KbIds.GobberNest,
            Name = "Gobber Nest",
            Description = "A spot that looks comfortable for Gobbers.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            Recipe = new BuildRecipe(
                new List<InventoryItem>()
                {
                    new InventoryItem(KbIds.Branch, 3),
                    new InventoryItem(KbIds.Leaves, 5)
                }),
            InventorySize = 10,
            Prefab = null
        };
        knowledgeBase.Add(GobberNest);

        KbIds.Hole10 = GetNextID();
        StructureSubject Hole10 = new StructureSubject()
        {
            Attributes =
                new SubjectAttributes()
                {
                    Attributes =
                    SubjectAttributesEnum.LooksDark
                    | SubjectAttributesEnum.SoundsQuiet
                    | SubjectAttributesEnum.FeelsHard
                },
            SubjectID = KbIds.Hole10,
            Name = "Hole",
            Description = "It looks large enough to contain several things.",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],

            Recipe = new BuildRecipe(
                new List<InventoryItem>()
                {
                    new InventoryItem(KbIds.Branch, 3),
                }),
            InventorySize = 10,
            Prefab = Resources.Load("GameObjects/Hole10") as GameObject
        };
        knowledgeBase.Add(Hole10);

        KbIds.Location = GetNextID();
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
            SubjectID = KbIds.Location,
            Prefab = Resources.Load("GameObjects/LocationMarker") as GameObject
        };
        knowledgeBase.Add(NewLocationOne);

        KbIds.Bush = GetNextID();
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
            SubjectID = KbIds.Bush,
            Name = "Bush",
            Description = "A Berry Bush",
            Icon = new UnityEngine.Sprite(),
            RelatedSubjects = new int[0],
            Prefab = Resources.Load("GameObjects/Bush") as GameObject,

            ProduceID = KbIds.Berry,
            ProduceTime = 2,
            MaxGrowth = 30,
            GrowthTime = 1,
            MatureGrowth = 2,
            InventorySize = 3,
            LootIDs = new int[2] { KbIds.Leaves, KbIds.Branch }
        };
        knowledgeBase.Add(Bush);

        KbIds.Plinkett = GetNextID();
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
            Traits = new NpcCharacterTraits(NpcTraits.Herbivore | NpcTraits.NestMaker)
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
            InventorySize = 3,
            LootID = KbIds.Meat,
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Plinkett",
            Nest = new StructureSubject(GetSubject(KbIds.PlinkettNest) as StructureSubject),
            RelatedSubjects = new int[0],
            SubjectID = KbIds.Plinkett,
            Prefab = Resources.Load("GameObjects/Plinkett") as GameObject
        };
        knowledgeBase.Add(plinkett);

        KbIds.Gobber = GetNextID();
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
            InventorySize = 3,
            LootID = KbIds.Meat,
            MatureTime = 200,
            MaxGrowth = 400,
            Name = "Gobber",
            Nest = new StructureSubject(GetSubject(KbIds.GobberNest) as StructureSubject),
            RelatedSubjects = new int[0],
            SubjectID = KbIds.Gobber,
            Prefab = Resources.Load("GameObjects/Gobber") as GameObject
        };
        knowledgeBase.Add(gobber);

    }

    /// <summary>
    /// Is called to ensure static constructor fires at game launch.
    /// </summary>
    public static void Init() { }

    /// <summary>
    /// Get Subject using subjectID.
    /// </summary>
    /// <param name="subjectID">subjectID of Subject to find.</param>
    /// <param name="subjectType">Required type of subject.</param>
    /// <returns>Returns null if subject is not found or the type of the returned subject does not match subjectType.</returns>
    static public Subject GetSubject(int subjectID, Type subjectType = null)
    {
        if (knowledgeBase.Exists(o => o.SubjectID == subjectID))
        {
            Subject tempSubject = knowledgeBase.Find(o => o.SubjectID == subjectID);
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
    static public int GetNextID()
    {
        maxID += 1;

        return maxID;
    }

    static public bool AddSubject(Subject newSubject)
    {
        //if we don't have a valid subject or if the ID is in use return false
        if (newSubject == null) return false;
        if (knowledgeBase.Exists(o => o.SubjectID == newSubject.SubjectID)) return false;

        //otherwise we should be able to add
        knowledgeBase.Add(newSubject);
        return true;
    }
}
