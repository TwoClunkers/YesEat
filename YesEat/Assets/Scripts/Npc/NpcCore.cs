using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core character AI.
/// </summary>
public partial class NpcCore
{
    #region Private member declarations
    private int health;
    private int food;
    private int safety;

    private int subjectID;

    private int foodSourceID;
    private int foodID;

    private MasterSubjectList db;
    private NpcStatus status;
    private NpcDefinition definition;
    private NpcDriversList drivers;
    private List<GameObject> considerObjects;
    private AnimalObjectScript objectScript;
    private List<NpcCore> combatTargets;
    private List<LocationSubject> unexploredLocations;
    private List<LocationSubject> reExploreLocations;
    private List<int> searchedObjects;
    private List<int> searchedLocations;
    private float lastFoodSearch;
    #endregion

    #region Constructors
    /// <summary>
    /// Create a generic NpcCharacter.
    /// </summary>
    /// <param name="ParentObjectScript">The in-game object that represents this NPC.</param>
    /// <param name="MasterSubjectListRef">A reference to the main MasterSubjectList.</param>
    public NpcCore(AnimalObjectScript ParentObjectScript, MasterSubjectList MasterSubjectListRef)
    {
        db = MasterSubjectListRef;
        objectScript = ParentObjectScript;
        health = 100;
        food = 100;
        safety = 100;
        definition = new NpcDefinition();
        status = new NpcStatus();
        drivers = new NpcDriversList();
        combatTargets = new List<NpcCore>();
        unexploredLocations = new List<LocationSubject>();
        subjectID = -1;
        searchedObjects = new List<int>();
        searchedLocations = new List<int>();
        reExploreLocations = new List<LocationSubject>();
        SetFoodPreference();
    }

