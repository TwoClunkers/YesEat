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

    private CharacterStatus status;
    private NpcDefinition definition;
    private List<Drivers> drivers;

    #endregion

    /// <summary>
    /// Create a generic NpcCharacter.
    /// </summary>
    public NpcCharacter()
    {
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
    public NpcCharacter(Subject subject)
    {
        if (subject is AnimalSubject)
        {
            AnimalSubject animalSubject = subject as AnimalSubject;
            if (subject is AnimalSubject)
            {
                definition = animalSubject.Definition;
                health = definition.HealthMax;
                food = definition.FoodMax;
                safety = definition.SafetyHigh;
                status = new CharacterStatus();
                drivers = new List<Drivers>();
            }
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
                FoodSubject foodSubject = FoodItem.GetSubject() as FoodSubject;
                food += foodSubject.FoodValue;
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
