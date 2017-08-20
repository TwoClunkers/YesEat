using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{

    private GameObject targetGameObject;
    private PlacementControllerScript _placementControllerScript;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTarget(GameObject newTarget, PlacementControllerScript placementControllerScript)
    {
        if (newTarget == null) return;

        _placementControllerScript = placementControllerScript;

        targetGameObject = newTarget;
        transform.SetParent(targetGameObject.transform);
    }
}
