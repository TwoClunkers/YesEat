﻿using System;
using UnityEngine;

/// <summary>
/// Animal Subject extends the Subject Class to describe Animals for AI
/// </summary>
	
public class AnimalSubject : Subject
{
	#region Private members
	private int maxGrowth;
	private int growthTime;
	private int matureTime;
	#endregion

	public AnimalSubject () : base()
	{
		name = new String("Animal");
		description = new String("Moving Thing");
		icon = new Sprite();

		maxGrowth = 3;
		growthTime = 0;
		matureTime = 1;
	}

	/// <summary>
	/// Max Growth tells us at which step you stop growing
	/// </summary>
	public int MaxGrowth
	{
		get { return maxGrowth; }
		set { maxGrowth = value; }
	}

	/// <summary>
	/// GrowthTime tells us when you can take a "step" in growth process
	/// </summary>
	public int GrowthTime
	{
		get { return growthTime; }
		set { growthTime = value; }
	}

	/// <summary>
	/// MatureTime tells us at which step we can start producing
	/// </summary>
	public int MatureTime
	{
		get { return matureTime; }
		set { matureTime = value; }
	}




}


