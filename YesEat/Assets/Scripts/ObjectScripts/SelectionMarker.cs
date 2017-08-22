using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{

    private GameObject targetGameObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTarget(GameObject newTarget)
    {
        if (newTarget == null) return;

        targetGameObject = newTarget;
        transform.SetParent(targetGameObject.transform);
    }
}
