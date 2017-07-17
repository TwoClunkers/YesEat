
public partial class NpcCharacter {

    /// <summary>
    /// Resource pool limits, thresholds for triggering changes in priorities.
    /// </summary>
    public class TriggerSet
    {
        public int HealthDanger = 0;
        public int HealthMax = 0;
        public int FoodHungry = 0;
        public int FoodMax = 0;
        public int SafetyDeadly = 0;
        public int SafetyHigh = 0;
    }
}
