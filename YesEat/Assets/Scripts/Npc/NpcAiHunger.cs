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
            if (lastFoodSearch > 30)
            {
                searchedObjects.Clear();
                searchedLocations.Clear();
            }

            // get list of all foodSource objects in near range
            // exclude objects we've already attempted harvesting from
            List<SubjectObjectScript> foodSource = considerObjects
                .Select(o => o.GetComponent<SubjectObjectScript>() as SubjectObjectScript)
                .Where(o => o.Subject.SubjectID == foodSourceID)
                .Where(o => !searchedObjects.Contains(o.GetInstanceID())).ToList();

            // go to the first food source and harvest from it
            if (foodSource.Count > 0)
            {
                SubjectObjectScript foodSourceObject = foodSource[0];

                // if it's within harvest range
                if (Vector3.Distance(foodSourceObject.transform.position, objectScript.transform.position) <= 0.5)
                {
                    // we're within range, stop chasing
                    objectScript.ChaseStop();
                    //  Attempt to harvest from the food source
                    InventoryItem harvestedItem = foodSourceObject.Harvest();
                    if (harvestedItem != null)
                    {
                        if (harvestedItem.StackSize > 0)
                        {
                            objectScript.Inventory.Add(harvestedItem);
                        }
                        else
                        {
                            // didn't get anything from this source
                            searchedObjects.Add(foodSourceObject.GetInstanceID());
                        }
                    }
                    else
                    {
                        searchedObjects.Add(foodSourceObject.GetInstanceID());
                    }
                }
                else //out of harvest range, chase this food source
                {
                    objectScript.ChaseStart(foodSourceObject);
                }
            }
            else
            {
                // there are no food sources in close range
                // find a location with foodSourceID
                searchedLocations.Add(objectScript.Location.SubjectID);
                List<LocationSubject> foodLocations = FindObject(db.GetSubject(foodSourceID), objectScript.transform.position, searchedLocations);
                if (foodLocations.Count > 0)
                {
                    LocationSubject foodLocation = foodLocations[0];
                    // travel to the location if we are not already there
                    if (objectScript.Location.SubjectID != foodLocation.SubjectID)
                    {
                        objectScript.MoveToNewLocation(foodLocation);
                    }
                }
                else
                {
                    // no known food locations were found, explore unexplored locations
                    if (unexploredLocations.Count > 0)
                    {
                        AiCoreSubprocessExplore();
                    }
                }
            }
        }
    }

}