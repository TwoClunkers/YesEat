﻿
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

    /// <summary>
    /// Create new LocationSubject object.
    /// </summary>
	public LocationSubject () : base()
	{
		name = new String("Location");
		description = new String("This is the Location Description");
		icon = new Sprite();

		coordinates = new Vector3(1, 1, 1);
		radius = 8;
		layer = 0;
	}

    /// <summary>
    /// The center of the location.
    /// </summary>
    public Vector3 Coordinates
	{
		get { return new Vector3(coordinates.x, coordinates.y, coordinates.z); }
		set { coordinates = value; }
	}

    /// <summary>
    /// The radius of the location. (around the y axis)
    /// </summary>
    public int Radius
	{
		get { return radius; }
        set { radius = value; }
	}

    /// <summary>
    /// The layer should allow for smaller divisions to overlap larger defined areas
    /// </summary>
    public int Layer 
	{
		get { return layer;	}
		set { layer = value; }
	}
		

}

