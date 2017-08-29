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
        AiTickRate = 0.5f;
        neededItems = new List<InventoryItem>();

        InitializeMemories();
    }

    /// <summary>
    /// Initialize a new NpcCharacter. 
    /// </summary>
    /// <param name="ParentObject">The in-game object that represents this NPC.</param>
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
            AiTickRate = 0.5f;
            neededItems = new List<InventoryItem>();

            InitializeMemories();
        }
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Set this NPC's food source and food based on NpcTraits.
    /// </summary>
    internal void InitializeMemories()
    {
        // remember the food we like
        if (definition.Traits.HasTrait(NpcTraits.Herbivore))
        {
            foodID = KbIds.Berry;
        }
        else if (definition.Traits.HasTrait(NpcTraits.Carnivore))
        {
            foodID = KbIds.Meat;
        }
        KnowledgeBase.GetSubject(foodID).TeachNpc(this);

        // remember how to build our nest
        if (Subject.Nest != null && Subject.Nest.Recipe != null && Subject.Nest.Recipe.Ingredients.Count > 0)
        {
            foreach (int ingredientId in Subject.Nest.Recipe.Ingredients.Select(o => o.SubjectID))
            {
                KnowledgeBase.GetSubject(ingredientId).TeachNpc(this);
            }
        }
    }

    /// <summary>
    /// Get a list of all known locations.
    /// </summary>
    /// <returns></returns>
    internal List<LocationSubject> GetAllKnownLocations(Vector3 sortByNearestTo = default(Vector3))
    {
        List<SubjectMemory> locationMemories = definition.Memories
                .FindAll(o => KnowledgeBase.GetSubject(o.SubjectID) is LocationSubject)
                .OrderBy(o => (o as LocationMemory).LastTimeSeen).ToList();
        List<LocationSubject> knownLocations = locationMemories
                .Select(o => (KnowledgeBase.GetSubject(o.SubjectID) as LocationSubject)).ToList();

        if (sortByNearestTo != default(Vector3))
        {
            knownLocations = knownLocations.OrderBy(o => Vector3.Distance(sortByNearestTo, o.Coordinates)).ToList();
        }
        return knownLocations;
    }

    /// <summary>
    /// Find the nearest location where subjectToFind can be found.
    /// </summary>
    /// <param name="SubjectToFindId">The subject to search for.</param>
    /// <param name="CurrentPosition">The current position of this game object.</param>
    /// <param name="ExcludeLocationIDs">The locations to exclude from the results.</param>
    /// <returns>The found locations or null if no locations were found.</returns>
    internal List<LocationSubject> GetObjectLocations(int SubjectToFindId, Vector3 CurrentPosition, List<int> ExcludeLocationIDs = null)
    {
        List<LocationSubject> foundObjectLocations = new List<LocationSubject>();
        foreach (SubjectMemory subMem in definition.Memories)
        {
            if (subMem is LocationMemory)
            {
                LocationMemory locMem = subMem as LocationMemory;
                if (locMem.ObjectMemories.Count > 0)
                {
                    foreach (ObjectMemory objMem in locMem.ObjectMemories)
                    {
                        if (objMem.SubjectID == SubjectToFindId)
                        {
                            if (ExcludeLocationIDs != null)
                            {
                                if (ExcludeLocationIDs.Contains(subMem.SubjectID)) continue;
                            }
                            foundObjectLocations.Add(KnowledgeBase.GetSubject(locMem.SubjectID) as LocationSubject);
                        }
                    }
                }
            }
        }
        // sort by distance
        foundObjectLocations = foundObjectLocations.OrderBy(o => Vector3.Distance(o.Coordinates, CurrentPosition)).ToList();

        return foundObjectLocations;
    }

    /// <summary>
    /// Get the quantity of known LocationSubjects.
    /// </summary>
    /// <returns>Quantity found.</returns>
    private int GetKnownLocationCount()
    {
        return definition.Memories.Count(o => KnowledgeBase.GetSubject(o.SubjectID) is LocationSubject);
    }

    /// <summary>
    /// Checks the attitude of the NPC towards conSubject. IsSubjectKnown() must be used to verify the Subject is known before IsSubjectDangerous().
    /// </summary>
    /// <param name="checkSubjectID">The Subject to be considered.</param>
    /// <returns>True = dangerous; False = not dangerous. If conSubject does not exist and Exception will be thrown.</returns>
    internal bool IsSubjectDangerous(int checkSubjectID)
    {
        if (IsSubjectKnown(checkSubjectID))
        {
            SubjectMemory subjectAttitude = definition.Memories.Find(o => o.SubjectID == checkSubjectID);
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
    /// <param name="checkSubjectId">The Subject to be considered.</param>
    /// <returns>True: known. False: not known.</returns>
    internal bool IsSubjectKnown(int checkSubjectId)
    {
        if (checkSubjectId > 0)
            return definition.Memories.Exists(o => o.SubjectID == checkSubjectId);
        else
            return false;
    }

    /// <summary>
    /// Change attitude about a subject based on an event.
    /// </summary>
    /// <param name="memoryChangeEvent">The event that has occured.</param>
    /// <param name="definition">The NPC to effect.</param>
    /// <param name="subjectID">The subject to adjust attitude towards.</param>
    internal void UpdateMemory(NpcMemoryChangeEvent memoryChangeEvent, int subjectID, object[] args = null)
    {
        switch (memoryChangeEvent)
        {
            case NpcMemoryChangeEvent.HealthDamage:
                if (IsSubjectKnown(subjectID))
                {
                    //known hurts, bad.
                    definition.Memories.Find(o => o.SubjectID == subjectID).AddSafety(-1);
                }
                else
                {
                    //new thing hurts me, bad.
                    definition.Memories.Add(new SubjectMemory(subjectID, -1, 0));
                }
                break;
            case NpcMemoryChangeEvent.FoodEaten:
                if (IsSubjectKnown(subjectID))
                {
                    //known food, good.
                    definition.Memories.Find(o => o.SubjectID == subjectID).AddFood(1);
                }
                else
                {
                    //new food, good.
                    definition.Memories.Add(new SubjectMemory(subjectID, 1, 1));
                }
                break;
            case NpcMemoryChangeEvent.LocationFound:
                // look at everything in this location and decide how to effect attitude for this location.
                LocationMemory locationMemory;
                if (!IsSubjectKnown(subjectID))
                {
                    locationMemory = new LocationMemory(subjectID, 0, 0);
                    definition.Memories.Add(locationMemory);
                }

                locationMemory = definition.Memories.Find(o => o.SubjectID == subjectID) as LocationMemory;
                if (locationMemory != null)
                {
                    int foodValue = 0;
                    int safetyValue = 0;
                    if (locationMemory.ObjectMemories != null)
                    {
                        foreach (ObjectMemory objMem in locationMemory.ObjectMemories)
                        {
                            SubjectMemory subjectMemory = definition.Memories.Find(o => o.SubjectID == objMem.SubjectID);
                            if (subjectMemory.Food > 0)
                                foodValue += objMem.Quantity;
                            if (subjectMemory.Safety > 0)
                                safetyValue += objMem.Quantity;
                            else if (subjectMemory.Safety < 0)
                                safetyValue -= objMem.Quantity;
                        }
                    }
                    locationMemory.SetValues((sbyte)foodValue, (sbyte)safetyValue);
                }

                break;
            case NpcMemoryChangeEvent.ItemHarvested:
                if (!IsSubjectKnown(subjectID))
                {
                    definition.Memories.Add(new SubjectMemory(subjectID, 0, 0));
                }
                // if there is no argument it was a bad function call
                if (args == null)
                    throw new Exception(" UpdateMemory(ItemHarvested): args[] must contian the harvested item's source.");

                // remember what we got this item from
                SubjectMemory memory = definition.Memories.Find(o => o.SubjectID == subjectID);
                if (memory != null)
                {
                    memory.AddSource((int)args[0]);
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
    /// <param name="definition">The NPC to check</param>
    /// <param name="objectScript">The GameObject's script.</param>
    /// <returns>The nearest safe location. Returns NULL if no safe locations are found.</returns>
    internal LocationSubject GetSafeLocation(AnimalObjectScript objectScript)
    {
        List<LocationSubject> foundSafeLocations = new List<LocationSubject>();
        foreach (SubjectMemory subMem in definition.Memories)
        {
            if (subMem is LocationMemory)
            {
                LocationMemory locMem = subMem as LocationMemory;
                if (locMem.Safety > 0)
                {
                    foundSafeLocations.Add(KnowledgeBase.GetSubject(locMem.SubjectID) as LocationSubject);
                }
            }
        }
        if (foundSafeLocations.Count > 0)
        {
            // sort by distance
            foundSafeLocations = foundSafeLocations.OrderBy(o => Vector3.Distance(o.Coordinates, objectScript.transform.position)).ToList();
            return foundSafeLocations[0];
        }
        else
        {
            return null;
        }
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
            // only add nesting if it's not already on the list somewhere
            if (!drivers.Contains(NpcDrivers.Nest))
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
    public AnimalSubject Subject { get { return KnowledgeBase.GetSubject(subjectID, typeof(AnimalSubject)) as AnimalSubject; } }

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
            SubjectObjectScript conScript = conObject.GetComponent<SubjectObjectScript>();
            if (conScript != null)
            {
                if (conScript.Subject != null)
                {
                    if (IsSubjectKnown(conObject.GetComponent<SubjectObjectScript>().Subject.SubjectID))
                    {
                        if (IsSubjectDangerous(conObject.GetComponent<SubjectObjectScript>().Subject.SubjectID))
                        {
                            // don't reduce safety if it's dead
                            if (conObject.GetComponent<SubjectObjectScript>() is AnimalObjectScript)
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
            if (reExploreLocations.Count > 0)
            {
                mob.MoveToLocation(reExploreLocations[0]);
            }
            else
            {
                Debug.Log("No locations to explore.");
            }
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
            ExploreAndGatherItem(foodID);

        }
    }

    private void AiNest()
    {
        if (definition.Nest != null)
        {
            SpecialObjectMemory holeMemory = definition.Nest.ObjectMemories.Find(o => o.SubjectID == KbIds.Hole10) as SpecialObjectMemory;

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
                        StructureObjectScript holeObject = null;
                        // we are next to our hole, get a reference to it
                        foreach (GameObject gObj in considerObjects)
                        {
                            SubjectObjectScript subObj = gObj.GetComponent<SubjectObjectScript>();
                            if (subObj is StructureObjectScript)
                            {
                                if (subObj.Subject.SubjectID == KbIds.Hole10)
                                {
                                    holeObject = subObj as StructureObjectScript;
                                    break;
                                }
                            }
                        }

                        if (holeObject != null)
                        {
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
                                // build nest using items in hole
                                StructureObjectScript nestObjectScript = mob.BuildNest(holeObject);

                                definition.Nest.ObjectMemories.Remove(holeMemory);
                                SpecialObjectMemory nestMemory =
                                    new SpecialObjectMemory()
                                    {
                                        Position = nestObjectScript.transform.position,
                                        Quantity = 1,
                                        SubjectID = nestObjectScript.Subject.SubjectID
                                    };
                                // save nest to our special Nest memory
                                definition.Nest.ObjectMemories.Add(nestMemory);
                                // we just built our nest, remove from drivers.
                                drivers.Remove(NpcDrivers.Nest);
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
                                    {
                                        // compare needs to what is already in the hole, add to neededItems if we still need some
                                        if (!holeObject.Inventory.Contains(ingredient))
                                            neededItems.Add(new InventoryItem(ingredient));
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Error: Hole object not found!");
                        }
                    }
                }
                else
                {
                    // neededIngredients has ingredients, go collect them.
                    ExploreAndGatherItem(neededItems[0].SubjectID);
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
                            SubjectID = KbIds.Hole10
                        };
                    definition.Nest.ObjectMemories.Add(newHoleMemory);
                }
            }
        }
        else
        {
            // find nearest safe location to build nest there
            LocationSubject safeLocation = GetSafeLocation(mob);
            if (safeLocation != null)
            {
                // a safe location is in memory, are we already there?
                if (safeLocation.SubjectID == mob.Location.SubjectID)
                {
                    // If not already saved, save this location as Nest location
                    if (definition.Nest == null)
                    {
                        definition.Nest = new LocationMemory(safeLocation.SubjectID, 10, 0);
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

    /// <summary>
    /// Search memories for a source object of itemSubjectId. 
    /// Travel to the location, search for the source object, attempt to harvest from it. 
    /// Removes found items from the neededIngredients list.
    /// </summary>
    /// <param name="itemSubjectId">The item to search for.</param>
    private void ExploreAndGatherItem(int itemSubjectId)
    {
        int[] SourceSubjectIds = GetItemSources(itemSubjectId);

        if (SourceSubjectIds != null)
        {
            SubjectObjectScript harvestObject = null;
            // search immediate surroundings for source objects
            foreach (GameObject gameObj in considerObjects)
            {
                // skip this object if we've already searched it
                if (searchedObjects.Contains(gameObj.GetInstanceID())) continue;

                // if no SubjectObjectScript is attached this is not an object we can interact with
                SubjectObjectScript script = gameObj.GetComponent<SubjectObjectScript>();
                if (script == null) continue;

                for (int i = 0; i < SourceSubjectIds.Length; i++)
                {
                    if (script.Subject == null) continue;
                    if (SourceSubjectIds[i] == script.Subject.SubjectID)
                    {
                        harvestObject = script;
                        break;
                    }
                }
            }

            if (harvestObject != null)
            {
                HarvestFromObject(harvestObject, itemSubjectId);
            }
            else
            {
                // if we've searched all the locations we known of, start over.
                if (GetKnownLocationCount() == searchedLocations.Count && unexploredLocations.Count == 0)
                {
                    searchedLocations.Clear();
                    searchedObjects.Clear();
                }

                // we've harvested from everything we could nearby
                // travel to known locations for source object
                List<LocationSubject> objectLocations = GetObjectLocations(itemSubjectId, mob.transform.position, searchedLocations);
                if (objectLocations.Count > 0)
                {
                    mob.MoveToLocation(objectLocations[0]);
                }
                else
                {
                    AiExplore();
                }
            }
        }
        else
        {
            // we don't know of a source for the item, we should explore to learn about more subjects
            AiExplore();
        }
    }

    /// <summary>
    /// Gets a list of the SubjectId's of objects we can harvest this item from.
    /// </summary>
    /// <param name="itemSubjectId">The SubjectID of the item.</param>
    /// <returns>null if no sources were found</returns>
    private int[] GetItemSources(int itemSubjectId)
    {
        int[] SourceSubjectIds = null;
        // check if we know a source for this item
        foreach (SubjectMemory memory in definition.Memories)
        {
            if (memory.SubjectID == itemSubjectId)
            {
                if (memory.Sources == null)
                { continue; }
                else
                {
                    SourceSubjectIds = new int[memory.Sources.Length];
                    for (int i = 0; i < SourceSubjectIds.Length; i++)
                    {
                        SourceSubjectIds[i] = memory.Sources[i];
                    }
                }
            }
        }

        return SourceSubjectIds;
    }

    /// <summary>
    /// Harvest from an object, moves within range of the object before harvesting.
    /// </summary>
    /// <param name="harvestObject">The object to harvest from</param>
    private void HarvestFromObject(SubjectObjectScript harvestObject, int itemIdToHarvest)
    {
        // harvestFromObject is a source for the desired item, move closer if we're not close enough
        if (Vector3.Distance(mob.transform.position, harvestObject.transform.position) > definition.RangeActionClose)
        {
            if (mob.GetChaseTarget() == null)
            {
                mob.ChaseStart(harvestObject);
            }
        }
        else
        {
            // we are close enough to harvest
            InventoryItem harvestedItem = harvestObject.Harvest(itemIdToHarvest);
            if (harvestedItem == null)
            {
                // harvesting was ineffective, we may need to interact in
                // in a special way to harvest an item from this object
                if (harvestObject is AnimalObjectScript)
                {
                    // currently we only harvest from dead animals, kill this one to harvest from it.
                    if (!(harvestObject as AnimalObjectScript).IsDead)
                    {
                        // animal is not dead, attack it
                        (harvestObject as AnimalObjectScript).Damage(this.Subject, definition.AttackDamage, this);
                    }
                }
            }
            else
            {
                // harvest may have been successful, did we get anything?
                if (harvestedItem.Quantity > 0)
                {
                    // something was harvested
                    for (int j = (neededItems.Count - 1); j >= 0; j--)
                    {
                        if (neededItems[j].SubjectID == harvestedItem.SubjectID)
                        {
                            UpdateMemory(NpcMemoryChangeEvent.ItemHarvested, harvestedItem.SubjectID,
                                new object[1]
                                {
                                                harvestObject.Subject.SubjectID
                                });
                            if (neededItems[j].Quantity <= harvestedItem.Quantity)
                            {
                                neededItems.RemoveAt(j);
                            }
                            else
                            {
                                neededItems[j].Quantity -= harvestedItem.Quantity;
                            }

                        }
                    }
                    // add the harvested item to inventory
                    mob.Inventory.Add(harvestedItem);
                }
                else
                {
                    // This assumes we will always harvest at least one item:
                    // nothing was harvested this means nothing can be harvested from this item for 
                    // now, remember that we already checked this one
                    searchedObjects.Add(harvestObject.gameObject.GetInstanceID());
                }
            }
        }
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
                mob.MoveToLocation(KnowledgeBase.GetSubject(definition.Nest.SubjectID) as LocationSubject);
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
        if (inspectObjectScript == null) return;
        if (inspectObjectScript is LocationObjectScript)
        {
            LocationObjectScript locObjScript = inspectObjectScript as LocationObjectScript;
            //only add location to memory if all waypoints are explored
            if (mob.IsCurrentLocationExplored)
            {
                UpdateMemory(NpcMemoryChangeEvent.LocationFound, inspectObjectScript.Subject.SubjectID);
                // if it's in the unexploredLocations list, remove it.
                unexploredLocations.Remove(locObjScript.Subject as LocationSubject);
                // if it's in the reExploreLocations list, remove it.
                reExploreLocations.Remove(locObjScript.Subject as LocationSubject);
            }
        }
        else
        {
            inspectObjectScript.Subject.TeachNpc(this);
            int[] foodSources = GetItemSources(foodID);
            if (foodSources != null)
            {
                if (foodSources.Length > 0)
                {
                    int foodSourceID = foodSources[0];
                    if (inspectObjectScript.Subject.SubjectID == foodSourceID)
                        definition.Memories.Find(o => o.SubjectID == foodSourceID).Safety = 1;
                }
            }
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
                FoodSubject foodSubject = KnowledgeBase.GetSubject(FoodItem.SubjectID, typeof(FoodSubject)) as FoodSubject;
                if (foodSubject != null)
                {
                    food += foodSubject.FoodValue;
                    food = System.Math.Min(food, definition.FoodMax);

                    UpdateMemory(NpcMemoryChangeEvent.FoodEaten, foodSubject.SubjectID);

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

        UpdateMemory(NpcMemoryChangeEvent.HealthDamage, subjectAttacker.SubjectID);

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
