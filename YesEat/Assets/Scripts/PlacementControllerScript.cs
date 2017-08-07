using UnityEngine;
using System.Collections;

public class PlacementControllerScript : MonoBehaviour
{
    public MasterSubjectList masterSubjectList;
    public GameObject currentSelection;
    public GameObject thoughtBubble;

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
        if (placeID < 1)
        {
            if (placeID < 0) //setting to kill
            {
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    if (HarmAtPosition(centerPosition, 1.0f))
                    {
                        PopMessage("Kill!", centerPosition, 0);
                    }
                }
            }
            else
            {
                //setting to delete objects
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    if (DeleteAtPosition(centerPosition, 1.0f))
                    {
                        PopMessage("DELETED", centerPosition, 1);
                    }
                }
            }
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
                        if (newSubject != null)
                        {
                            placedObject = Instantiate(newSubject.Prefab, centerPosition, Quaternion.identity);
                            if (placedObject != null)
                            {
                                SubjectObjectScript script = placedObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                                script.InitializeFromSubject(masterSubjectList, newSubject);
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
                    }

                    //Look for a second mouse click to finalize the location
                    if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                    {
                        placementStarted = false;
                        //grab the script, and we will place a new LocationSubject in it.
                        SubjectObjectScript script = placedObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                        LocationSubject newLocation = script.Subject as LocationSubject;
                        if (newLocation != null)
                        {
                            newLocation.Radius = distance;
                            newLocation.Coordinates = centerPosition;
                            newLocation.Description = "New Location " + +Time.time;
                            newLocation.Icon = null;
                            newLocation.Layer = 1;
                            newLocation.Name = "Location " + Time.time;
                            //add the next id available
                            newLocation.SubjectID = masterSubjectList.GetNextID();
                            if (!masterSubjectList.AddSubject(newLocation)) Debug.Log("FAIL ADD");
                        }
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

    /// <summary>
    /// Returns whether our item can be placed here
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="excludeObject"></param>
    /// <returns></returns>
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

    public bool DeleteAtPosition(Vector3 center, float radius)
    {
        //if (currentSelection == null) return false; //nothing to place;
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            else
            {
                Destroy(hitColliders[i].gameObject);
                return true;
            }
        }
        //if we are not bumped prior to here, we are still good
        return false;
    }

    public bool HarmAtPosition(Vector3 center, float radius)
    {
        //if (currentSelection == null) return false; //nothing to place;
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            else
            {
                AnimalObjectScript script = hitColliders[i].GetComponent<AnimalObjectScript>() as AnimalObjectScript;
                if (script != null)
                {
                    if (script.Damage(script.Subject, 10000)) return true;
                }
                else continue;
            }
        }
        //if we are not bumped prior to here, we are still good
        return false;
    }

    /// <summary>
    /// Shortened access to an eventsystem check
    /// </summary>
    /// <returns></returns>
    public bool IsOverMenu()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    /// <summary>
    /// Creates a temporary message bubble at location specified
    /// </summary>
    /// <param name="message"></param>
    /// <param name="position"></param>
    /// <param name="colorSelect"></param>
    public void PopMessage(string message, Vector3 position, int colorSelect = 0)
    {
        position = Camera.main.WorldToScreenPoint(position);
        GameObject newBubble = Instantiate(thoughtBubble, position, Quaternion.identity);
        GameObject canvas = GameObject.FindGameObjectWithTag("Message");
        newBubble.transform.SetParent(canvas.transform);
        newBubble.GetComponent<ThoughtBubbleScript>().PopMessage(message, colorSelect);

    }


    //Button attachments
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

    public void OnSelectSmite()
    {
        placeID = -1;
        placementStarted = false;
    }
}
