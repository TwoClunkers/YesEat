using UnityEngine;
using System.Collections;

public class PlacementControllerScript : MonoBehaviour
{
    public MasterSubjectList masterSubjectList;
    public GameObject currentSelection;
    private bool readyToPlace;
    private bool placementStarted;
    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private GameObject placedObject;

    // Use this for initialization
    void Start()
    {
        centerPosition = new Vector3();
        edgePosition = new Vector3();
        readyToPlace = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!placementStarted)
        {
            //Since we have not started, we can look for our mouse for placement
            if(Input.GetMouseButtonDown(0))
            {
                //Set the center based on mouse position
                centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

                //We will let everything start with a radius of 1.0
                if (CheckPlacementPosition(centerPosition, 1.0f, null))
                {
                    placedObject = Instantiate(currentSelection, centerPosition, Quaternion.identity);
                    placedObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    placementStarted = true;
                }
            }
        }
        else
        {
            //We have started to place
            if (currentSelection.tag == "Location")
            {
                //calculate our edge and manipulate the scale until finalized
                edgePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                float distance = Vector3.Distance(centerPosition, edgePosition);
                if(CheckPlacementPosition(centerPosition, distance, placedObject))
                {
                    placedObject.transform.localScale = new Vector3(distance * 2, 0.1f, distance * 2);
                }

                //Look for a second mouse click to finalize the location
                if (Input.GetMouseButtonDown(0))
                {
                    placementStarted = false;
                }
            }
            else
            {
                //We can finalize everything else automatically
                placementStarted = false;
            }
        }
    }

    public bool CheckPlacementPosition(Vector3 center, float radius, GameObject excludeObject)
    {
        if (currentSelection == null) return false; //nothing to place;
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject == excludeObject) continue;
            //Are any Colliders a LocationObject?
            //If we have one location, (not two) we are fine to place
            if ((hitColliders[i].tag == "Location") ^ (currentSelection.tag == "Location"))
            {
                continue;
            }
            else return false;

        }
        //if we are not bumped prior to here, we are still good
        return true;

    }
}