    /// <summary>
    /// Initialize a new NpcCharacter. 
    /// </summary>
    /// <param name="ParentObject">The in-game object that represents this NPC.</param>
    /// <param name="MasterSubjectListRef">A reference to the main MasterSubjectList.</param>
    /// <param name="BasedOnSubject">Subject's NpcDefinition will define the character's initial resource pools, thresholds for fulfilling basic needs, and memories.</param>
    public NpcCore(AnimalObjectScript ParentObjectScript, MasterSubjectList MasterSubjectListRef, Subject BasedOnSubject)
    {
        db = MasterSubjectListRef;
        if (BasedOnSubject is AnimalSubject)
        {
            objectScript = ParentObjectScript;
            AnimalSubject animalSubject = BasedOnSubject as AnimalSubject;
            definition = animalSubject.Definition;
            subjectID = animalSubject.SubjectID;
            health = definition.HealthMax;
            food = definition.FoodMax;
            safety = definition.SafetyHigh;
            status = new NpcStatus();
            drivers = new NpcDriversList();
            combatTargets = new List<NpcCore>();
            unexploredLocations = new List<LocationSubject>();
            searchedObjects = new List<int>();
            searchedLocations = new List<int>();
            reExploreLocations = new List<LocationSubject>();
            SetFoodPreference();
        }
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Set this NPC's food source and food based on NpcTraits.
    /// </summary>
    internal void SetFoodPreference()
    {
        if (definition.Traits.HasTrait(NpcTraits.Herbivore))
        {
            foodID = 4; // berry
            foodSourceID = 3; // bush
        }
        else if (definition.Traits.HasTrait(NpcTraits.Carnivore))
        {
            foodID = 5; // meat
            foodSourceID = 1; // plinkett
        }
    }

    /// <summary>
    /// Get a list of all known locations.
    /// </summary>
    /// <returns></returns>
    internal List<LocationSubject> GetAllKnownLocations(Vector3 sortByNearestTo = default(Vector3))
    {
        List<SubjectMemory> locationMemories = definition.Memories
                .FindAll(o => db.GetSubject(o.SubjectID).GetType() == typeof(LocationSubject));
        List<LocationSubject> knownLocations = locationMemories
                .Select(o => (db.GetSubject(o.SubjectID) as LocationSubject)).ToList();
        if (sortByNearestTo != default(Vector3))
        {
            knownLocations = knownLocations.OrderBy(o => Vector3.Distance(sortByNearestTo, o.Coordinates)).ToList();
        }
        return knownLocations;
    }

    /// <summary>
    /// Find the nearest location where subjectToFind can be found.
    /// </summary>
    /// <param name="SubjectToFind">The subject to search for.</param>
    /// <param name="CurrentPosition">The current position of this game object.</param>
    /// <param name="ExcludeLocationIDs">The locations to exclude from the results.</param>
    /// <returns>The found locations or null if no locations were found.</returns>
    internal List<LocationSubject> FindObject(Subject SubjectToFind, Vector3 CurrentPosition, List<int> ExcludeLocationIDs = null)
    {
        List<LocationSubject> foundObjects;
        if (ExcludeLocationIDs == null)
        {
            // Find nearest location where subjectToFind can be found.
            List<SubjectMemory> locMems = definition.Memories.FindAll(o => o.GetType() == typeof(LocationMemory)).ToList();
            locMems = locMems.FindAll(o => (o as LocationMemory).ObjectMemories.Exists(x => x.SubjectID == SubjectToFind.SubjectID));
            locMems = locMems.OrderBy(o => Vector3.Distance(CurrentPosition, (db.GetSubject(o.SubjectID) as LocationSubject).Coordinates)).ToList();
            foundObjects = locMems.Select(o => db.GetSubject(o.SubjectID) as LocationSubject).ToList();
        }
        else
        {
            List<SubjectMemory> locMems = definition.Memories.FindAll(o => o.GetType() == typeof(LocationMemory)).ToList();
            locMems = locMems.FindAll(o => (o as LocationMemory).ObjectMemories.Exists(x => x.SubjectID == SubjectToFind.SubjectID));
            locMems = locMems.OrderBy(o => Vector3.Distance(CurrentPosition, (db.GetSubject(o.SubjectID) as LocationSubject).Coordinates)).ToList();
            foundObjects = locMems.Select(o => db.GetSubject(o.SubjectID) as LocationSubject).ToList();
            // exclude
            foundObjects = foundObjects.FindAll(o => !ExcludeLocationIDs.Contains(o.SubjectID));
        }
        return foundObjects;
    }

    /// <summary>
    /// Checks the attitude of the NPC towards conSubject. IsSubjectKnown() must be used to verify the Subject is known before IsSubjectDangerous().
    /// </summary>
    /// <param name="conSubject">The Subject to be considered.</param>
    /// <returns>True = dangerous; False = not dangerous. If conSubject does not exist and Exception will be thrown.</returns>
    internal bool IsSubjectDangerous(Subject conSubject)
    {
        if (IsSubjectKnown(conSubject))
        {
            SubjectMemory subjectAttitude = definition.Memories.Find(o => o.SubjectID == conSubject.SubjectID);
            return (subjectAttitude.Safety < 0);
        }
        else
        {
            throw new Exception("The queried Subject is not in the memories list.");
        }
    }

    /// <summary>
    /// Checks if conSubject exists in the npcDefinition.
    /// </summary>
    /// <param name="conSubject">The Subject to be considered.</param>
    /// <returns>True: known. False: not known.</returns>
    internal bool IsSubjectKnown(Subject conSubject)
    {
        if (conSubject != null)
            return definition.Memories.Exists(o => o.SubjectID == conSubject.SubjectID);
        else
            return false;
    }

    /// <summary>
    /// Change attitude about a subject based on an event.
    /// </summary>
    /// <param name="memoryChangeEvent">The event that has occured.</param>
    /// <param name="definition">The NPC to effect.</param>
    /// <param name="subject">The subject to adjust attitude towards.</param>
    internal void UpdateMemory(NpcMemoryChangeEvent memoryChangeEvent, Subject subject)
    {
        switch (memoryChangeEvent)
        {
            case NpcMemoryChangeEvent.HealthDamage:
                if (IsSubjectKnown(subject))
                {
                    //known hurts, bad.
                    definition.Memories.Find(o => o.SubjectID == subject.SubjectID).AddSafety(-1);
                }
                else
                {
                    //new thing hurts me, bad.
                    definition.Memories.Add(new SubjectMemory(subject.SubjectID, -1, 0));
                }
                break;
            case NpcMemoryChangeEvent.FoodEaten:
                if (IsSubjectKnown(subject))
                {
                    //known food, good.
                    definition.Memories.Find(o => o.SubjectID == subject.SubjectID).AddFood(1);
                }
                else
                {
                    //new food, good.
                    definition.Memories.Add(new SubjectMemory(subject.SubjectID, 1, 1));
                }
                break;
            case NpcMemoryChangeEvent.LocationFound:
                // look at everything in this location and decide how to effect attitude for this location.
                if (IsSubjectKnown(subject))
                {
                    // known location
                    LocationMemory locationMemory = definition.Memories.Find(o => o.SubjectID == subject.SubjectID) as LocationMemory;
                    int foodValue = 0;
                    int safetyValue = 0;
                    foreach (ObjectMemory objMem in locationMemory.ObjectMemories)
                    {
                        if (definition.Memories.Find(o => o.SubjectID == objMem.SubjectID).Food > 0)
                            foodValue += objMem.Quantity;
                        if (definition.Memories.Find(o => o.SubjectID == objMem.SubjectID).Safety > 0)
                            safetyValue += objMem.Quantity;
                    }
                    locationMemory.SetValues((sbyte)foodValue, (sbyte)safetyValue);
                }
                else
                {
                    //unknown location
                    definition.Memories.Add(new SubjectMemory(subject.SubjectID, 0, 0));
                }
                break;
            default:
                throw new Exception("Invalid NpcAttitudeChangeEvent");
        }
    }

    /// <summary>
    /// Consider whether I should attack another NPC.
    /// </summary>
    /// <param name="npcToConsider">The NPC to consider attacking.</param>
    /// <param name="damageAmount">The amount of damage we received from the NPC if applicable.</param>
    /// <returns>True = attack, False = do not attack.</returns>
    internal bool ShouldFight(NpcCore npcToConsider, int? damageAmount = null)
    {
        bool shouldAttack = false;
        if (damageAmount != null)
        {
            //we were attacked
            //TODO: assess whether I should attack or not
        }
        else
        {
            //haven't been attacked by them
            //TODO: assess whether I should attack or not
        }
        return shouldAttack;
    }

    /// <summary>
    /// Check if the NPC has a nest.
    /// </summary>
    /// <returns>True: have nest, False: do not have nest</returns>
    internal bool HaveNest()
    {
        if (definition.Nest != null)
            return (definition.Nest.ObjectMemories.Count > 0);
        else
            return false;
    }

    /// <summary>
    /// Find a the nearest safe location.
    /// </summary>
    /// <param name="masterSubjectList">Reference to the MasterSubjectList</param>
    /// <param name="definition">The NPC to check</param>
    /// <param name="objectScript">The GameObject's script.</param>
    /// <returns>The nearest safe location. Returns NULL if no safe locations are found.</returns>
    internal LocationSubject FindSafeLocation(AnimalObjectScript objectScript)
    {
        // get a list of all safe locations we remember
        List<SubjectMemory> foundLocations = definition.Memories
                                            .FindAll(o => (o.Safety > 0))
                                            .OrderBy(o => Vector3.Distance((db.GetSubject(o.SubjectID) as LocationSubject).Coordinates,
                                                objectScript.gameObject.transform.position)).ToList();
        if (foundLocations.Count > 0)
            return db.GetSubject(foundLocations[0].SubjectID) as LocationSubject;
        else
            return null;
    }

    /// <summary>
    /// Update drivers based on current values.
    /// </summary>
    internal void UpdateDrivers()
    {
        // Default driver: Explore.
        if (drivers.Count == 0) drivers.Add(NpcDrivers.Explore);

        if (definition.Traits.IsNestMaker)
        {
            // Are we fed, healthy, & safe?
            if (food > definition.FoodHungry && health > definition.HealthDanger && safety > definition.SafetyDeadly)
            {
                // if we do not have a nest, make one.
                if (!HaveNest())
                {
                    drivers.SetTopDriver(NpcDrivers.Nest);
                }
            }
        }

        // of the basic needs hunger is lowest priority
        if (food <= definition.FoodHungry)
        {
            drivers.SetTopDriver(NpcDrivers.Hunger);
        }

        // of the basic needs health is 2nd lowest priority
        if (health <= 0)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            // dangerously low on health, safety is now max priority
            drivers.SetTopDriver(NpcDrivers.Safety);
        }

        // of the basic needs safety is the highest priority
        if (safety <= definition.SafetyDeadly)
        {
            status.UnsetState(NpcStates.Fighting);
            drivers.SetTopDriver(NpcDrivers.Safety);
        }
    }

