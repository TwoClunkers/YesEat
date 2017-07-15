using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NpcCharacter : MonoBehaviour {

    private TransientStates states;
    private int health = 0;
    private int mind = 0;
    private int stamina = 0;
    private int fluid = 0;
    private int food = 0;
    private int air = 0;
    private int safety = 0;
    private int endurance = 0;
    private int sanity = 0;
    private int aggression = 0;
    private int temperament = 0;
    private int focus = 0;
    private List<Subject> subjectsKnown = new List<Subject>();

    /// <summary>
    /// Current transient states.
    /// </summary>
    public TransientStates States
    {
        get { return states; }
        set { states = value; }
    }

    /// <summary>
    /// Current health.
    /// </summary>
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    
    /// <summary>
    /// Current mind.
    /// </summary>
    public int Mind
    {
        get { return mind; }
        set { mind = value; }
    }
    
    /// <summary>
    /// Current stamina.
    /// </summary>
    public int Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    
    /// <summary>
    /// Current fluid (water).
    /// </summary>
    public int Water
    {
        get { return fluid; }
        set { fluid = value; }
    }
    
    /// <summary>
    /// Current food.
    /// </summary>
    public int Food
    {
        get { return food; }
        set { food = value; }
    }
    
    /// <summary>
    /// Current air (breath).
    /// </summary>
    public int Air
    {
        get { return air; }
        set { air = value; }
    }
    
    /// <summary>
    /// All subjects known by this character.
    /// </summary>
    public List<Subject> SubjectsKnown
    {
        get { return subjectsKnown; }
        set { subjectsKnown = value; }
    }
    
    /// <summary>
    /// Current perception of safety.
    /// </summary>
    public int Safety
    {
        get { return safety; }
        set { safety = value; }
    }
    
    /// <summary>
    /// Current endurance.
    /// </summary>
    public int Endurance
    {
        get { return endurance; }
        set { endurance = value; }
    }
    
    /// <summary>
    /// Current sanity.
    /// </summary>
    public int Sanity
    {
        get { return sanity; }
        set { sanity = value; }
    }

    /// <summary>
    /// Current aggression tendency.
    /// </summary>
    public int Aggression
    {
        get { return aggression; }
        set { aggression = value; }
    }
    
    /// <summary>
    /// Current temperament.
    /// </summary>
    public int Temperament
    {
        get { return temperament; }
        set { temperament = value; }
    }

    public int Focus
    {
        get { return focus; }
        set { focus = value; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
