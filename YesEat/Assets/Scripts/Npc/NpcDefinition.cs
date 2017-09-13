using System.Collections.Generic;

/// <summary>
/// The definition of an NPC including: <para />
/// Resource pool limits, thresholds for triggering changes in priorities, traits, initial memories, initial memories.
/// </summary>
public struct NpcDefinition
{
    /// <summary>
    /// The attributes this NPC likes.
    /// </summary>
    public SubjectAttributes Likes;
    /// <summary>
    /// The attributes this NPC hates.
    /// </summary>
    public SubjectAttributes Dislikes;

    /// <summary>
    /// Memory of nest location.
    /// </summary>
    public LocationMemory Nest;

    /// <summary>
    /// Memories of every known subject.
    /// </summary>
    public List<SubjectMemory> Memories;

    /// <summary>
    /// Static traits like NestMaker, Herbivore, Carnivore, etc.
    /// </summary>
    public NpcCharacterTraits Traits;

    #region Metabolize() parameters
    /// <summary>
    /// The time between Metabolize() ticks.
    /// </summary>
    public float MetabolizeInterval;
    /// <summary>
    /// The amount of health regenerated each MetabolizeInterval.
    /// </summary>
    public int HealthRegen;
    /// <summary>
    /// The amount of health lost each MetabolizeInterval while starving.
    /// </summary>
    public int StarvingDamage;
    /// <summary>
    /// The amount of Endurance lost each MetabolizeInterval.
    /// </summary>
    public float EnduranceLossRate;
    #endregion

    /// <summary>
    /// Maximum Endurance.
    /// </summary>
    public int EnduranceMax;
    /// <summary>
    /// Low Endurance makes NPC tired, sleep to restore.
    /// </summary>
    public int EnduranceLow;

    /// <summary>
    /// Threshold at which eating food becomes a priority.
    /// </summary>
    public int FoodHungry;
    /// <summary>
    /// Maximum food.
    /// </summary>
    public int FoodMax;
    /// <summary>
    /// The amount of food metabolized each MetabolizeInterval.
    /// </summary>
    public int FoodMetabolizeRate;

    /// <summary>
    /// Threshold at which health is considered dangerously low, greatly reducing safety.
    /// </summary>
    public int HealthDanger;
    /// <summary>
    /// Maximum health.
    /// </summary>
    public int HealthMax;

    /// <summary>
    /// Threshold at which safety becomes top priority.
    /// </summary>
    public int SafetyDeadly;
    /// <summary>
    /// Maximum Safety.
    /// </summary>
    public int SafetyHigh;

    // world interaction
    /// <summary>
    /// Typical movement speed.
    /// </summary>
    public float MoveSpeed;
    /// <summary>
    /// Fastest possible movement speed.
    /// </summary>
    public float MoveSpeedFastest;
    /// <summary>
    /// Near range of sight.
    /// </summary>
    public float RangeSightNear;
    /// <summary>
    /// Mid range of sight.
    /// </summary>
    public float RangeSightMid;
    /// <summary>
    /// Far range of sight.
    /// </summary>
    public float RangeSightFar;
    /// <summary>
    /// Amount of damage dealt when attacking.
    /// </summary>
    public int AttackDamage;
    /// <summary>
    /// The maximum range required to perform 'Close' actions.
    /// </summary>
    public float RangeActionClose;

    /// <summary>
    /// Copy and existing Definition.
    /// </summary>
    /// <param name="newDefinition">Definition to copy.</param>
    public NpcDefinition(NpcDefinition newDefinition)
    {
        Likes = newDefinition.Likes;
        Dislikes = newDefinition.Dislikes;

        Nest = (newDefinition.Nest == null) ? null : new LocationMemory(newDefinition.Nest);

        Memories = new List<SubjectMemory>(newDefinition.Memories);

        NpcCharacterTraits newTraits = new NpcCharacterTraits(newDefinition.Traits.Traits);
        Traits = newTraits;

        MetabolizeInterval = newDefinition.MetabolizeInterval;
        HealthRegen = newDefinition.HealthRegen;
        StarvingDamage = newDefinition.StarvingDamage;
        EnduranceLossRate = newDefinition.EnduranceLossRate;

        HealthDanger = newDefinition.HealthDanger;
        HealthMax = newDefinition.HealthMax;
        EnduranceLow = newDefinition.EnduranceLow;
        EnduranceMax = newDefinition.EnduranceMax;
        FoodHungry = newDefinition.FoodHungry;
        FoodMax = newDefinition.FoodMax;
        FoodMetabolizeRate = newDefinition.FoodMetabolizeRate;
        SafetyDeadly = newDefinition.SafetyDeadly;
        SafetyHigh = newDefinition.SafetyHigh;

        MoveSpeed = newDefinition.MoveSpeed;
        MoveSpeedFastest = newDefinition.MoveSpeedFastest;
        RangeSightNear = newDefinition.RangeSightNear;
        RangeSightMid = newDefinition.RangeSightMid;
        RangeSightFar = newDefinition.RangeSightFar;
        AttackDamage = newDefinition.AttackDamage;
        RangeActionClose = newDefinition.RangeActionClose;
    }

}