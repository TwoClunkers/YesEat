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
    private List<Drivers> drivers;
    private List<Subject> considerSubjects;

    #endregion

    /// <summary>
    /// Contains methods for extensive parsing and evaluation of subjects and attitudes.
    /// </summary>
    public static class Think
    {
        /// <summary>
        /// Find the nearest location where subjectToFind can be found.
        /// </summary>
        /// <param name="subjectToFind">The subject to search for.</param>
        /// <returns>The found location or null if no location was found.</returns>
        public static LocationSubject FindNearest(Subject subjectToFind)
        {
            //TODO: Find nearest location where subjectToFind can be found.
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
        drivers = new List<Drivers>();
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
            drivers = new List<Drivers>();
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
        //|     Consider each subject starting with the closest.
        //|     Check each subject against attitudes to see if it is a danger.
        //|     If the subject is unknown skip it for now.
        //|     If any danger subjects are found set safety as top priority.
        //| []Which driver is max priority?
        //|
        //|   =====> AiCoreSubprocessSafety()
        AiCoreSubprocessSafety();
        //|   =====> AiCoreSubprocessHunger()
        AiCoreSubprocessHunger();
        //|   =====> AiCoreSubprocessNest()
        AiCoreSubprocessNest();

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
            if (drivers.Contains(Drivers.Health))
                drivers.Remove(Drivers.Health);
            drivers.Insert(0, Drivers.Health);
        }

        if (food <= definition.FoodHungry)
        {
            if (drivers.Contains(Drivers.Hunger))
                drivers.Remove(Drivers.Hunger);
            drivers.Insert(0, Drivers.Hunger);
        }

        if (safety <= definition.SafetyDeadly)
        {
            if (!drivers.Contains(Drivers.Safety))
                drivers.Remove(Drivers.Safety);
            drivers.Insert(0, Drivers.Safety);
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
                if (drivers.Contains(Drivers.Hunger))
                    drivers.Remove(Drivers.Hunger);
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
            if (drivers.Contains(Drivers.Safety))
                drivers.Remove(Drivers.Safety);
            drivers.Insert(0, Drivers.Safety);
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