    /// <summary>
    /// Character has died, set Dead status and clear out all drivers.
    /// </summary>
    internal void Die()
    {
        status.SetState(NpcStates.Dead);
        drivers.Clear();
    }

    #endregion

    #region Public Methods

    #region Expose Definition values as read only
    public float SightRangeFar { get { return definition.SightRangeFar; } }
    public float SightRangeNear { get { return definition.SightRangeNear; } }
    public float MoveSpeed { get { return definition.MoveSpeed; } }
    #endregion

    /// <summary>
    /// This NPC's Definition.
    /// </summary>
    public NpcDefinition Definition { get { return definition; } }

    /// <summary>
    /// This NPC's associated subject.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

    /// <summary>
    /// This NPC's subject.
    /// </summary>
    public AnimalSubject Subject { get { return db.GetSubject(subjectID, typeof(AnimalSubject)) as AnimalSubject; } }

    /// <summary>
    /// Reduce food. Reduce health if starving. Regenerate health if not starving.
    /// </summary>
    public int Metabolize()
    {
        if (IsDead) return 0;
        int preHealth = health;
        if (food == 0)
        {
            health -= definition.StarvingDamage;
            if (health <= 0)
                Die();
        }
        else
        {
            if (health < definition.HealthMax)
            {
                health += definition.HealthRegen;
            }
            food -= definition.FoodMetabolizeRate;
        }
        return health - preHealth;
    }

