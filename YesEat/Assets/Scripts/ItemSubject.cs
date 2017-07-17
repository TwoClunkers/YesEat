using System;
using UnityEngine;

/// <summary>
/// The Item Subject should describe and encapsulate everything that can be placed in inventory
/// </summary>

public class ItemSubject : Subject
{
	#region Private members
	//private int[] qualities;
	private BuildRecipe buildDirections;
	//private int bulk;
	//private int weight;
	#endregion

	public ItemSubject () : base()
	{
		name = new String("Item");
		description = new String("An Item is anything that can fit in your inventory");
		icon = new Sprite();

		//qualities = new int[2];
		buildDirections = new BuildRecipe ();

	}
		
	/// <summary>
	/// The BuildRecipe used to make this Item
	/// </summary>
	public BuildRecipe BuildDirections
	{
		get { return buildDirections; }
		set { buildDirections = value; }
	}


}



