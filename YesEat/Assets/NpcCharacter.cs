using System.Collections.Generic;

/// <summary>
/// Core character AI.
/// </summary>
public partial class NpcCharacter {

    #region private member declarations
    private int health = 0;
    private int mind = 0;
    private int stamina = 0;
    private int water = 0;
    private int food = 0;
    private int air = 0;
    private int safety = 0;
    private int endurance = 0;
    private int sanity = 0;
    private int aggression = 0;
    private int temperament = 0;
    private int focus = 0;
    private List<Subject> subjectsKnown = new List<Subject>();
    private List<SubjectAttitude> attitudes = new List<SubjectAttitude>();
    private CharacterStatus status;
    private TriggerSet triggers = new TriggerSet();
    #endregion

    ///// <summary>
    ///// Current health.
    ///// </summary>
    //public int Health
    //{
    //    get { return health; }
    //    private set { health = value; }
    //}

    ///// <summary>
    ///// Current mind.
    ///// </summary>
    //public int Mind
    //{
    //    get { return mind; }
    //    private set { mind = value; }
    //}

    ///// <summary>
    ///// Current stamina.
    ///// </summary>
    //public int Stamina
    //{
    //    get { return stamina; }
    //    private set { stamina = value; }
    //}

    ///// <summary>
    ///// Current fluid (water).
    ///// </summary>
    //public int Water
    //{
    //    get { return water; }
    //    set { water = value; }
    //}

    ///// <summary>
    ///// Current food.
    ///// </summary>
    //public int Food
    //{
    //    get { return food; }
    //    private set { food = value; }
    //}

    ///// <summary>
    ///// Current air (breath).
    ///// </summary>
    //public int Air
    //{
    //    get { return air; }
    //    private set { air = value; }
    //}

    ///// <summary>
    ///// All subjects known by this character.
    ///// </summary>
    //public List<Subject> SubjectsKnown
    //{
    //    get { return subjectsKnown; }
    //    private set { subjectsKnown = value; }
    //}

    ///// <summary>
    ///// Current perception of safety.
    ///// </summary>
    //public int Safety
    //{
    //    get { return safety; }
    //    private set { safety = value; }
    //}

    ///// <summary>
    ///// Current endurance.
    ///// </summary>
    //public int Endurance
    //{
    //    get { return endurance; }
    //    private set { endurance = value; }
    //}

    ///// <summary>
    ///// Current sanity.
    ///// </summary>
    //public int Sanity
    //{
    //    get { return sanity; }
    //    private set { sanity = value; }
    //}

    ///// <summary>
    ///// Current aggression tendency.
    ///// </summary>
    //public int Aggression
    //{
    //    get { return aggression; }
    //    private set { aggression = value; }
    //}

    ///// <summary>
    ///// Current temperament.
    ///// </summary>
    //public int Temperament
    //{
    //    get { return temperament; }
    //    private set { temperament = value; }
    //}

    //public int Focus
    //{
    //    get { return focus; }
    //    private set { focus = value; }
    //}

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
