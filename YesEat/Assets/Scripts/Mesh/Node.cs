using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{
    #region Private members
    private Quaternion rotation;
    private float radius;
    private float height;
    private float scale;
    private float max;
    private Vector3 position;
    private Node parentNode;
    private List<Node> childNodes;
    #endregion

    /// <summary>
    /// Node constructor
    /// </summary>
    public Node()
    {
        rotation = new Quaternion();
        radius = 1;
        height = 1;
        scale = 1;
        max = 2f;
        position = new Vector3();
        parentNode = null;
        childNodes = new List<Node>();
    }


    public Node(Quaternion newRotation, float newRadius, float newHeight, float newScale, float newMax, Vector3 newPosition)
    {
        rotation = newRotation;
        radius = newRadius;
        height = newHeight;
        scale = newScale;
        max = newMax;
        position = newPosition;
        parentNode = null;
        childNodes = new List<Node>();
    }

    /// <summary>
    /// Rotation of this node
    /// </summary>
    public Quaternion Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }

    /// <summary>
    /// Base Radius of this node
    /// </summary>
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    /// <summary>
    /// Height
    /// </summary>
    public float Height
    {
        get { return height; }
        set { height = value; }
    }

    /// <summary>
    /// Scale to apply to this node
    /// </summary>
    public float Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    /// <summary>
    /// Maximum scale for this node
    /// </summary>
    public float Max
    {
        get { return max; }
        set { max = value; }
    }
    /// <summary>
    /// Position relative to Parent
    /// </summary>
    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    /// <summary>
    /// Scales the position by supplied float
    /// </summary>
    /// <param name="parentScale"></param>
    /// <returns></returns>
    public Vector3 ScaledPosition(float parentScale)
    {
        Vector3 newPosition = new Vector3(position.x * parentScale, position.y * parentScale, position.z * parentScale);
       
        return newPosition;
    }

    /// <summary>
    /// Parent this node is attached to
    /// </summary>
    public Node ParentNode
    {
        get { return parentNode; }
        set { parentNode = value; }
    }

    /// <summary>
    /// Attaches a new child to this node
    /// </summary>
    /// <param name="newNode"></param>
    public void Attach(Node newNode)
    {
        if(newNode != null)
        {
            childNodes.Add(newNode);
            newNode.parentNode = this;
        }
    }

    /// <summary>
    /// Removes the specified child node
    /// </summary>
    /// <param name="targetNode"></param>
    public void Remove(Node targetNode)
    {
        childNodes.Remove(targetNode);
        targetNode.parentNode = null;
    }

    /// <summary>
    /// Returns array of child nodes
    /// </summary>
    /// <returns></returns>
    public Node[] GetChildren()
    {
        return childNodes.ToArray();
    }

    /// <summary>
    /// Returns a copy of this node (doesn't copy children)
    /// </summary>
    /// <returns></returns>
    public Node GetNode()
    {
        Node newNode = new Node();
        newNode.rotation = rotation;
        newNode.radius = radius;
        newNode.height = height;
        newNode.max = max;
        newNode.scale = scale;
        newNode.position = position;
        newNode.parentNode = null;
        newNode.childNodes = new List<Node>();

        return newNode;
    }

    /// <summary>
    /// Returns a copy of this node
    /// Also builds a copy of childNodes (No grandchildren)
    /// </summary>
    /// <returns></returns>
    public Node GetCopy()
    {
        Node newNode = GetNode();

        for (int i = 0; i < childNodes.Count; i++)
        {
            newNode.Attach(childNodes[i].GetNode());
        }

        return newNode;
    }

    /// <summary>
    /// Manually set the values of this Node
    /// </summary>
    /// <param name="newRotation"></param>
    /// <param name="newRadius"></param>
    /// <param name="newHeight"></param>
    /// <param name="newScale"></param>
    /// <param name="newPosition"></param>
    public void SetNode(Quaternion newRotation, float newRadius, float newHeight, float newScale, float newMax, Vector3 newPosition)
    {
        rotation = newRotation;
        radius = newRadius;
        height = newHeight;
        scale = newScale;
        max = newMax;
        position = newPosition;
        parentNode = null;
        childNodes = new List<Node>();
    }

}
