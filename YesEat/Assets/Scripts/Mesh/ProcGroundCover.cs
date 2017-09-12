using UnityEngine;
using System.Collections;

public class ProcGroundCover : ProcBase
{
    //base class for the data for each part of the flower:
    [System.Serializable]
    public class PartData
    {
        //should this part be generated?
        public bool m_Build = true;
    }

    //contains data for building a sphere:
    [System.Serializable]
    public class SphereData : PartData
    {
        //radius of the sphere:
        public float m_Radius = 0.05f;

        //vertical scale value to apply to the sphere:
        public float m_VerticalScale = 0.2f;

        //the number of radial segments:
        public int m_RadialSegmentCount = 10;

        //the number of height segments:
        public int m_HeightSegmentCount = 10;
    }

    //creation variables
    Vector3 currentPosition = new Vector3();
    Quaternion currentRotation = new Quaternion();
    public SphereData m_HeadData;

    private void Awake()
    {
        currentPosition = Vector3.zero;
        currentRotation = Quaternion.identity;
        m_HeadData = new SphereData();
        m_variant = Random.Range(1, 10);
    }

    /// <summary>
    /// Method for setting variables that only change when Gene is assigned. 
    /// </summary>
    /// <param name="newGene"></param>
    public override void AssignGene(Gene newGene)
    {
        if (newGene != null) m_Gene = newGene;
        else return;

        m_HeadData.m_Radius = 0.1f + m_GrowthIndex * m_Gene.ReadFloat(0) * 2;
        m_HeadData.m_VerticalScale = 0.03f + (m_Gene.ReadFloat(1) * 0.08f);
    }

    /// <summary>
    /// Method for updating variables that change with time
    /// </summary>
    public override void UpdateValues()
    {
        m_HeadData.m_Radius = 0.2f + m_GrowthIndex * m_Gene.ReadFloat(0);
        m_HeadData.m_VerticalScale = 0.01f + (m_GrowthIndex * m_Gene.ReadFloat(1) * 0.08f);
    }

    //Build the mesh:
    public override Mesh BuildMesh()
    {
        //Create a new mesh builder:
        MeshBuilder meshBuilder = new MeshBuilder();

        //build blob
        BuildHead(meshBuilder, currentPosition, currentRotation, m_HeadData);

        return meshBuilder.CreateMesh();
    }

    private void BuildHead(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, SphereData partData)
    {
        //bail if this part has been disabled:
        if (!partData.m_Build)
            return;

        //the angle increment per height segment:
        float angleInc = Mathf.PI / partData.m_HeightSegmentCount;

        //the vertical (scaled) radius of the sphere:
        //float verticalRadius = partData.m_Radius * partData.m_VerticalScale;

        //build the rings:
        for (int i = (int)(partData.m_HeightSegmentCount*0.5f); i <= partData.m_HeightSegmentCount; i++)
        {
            Vector3 centrePos = Vector3.zero;

            //calculate a height offset and radius based on a vertical circle calculation:
            centrePos.y = -Mathf.Cos(angleInc * i);
            float radius = Mathf.Sin(angleInc * i);

            //calculate the slope of the shpere at this ring based on the height and radius:
            Vector2 slope = new Vector3(-centrePos.y / partData.m_VerticalScale, radius);
            slope.Normalize();

            //multiply the unit height by the vertical radius, and then add the radius to the height to make this sphere originate from its base rather than its centre:
            //centrePos.y = centrePos.y * verticalRadius + verticalRadius;

            //scale the radius by the one stored in the partData:
            radius *= partData.m_Radius;

            //calculate the final position of the ring centre:
            Vector3 finalRingCentre = rotation * centrePos + offset;

            //V coordinate:
            float v = (float)i / partData.m_HeightSegmentCount;

            //build the ring:
            BuildRing(meshBuilder, partData.m_RadialSegmentCount, finalRingCentre, radius, v, i > (int)(partData.m_HeightSegmentCount * 0.5f), rotation, slope);
        }
    }



}