    /// <summary>
    /// The main AI process.
    /// </summary>
    public void AiCoreProcess()
    {
        considerObjects = objectScript.Observe();
        unexploredLocations = objectScript.GetObservedLocations();
        unexploredLocations = unexploredLocations
            .FindAll(o => !definition.Memories.Exists(x => x.SubjectID == o.SubjectID));
        unexploredLocations = unexploredLocations
            .OrderBy(o => Vector3.Distance(objectScript.transform.position, o.Coordinates)).ToList();

        // Consider each subject starting with the closest.
        bool dangerFound = false;
        foreach (GameObject conObject in considerObjects)
        {
            if (IsSubjectKnown(conObject.GetComponent<SubjectObjectScript>().Subject))
            {
                if (IsSubjectDangerous(conObject.GetComponent<SubjectObjectScript>().Subject))
                {
                    // don't reduce safety if it's dead
                    if (conObject.GetComponent<SubjectObjectScript>().GetType() == typeof(AnimalObjectScript))
                    {
                        if (conObject.GetComponent<AnimalObjectScript>().IsDead) continue;
                    }
                    // danger! decrease safety
                    dangerFound = true;
                    safety--;
                }
            }
            else
            {
                Inspect(conObject);
            }
        }
        // reset safety if no danger was found near us
        if (!dangerFound) safety = 0;

        UpdateDrivers();

        // Act on max priority driver
        switch (drivers[0])
        {
            case NpcDrivers.Nest:
                AiNest();
                break;
            case NpcDrivers.Safety:
                AiSafety();
                break;
            case NpcDrivers.Hunger:
                AiHunger();
                break;
            case NpcDrivers.Explore:
                AiExplore();
                break;
            default:
                //There are no critical drivers, default to exploration
                drivers.Clear();
                drivers.Add(NpcDrivers.Explore);
                break;
        }

    }

