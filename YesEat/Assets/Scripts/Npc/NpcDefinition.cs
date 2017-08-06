﻿using System.Collections.Generic;

/// <summary>
/// The definition of an NPC including: <para />
/// Resource pool limits, thresholds for triggering changes in priorities, traits, initial memories, initial memories.
/// </summary>
public class NpcDefinition
{
    /// <summary>
    /// Memory of nest location.
    /// </summary>
    public LocationMemory Nest = null;
    /// <summary>
    /// Memories of every known subject.
    /// </summary>
    public List<SubjectMemory> Memories = null;
    /// <summary>
    /// Static traits like NestMaker, Herbivore, Carnivore, etc.
    /// </summary>
    public NpcCharacterTraits Traits = null;
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

    /// <summary>
    /// Typical movement speed.
    /// </summary>
    public float MoveSpeed = 0.0f;
    /// <summary>
    /// Near range of sight.
    /// </summary>
    public float SightRangeNear = 0.0f;
    /// <summary>
    /// Far range of sight.
    /// </summary>
    public float SightRangeFar = 0.0f;
}