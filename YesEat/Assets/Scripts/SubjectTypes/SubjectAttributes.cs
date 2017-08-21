using System.Collections;
using System.Collections.Generic;

[System.Flags]
public enum SubjectAttributesEnum
{
    LooksBright = (1 << 1),
    LooksDark = (1 << 2),

    SoundsLoud = (1 << 3),
    SoundsQuiet = (1 << 4),

    FeelsHard = (1 << 5),
    FeelsSoft = (1 << 6),

    TastesSweet = (1 << 7),
    TastesSalty = (1 << 8),

    SmellsFloral = (1 << 9),
    SmellsPungent = (1 << 10)
}

public class SubjectAttributes
{
    private SubjectAttributesEnum attribs;

    public SubjectAttributes() { }

    public SubjectAttributes(SubjectAttributesEnum copyAttributes) { this.attribs = copyAttributes; }

    public SubjectAttributesEnum Attributes { get { return attribs; } set { attribs = value; } }

    /// <summary>
    /// Turns on the changeAttribute flag.
    /// </summary>
    /// <param name="changeAttribute">The flag to turn on.</param>
    public void SetAttribute(SubjectAttributesEnum changeAttribute) { attribs |= changeAttribute; }

    /// <summary>
    /// Turns off the changeAttribute flag.
    /// </summary>
    /// <param name="changeAttribute">The flag to be turned off.</param>
    public void UnsetAttribute(SubjectAttributesEnum changeAttribute) { attribs &= ~changeAttribute; }

    /// <summary>
    /// Check if checkAttribute is set.
    /// </summary>
    /// <param name="checkAttribute">The Attribute to check for.</param>
    /// <returns>True:set, False: not set.</returns>
    public bool HasAttribute(SubjectAttributesEnum checkAttribute) { return (attribs & checkAttribute) != 0; }

}
