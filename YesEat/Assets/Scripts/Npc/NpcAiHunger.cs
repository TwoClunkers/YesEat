using System;

public partial class NpcCore
{
    private void AiCoreSubprocessHunger()
    {
        throw new NotImplementedException();
        //Hunger steps

        //Selection of Available Food
        //  Get list of food in inventory
        //  Send selection to: public bool Eat(InventoryItem FoodItem)

        //Harvesting food Action
        //  Move within range of targeted Bush
        //  Attempt a "Take" on the targeted inventory
        //  If successful go to Selection of Food

        //Moving to Bush Action
        //  Look at bushes in known area
        //  Take position of closest non-empty
        //  If successful move to location and go to Harvesting

        //Moving to Location associated with food or Bushes?
        //  Think about locations associated with bushes or food
        //  If found, move to closest location and go to Moving to Bush

        //Searching Unknown Locations
        //  Think about Unknown locations
        //  If found, move to closest of locations to explore and go to Moving to Location

    }

}