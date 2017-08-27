using UnityEngine;
using System.Collections;

public class Node
{
    #region Private members
    private Quaternion rotation;
    private float radius;
    private Vector3 position;
    private Vector3 scale;
    #endregion

    public Node()
    {
        rotation = new Quaternion();
        radius = 0;
        position = new Vector3();
        scale = new Vector3();
    }

    public Node(Quaternion newRotation, float newRadius, Vector3 newPosition, Vector3 newScale)
    {
        rotation = newRotation;
        radius = newRadius;
        position = newPosition;
        scale = newScale;
    }

    public Quaternion Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public float ScaledRadius()
    {
        return radius * scale.x;
    }

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public Vector3 ScaledPosition()
    {
        return new Vector3(position.x* scale.x, position.y* scale.y, position.z* scale.z);
}

    public Vector3 Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    public Node GetNode()
    {
        Node newNode = new Node();
        newNode.rotation = rotation;
        newNode.radius = radius;
        newNode.position = position;
        newNode.scale = scale;

        return newNode;
    }

    public void SetNode(Quaternion newRotation, float newRadius, Vector3 newPosition, Vector3 newScale)
    {
        rotation = newRotation;
        radius = newRadius;
        position = newPosition;
        scale = newScale;
    }

    public Node GetScaledNode()
    {
        Node newNode = new Node();
        newNode.rotation = rotation;
        newNode.radius = radius*scale.x;
        newNode.position = new Vector3(position.x * scale.x, position.y * scale.y, position.z * scale.z);
        newNode.scale = new Vector3(1, 1, 1);

        return newNode;
    }
}
