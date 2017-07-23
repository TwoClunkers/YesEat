using System;
using System.Collections.Generic;

/// <summary>
/// Core character AI.
/// </summary>
public partial class NpcCharacter
{
    #region private member declarations
    private int health;
    private int food;
    private int safety;

    private MasterSubjectList masterSubjectList;
    private CharacterStatus status;
    private NpcDefinition definition;
    private NpcDriversList drivers;
    private List<Subject> considerSubjects;

    #endregion

    /// <summary>
    /// Contains methods for extensive parsing and evaluation of subjects and attitudes.
    /// </summary>
    internal static class Think
    {
        /// <summary>
        /// Find the nearest location where subjectToFind can be found.
        /// </summary>
        /// <param name="subjectToFind">The subject to search for.</param>
        /// <returns>The found location or null if no location was found.</returns>
        internal static LocationSubject FindNearest(Subject subjectToFind)
        {
            //TODO: Find nearest location where subjectToFind can be found.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the attitude of the NPC towards conSubject. IsSubjectKnown() must be used to verify the Subject is known before IsSubjectDangerous().
        /// </summary>
        /// <param name="definition">The NpcDefinition that contains the attitudes to check.</param>
        /// <param name="conSubject">The Subject to be considered.</param>
        /// <returns>True = dangerous; False = not dangerous. If conSubject does not exist and Exception will be thrown.</returns>
        internal static bool IsSubjectDangerous(NpcDefinition definition, Subject conSubject)
        {
            if (IsSubjectKnown(definition, conSubject))
            {
                SubjectAttitude subjectAttitude = definition.attitudes.Find(o => o.SubjectID == conSubject.SubjectID);
                return (subjectAttitude.Goodness < 0 && subjectAttitude.Importance > 0);
            }
            else
            {
                throw new Exception("The queried Subject is not in the attitudes list.");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if conSubject exists in the npcDefinition.
        /// </summary>
        /// <param name="npcDefinition">The NpcDefinition that contains the attitudes to check.</param>
        /// <param name="conSubject">The Subject to be considered.</param>
        /// <returns>True: known. False: not known.</returns>
        internal static bool IsSubjectKnown(NpcDefinition npcDefinition, Subject conSubject)
        {
            return npcDefinition.attitudes.Exists(o => o.SubjectID == conSubject.SubjectID);
        }

        internal static void UpdateAttitude(NpcAttitudeChangeEvent attitudeChangeEvent, NpcDefinition definition, Subject subjectAttacker)
        {
            switch (attitudeChangeEvent)
            {
                case NpcAttitudeChangeEvent.HealthDamage:
                    if(IsSubjectKnown(definition, subjectAttacker))
                    {
                        SubjectAttitude attackerSubjectAttitude = definition.attitudes.Find(o => o.SubjectID == subjectAttacker.SubjectID);
                        attackerSubjectAttitude.AddGoodness(-1);
                        attackerSubjectAttitude.AddImportance(1);
                    }
                    else
                    {
                        SubjectAttitude subjectAttitude = new SubjectAttitude(subjectAttacker.SubjectID, -1, 1);
                        definition.attitudes.Add(subjectAttitude);
                    }
                    break;
                case NpcAttitudeChangeEvent.FoodEaten:
                    break;
                case NpcAttitudeChangeEvent.LocationFound:
                    break;
                default:
                    throw new Exception("Invalid NpcAttitudeChangeEvent");
            }
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Create a generic NpcCharacter.
    /// </summary>
    public NpcCharacter(ref MasterSubjectList _masterSubjectList)
    {
        masterSubjectList = _masterSubjectList;
        health = 100;
        food = 100;
        safety = 100;
        definition = new NpcDefinition();
        status = new CharacterStatus();
        drivers = new NpcDriversList();
    }

    /// <summary>
    /// Initialize a new NpcCharacter. 
    /// </summary>
    /// <param name="subject">Subject's NpcDefinition will define the character's initial resource pools, thresholds for fulfilling basic needs, known subjects, and subject attitudes.</param>
    public NpcCharacter(ref MasterSubjectList _masterSubjectList, Subject subject)
    {
        masterSubjectList = _masterSubjectList;
        if (subject is AnimalSubject)
        {
            AnimalSubject animalSubject = subject as AnimalSubject;
            definition = animalSubject.Definition;
            health = definition.HealthMax;
            food = definition.FoodMax;
            safety = definition.SafetyHigh;
            status = new CharacterStatus();
            drivers = new NpcDriversList();
        }
    }

    /// <summary>
    /// True = Dead. False = Not dead.
    /// </summary>
    public bool IsDead { get { return status.IsStateSet(NpcStates.Dead); } }

    /// <summary>
    /// Reduce food. Reduce health if starving. Regenerate health if not starving.
    /// </summary>
    public void Metabolize()
    {
        if (IsDead) return;
        if (food == 0)
        {
            health -= definition.StarvingDamage;
            if (health <= 0)
                Die();
        }
        if (health < definition.HealthMax)
        {
            health += definition.HealthRegen;
        }
    }

    public void AiCoreProcess()
    {
        // TODO:    Add all game objects in close range to the considerSubjects list
        //          sorted by distance with the lowest index being the closest.

        // Assume we're safe- clear safety from drivers.
        if (drivers.Contains(NpcDrivers.Safety))
            drivers.Remove(NpcDrivers.Safety);

        // Consider each subject starting with the closest.
        foreach (Subject conSubject in considerSubjects)
        {
            if (Think.IsSubjectKnown(definition, conSubject))
            {
                if (Think.IsSubjectDangerous(definition, conSubject))
                {
                    // danger! set safety as top priority
                    drivers.SetTopDriver(NpcDrivers.Safety);
                }
            }
        }
        //| []Which driver is max priority?
        switch (drivers[0])
        {
            case NpcDrivers.Health:
                AiCoreSubprocessNest();
                break;
            case NpcDrivers.Safety:
                AiCoreSubprocessSafety();
                break;
            case NpcDrivers.Hunger:
                AiCoreSubprocessHunger();
                break;
            case NpcDrivers.Explore:
                AiCoreSubprocessExplore();
                break;
            default:
                //There are no critical drivers, default to exploration
                drivers.Clear();
                drivers.Add(NpcDrivers.Explore);
                break;
        }

    }

    /// <summary>
    /// Update drivers based on current values.
    /// </summary>
    public void UpdateDrivers()
    {
        if (health <= 0)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            drivers.SetTopDriver(NpcDrivers.Health);
        }

        if (food <= definition.FoodHungry)
        {
            drivers.SetTopDriver(NpcDrivers.Hunger);
        }

        if (safety <= definition.SafetyDeadly)
        {
            drivers.SetTopDriver(NpcDrivers.Safety);
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
                FoodSubject foodSubject = masterSubjectList.GetSubject(FoodItem.SubjectID, typeof(FoodSubject)) as FoodSubject;
                if (foodSubject != null)
                {
                    food += foodSubject.FoodValue;
                    food = System.Math.Min(food, definition.FoodMax);
                    wasConsumed = true;
                }
                status.UnsetState(NpcStates.Eating); //unset eating flag
            }

            if (food >= definition.FoodHungry)
                drivers.Remove(NpcDrivers.Hunger);
        }
        return wasConsumed;
    }

    /// <summary>
    /// Inflict DamageAmount of health damage to character.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to inflict.</param>
    /// <returns>True: Character was killed by the damage. False: still alive.</returns>
    public bool Harm(Subject subjectAttacker, int damageAmount)
    {
        if (IsDead) return false;

        status.SetState(NpcStates.Fighting);
        health += damageAmount;
        health = Math.Max(health, 0);

        if (health <= 0)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            drivers.SetTopDriver(NpcDrivers.Safety);
        }

        //|                 []Known attacker?
        if (Think.IsSubjectKnown(definition, subjectAttacker))
        {
            Think.UpdateAttitude(NpcAttitudeChangeEvent.HealthDamage, definition, subjectAttacker);
        }
        //|                     Check knownSubjects
        //|                     [No]Save attitude: bad / danger
        //|                         Think.GotHurtBy(Subject, damageAmount) - adjust attitude based 
        //|                         on how much damage was done and how we feel about this subject
        //|                     [Yes]Fight?
        //|                         [No]Search for safe location
        //|                             Think.FindNearest(FindEnum.SafeLocation) - check knownSubjects for a location that is safe
        //|                             travel to safe location or search for presently unknown location that is safe
        //|                             ()Return
        //|                         [Yes]Attack damage source
        //|                             set fighting state, save current subject to combat target list
        //|                             inflict damage on combat target
        //|                             ()Return

        return !IsDead;
    }

    /// <summary>
    /// Character has died, set Dead status and clear out all drivers.
    /// </summary>
    private void Die()
    {
        status.SetState(NpcStates.Dead);
        drivers.Clear();
    }
}
