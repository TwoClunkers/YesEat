using UnityEngine;
using System.Collections;

public class ProcHerb : ProcBase
{
    //base class for the data for each part of the flower:
    [System.Serializable]
    public class PartData
    {
        //should this part be generated?
        public bool m_Build = true;
    }

    //leafy parts (petals, sepals etc.) of the flower:
    [System.Serializable]
    public class LeafPartData : PartData
    {
        //width and length:
        public float m_Width = 0.2f;
        public float m_Length = 0.3f;

        //the the angle to bend:
        public float m_BendAngle = 90.0f;

        //the starting angle:
        public float m_StartAngle = 0.0f;

        //the maximum amount of variation in the bend angle:
        public float m_BendAngleVariation = 10.0f;

        //the maximum amount of variation in the starting angle:
        public float m_StartAngleVariation = 0.0f;

        //the number of petals in a ring:
        public int m_Count = 6;

        //the number of width segments:
        public int m_WidthSegmentCount = 4;

        //the number of length segments:
        public int m_LengthSegmentCount = 4;

        //should the backfaces be built?
        public bool m_BuildBackfaces = true;
    }

    //contains data for building a cylinder:
    [System.Serializable]
    public class CylinderData : PartData
    {
        //the radii at the start and end of the cylinder:
        public float m_RadiusStart = 0.5f;
        public float m_RadiusEnd = 0.0f;

        //the height of the cylinder:
        public float m_Height = 2.0f;

        //the angle to bend the cylinder:
        public float m_BendAngle = 90.0f;

        //the number of radial segments:
        public int m_RadialSegmentCount = 5;

        //the number of height segments:
        public int m_HeightSegmentCount = 4;
    }

    //contains data for building a sphere:
    [System.Serializable]
    public class SphereData : PartData
    {
        //radius of the sphere:
        public float m_Radius = 0.05f;

        //vertical scale value to apply to the sphere:
        public float m_VerticalScale = 1.0f;

        //the number of radial segments:
        public int m_RadialSegmentCount = 10;

        //the number of height segments:
        public int m_HeightSegmentCount = 10;
    }

    //the parts of the flower:
    public CylinderData m_StemData;
    public SphereData m_HeadData;
    public LeafPartData m_SepalData;
    public LeafPartData m_PetalData;

    private void Awake()
    {
        m_StemData = new CylinderData();
        m_HeadData = new SphereData();
        m_SepalData = new LeafPartData();
        m_PetalData = new LeafPartData();
    }

    /// <summary>
    /// Method for setting variables that only change when Gene is assigned. 
    /// </summary>
    /// <param name="newGene"></param>
    public override void AssignGene(Gene newGene)
    {
        if (newGene != null) m_Gene = newGene;
        else return;

        m_PetalData.m_StartAngle = -30 + (-30 * m_Gene.ReadFloat(3));
        m_PetalData.m_Count = 2 + (int)(8 * m_Gene.ReadFloat(4));
        m_PetalData.m_StartAngleVariation = 20 * m_Gene.ReadFloat(5);
        m_PetalData.m_BendAngle = -5 * m_Gene.ReadFloat(2) - 1;
        m_PetalData.m_BendAngleVariation = 40 * m_Gene.ReadFloat(7);

        m_SepalData.m_StartAngle = -30 + (-30 * m_Gene.ReadFloat(3));
        m_SepalData.m_Count = 1 + (int)(5 * m_Gene.ReadFloat(6));
        m_SepalData.m_StartAngleVariation = 20 * m_Gene.ReadFloat(5);
        m_SepalData.m_BendAngle = -5 * m_Gene.ReadFloat(2) - 1;
        m_SepalData.m_BendAngleVariation = 40 * m_Gene.ReadFloat(7);

        m_StemData.m_BendAngle = -30 + (30 * m_Gene.ReadFloat(3));
        m_StemData.m_HeightSegmentCount = m_PetalData.m_Count;

        m_HeadData.m_Build = false;
        m_SepalData.m_Build = (0.5f < m_Gene.ReadFloat(0) * m_Gene.ReadFloat(8));

    }

    /// <summary>
    /// Method for updating variables that change with time
    /// </summary>
    public override void UpdateValues()
    {
        m_StemData.m_Height = 0.2f + (0.1f + m_Gene.ReadFloat(4)) * m_GrowthIndex;
        m_StemData.m_RadiusStart = 0.04f + (0.04f * m_Gene.ReadFloat(0)) * m_GrowthIndex;

        m_PetalData.m_Length = 0.3f + (0.2f + m_Gene.ReadFloat(1)) * m_GrowthIndex;
        m_PetalData.m_Width = 0.3f + (0.1f + m_Gene.ReadFloat(2)) * m_GrowthIndex * 0.5f;

        m_SepalData.m_Length = 0.3f + (0.2f + m_Gene.ReadFloat(1)) * m_GrowthIndex;
        m_SepalData.m_Width = 0.3f + (0.1f + m_Gene.ReadFloat(2)) * m_GrowthIndex * 0.2f;
    }

