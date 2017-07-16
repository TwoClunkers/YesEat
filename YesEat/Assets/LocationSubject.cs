
using System;
using UnityEngine;

/// <summary>
/// Location Subject extends the Subject Class to describe locations for AI
/// </summary>

public class LocationSubject : Subject
{
	#region Private members
	private Vector3 coordinates;
	private int radius;
	private int layer;
	#endregion

	public LocationSubject () : base()
	{
		name = new String("Location");
		description = new String("This is the Location Description");
		icon = new Sprite();

		coordinates = new Vector3(1, 1, 1);
		radius = 8;
		layer = 0;
	}

	//This would represent the center of the located area
	public Vector3 Coordinates
	{
		get { 
			return new Vector3 (coordinates.x, coordinates.y, coordinates.z); }
		set {
			coordinates = value;	}
	}

	//The radius of the area (around the y axis)
	public int Radius
	{
		get { 
			return radius;	}
		set { 
			radius = value;	}
	}

	//The layer should allow for smaller divisions to overlap larger defined areas
	public int Layer 
	{
		get { return layer;	}
		set { layer = Mathf.Max (0, value); }
	}
		

}


