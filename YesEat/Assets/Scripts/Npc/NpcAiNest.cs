using System;

public partial class NpcCore
{
    private void AiCoreSubprocessNest()
    {
        throw new NotImplementedException();
        //|     []Nest:
        //|         []Current location qualifies for nesting?
        //|             [No]Search for nesting location
        //|                 ()Return
        //|             [Yes]Have Item for building nest?
        //|                 [No]Search for nest building item
        //|                     []Collect nest building item
        //|                         ()Return
        //|                 [Yes]Build nest
        //|                     []Done building nest?
        //|                         (No)Return
        //|                         [Yes]Save nest location to memory
        //|                             []Remove nest from drivers
    }
}