using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public partial class NpcCore
{
    private void AiCoreSubprocessHunger()
    {
        //Hunger steps

        //Selection of Available Food
        //  Get list of food in inventory
        int foodSubjectID = 4; // berry
        int foodSourceID = 3; // bush
        if (definition.Traits.HasTrait(NpcTraits.Herbivore))
        {
            foodSubjectID = 4; // berry
            foodSourceID = 3; // bush
        }
        else if (definition.Traits.HasTrait(NpcTraits.Carnivore))
        {
            foodSubjectID = 5; // meat
            foodSourceID = 1; // plinkett
        }

        InventoryItem foodItem = objectScript.Inventory.Take(new InventoryItem(foodSubjectID, 1));
        if (foodItem.StackSize > 0)
        {
            // eat food in inventory
            Eat(foodItem);
        }
        else
        {
            // if it's been a long time since last search clear out attemptedHarvest list
            lastFoodSearch += Time.deltaTime;
            if (lastFoodSearch > 30) attemptedHarvest.Clear();

            // get list of all foodSource objects in near range
            // exclude objects we've already attempted harvesting from
            List<SubjectObjectScript> foodSource = considerObjects
                .Select(o => o.GetComponent<SubjectObjectScript>() as SubjectObjectScript)
                .Where(o => o.Subject.SubjectID == foodSourceID)
                .Where(o => !attemptedHarvest.Contains(o.GetInstanceID())).ToList();
            // if any food sources are in range, harvest from them
            bool gotFood = false;
            foreach (SubjectObjectScript foodSourceObject in foodSource)
            {
                // if it's within harvest range
                if (Vector3.Distance(foodSourceObject.transform.position, objectScript.transform.position) <= 1)
                {
                    //  Attempt to harvest from the food source
                    InventoryItem harvestedItem = foodSourceObject.Harvest();
                    if (harvestedItem != null)
                    {
                        if (harvestedItem.StackSize > 0)
                        {
                            objectScript.Inventory.Add(harvestedItem);
                            gotFood = true;
                        }
                        else
                        {
                            // didn't get anything from this source
                            attemptedHarvest.Add(foodSourceObject.GetInstanceID());
                        }
                    }
                    else
                    {
                        attemptedHarvest.Add(foodSourceObject.GetInstanceID());
                    }
                }
            }
            if (!gotFood)
            {
                // didn't get any food from sources within reach, 
                /// TODO: start moving to food sources within short range sight
                if (foodSource.Count() > 0)
                {
                    // Move to food source

                }
                // find a location with foodSourceID
                LocationSubject foodLocation = FindNearestLocationOfObject(db.GetSubject(foodSourceID), objectScript.transform.position);
                // travel to the location if we are not already there
                if (objectScript.Location.SubjectID != foodLocation.SubjectID)
                {
                    objectScript.MoveToNewLocation(foodLocation);
                }
                else
                {
                    // already at location for food
                }
            }

            //Searching Unknown Locations
            //  Think about Unknown locations
            //  If found, move to closest of locations to explore and go to Moving to Location
        }
    }

}