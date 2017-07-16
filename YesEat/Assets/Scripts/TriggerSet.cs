
public partial class NpcCharacter {
    /// <summary>
    /// Resource pool limits, thresholds for triggering changes in priorities.
    /// </summary>
    public class TriggerSet
    {
        public int HealthFlee = 0;
        public int HealthDefensive = 0;
        public int HealthMax = 0;
        public int FoodStarving = 0;
        public int FoodRavenous = 0;
        public int FoodHungry = 0;
        public int FoodSated = 0;
        public int FoodMax = 0;
        public int SafetyDeadly = 0;
        public int SafetyCombative = 0;
        public int SafetyCautious = 0;
        public int SafetyAverage = 0;
        public int SafetyHigh = 0;
        public int SafetyMax = 0;
    }
}
