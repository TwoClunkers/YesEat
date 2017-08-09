using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public partial class PlacementControllerScript : MonoBehaviour
{
    public MasterSubjectList masterSubjectList;
    public GameObject currentSelection;
    public GameObject thoughtBubble;
    public GameObject selectionMarker;
    public GameObject lastSelector;
    public GameObject testPanel;
    public GameObject plantPanel;
    public GameObject animalPanel;
    public GameObject locationStart;

    public Text _ID;
    public Text _Name;
    public Text _Description;
    public Text _Produce;
    public Text _Growth;
    public Text _Inventory;
    public Text _Maturity;
    public Text _Health;
    public Text _Safety;
    public Text _Food;

    public int masterCount;
    public int placeID;
    private bool placementStarted;
    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private float lastDistance;
    private GameObject placedObject;

    // Use this for initialization
    void Start()
    {
        masterSubjectList = new MasterSubjectList();
        centerPosition = new Vector3();
        edgePosition = new Vector3();
        currentSelection = null;
        placeID = 3;
        lastDistance = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if ((lastSelector != null) && (lastSelector.transform.parent != null))
        {
            SubjectObjectScript script = lastSelector.transform.parent.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
            _ID.text = "ID: <b>" + script.Subject.SubjectID.ToString() + "</b>";
            _Name.text = "Name: <b>" + script.Subject.Name.ToString() + "</b>";
            _Description.text = "Desc: <b>" + script.Subject.Description.ToString() + "</b>";

            testPanel.SetActive(true);
            PlantSubject plantSub = script.Subject as PlantSubject;
            if (plantSub != null)
            {
                PlantObjectScript plantScript = script as PlantObjectScript;
                Subject produceSubject = masterSubjectList.GetSubject(plantSub.ProduceID);
                float maturePercent = Mathf.Min(plantScript.CurrentGrowth / plantSub.MatureGrowth, 1.0f);
                plantPanel.SetActive(true);
                animalPanel.SetActive(false);
                _Produce.text = "Produce: <b>" + plantSub.ProduceID.ToString() + " - " + produceSubject.Name.ToString() + "</b>";
                _Growth.text = "Growth: <b>" + plantScript.CurrentGrowth.ToString() + " / " + plantSub.MaxGrowth.ToString() + "</b>";
                _Maturity.text = "Maturity: <b>" + maturePercent.ToString() + "</b>";
                _Inventory.text = "Inventory: <b>" + plantScript.InventoryPercent().ToString() + "</b>";
            }
            else
            {
                plantPanel.SetActive(false);
                AnimalSubject animalSub = script.Subject as AnimalSubject;
                if (animalSub != null)
                {
                    animalPanel.SetActive(true);
                    AnimalObjectScript animalScript = script as AnimalObjectScript;
                    _Health.text = "Health: <b>" + animalScript.GetHealth().ToString() + "</b>";
                    _Safety.text = "Safety: <b>" + animalScript.GetSafety().ToString() + "</b>";
                    _Food.text = "Food: <b>" + animalScript.GetFood().ToString() + "</b>";
                }
                else
                {
                    animalPanel.SetActive(false);
                }
            }
        }
        else
        {
            testPanel.SetActive(false);
        }

        //Place or use tool
        if (placeID < 1)
        {
            if (placeID < -1) //placeID == -2
            {
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    SelectAtPosition(centerPosition, 1.0f);
                }
            }
            else if (placeID < 0) //placeID == -1
            {
                //setting to Damage
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    if (HarmAtPosition(centerPosition, 1.0f))
                    {
                        PopMessage("Kill!", centerPosition, 0);
                    }
                }
            }
            else //placeID == 0
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
                        if(placeID == 2) //this is a location, which requires 2 steps
                        {
                            placedObject = Instantiate(locationStart, centerPosition, Quaternion.identity);
                            //calculate our edge and manipulate the scale until finalized
                            edgePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                            float distance = Vector3.Distance(centerPosition, edgePosition);
                            if (CheckPlacementPosition(centerPosition, distance, placedObject))
                            {
                                placedObject.transform.localScale = new Vector3(distance * 2, 0.1f, distance * 2);
                            }
                            placementStarted = true;
                        }
                        else
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
                        lastDistance = distance;
                    }

                    //Look for a second mouse click to finalize the location
                    //No more changes to distance
                    if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                    {
                        placementStarted = false;
                        Destroy(placedObject); //get rid of our temp area
                        //pull the default location card, and create the prefab (reusing the values from above)
                        LocationSubject newLocation = masterSubjectList.GetSubject(2) as LocationSubject;
                        placedObject = Instantiate(newLocation.Prefab, centerPosition, Quaternion.identity);
                        placedObject.transform.localScale = new Vector3(lastDistance * 2, 0.1f, lastDistance * 2);
                        newLocation = new LocationSubject();
                        LocationObjectScript script = placedObject.GetComponent<LocationObjectScript>() as LocationObjectScript;
                        //now lets set the values to make a new locationSubject card
                        if (newLocation != null)
                        {
                            newLocation.Radius = lastDistance;
                            newLocation.Coordinates = centerPosition;
                            newLocation.Description = "New Location " + +Time.time;
                            newLocation.Icon = null;
                            newLocation.Layer = 1;
                            newLocation.Name = "Location " + Time.time;
                            //add the next id available
                            newLocation.SubjectID = masterSubjectList.GetNextID();
                            script.Subject = newLocation;
                            //now add our card to the master list
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
                    if (script.Damage(script.Subject, 20)) return true;
                }
                else continue;
            }
        }
        //if we are not bumped prior to here, we are still good
        return false;
    }

    public void SelectAtPosition(Vector3 center, float radius)
    {
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            if (hitColliders[i].tag == "Location") continue;
            else
            {
                if (lastSelector != null) Destroy(lastSelector);
                GameObject newSelector = Instantiate(selectionMarker, hitColliders[i].transform);
                lastSelector = newSelector;
            }
        }
        //if we are not bumped prior to here, we are still good
        return;
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

    public void OnSelectSelect()
    {
        placeID = -2;
        placementStarted = false;
    }
}
