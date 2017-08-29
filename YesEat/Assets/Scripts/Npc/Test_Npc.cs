using UnityEngine;
using System.Collections.Generic;

public partial class NpcCore
{
    /// <summary>
    /// Testing: Directly override health, safety, and food values. Leave blank or NULL any value you don't wish to change.
    /// </summary>
    /// <param name="newHealth">New health value.</param>
    /// <param name="newSafety">New safety value.</param>
    /// <param name="newFood">New food value.</param>
    /// <param name="newEndurance">New endurance value.</param>
    public void T_SetValues(int? newHealth = null, int? newSafety = null, int? newFood = null, int? newEndurance = null)
    {
        health = newHealth ?? health;
        safety = newSafety ?? safety;
        food = newFood ?? food;
        endurance = newEndurance ?? endurance;
    }

    public void T_DeleteMemories()
    {
        definition.Memories.Clear();
    }

    public SubjectMemory T_GetMemory(int getSubjectId)
    {
        return definition.Memories.Find(o => o.SubjectID == getSubjectId);
    }

    public List<SubjectMemory> T_Memories { get { return definition.Memories; } set { definition.Memories = value; } }
}
