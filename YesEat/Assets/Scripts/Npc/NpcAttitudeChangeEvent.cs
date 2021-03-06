﻿
/// <summary>
/// All possible events for which an NPC will Think.UpdateAttitude() for.
/// </summary>
public enum NpcMemoryChangeEvent
{
    /// <summary>
    /// Received damage from something.
    /// </summary>
    HealthDamage,
    /// <summary>
    /// Ate some food.
    /// </summary>
    FoodEaten,
    /// <summary>
    /// Surveyed a location. <para />
    /// Happens every time a location is encountered whether it is new or already known.
    /// </summary>
    LocationFound
}

