using System.Collections.Generic;

/// <summary>
/// Core character AI.
/// </summary>
public partial class NpcCharacter
{
    #region private member declarations
    private int health = 0;
    private int food = 0;
    private int safety = 0;
    private List<Subject> subjectsKnown = new List<Subject>();
    private List<SubjectAttitude> attitudes = new List<SubjectAttitude>();
    private CharacterStatus status;
    private TriggerSet triggers = new TriggerSet();
    private List<Drivers> drivers = new List<Drivers>();
    #endregion

    /// <summary>
    /// Reduce food like the body is consuming it. Reduce health if starving.
    /// </summary>
    public void Metabolize()
    {

    }

    /// <summary>
    /// Update drivers based on current values.
    /// </summary>
    public void UpdateDrivers()
    {
        if (health <= 0)
        {
            //character is dead
            drivers.Clear();
            status.SetState(TransientStates.Dead);
        }
        else if (health <= triggers.HealthDanger)
        {
            if (drivers.Contains(Drivers.Health))
                drivers.Remove(Drivers.Health);
            drivers.Insert(0, Drivers.Health);
        }

        if (food <= triggers.FoodHungry)
        {
            if (drivers.Contains(Drivers.Hunger))
                drivers.Remove(Drivers.Hunger);
            drivers.Insert(0, Drivers.Hunger);
        }

        if (safety <= triggers.SafetyDeadly)
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
    /// <returns>True: FoodItem was fully consumed. False: FoodItem could not be completely eaten.</returns>
    public bool Eat(Item FoodItem)
    {
        //check flags to make sure we're able to eat
        bool wasConsumed = false;
        if (status.CanEat())
        {
            status.SetState(TransientStates.Eating); //set eating flag
            if (food + FoodItem.FoodValue > triggers.FoodMax)
            {
                food += FoodItem.FoodValue;
                FoodItem.FoodValue = food - triggers.FoodMax; //literally too much to eat, puke the rest
                food = triggers.FoodMax;
            }
            else if (food + FoodItem.FoodValue <= triggers.FoodMax)
            {
                food += FoodItem.FoodValue;
                wasConsumed = true;
            }

            if (food >= triggers.FoodHungry)
            {
                if (drivers.Contains(Drivers.Hunger))
                    drivers.Remove(Drivers.Hunger);
            }

            status.UnsetState(TransientStates.Eating); //unset eating flag
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
        status.SetState(TransientStates.Fighting);
        health += damageAmount;
        health = System.Math.Max(health, 0);

        if (health <= 0)
        {
            // zero health, it died.
            status.SetState(TransientStates.Dead);
        }
        else if (health <= triggers.HealthDanger)
        {
            if (drivers.Contains(Drivers.Safety))
                drivers.Remove(Drivers.Safety);
            drivers.Insert(0, Drivers.Safety);
        }
        
        return !status.IsStateSet(TransientStates.Dead);
    }

    /// <summary>
    /// Replaces the current TriggerSet with a new one. This defines the character's resource pools and thresholds for fulfilling basic needs.
    /// </summary>
    /// <param name="newTriggerSet">The new TriggerSet.</param>
    /// <returns>False = System Exception</returns>
    public bool SetCharacterTriggers(TriggerSet newTriggerSet)
    {
        try
        {
            this.triggers = newTriggerSet;
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
