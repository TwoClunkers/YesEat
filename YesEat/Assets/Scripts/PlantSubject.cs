﻿using System;
using UnityEngine;

/// <summary>
/// Plant Subject extends the Subject Class to describe plants for AI
/// </summary>

public class PlantSubject : Subject
{
    #region Private members
    private int produceID;
    private int produceTime;
    private int maxGrowth;
    private int growthTime;
    private int matureTime;
    #endregion

    public PlantSubject() : base()
    {
        name = "Plant";
        description = "Growing Thing";
        icon = new Sprite();

        produceID = 0;
        produceTime = 0;
        maxGrowth = 3;
        growthTime = 0;
        matureTime = 1;
    }

    /// <summary>
    /// What does this plant produce?
    /// </summary>
    public int ProduceID
    {
        get { return produceID; }
        set { produceID = value; }
    }

    /// <summary>
    /// How long does it take to produce anything?
    /// </summary>
    public int ProduceTime
    {
        get { return produceTime; }
        set { produceTime = value; }
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


