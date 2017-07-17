
using System;
using UnityEngine;

/// <summary>
/// Food Subject to describe our items that are used as food
/// </summary>

public class FoodSubject : ItemSubject
{
	#region Private members
	private int foodType;
	private int maxFoodValue;
	#endregion

	public FoodSubject () : base()
	{
		name = new String("Food");
		description = new String("Something Eaten");
		icon = new Sprite();

		foodType = 0;
		maxFoodValue = 1;
	}

	/// <summary>
	/// What kind of food is this?
	/// </summary>
	public int FoodType
	{
		get { return foodType; }
		set { foodType = value; }
	}
		
	/// <summary>
	/// The how filling is this item
	/// </summary>
	public int MaxFoodValue
	{
		get { return maxFoodValue; }
		set { maxFoodValue = value; }
	}

}


