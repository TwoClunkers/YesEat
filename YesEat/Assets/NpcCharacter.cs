using System.Collections.Generic;

/// <summary>
/// Core character AI.
/// </summary>
public partial class NpcCharacter {

    #region private member declarations
    private int health = 0;
    private int food = 0;
    private int safety = 0;
    private List<Subject> subjectsKnown = new List<Subject>();
    private List<SubjectAttitude> attitudes = new List<SubjectAttitude>();
    private CharacterStatus status;
    private TriggerSet triggers = new TriggerSet();
    #endregion

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

            //TODO: adjust food in priority matrix
            if (food >= triggers.FoodSated) { }
            else if (food >= triggers.FoodHungry) { }
            else if (food >= triggers.FoodStarving) { }

            status.UnsetState(TransientStates.Eating); //unset eating flag
        }
        return wasConsumed;
    }

    /// <summary>
    /// Replaces the current TriggerSet with a new one. This defines the character's resource pools and thresholds for fulfilling basic needs.
    /// </summary>
    /// <param name="NewTriggerSet">The new TriggerSet.</param>
    /// <returns>False = System Exception</returns>
    public bool SetCharacterTriggers(TriggerSet NewTriggerSet)
    {
        try
        {
            this.triggers = NewTriggerSet;
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
