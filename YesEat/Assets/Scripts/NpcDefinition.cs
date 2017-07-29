using System.Collections.Generic;

/// <summary>
/// The definition of an NPC including: <para />
/// Resource pool limits, thresholds for triggering changes in priorities, traits, initial attitudes, initial memories.
/// </summary>
public class NpcDefinition
{
    public List<NpcLocationMemory> LocationMemories;
    public List<SubjectAttitude> Attitudes;
    public NpcTraits Traits;
    public float MetabolizeInterval = 0.0f;
    public int HealthRegen = 0;
    public int StarvingDamage = 0;

    public int HealthDanger = 0;
    public int HealthMax = 0;
    public int FoodHungry = 0;
    public int FoodMax = 0;
    public int FoodMetabolizeRate = 0;
    public int SafetyDeadly = 0;
    public int SafetyHigh = 0;
}