    /// <summary>
    /// Add objectToInspect to the NPC's Memories.
    /// </summary>
    /// <param name="objectToInspect">The GameObject to learn about.</param>
    internal void Inspect(GameObject objectToInspect)
    {
        // inspect the object, add to memories.
        SubjectObjectScript objectScript = objectToInspect.GetComponent<SubjectObjectScript>();
        if (objectScript.GetType() == typeof(LocationObjectScript))
        {
            (objectScript as LocationObjectScript).TeachNpc(this);
            // if it's in the unexploredLocations list, remove it.
            unexploredLocations.Remove(objectScript.Subject as LocationSubject);
            // if it's in the reExploreLocations list, remove it.
            reExploreLocations.Remove(objectScript.Subject as LocationSubject);
        }
        else
        {
            objectScript.Subject.TeachNpc(this);
        }
    }

    /// <summary>
    /// Modify status by eating something.
    /// </summary>
    /// <param name="FoodItem">The food item to be eaten.</param>
    /// <returns>True: Item consumed. False: Item not consumed.</returns>
    public bool Eat(InventoryItem FoodItem)
    {
        if (this.IsDead) return false;
        bool wasConsumed = false;
        //check flags to make sure we're able to eat
        if (status.CanEat())
        {
            if (food < definition.FoodMax)
            {
                status.SetState(NpcStates.Eating); //set eating flag
                FoodSubject foodSubject = db.GetSubject(FoodItem.SubjectID, typeof(FoodSubject)) as FoodSubject;
                if (foodSubject != null)
                {
                    food += foodSubject.FoodValue;
                    food = System.Math.Min(food, definition.FoodMax);
                    wasConsumed = true;
                }
                status.UnsetState(NpcStates.Eating); //unset eating flag
            }

            if (food >= definition.FoodMax)
                drivers.Remove(NpcDrivers.Hunger);
        }
        return wasConsumed;
    }

    /// <summary>
    /// Inflict DamageAmount of health damage to character.
    /// </summary>
    /// <param name="subjectAttacker">Source of damage.</param>
    /// <param name="damageAmount">Amount of damage to inflict.</param>
    /// <param name="NpcAttacker">Attacking NPC if applicable.</param>
    /// <returns>True: Character was killed by the damage. False: still alive.</returns>
    public bool Damage(Subject subjectAttacker, int damageAmount, NpcCore NpcAttacker = null)
    {
        if (IsDead) return false; //cannot damage because it is already dead

        health -= damageAmount;
        health = Math.Max(health, 0);

        if (health < 1)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            // decrease safety by percentage of max health that damage received was
            safety -= (damageAmount / definition.HealthMax) * 100;
        }

        UpdateMemory(NpcMemoryChangeEvent.HealthDamage, subjectAttacker);


        if (NpcAttacker != null)
        {
            if (ShouldFight(NpcAttacker, damageAmount))
            {
                combatTargets.Add(NpcAttacker);
                status.SetState(NpcStates.Fighting);
                //TODO: add a subroutine that will trigger a fight action if fighting state is set.
            }
            else
            {
                // set safety minimum to flee
                drivers.SetTopDriver(NpcDrivers.Safety);
            }
        }

        return IsDead;
    }

    /// <summary>
    /// True = Dead. False = Not dead.
    /// </summary>
    public bool IsDead { get { return status.IsStateSet(NpcStates.Dead); } }

    public int Health { get { return health; } }
    public int Food { get { return food; } }
    public int Safety { get { return safety; } }
    #endregion
}
