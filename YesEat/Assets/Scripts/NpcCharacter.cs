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
    private List<NpcDrivers> drivers;
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
        /// <param name="npcDefinition">The NpcDefinition that contains the attitudes to check.</param>
        /// <param name="conSubject">The Subject to be considered.</param>
        /// <returns>True = dangerous; False = not dangerous. If conSubject does not exist and Exception will be thrown.</returns>
        internal static bool IsSubjectDangerous(NpcDefinition npcDefinition, Subject conSubject)
        {
            if (IsSubjectKnown(npcDefinition, conSubject))
            {
                SubjectAttitude subjectAttitude = npcDefinition.attitudes.Find(o => o.SubjectID == conSubject.SubjectID);
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
        drivers = new List<NpcDrivers>();
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
            drivers = new List<NpcDrivers>();
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
        //| === Meta Code ===   []=flow chart box
        //| []Survey current position.
        //|     Check saved collides and check distance?
        //| []Save all discovered subjects.
        //|     Save subjects to List<Subject> considerSubjects
        //|     Sort considerSubjects from closest to farthest subject.
        //| []Am I safe?
        //|     Assume we're safe- clear safety from drivers.
        if (drivers.Contains(NpcDrivers.Safety))
            drivers.Remove(NpcDrivers.Safety);
        //|     Consider each subject starting with the closest.
        //|     Check each subject against attitudes to see if it is a danger.
        //|     If the subject is unknown skip it for now.
        //|     If any danger subjects are found set safety as top priority.
        foreach (Subject conSubject in considerSubjects)
        {
            if (Think.IsSubjectKnown(definition, conSubject))
            {
                if (Think.IsSubjectDangerous(definition, conSubject))
                {
                    if (drivers.Contains(NpcDrivers.Safety))
                    {
                        drivers.Remove(NpcDrivers.Safety);
                        drivers.Insert(0, NpcDrivers.Safety);
                    }
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
            if (drivers.Contains(NpcDrivers.Health))
                drivers.Remove(NpcDrivers.Health);
            drivers.Insert(0, NpcDrivers.Health);
        }

        if (food <= definition.FoodHungry)
        {
            if (drivers.Contains(NpcDrivers.Hunger))
                drivers.Remove(NpcDrivers.Hunger);
            drivers.Insert(0, NpcDrivers.Hunger);
        }

        if (safety <= definition.SafetyDeadly)
        {
            if (!drivers.Contains(NpcDrivers.Safety))
                drivers.Remove(NpcDrivers.Safety);
            drivers.Insert(0, NpcDrivers.Safety);
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
            {
                if (drivers.Contains(NpcDrivers.Hunger))
                    drivers.Remove(NpcDrivers.Hunger);
            }
        }
        return wasConsumed;
    }

    /// <summary>
    /// Inflict DamageAmount of health damage to character.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to inflict.</param>
    /// <returns>True: Character was killed by the damage. False: still alive.</returns>
    public bool Harm(int damageAmount)
    {
        if (this.IsDead) return false;

        status.SetState(NpcStates.Fighting);
        health += damageAmount;
        health = System.Math.Max(health, 0);

        if (health <= 0)
        {
            Die();
        }
        else if (health <= definition.HealthDanger)
        {
            if (drivers.Contains(NpcDrivers.Safety))
                drivers.Remove(NpcDrivers.Safety);
            drivers.Insert(0, NpcDrivers.Safety);
        }

        return !status.IsStateSet(NpcStates.Dead);
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
