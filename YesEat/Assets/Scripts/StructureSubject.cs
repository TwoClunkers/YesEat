using System;
using UnityEngine;

/// <summary>
/// Structures are built
/// </summary>

public class StructureSubject : Subject
{
	#region Private members
	private BuildRecipe buildDirections;
	#endregion

	public StructureSubject () : base()
	{
		name = new String("Structure");
		description = new String("A Structure that is Built");
		icon = new Sprite();

		buildDirections = new BuildRecipe ();
	}

	/// <summary>
	/// The BuildRecipe used to make this Structure
	/// </summary>
	public BuildRecipe BuildDirections
	{
		get { return buildDirections; }
		set { buildDirections = value; }
	}


}

