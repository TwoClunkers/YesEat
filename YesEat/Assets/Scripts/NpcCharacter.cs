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

    private int subjectID;
    private MasterSubjectList masterSubjectList;
    private CharacterStatus status;
    private NpcDefinition definition;
    private NpcDriversList drivers;
    private List<Subject> considerSubjects;

    private List<NpcCharacter> combatTargets;
    #endregion

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
        combatTargets = new List<NpcCharacter>();
        subjectID = -1;
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
            subjectID = animalSubject.SubjectID;
            health = definition.HealthMax;
            food = definition.FoodMax;
            safety = definition.SafetyHigh;
            status = new CharacterStatus();
            drivers = new NpcDriversList();
            combatTargets = new List<NpcCharacter>();
        }
    }

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
                SubjectAttitude subjectAttitude = definition.Attitudes.Find(o => o.SubjectID == conSubject.SubjectID);
                return (subjectAttitude.Goodness < 0 && subjectAttitude.Importance > 0);
            }
            else
            {
                throw new Exception("The queried Subject is not in the attitudes list.");
            }
        }

        /// <summary>
        /// Checks if conSubject exists in the npcDefinition.
        /// </summary>
        /// <param name="npcDefinition">The NpcDefinition that contains the attitudes to check.</param>
        /// <param name="conSubject">The Subject to be considered.</param>
        /// <returns>True: known. False: not known.</returns>
        internal static bool IsSubjectKnown(NpcDefinition npcDefinition, Subject conSubject)
        {
            return npcDefinition.Attitudes.Exists(o => o.SubjectID == conSubject.SubjectID);
        }

        /// <summary>
        /// Change attitude about a subject based on an event.
        /// </summary>
        /// <param name="attitudeChangeEvent">The event that has occured.</param>
        /// <param name="definition">The NPC to effect.</param>
        /// <param name="subject">The subject to adjust attitude towards.</param>
        internal static void UpdateAttitude(NpcAttitudeChangeEvent attitudeChangeEvent, NpcDefinition definition, Subject subject)
        {
            switch (attitudeChangeEvent)
            {
                case NpcAttitudeChangeEvent.HealthDamage:
                    if (IsSubjectKnown(definition, subject))
                    {
                        //known hurts, bad.
                        SubjectAttitude attackerSubjectAttitude = definition.Attitudes.Find(o => o.SubjectID == subject.SubjectID);
                        attackerSubjectAttitude.AddGoodness(-1);
                        attackerSubjectAttitude.AddImportance(1);
                    }
                    else
                    {
                        //new thing hurts me, bad.
                        SubjectAttitude subjectAttitude = new SubjectAttitude(subject.SubjectID, -1, 1);
                        definition.Attitudes.Add(subjectAttitude);
                    }
                    break;
                case NpcAttitudeChangeEvent.FoodEaten:
                    if (IsSubjectKnown(definition, subject))
                    {
                        //known food, good.
                        SubjectAttitude foodSubjectAttitude = definition.Attitudes.Find(o => o.SubjectID == subject.SubjectID);
                        foodSubjectAttitude.AddGoodness(1);
                        foodSubjectAttitude.AddImportance(1);
                    }
                    else
                    {
                        //new food, good.
                        SubjectAttitude subjectAttitude = new SubjectAttitude(subject.SubjectID, 1, 1);
                        definition.Attitudes.Add(subjectAttitude);
                    }
                    break;
                case NpcAttitudeChangeEvent.LocationFound:
                    // TODO: look at everything in this location and decide how to effect goodness and importance for this location.
                    break;
                default:
                    throw new Exception("Invalid NpcAttitudeChangeEvent");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Consider whether I should attack another NPC.
        /// </summary>
        /// <param name="npcToConsider">The NPC to consider attacking.</param>
        /// <param name="damageAmount">The amount of damage we received from the NPC if applicable.</param>
        /// <returns>True = attack, False = do not attack.</returns>
        internal static bool ShouldFight(NpcCharacter npcToConsider, int? damageAmount = null)
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
    }

    /// <summary>
    /// This NPC's associated subject.
    /// </summary>
    public int SubjectID
    {
        get { return subjectID; }
        set { subjectID = value; }
    }

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
        else
        {
            if (health < definition.HealthMax)
            {
                health += definition.HealthRegen;
                food -= definition.FoodMetabolizeRate;
            }
        }
    }

    public void AiCoreProcess()
    {
        // TODO:    Add all game objects in close range to the considerSubjects list
        //          sorted by distance with the lowest index being the closest.

        // Assume we're safe- clear safety from drivers.
        drivers.Remove(NpcDrivers.Safety);

        // Consider each subject starting with the closest.
        foreach (Subject conSubject in considerSubjects)
        {
            if (Think.IsSubjectKnown(definition, conSubject))
            {
                if (Think.IsSubjectDangerous(definition, conSubject))
                {
                    // danger! decrease safety
                    safety -= 1;
                }
            }
        }

        UpdateDrivers();

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
            drivers.SetTopDriver(NpcDrivers.Health);
        }

        // of the basic needs safety is the highest priority
        if (safety <= definition.SafetyDeadly)
        {
            status.UnsetState(NpcStates.Fighting);
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
    /// <param name="subjectAttacker">Source of damage.</param>
    /// <param name="damageAmount">Amount of damage to inflict.</param>
    /// <param name="NpcAttacker">Attacking NPC if applicable.</param>
    /// <returns>True: Character was killed by the damage. False: still alive.</returns>
    public bool Damage(Subject subjectAttacker, int damageAmount, NpcCharacter NpcAttacker = null)
    {
        if (IsDead) return false;

        health -= damageAmount;
        health = Math.Max(health, 0);

        if (health <= 0)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            // decrease safety by percentage of max health that damage received was
            safety -= (damageAmount / definition.HealthMax) * 100;
        }

        // known attacker?
        if (Think.IsSubjectKnown(definition, subjectAttacker))
        {
            Think.UpdateAttitude(NpcAttitudeChangeEvent.HealthDamage, definition, subjectAttacker);
        }

        if (NpcAttacker != null)
        {
            if (Think.ShouldFight(NpcAttacker, damageAmount))
            {
                combatTargets.Add(NpcAttacker);
                status.SetState(NpcStates.Fighting);
                //TODO: add a subroutine that will trigger a fight action if fighting state is set.
            }
            else
            {
                // set safety minimum to flee
                safety = int.MinValue;
            }
        }

        return !IsDead;
    }

    /// <summary>
    /// True = Dead. False = Not dead.
    /// </summary>
    public bool IsDead { get { return status.IsStateSet(NpcStates.Dead); } }

    /// <summary>
    /// Character has died, set Dead status and clear out all drivers.
    /// </summary>
    private void Die()
    {
        status.SetState(NpcStates.Dead);
        drivers.Clear();
    }
}
