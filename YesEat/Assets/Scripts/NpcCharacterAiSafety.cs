

public partial class NpcCharacter
{
    private void AiCoreSubprocessSafety()
    {
        //|     []Safety:
        //|         []Source of danger
        //|             []Low Health
        //|                 If hunger is below Hunger trigger set hunger as top priority
        //|             []Received Damage
        //|                 []Known attacker?
        //|                     Check knownSubjects
        //|                     [No]Save attitude: bad / danger
        //|                         Think.GotHurtBy(Subject, damageAmount) - adjust attitude based 
        //|                         on how much damage was done and how we feel about this subject
        //|                     [Yes]Fight?
        //|                         [No]Search for safe location
        //|                             Think.FindNearest(FindEnum.SafeLocation) - check knownSubjects for a location that is safe
        //|                             travel to safe location or search for presently unknown location that is safe
        //|                             ()Return
        //|                         [Yes]Attack damage source
        //|                             set fighting state, save current subject to combat target list
        //|                             inflict damage on combat target
        //|                             ()Return
    }
}