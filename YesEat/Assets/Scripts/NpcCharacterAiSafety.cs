

public partial class NpcCharacter
{
    private void AiCoreSubprocessSafety()
    {
        //|     []Safety:
        //|         []Source of danger
        //|             []Low Health
        //|                 
        //|                 []Have Food?
        //|                     [Yes]Eat food
        //|                     [No]Have Nest?
        //|                         [No]Search for safe location
        //|                         [Yes]Goto nest
        //|                             []Nest safe?
        //|                                 [Yes]Remove safety from drivers
        //|                                     ()Return
        //|                                 [No]Search for safe location
        //|                                     ()Return
        //|             []Received Damage
        //|                 []Known attacker?
        //|                     [No]Save attitude: bad / danger
        //|                     [Yes]Fight?
        //|                         [No]Search for safe location
        //|                             ()Return
        //|                         [Yes]Attack damage source
        //|                             ()Return
    }
}