    public override Mesh BuildMesh()
    {
        MeshBuilder meshBuilder = new MeshBuilder();

        //store the current positions and rotations of the stem for future use:
        Vector3[] offsetList;
        Quaternion[] rotationList;

        //build the main stem:
        BuildStem(meshBuilder, out offsetList, out rotationList, m_StemData);
        //BuildHead(meshBuilder, currentPosition, currentRotation, m_HeadData);

        //build the sepals:
        //BuildLeafRing(meshBuilder, currentPosition, currentRotation, m_StemData.m_Radius * 0.3f, m_SepalData);

        //build the petals:
        BuildLeafRing(meshBuilder, offsetList, rotationList, 0.01f, m_PetalData);



        return meshBuilder.CreateMesh();
    }

    /// <summary>
    /// Builds the stem.
    /// </summary>
    /// <param name="meshBuilder">The mesh builder currently being added to.</param>
    /// <param name="currentOffset">Vector3 to store the position at the end of the stem.</param>
    /// <param name="currentRotation">Quaternion to store the rotation at the end of the stem.</param>
    /// <param name="partData">The parameters describing the cylinder to be built.</param>
    private void BuildStem(MeshBuilder meshBuilder, out Vector3[] offsetList, out Quaternion[] rotationList, CylinderData partData)
    {
        Vector3 currentPosition = new Vector3();
        Quaternion currentRotation = new Quaternion();
        currentPosition = Vector3.zero;
        currentRotation = Quaternion.identity;

        //bail if this part has been disabled:
        if (!partData.m_Build)
        {
            offsetList = new Vector3[0];
            rotationList = new Quaternion[0];
            return;
        }

        offsetList = new Vector3[partData.m_HeightSegmentCount];
        rotationList = new Quaternion[partData.m_HeightSegmentCount];

        //our bend code breaks if m_BendAngle is zero:
        if (partData.m_BendAngle == 0.0f)
        {
            //taper only:
            float heightInc = partData.m_Height / partData.m_HeightSegmentCount;

            //calculate the slope of the cylinder based on the height and difference between radii:
            Vector2 slope = new Vector2(partData.m_RadiusEnd - partData.m_RadiusStart, partData.m_Height);
            slope.Normalize();

            //build the rings:
            for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
            {
                //centre position of this ring:
                Vector3 centrePos = Vector3.up * heightInc * i;

                //V coordinate is based on height:
                float v = (float)i / partData.m_HeightSegmentCount;

                //interpolate between the radii:
                float radius = Mathf.Lerp(partData.m_RadiusStart, partData.m_RadiusEnd, (float)i / partData.m_HeightSegmentCount);

                //build the ring:
                BuildRing(meshBuilder, partData.m_RadialSegmentCount, centrePos, radius, v, i > 0, Quaternion.identity, slope);

                if(i>0)
                {
                    offsetList[i - 1] = new Vector3(centrePos.x, centrePos.y, centrePos.z);
                    rotationList[i - 1] = new Quaternion();
                }
            }
        }
        else
        {
            //bend and taper:

            //get the angle in radians:
            float bendAngleRadians = partData.m_BendAngle * Mathf.Deg2Rad;

            //the radius of our bend (vertical) circle:
            float bendRadius = partData.m_Height / bendAngleRadians;

            //the angle increment per height segment (based on arc length):
            float angleInc = bendAngleRadians / partData.m_HeightSegmentCount;

            //calculate a start offset that will place the centre of the first ring (angle 0.0f) on the mesh origin:
            //(x = cos(0.0f) * bendRadius, y = sin(0.0f) * bendRadius)
            Vector3 startOffset = new Vector3(bendRadius, 0.0f, 0.0f);

            //calculate the slope of the cylinder based on the height and difference between radii:
            Vector2 slope = new Vector2(partData.m_RadiusEnd - partData.m_RadiusStart, partData.m_Height);
            slope.Normalize();

            //build the rings:
            for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
            {
                //unit position along the edge of the vertical circle:
                Vector3 centrePos = Vector3.zero;
                centrePos.x = Mathf.Cos(angleInc * i);
                centrePos.y = Mathf.Sin(angleInc * i);

                //rotation at that position on the circle:
                float zAngleDegrees = angleInc * i * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, zAngleDegrees);

                //multiply the unit postion by the radius:
                centrePos *= bendRadius;

                //offset the position so that the base ring (at angle zero) centres around zero:
                centrePos -= startOffset;

                //interpolate between the radii:
                float radius = Mathf.Lerp(partData.m_RadiusStart, partData.m_RadiusEnd, (float)i / partData.m_HeightSegmentCount);

                //V coordinate is based on height:
                float v = (float)i / partData.m_HeightSegmentCount;

                //build the ring:
                BuildRing(meshBuilder, partData.m_RadialSegmentCount, centrePos, radius, v, i > 0, rotation, slope);

                if (i > 0)
                {
                    offsetList[i - 1] = new Vector3(centrePos.x, centrePos.y, centrePos.z);
                    rotationList[i - 1] = rotation;
                }
            }
        }
    }

    /// <summary>
    /// Builds the "head" of the flower (a sphere that sits on top of the stem).
    /// </summary>
    /// <param name="meshBuilder">The mesh builder currently being added to.</param>
    /// <param name="offset">The position offset to apply to the head (position at the top of the stem).</param>
    /// <param name="rotation">The rotation offset to apply to the head (rotation at the top of the stem).</param>
    /// <param name="partData">The parameters describing the sphere to be built.</param>
    private void BuildHead(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, SphereData partData)
    {
        //bail if this part has been disabled:
        if (!partData.m_Build)
            return;

        //the angle increment per height segment:
        float angleInc = Mathf.PI / partData.m_HeightSegmentCount;

        //the vertical (scaled) radius of the sphere:
        float verticalRadius = partData.m_Radius * partData.m_VerticalScale;

        //build the rings:
        for (int i = 0; i <= partData.m_HeightSegmentCount; i++)
        {
            Vector3 centrePos = Vector3.zero;

            //calculate a height offset and radius based on a vertical circle calculation:
            centrePos.y = -Mathf.Cos(angleInc * i);
            float radius = Mathf.Sin(angleInc * i);

            //calculate the slope of the shpere at this ring based on the height and radius:
            Vector2 slope = new Vector3(-centrePos.y / partData.m_VerticalScale, radius);
            slope.Normalize();

            //multiply the unit height by the vertical radius, and then add the radius to the height to make this sphere originate from its base rather than its centre:
            centrePos.y = centrePos.y * verticalRadius + verticalRadius;

            //scale the radius by the one stored in the partData:
            radius *= partData.m_Radius;

            //calculate the final position of the ring centre:
            Vector3 finalRingCentre = rotation * centrePos + offset;

            //V coordinate:
            float v = (float)i / partData.m_HeightSegmentCount;

            //build the ring:
            BuildRing(meshBuilder, partData.m_RadialSegmentCount, finalRingCentre, radius, v, i > 0, rotation, slope);
        }
    }

    /// <summary>
    /// Builds a ring of leafy parts (petals, sepals, etc.).
    /// </summary>
    /// <param name="meshBuilder">The mesh builder currently being added to.</param>
    /// <param name="offset">The position offset to apply (position at the top of the stem).</param>
    /// <param name="rotation">The rotation offset to apply(rotation at the top of the stem).</param>
    /// <param name="radius">The radius at the top of the stem.</param>
    /// <param name="partData">The parameters describing the part to be built.</param>
    private void BuildLeafRing(MeshBuilder meshBuilder, Vector3[] offsetList, Quaternion[] rotationList, float radius, LeafPartData partData)
    {
        //bail if this part has been disabled:
        if (!partData.m_Build)
            return;

        Vector3 offset = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        float cumulativeAngle = 0;

        for (int i = 0; i < partData.m_Count; i++)
        {
            if(i < offsetList.Length)
            {
                offset = offsetList[i];
                rotation = rotationList[i];
            }

            //calculate the rotation of this part:
            cumulativeAngle = cumulativeAngle+137.5f;
            Quaternion radialRotation = rotation * Quaternion.Euler(0.0f, cumulativeAngle, 0.0f);

            //set the postion at the top of the stem, away from the middle:
            Vector3 position = offset + radialRotation * (Vector3.forward * radius);

            //calculate a bend angle with random variation:
            float bendAngleRandom = (partData.m_BendAngleVariation * m_Gene.ReadFloat(i + 5)) - partData.m_BendAngleVariation * 0.5f;
            float bendAngle = partData.m_BendAngle + bendAngleRandom;

            //calculate a starting angle with random variation:
            float startAngleRandom = (partData.m_StartAngleVariation * m_Gene.ReadFloat(i + 5)) - partData.m_StartAngleVariation * 0.5f;
            float startAngle = partData.m_StartAngle + startAngleRandom;

            //build the leaf part:
            BuildLeafPart(meshBuilder, position, radialRotation * Quaternion.Euler(0, 45 - 90 * m_Gene.ReadFloat(9), 0), partData, false, bendAngle, startAngle);
        }
    }

    /// <summary>
    /// Build a single leaf part (petals, sepals, etc.).
    /// </summary>
    /// <param name="meshBuilder">The mesh builder currently being added to.</param>
    /// <param name="offset">The position offset to apply (position at the base of the leaf part).</param>
    /// <param name="rotation">The rotation offset to apply.</param>
    /// <param name="partData">The parameters describing the part to be built.</param>
    /// <param name="isBackFace">Is this the back side of the part?</param>
    /// <param name="bendAngle">The bend angle</param>
    /// <param name="startAngle">The starting angle.</param>
    private void BuildLeafPart(MeshBuilder meshBuilder, Vector3 offset, Quaternion rotation, LeafPartData partData, bool isBackFace, float bendAngle, float startAngle)
    {
        //get the angle in radians:
        float bendAngleRadians = bendAngle * Mathf.Deg2Rad;

        //the radius of our bend (vertical) circle:
        float bendRadius = partData.m_Length / bendAngleRadians;

        //the angle increment per height segment (based on arc length):
        float angleInc = bendAngleRadians / partData.m_LengthSegmentCount;

        //get the starting angle in radians:
        float startAngleRadians = startAngle * Mathf.Deg2Rad;

        //calculate a startOffset based on the starting angle:
        Vector3 startOffset = Vector3.zero;
        startOffset.y = Mathf.Cos(startAngleRadians) * bendRadius;
        startOffset.z = Mathf.Sin(startAngleRadians) * bendRadius;

        //a multiplier to reverse some values for the back of the leaf part:
        float backFaceMultiplier = isBackFace ? -1.0f : 1.0f;

        //build the rows:
        for (int i = 0; i <= partData.m_LengthSegmentCount; i++)
        {
            //V coordinate:
            float v = (1.0f / partData.m_LengthSegmentCount) * i;

            //width of the current row, scaled to shape the leaf part:
            float localWidth = partData.m_Width * Mathf.Sin(v * Mathf.PI) * backFaceMultiplier;
            ////use this instead for rectangular leaves:
            //float localWidth = partData.m_Width * backFaceMultiplier;

            //offset the x value to put the origin of the leaf part at bottom-centre:
            float xOffset = -localWidth * 0.5f;

            //unit position along the edge of the vertical circle:
            Vector3 centrePos = Vector3.zero;
            centrePos.y = Mathf.Cos(angleInc * i + startAngleRadians);
            centrePos.z = Mathf.Sin(angleInc * i + startAngleRadians);

            //rotation at that position on the circle:
            float bendAngleDegrees = (angleInc * i + startAngleRadians) * Mathf.Rad2Deg;
            Quaternion bendRotation = Quaternion.Euler(bendAngleDegrees, 0.0f, 0.0f);

            //multiply the unit postion by the radius:
            centrePos *= bendRadius;

            //offset the position so that the base row (at the starting angle) sits on zero:
            centrePos -= startOffset;

            //calculate the normal for this row:
            Vector3 normal = rotation * (bendRotation * Vector3.up) * backFaceMultiplier;

            //build the row:
            for (int j = 0; j <= partData.m_WidthSegmentCount; j++)
            {
                //X position:
                float x = (localWidth / partData.m_WidthSegmentCount) * j;

                //U coordinate:
                float u = (1.0f / partData.m_WidthSegmentCount) * j;

                //calculate the final position of this quad:
                Vector3 position = offset + rotation * new Vector3(x + xOffset, centrePos.y, centrePos.z);

                Vector2 uv = new Vector2(u, v);
                bool buildTriangles = i > 0 && j > 0;

                //build the quad:
                BuildQuadForGrid(meshBuilder, position, uv, buildTriangles, partData.m_WidthSegmentCount + 1, normal);
            }
        }

        //if not building the back side of the leaf parts, and the part data does not have backfaces disabled, 
        //rebuild this part, facing in the other direction:
        if (!isBackFace && partData.m_BuildBackfaces)
            BuildLeafPart(meshBuilder, offset, rotation, partData, true, bendAngle, startAngle);
    }

}
