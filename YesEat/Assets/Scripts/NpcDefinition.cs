
using System.Collections.Generic;

public partial class NpcCharacter
{

    /// <summary>
    /// Resource pool limits, thresholds for triggering changes in priorities.
    /// </summary>
    public class NpcDefinition
    {
        public List<Subject> subjectsKnown;
        public List<SubjectAttitude> attitudes;
        public float MetabolizeInterval = 0.0f;
        public int HealthRegen = 0;
        public int StarvingDamage = 0;

        public int HealthDanger = 0;
        public int HealthMax = 0;
        public int FoodHungry = 0;
        public int FoodMax = 0;
        public int SafetyDeadly = 0;
        public int SafetyHigh = 0;
    }
}
