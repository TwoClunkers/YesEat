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
    public float ReadUnit(int position = -1)
    {
        if (position < 0)
        { //by default we use a readPosition
            position = readPosition;
            readPosition += 1;
            if (!(readPosition < genes.Count))
            { //set readPosition to front
                readPosition = 0;
            }
        }
        else position = position % genes.Count;

        GeneUnit geneUnit = genes[position];
        return (int)geneUnit * unitVariance;
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
            sequence[i] = ReadUnit(position);
        }
        return sequence;
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
        return Mathf.Lerp(0.0f, source, ReadUnit());
    }

    public int Value(int source)
    {
        return (int)Mathf.Lerp(0, source, ReadUnit());
    }
}
