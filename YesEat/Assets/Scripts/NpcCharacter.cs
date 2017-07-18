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
    private List<Subject> subjectsKnown;
    private List<SubjectAttitude> attitudes;
    private CharacterStatus status;
    private NpcDefinition definition;
    private List<Drivers> drivers;

    #endregion

    public NpcCharacter()
    {
        health = 100;
        food = 100;
        safety = 100;
        subjectsKnown = new List<Subject>();
        attitudes = new List<SubjectAttitude>();
        status = new CharacterStatus();
        definition = new NpcDefinition();
        drivers = new List<Drivers>();
    }

    /// <summary>
    /// Whether this NPC is dead or not.
    /// </summary>
    public bool IsDead { get { return status.IsStateSet(NpcStates.Dead); } }

    private void Die()
    {
        status.SetState(NpcStates.Dead);
        drivers.Clear();
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
        if (health < definition.HealthMax)
        {
            health += definition.HealthRegen;
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
        //check flags to make sure we're able to eat
        bool wasConsumed = false;
        if (status.CanEat())
        {
            if (food < definition.FoodMax)
            {
                status.SetState(NpcStates.Eating); //set eating flag
                food += FoodItem.FoodValue;
                food = System.Math.Min(food, definition.FoodMax);
                wasConsumed = true;
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
            // zero health, it died.
            status.SetState(NpcStates.Dead);
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
    /// Replaces the current TriggerSet with a new one. This defines the character's resource pools and thresholds for fulfilling basic needs.
    /// </summary>
    /// <param name="newTriggerSet">The new TriggerSet.</param>
    /// <returns>False = System Exception</returns>
    public  NpcCharacter(NpcDefinition triggerSet, List<Subject> InitialSubjectsKnown, List<SubjectAttitude> initialAttitudes, CharacterStatus initialCharacterStates, List<Drivers> initialDrivers)
    {
        definition = triggerSet;
        health = definition.HealthMax;
        food = definition.FoodMax;
        safety = definition.SafetyHigh;
        subjectsKnown = InitialSubjectsKnown;
        attitudes = initialAttitudes;
        status = initialCharacterStates;
        drivers = initialDrivers;
    }

    public NpcCharacter(Subject subject)
    {
        if (subject is AnimalSubject)
        {
            definition = subject.Definition;
            health = definition.HealthMax;
            food = definition.FoodMax;
            safety = definition.SafetyHigh;
            subjectsKnown = subject.SubjectsKnown;
            attitudes = subject.Attitudes;
            status = new CharacterStatus();
            drivers = new List<Drivers>();
        }
    }
}
