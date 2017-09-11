using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gene
{
    #region Private members
    private float spanVariance;
    private float unitVariance;
    private List<GeneUnit> genes;
    private int readPosition;
    #endregion

    public Gene()
    {
        readPosition = 0;
        genes = new List<GeneUnit>();
        SetVariance(1.0f);

    }

    public Gene(int length)
    {
        readPosition = 0;
        genes = new List<GeneUnit>();
        SetVariance(1.0f);
        CreateRandom(length);
    }

    public enum GeneUnit
    {
        A=0, B=1, C=2, D=3, E=4, F=5, G=6, H=7, I=8, J=9, K=10, L=11, M=12, N=13, O=14, P=15,
        Q=16, R=17, S=18, T=19, U=20, V=21, W=22, X=23, Y=24, Z=25
    }

    public float Unit(GeneUnit geneUnit)
    {
        return (int)geneUnit * unitVariance;
    }

    /// <summary>
    /// ReadUnit returns the float at the next position or specified position.
    /// NOTE: Will wrap on overflow (position farther than gene count).
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public float ReadFloat(int position = -1)
    {
        if (genes.Count < 1) return 0.1f;

        if (position < 0)
        { //by default we use a readPosition
            position = readPosition;
        }
        position = position % genes.Count;
        readPosition = position + 1;

        GeneUnit geneUnit = genes[position];
        return (int)geneUnit * unitVariance;
    }

    public string ReadCharacter(int position = -1)
    {
        if (genes.Count < 1) return "";

        if (position < 0)
        { //by default we use a readPosition
            position = readPosition;
        }
        position = position % genes.Count;
        readPosition = position + 1;

        GeneUnit geneUnit = genes[position];
        return geneUnit.ToString();
    }

    public Color32 GetColor(int position = -1)
    {
        int index = 1;
        if (position < 0) index = 0;
        byte red = (byte)Mathf.Lerp(64, 255, ReadFloat(position));
        byte green = (byte)Mathf.Lerp(64, 255, ReadFloat(position+index));
        byte blue = (byte)Mathf.Lerp(64, 255, ReadFloat(position+index*2));
        byte alpha = (byte)Mathf.Lerp(128, 255, ReadFloat(position+index*3));
        Color32 newColor = new Color32(red, blue, green, alpha);

        return newColor;
    }

    public void SetVariance(float newVariance)
    {
        spanVariance = newVariance;
        unitVariance = newVariance / 25;
    }

    public void AddGene(GeneUnit newUnit)
    {
        genes.Add(newUnit);
    }

    public void AddGene(float newFloat)
    {
        float percent = newFloat / spanVariance;
        GeneUnit position = (GeneUnit)(25 * percent);

        genes.Add(position);
    }

    public void AddGene(string newChar)
    {
        GeneUnit newUnit = new GeneUnit();
        newUnit = (GeneUnit)System.Enum.Parse(typeof(GeneUnit), newChar.ToUpper());

        genes.Add(newUnit);
    }

    public void AddString(string newSequence)
    {
        char[] charSequence = newSequence.ToCharArray();

        for (int i = 0; i < newSequence.Length; i++)
        {
            AddGene(charSequence[i].ToString());
        }
    }

    public int Size()
    {
        return genes.Count;
    }

    public float[] GetSequence(int reqestedLength, int position = -1)
    {
        if (genes.Count < 1) return null;
        float[] sequence = new float[reqestedLength];

        for (int i = 0; i < reqestedLength; i++)
        {
            if (position > -1) position = i;
            sequence[i] = ReadFloat(position);
        }
        return sequence;
    }

    public string GetString(int requestedLength = -1, int position = -1)
    {
        if (genes.Count < 1) return null;
        List<string> sequence = new List<string>();
        if (requestedLength < 1) requestedLength = genes.Count;

        for (int i = 0; i < requestedLength; i++)
        {
            if (position > -1) position += i;
            sequence.Add(ReadCharacter(position));
        }

        return string.Concat(sequence.ToArray());
    }

    public void CreateRandom(int length)
    {
        genes.Clear();
        for (int i = 0; i < length; i++)
        {
            AddGene(Random.value);
        }
    }

    public float Value(float source)
    {
        return Mathf.Lerp(0.0f, source, ReadFloat());
    }

    public int Value(int source)
    {
        return (int)Mathf.Lerp(0, source, ReadFloat());
    }


}
