using UnityEngine;
using System.Collections;

public class PlacementControllerScript : MonoBehaviour
{
    public MasterSubjectList masterSubjectList;
    public GameObject currentSelection;

    public int masterCount;
    public int placeID;
    private bool placementStarted;
    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private GameObject placedObject;

    // Use this for initialization
    void Start()
    {
        masterSubjectList = new MasterSubjectList();
        centerPosition = new Vector3();
        edgePosition = new Vector3();
        currentSelection = null;
        placeID = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (placeID == 0)
        {
            //setting to delete objects
        }
        else //currentSelection is placeable
        {
            if (!placementStarted)
            {
                //Since we have not started, we can look for our mouse for placement
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    //Set the center based on mouse position
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

                    //We will let everything start with a radius of 1.0
                    if (CheckPlacementPosition(centerPosition, 1.0f, null))
                    {
                        //Use the id to pull the Subject card
                        Subject newSubject = masterSubjectList.GetSubject(placeID);
                        if(newSubject != null)
                        {
                            placedObject = Instantiate(newSubject.Prefab, centerPosition, Quaternion.identity);
                            if(placedObject != null)
                            {
                                SubjectObjectScript script = placedObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                                script.InitializeFromSubject(masterSubjectList, newSubject);
                                placedObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                                placementStarted = true;
                            }
                        }
                        
                        
                    }
                }
            }
            else
            {
                //We have started to place - is it a location?
                if (placeID == 2)
                {
                    //calculate our edge and manipulate the scale until finalized
                    edgePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    float distance = Vector3.Distance(centerPosition, edgePosition);
                    if (CheckPlacementPosition(centerPosition, distance, placedObject))
                    {
                        placedObject.transform.localScale = new Vector3(distance * 2, 0.1f, distance * 2);
                        SubjectObjectScript script = placedObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                        LocationSubject newLocation = script.Subject as LocationSubject;
                        if (newLocation != null)
                        {
                            newLocation.Radius = distance;
                            newLocation.Coordinates = centerPosition;
                            newLocation.Description = "New Location " + centerPosition.x + "," + centerPosition.y + "," + centerPosition.z;
                            newLocation.Icon = null;
                            newLocation.Layer = 1;
                            newLocation.Name = "Location" +
                                centerPosition.x.ToString().Substring(0, 4) + "," +
                                centerPosition.y.ToString().Substring(0, 4) + "," +
                                centerPosition.z.ToString().Substring(0, 4);
                            newLocation.SubjectID = masterSubjectList.GetNextID();
                            if(!masterSubjectList.AddSubject(newLocation)) Debug.Log("FAIL ADD");
                        }

                    }

                    //Look for a second mouse click to finalize the location
                    if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
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

    }

    public bool CheckPlacementPosition(Vector3 center, float radius, GameObject excludeObject)
    {

        //if (currentSelection == null) return false; //nothing to place;
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            if (hitColliders[i].gameObject == excludeObject) continue;
            //Are any Colliders a LocationObject?
            //If we have one location, (not two) we are fine to place
            if ((hitColliders[i].tag == "Location") ^ (placeID == 2))
            {
                continue;
            }
            else return false;

        }
        //if we are not bumped prior to here, we are still good
        return true;

    }

    /// <summary>
    /// Shortened access to an eventsystem check
    /// </summary>
    /// <returns></returns>
    public bool IsOverMenu()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    public void OnSelectLocation(bool isClicked)
    {
        placeID = 2;
        placementStarted = false;
    }

    public void OnSelectBush(bool isClicked)
    {
        placeID = 3;
        placementStarted = false;
    }

    public void OnSelectPlinket()
    {
        placeID = 1;
        placementStarted = false;
    }

    public void OnSelectGobber()
    {
        placeID = 6;
        placementStarted = false;
    }

    public void OnSelectDelete()
    {
        placeID = 0;
        placementStarted = false;
    }
}
