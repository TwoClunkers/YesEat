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

    private NpcStatus status;
    private NpcDefinition definition;
    private NpcDriversList drivers;
    private List<GameObject> considerObjects;
    private AnimalObjectScript mob;
    private List<LocationSubject> unexploredLocations;
    private List<LocationSubject> reExploreLocations;
    private List<int> searchedObjects;
    private List<int> searchedLocations;
    private List<InventoryItem> neededItems;

    private float AiCoreTickCounter;
    private float AiTickRate;
    private float MetabolizeTickCounter;
    #endregion

    #region Constructors
    /// <summary>
    /// Create a generic NpcCharacter.
    /// </summary>
    /// <param name="ParentObjectScript">The in-game object that represents this NPC.</param>
    /// <param name="MasterSubjectListRef">A reference to the main MasterSubjectList.</param>
    public NpcCore(AnimalObjectScript ParentObjectScript)
    {
        mob = ParentObjectScript;
        health = 100;
        food = 100;
        safety = 100;
        definition = new NpcDefinition();
        status = new NpcStatus();
        drivers = new NpcDriversList();
        unexploredLocations = new List<LocationSubject>();
        subjectID = -1;
        searchedObjects = new List<int>();
        searchedLocations = new List<int>();
        reExploreLocations = new List<LocationSubject>();
        SetFoodPreference();
        AiTickRate = 0.5f;
        neededItems = new List<InventoryItem>();
    }

    /// <summary>
    /// Initialize a new NpcCharacter. 
    /// </summary>
    /// <param name="ParentObject">The in-game object that represents this NPC.</param>
    /// <param name="MasterSubjectListRef">A reference to the main MasterSubjectList.</param>
    /// <param name="BasedOnSubject">Subject's NpcDefinition will define the character's initial resource pools, thresholds for fulfilling basic needs, and memories.</param>
    public NpcCore(AnimalObjectScript ParentObjectScript, Subject BasedOnSubject)
    {
        if (BasedOnSubject is AnimalSubject)
        {
            mob = ParentObjectScript;
            AnimalSubject animalSubject = BasedOnSubject as AnimalSubject;
            definition = animalSubject.Definition;
            subjectID = animalSubject.SubjectID;
            health = definition.HealthMax;
            food = definition.FoodMax;
            safety = definition.SafetyHigh;
            status = new NpcStatus();
            drivers = new NpcDriversList();
            unexploredLocations = new List<LocationSubject>();
            searchedObjects = new List<int>();
            searchedLocations = new List<int>();
            reExploreLocations = new List<LocationSubject>();
            SetFoodPreference();
            AiTickRate = 0.5f;
            neededItems = new List<InventoryItem>();
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
            foodID = DbIds.Berry;
            foodSourceID = DbIds.Bush;
        }
        else if (definition.Traits.HasTrait(NpcTraits.Carnivore))
        {
            foodID = DbIds.Meat;
            foodSourceID = DbIds.Plinkett;
        }
    }

    /// <summary>
    /// Get a list of all known locations.
    /// </summary>
    /// <returns></returns>
    internal List<LocationSubject> GetAllKnownLocations(Vector3 sortByNearestTo = default(Vector3))
    {
        List<SubjectMemory> locationMemories = definition.Memories
                .FindAll(o => MasterSubjectList.GetSubject(o.SubjectID).GetType() == typeof(LocationSubject))
                .OrderBy(o => (o as LocationMemory).LastTimeSeen).ToList();
        List<LocationSubject> knownLocations = locationMemories
                .Select(o => (MasterSubjectList.GetSubject(o.SubjectID) as LocationSubject)).ToList();

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
    internal List<LocationSubject> GetObjectLocations(Subject SubjectToFind, Vector3 CurrentPosition, List<int> ExcludeLocationIDs = null)
    {
        List<LocationSubject> foundObjects = new List<LocationSubject>();
        foreach (SubjectMemory subMem in definition.Memories)
        {
            if (subMem.GetType() == typeof(LocationMemory))
            {
                LocationMemory locMem = subMem as LocationMemory;
                if (locMem.ObjectMemories.Count > 0)
                {
                    foreach (ObjectMemory objMem in locMem.ObjectMemories)
                    {
                        if (objMem.SubjectID == SubjectToFind.SubjectID)
                        {
                            if (ExcludeLocationIDs != null)
                            {
                                if (ExcludeLocationIDs.Contains(subMem.SubjectID)) continue;
                            }
                            foundObjects.Add(MasterSubjectList.GetSubject(locMem.SubjectID) as LocationSubject);
                        }
                    }
                }
            }
        }
        // sort by distance
        foundObjects = foundObjects.OrderBy(o => Vector3.Distance(o.Coordinates, CurrentPosition)).ToList();

        return foundObjects;
    }

    /// <summary>
    /// Get the quantity of known LocationSubjects.
    /// </summary>
    /// <returns>Quantity found.</returns>
    private int GetKnownLocationCount()
    {
        return definition.Memories.Count(o => MasterSubjectList.GetSubject(o.SubjectID).GetType() == typeof(LocationSubject));
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
        {
            foreach (ObjectMemory objMem in definition.Nest.ObjectMemories)
            {
                if (objMem.SubjectID == Subject.Nest.SubjectID)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Find a the nearest safe location.
    /// </summary>
    /// <param name="masterSubjectList">Reference to the MasterSubjectList</param>
    /// <param name="definition">The NPC to check</param>
    /// <param name="objectScript">The GameObject's script.</param>
    /// <returns>The nearest safe location. Returns NULL if no safe locations are found.</returns>
    internal LocationSubject GetSafeLocation(AnimalObjectScript objectScript)
    {
        // get a list of all safe locations we remember
        List<SubjectMemory> foundLocations = definition.Memories
                                            .FindAll(o => (o.Safety > 0))
                                            .OrderBy(o => Vector3.Distance((MasterSubjectList.GetSubject(o.SubjectID) as LocationSubject).Coordinates,
                                                objectScript.gameObject.transform.position)).ToList();
        if (foundLocations.Count > 0)
            return MasterSubjectList.GetSubject(foundLocations[0].SubjectID) as LocationSubject;
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
            // dangerously low on health if starving find food, else find safety
            if (food <= 0)
                drivers.SetTopDriver(NpcDrivers.Hunger);
            else
                drivers.SetTopDriver(NpcDrivers.Safety);
        }

        // of the basic needs safety is the highest priority
        if (safety <= definition.SafetyDeadly)
        {
            status.UnsetState(NpcStates.Fighting);
            drivers.SetTopDriver(NpcDrivers.Safety);
        }
        else
        {
            // if safety is above deadly remove safety as priority
            drivers.Remove(NpcDrivers.Safety);
        }
    }

    /// <summary>
    /// Character has died, set Dead status and clear out all drivers.
    /// </summary>
    internal void Die()
    {
        drivers.Clear();
        status.SetState(NpcStates.Dead);
        mob.SetDeathDecay(20.0f);
    }

    #endregion

    #region Public Methods

    #region Expose read only values
    public float RangeSightFar { get { return definition.RangeSightFar; } }
    public float RangeSightMid { get { return definition.RangeSightMid; } }
    public float RangeSightNear { get { return definition.RangeSightNear; } }
    public float RangeActionClose { get { return definition.RangeActionClose; } }
    public float MoveSpeed { get { return definition.MoveSpeed; } }
    public int Health { get { return health; } }
    public int Food { get { return food; } }
    public int Safety { get { return safety; } }
    public NpcDrivers GetDriver() { return drivers[0]; }
    #endregion

    /// <summary>
    /// Add a Location to the searchedLocations list.
    /// </summary>
    /// <param name="searchedLocation">The LocationSubject's SubjectID to add.</param>
    public void AddSearchedLocation(int searchedLocationId)
    {
        if (!searchedLocations.Contains(searchedLocationId))
        {
            searchedLocations.Add(searchedLocationId);
        }
    }

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
    public AnimalSubject Subject { get { return MasterSubjectList.GetSubject(subjectID, typeof(AnimalSubject)) as AnimalSubject; } }

    public void Update()
    {
        if (!IsDead)
        {
            MetabolizeTickCounter += Time.deltaTime;
            if (MetabolizeTickCounter >= Definition.MetabolizeInterval)
            {
                Metabolize();
                MetabolizeTickCounter -= Definition.MetabolizeInterval;
            }

            AiCoreTickCounter += Time.deltaTime;
            if (AiCoreTickCounter > AiTickRate)
            {
                AiCoreProcess();
                AiCoreTickCounter -= AiTickRate;
            }

            // ===  Movement ===
            mob.DoMovement();
        }
    }

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
        considerObjects = mob.Observe();
        unexploredLocations = mob.GetObservedLocations()
            .FindAll(o => !definition.Memories.Exists(x => x.SubjectID == o.SubjectID))
            .OrderBy(o => Vector3.Distance(mob.transform.position, o.Coordinates)).ToList();

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
        // increase safety if no danger was found near us
        if (!dangerFound)
        {
            if (safety < 0) safety++;
        }

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

    private void AiExplore()
    {
        // explore unknown locations
        if (unexploredLocations.Count > 0)
        {
            mob.MoveToLocation(unexploredLocations[0]);
        }
        else
        {
            // explore known locations for new stuff
            // populate locations to re-explore
            if (reExploreLocations.Count == 0)
            {
                reExploreLocations = GetAllKnownLocations();
            }
            mob.MoveToLocation(reExploreLocations[0]);
        }
    }

    private void AiHunger()
    {
        // Get list of food in inventory
        InventoryItem foodItem = mob.Inventory.Take(new InventoryItem(foodID, 1));
        if (foodItem.Quantity > 0)
        {
            // eat food in inventory
            Eat(foodItem);
        }
        else
        {
            // get list of all foodSource objects in near range
            // exclude objects we've already attempted harvesting from
            List<SubjectObjectScript> foodSource = considerObjects
                .Select(o => o.GetComponent<SubjectObjectScript>() as SubjectObjectScript)
                .Where(o => o.Subject.SubjectID == foodSourceID)
                .Where(o => !searchedObjects.Contains(o.GetInstanceID())).ToList();

            // go to the first food source and harvest from it
            if (foodSource.Count > 0)
            {
                SubjectObjectScript foodSourceObject = foodSource[0];

                // if it's within harvest range
                if (Vector3.Distance(foodSourceObject.transform.position, mob.transform.position) <= definition.RangeActionClose)
                {
                    // we're within range, stop chasing
                    //mob.ChaseStop();

                    //  Attempt to harvest from the food source
                    InventoryItem harvestedItem = foodSourceObject.Harvest();
                    if (harvestedItem != null)
                    {
                        if (harvestedItem.Quantity > 0)
                        {
                            mob.Inventory.Add(harvestedItem);
                        }
                        else
                        {
                            // didn't get anything from this source
                            searchedObjects.Add(foodSourceObject.GetInstanceID());
                            mob.ChaseStop();
                        }
                    }
                    else // null means this is an animal that isn't dead, attack it.
                    {
                        AnimalObjectScript animal = foodSourceObject as AnimalObjectScript;
                        animal.Damage(Subject, definition.AttackDamage, this);
                    }
                }
                else //out of harvest range, chase this food source
                {
                    // if the current target is still within view don't switch targets
                    if (mob.GetChaseTarget() == null)
                    {
                        mob.ChaseStart(foodSourceObject);
                    }
                    else
                    {
                        if (Vector3.Distance(mob.GetChaseTarget().transform.position, mob.transform.position) > definition.RangeSightMid)
                        {
                            mob.ChaseStart(foodSourceObject);
                        }
                    }
                }
            }
            else
            {
                // if we've searched all the locations we known of, start over.
                if (GetKnownLocationCount() == searchedLocations.Count && unexploredLocations.Count == 0)
                {
                    searchedLocations.Clear();
                    searchedObjects.Clear();
                }
                // there are no food sources in close range
                // find a location with foodSourceID
                List<LocationSubject> foodLocations =
                    GetObjectLocations(MasterSubjectList.GetSubject(foodSourceID), mob.transform.position, searchedLocations);

                if (foodLocations.Count > 0)
                {
                    mob.MoveToLocation(foodLocations[0]);
                }
                else
                {
                    AiExplore();
                }

            }
        }
    }

    private void AiNest()
    {
        if (definition.Nest != null)
        {
            SpecialObjectMemory holeMemory = definition.Nest.ObjectMemories.Find(o => o.SubjectID == DbIds.Hole10) as SpecialObjectMemory;

            // we are currently in the nearest safe location and it is saved
            if (holeMemory != null)
            {
                // do we already have neededIngredients?
                if (neededItems.Count == 0)
                {
                    // we don't know that we need ingredients, go to hole
                    if (Vector3.Distance(mob.transform.position, holeMemory.Position) > definition.RangeActionClose)
                    {
                        //not next to hole, move to it
                        mob.MoveToPosition(holeMemory.Position);
                    }
                    else
                    {
                        // we are next to our hole, get a reference to it
                        StructureObjectScript holeObject = considerObjects
                            .Find(o => o.GetComponent<SubjectObjectScript>().Subject.SubjectID == Subject.Nest.SubjectID)
                            .GetComponent<StructureObjectScript>();

                        //do we have all items in hole for building nest?
                        List<InventoryItem> nestNeeds = new List<InventoryItem>();
                        foreach (InventoryItem ingredient in Subject.Nest.Recipe.Ingredients)
                        {
                            int qtyNeeded = ingredient.Quantity - holeObject.Inventory.Count(ingredient.SubjectID);
                            if (qtyNeeded > 0)
                                nestNeeds.Add(new InventoryItem(ingredient.SubjectID, qtyNeeded));
                        }
                        if (nestNeeds.Count == 0)
                        {
                            // everything needed to build nest is already in the hole
                            // TODO:    build nest using items in hole
                            //          instantiate nest and transfer any hole contents to it
                            //          remove Nest driver & save nest as SpecialObjectMemory
                        }
                        else
                        {
                            // do we have any needed ingredients for nest with us?
                            for (int i = 0; i < nestNeeds.Count; i++)
                            {
                                if (mob.Inventory.Count(nestNeeds[i].SubjectID) > 0)
                                {
                                    // inventory does have this ingredient, deposit in hole
                                    InventoryItem taken = mob.Inventory.Take(new InventoryItem(nestNeeds[i]));
                                    // put back anything that didn't fit
                                    nestNeeds[i].Quantity -= taken.Quantity;
                                    InventoryItem leftoverItems = holeObject.Inventory.Add(taken);
                                    if (leftoverItems.Quantity > 0)
                                    {
                                        nestNeeds[i].Quantity += leftoverItems.Quantity;
                                        mob.Inventory.Add(leftoverItems);
                                    }
                                }
                            }
                            neededItems.Clear();
                            foreach (InventoryItem ingredient in nestNeeds)
                            {
                                if (ingredient.Quantity > 0)
                                    neededItems.Add(new InventoryItem(ingredient));
                            }
                        }
                    }
                }
                else
                {
                    // neededIngredients has ingredients, go collect them.
                    ExploreForItem(DbIds.Bush, neededItems[0].SubjectID);
                }
            }
            else
            {
                // no hole made yet, dig hole if we are at Nest location
                if (mob.Location.SubjectID == definition.Nest.SubjectID)
                {
                    SpecialObjectMemory newHoleMemory =
                        new SpecialObjectMemory()
                        {
                            Position = mob.DigHole().transform.position,
                            Quantity = 1,
                            SubjectID = DbIds.Hole10
                        };
                    definition.Nest.ObjectMemories.Add(newHoleMemory);
                }
            }
        }
        else
        {
            // find nearest safe location
            LocationSubject safeLocation = GetSafeLocation(mob);
            if (safeLocation != null)
            {
                // a safe location is in memory, are we already there?
                if (safeLocation == mob.Location)
                {
                    // If not already saved, save this location as Nest location
                    if (definition.Nest == null)
                    {
                        definition.Nest = new LocationMemory(definition.Memories.Find(o => o.SubjectID == safeLocation.SubjectID) as LocationMemory);
                    }
                }
                else
                {
                    // go to the nearest safe location
                    mob.MoveToLocation(safeLocation);
                }
            }
            else
            {
                // do not know of any safe locations, explore.
                AiExplore();
            }
        }

    }

    private void ExploreForItem(int sourceSubjectId, int itemSubjectId)
    {
        // TODO:    search memories for sourceSubjectId
        //          travel to source subject locations
        //          harvest itemSubjectId from all seen sources while travelling

        // TODO: make sure found items are removed from neededIngredients list
    }

    private void AiSafety()
    {
        // If safety is the top priority, combat is an unsafe action.
        // Stop combat, run for your life.
        status.UnsetState(NpcStates.Fighting);
        // if we have a nest assume it is safe and go there first
        if (HaveNest())
        {
            // if not at nest move there first
            if (mob.Location.SubjectID != definition.Nest.LocationSubjectID)
            {
                mob.MoveToLocation(MasterSubjectList.GetSubject(definition.Nest.SubjectID) as LocationSubject);
                return;
            }
        }

        // don't have nest or we're at nest and it isn't safe. move to safe location
        LocationSubject safeLocation = GetSafeLocation(mob);
        if (safeLocation != null)
        {
            mob.MoveToLocation(safeLocation);
        }
        else
        {
            AiExplore();
        }
    }

    /// <summary>
    /// Add objectToInspect to the NPC's Memories.
    /// </summary>
    /// <param name="objectToInspect">The GameObject to learn about.</param>
    internal void Inspect(GameObject objectToInspect)
    {
        // inspect the object, add to memories.
        SubjectObjectScript inspectObjectScript = objectToInspect.GetComponent<SubjectObjectScript>();
        if (inspectObjectScript.GetType() == typeof(LocationObjectScript))
        {
            LocationObjectScript locObjScript = inspectObjectScript as LocationObjectScript;
            //only add location to memory if all waypoints are explored
            if (mob.IsCurrentLocationExplored)
            {
                // if it's in the unexploredLocations list, remove it.
                unexploredLocations.Remove(locObjScript.Subject as LocationSubject);
                // if it's in the reExploreLocations list, remove it.
                reExploreLocations.Remove(locObjScript.Subject as LocationSubject);
            }
        }
        else
        {
            inspectObjectScript.Subject.TeachNpc(this);
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
                FoodSubject foodSubject = MasterSubjectList.GetSubject(FoodItem.SubjectID, typeof(FoodSubject)) as FoodSubject;
                if (foodSubject != null)
                {
                    food += foodSubject.FoodValue;
                    food = System.Math.Min(food, definition.FoodMax);

                    // heal a little when we eat.
                    health += definition.HealthRegen * 2;
                    health = Mathf.Min(health, definition.HealthMax);

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
    public bool IsDead { get { return status.IsDead; } }

    #endregion
}
