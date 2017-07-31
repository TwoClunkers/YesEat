using System.Collections.Generic;

/// <summary>
/// The definition of an NPC including: <para />
/// Resource pool limits, thresholds for triggering changes in priorities, traits, initial attitudes, initial memories.
/// </summary>
public class NpcDefinition
{
    /// <summary>
    /// Memories of each location that has been observed.
    /// </summary>
    public List<NpcLocationMemory> LocationMemories;
    /// <summary>
    /// Memory of nest location.
    /// </summary>
    public NpcLocationMemory Nest;
    /// <summary>
    /// Attitude toward every known subject.
    /// </summary>
    public List<SubjectAttitude> Attitudes;
    /// <summary>
    /// Static traits like NestMaker, Herbivore, Carnivore, etc.
    /// </summary>
    public NpcCharacterTraits Traits;
    /// <summary>
    /// The time between Metabolize() ticks.
    /// </summary>
    public float MetabolizeInterval = 0.0f;
    /// <summary>
    /// The amount of health regenerated each MetabolizeInterval.
    /// </summary>
    public int HealthRegen = 0;
    /// <summary>
    /// The amount of health lost each MetabolizeInterval while starving.
    /// </summary>
    public int StarvingDamage = 0;

    /// <summary>
    /// Threshold at which health is considered dangerously low, greatly reducing safety.
    /// </summary>
    public int HealthDanger = 0;
    /// <summary>
    /// Maximum health.
    /// </summary>
    public int HealthMax = 0;
    /// <summary>
    /// Threshold at which eating food becomes a priority.
    /// </summary>
    public int FoodHungry = 0;
    /// <summary>
    /// Maximum food.
    /// </summary>
    public int FoodMax = 0;
    /// <summary>
    /// The amount of food metabolized each MetabolizeInterval.
    /// </summary>
    public int FoodMetabolizeRate = 0;
    /// <summary>
    /// Threshold at which safety becomes top priority.
    /// </summary>
    public int SafetyDeadly = 0;
    /// <summary>
    /// Maximum Safety.
    /// </summary>
    public int SafetyHigh = 0;
}