
using System;
using UnityEngine;

/// <summary>
/// Base Class for all subject types. 
/// </summary>

public class Subject
{
	#region Private members
	protected string name;
	protected string description;
	protected Sprite icon;
	protected int[] relatedSubjects;
	#endregion

	public Subject ()
	{
		name = new String("Name");
		description = new String("This is the description");
		icon = new Sprite();
		relatedSubjects = new int[0];

	}

	//for use by derived classes
	public Subject (String newName, String newDescription, Sprite newSprite, int[] newRelated)
	{
		name = newName;
		description = newDescription;
		icon = newSprite;
		relatedSubjects = newRelated;
	}

	public string Name
	{
		get { 
			string temp;
			temp = String.Copy (name);
			return temp; }
		set { 
			string temp;
			temp = value;
			if (!String.IsNullOrEmpty (temp)) {
				name = String.Copy (temp);	}	}
	}

	public string Description
	{
		get { 
			string temp;
			temp = String.Copy (description);
			return temp; }
		set { 
			string temp;
			temp = value;
			if (!String.IsNullOrEmpty (temp)) {
				description = String.Copy (temp);
			} else
				description = String.Empty;
			}
	}

	//For later UI developement
	public Sprite Icon
	{
		get { return icon; }
		set { icon = value; }
	}

	//this is also likely to be used in later development
	public int[] RelatedSubjects 
	{
		get { return relatedSubjects; }
		set { relatedSubjects = value; }
	}



}


