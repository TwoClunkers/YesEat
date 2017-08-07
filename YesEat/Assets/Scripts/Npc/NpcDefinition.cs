using System.Collections.Generic;

/// <summary>
/// The definition of an NPC including: <para />
/// Resource pool limits, thresholds for triggering changes in priorities, traits, initial memories, initial memories.
/// </summary>
public struct NpcDefinition
{
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
    /// Threshold at which health is considered dangerously low, greatly reducing safety.
    /// </summary>
    public int HealthDanger;
    /// <summary>
    /// Maximum health.
    /// </summary>
    public int HealthMax;
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
    /// Threshold at which safety becomes top priority.
    /// </summary>
    public int SafetyDeadly;
    /// <summary>
    /// Maximum Safety.
    /// </summary>
    public int SafetyHigh;

    /// <summary>
    /// Typical movement speed.
    /// </summary>
    public float MoveSpeed;
    /// <summary>
    /// Near range of sight.
    /// </summary>
    public float SightRangeNear;
    /// <summary>
    /// Far range of sight.
    /// </summary>
    public float SightRangeFar;
    /// <summary>
    /// Amount of damage dealt when attacking.
    /// </summary>
    public int AttackDamage;
}