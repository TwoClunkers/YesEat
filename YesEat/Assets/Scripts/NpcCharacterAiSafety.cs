

public partial class NpcCharacter
{
    private void AiCoreSubprocessSafety()
    {
        //|     []Safety:
        //|         []Source of danger
        //|             []Low Health
        //|                 If hunger is below Hunger trigger set hunger as top priority

        // I think this should be removed, Metabolize should set this driver reliably
        if (food < definition.FoodHungry)
            drivers.SetTopDriver(NpcDrivers.Hunger);

        // I think receiving damage should trigger the logic below, it shouldn't be part of the AiCoreProcess()

        // received damage logic moved to NpcCharacter.Harm()
    }
}