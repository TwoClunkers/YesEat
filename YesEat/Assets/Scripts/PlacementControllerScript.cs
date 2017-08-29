using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public partial class PlacementControllerScript : MonoBehaviour
{
    #region Public Field declarations
    public GameObject thoughtBubble;
    public GameObject selectionMarker;
    public GameObject lastSelector;
    public GameObject testPanel;
    public GameObject plantPanel;
    public GameObject animalPanel;
    public GameObject locationPanel;
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
    public Text _Endurance;
    public Text _Driver;
    public Text _Center;
    public Text _Radius;
    #endregion

    public int masterCount;
    public int placeID;
    private bool placementStarted;
    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private float lastDistance;
    private GameObject placedObject;
    private Vector3 cameraDestination;
    bool cameraFollowTarget;

    // Use this for initialization
    void Start()
    {
        centerPosition = new Vector3();
        edgePosition = new Vector3();
        placeID = -2;
        testPanel.SetActive(true);
        lastDistance = 0;
        cameraDestination = Camera.main.transform.position;
        cameraFollowTarget = false;
        KnowledgeBase.Init();
    }

    // Update is called once per frame
    void Update()
    {
        DoCameraMovement();

        if ((lastSelector != null) && (lastSelector.transform.parent != null))
        {
            ShowSelectionPanel();
        }

        //Place or use tool
        if (placeID < 1)
        {
            if (placeID < -1) //placeID == -2
            {
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
                    SelectAtPosition(centerPosition, 0.4f);
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
        else // is placeable
        {
            if (!placementStarted)
            {
                //Since we have not started, we can look for our mouse for placement
                if (!IsOverMenu() && (Input.GetMouseButtonDown(0)))
                {
                    //Set the center based on mouse position
                    centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

                    //We will let everything start with a radius of 0.5
                    if (CheckPlacementPosition(centerPosition, 0.5f, null))
                    {
                        if (placeID == KbIds.Location) //this is a location, which requires 2 steps
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
                            Subject newSubject = KnowledgeBase.GetSubject(placeID);
                            if (newSubject != null)
                            {
                                placedObject = Instantiate(newSubject.Prefab, centerPosition, Quaternion.identity);
                                if (placedObject != null)
                                {
                                    PlantSubject maybePlant = newSubject as PlantSubject;
                                    if(maybePlant != null)
                                    {
                                        maybePlant.PlantGene = new Gene(5);
                                        newSubject = maybePlant;
                                    }
                                    placedObject.transform.Rotate(Vector3.up, Random.value * 360);
                                    SubjectObjectScript script = placedObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                                    script.InitializeFromSubject(newSubject);
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
                if (placeID == KbIds.Location)
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
                        //create a new location using the above values
                        CreateLocation(centerPosition, lastDistance);
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
    /// Display data based on the selected object.
    /// </summary>
    private void ShowSelectionPanel()
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
            Subject produceSubject = KnowledgeBase.GetSubject(plantSub.ProduceID);
            float maturePercent = Mathf.Min(plantScript.CurrentGrowth / plantSub.MatureGrowth, 1.0f);
            plantPanel.SetActive(true);
            animalPanel.SetActive(false);
            locationPanel.SetActive(false);
            _Produce.text = "Produce: <b>" + plantSub.ProduceID.ToString() + " - " + produceSubject.Name.ToString() + "</b>";
            _Growth.text = "Growth: <b>" + plantScript.CurrentGrowth.ToString() + " / " + plantSub.MaxGrowth.ToString() + "</b>";
            _Maturity.text = "Maturity: <b>" + maturePercent.ToString() + "</b>";
            _Inventory.text = "Inventory: <b>" + plantScript.InventoryPercent().ToString() + "</b>";
        }
        else
        {
            AnimalSubject animalSub = script.Subject as AnimalSubject;
            if (animalSub != null)
            {
                animalPanel.SetActive(true);
                plantPanel.SetActive(false);
                locationPanel.SetActive(false);
                AnimalObjectScript animalScript = script as AnimalObjectScript;
                _Health.text = "Health: <b>" + animalScript.GetHealth().ToString() + "</b>";
                _Safety.text = "Safety: <b>" + animalScript.GetSafety().ToString() + "</b>";
                _Food.text = "Food: <b>" + animalScript.GetFood().ToString() + "</b>";
                _Endurance.text = "Endurance: <b>" + animalScript.GetEndurance().ToString() + "</b>";
                _Driver.text = "Driver: <b>" + animalScript.GetDriver().ToString() + "</b>";
            }
            else
            {
                LocationSubject locationSub = script.Subject as LocationSubject;
                if (locationSub != null)
                {
                    locationPanel.SetActive(true);
                    plantPanel.SetActive(false);
                    animalPanel.SetActive(false);
                    _Center.text = "Center: <b>" + locationSub.Coordinates.ToString() + "</b>";
                    _Radius.text = "Radius: <b>" + locationSub.Radius.ToString() + "</b>";
                }
                else
                {
                    locationPanel.SetActive(false);
                    plantPanel.SetActive(false);
                    animalPanel.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Move the camera based on user inputs
    /// </summary>
    private void DoCameraMovement()
    {
        if (Input.GetKeyUp(KeyCode.F))
            cameraFollowTarget = !cameraFollowTarget;

        if (lastSelector != null)
        {
            if (cameraFollowTarget)
            {
                cameraDestination = new Vector3(lastSelector.transform.position.x,
                    cameraDestination.y,
                    lastSelector.transform.position.z);
            }
        }
        if (Input.GetMouseButton(2))
        {
            cameraFollowTarget = false;
            // panning camera
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseY != 0)
            {
                cameraDestination = new Vector3(cameraDestination.x,
                    cameraDestination.y,
                    Camera.main.transform.position.z + mouseY * (cameraDestination.y / 25));
            }
            float mouseX = Input.GetAxis("Mouse X");
            if (mouseX != 0)
            {
                cameraDestination = new Vector3(Camera.main.transform.position.x + mouseX * (cameraDestination.y / 25),
                    cameraDestination.y,
                    cameraDestination.z);
            }
        }
        float mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheelDelta != 0)
        {
            Vector3 camPosition = Camera.main.transform.position;
            cameraDestination = new Vector3(camPosition.x,
                camPosition.y - mouseWheelDelta * (camPosition.y),
            Camera.main.transform.position.z);

        }
        if (Vector3.Distance(Camera.main.transform.position, cameraDestination) > 0.1f)
        {
            Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, cameraDestination, Time.deltaTime * 20.0f);
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
        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //If we caught any, will have to check them
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            if (hitColliders[i].gameObject == excludeObject) continue;
            //Are any Colliders a LocationObject?
            //If we have one location, (not two) we are fine to place
            if ((hitColliders[i].tag == "Location") ^ (placeID == KbIds.Location))
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
        GameObject locationObj = null;

        //clear selection
        if (lastSelector != null) Destroy(lastSelector);

        //First we catch all the colliders in our area
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        //go through colliders first looking for non-location objects (since they are below everything)
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Ground") continue;
            if (hitColliders[i].tag == "Location")
            {
                //grab a location if you find it
                locationObj = hitColliders[i].gameObject;
                continue;
            }
            else
            {
                //if not a location, we can create a selectionMarker as a child
                GameObject newSelector = Instantiate(selectionMarker, hitColliders[i].transform);
                lastSelector = newSelector;
                return;
            }
        }
        //if we are here, there were no non-location objects found
        if (locationObj != null)
        {
            GameObject newSelector = Instantiate(selectionMarker, locationObj.transform);
            lastSelector = newSelector;
        }
        //no objects of any kind
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

    /// <summary>
    /// Create and register a new location
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    public GameObject CreateLocation(Vector3 center, float radius)
    {
        //first, pull the genaric LocationSubject from the KnowledgeBase and create prefab
        LocationSubject locFromMaster = KnowledgeBase.GetSubject(KbIds.Location) as LocationSubject;
        GameObject newLocationObject = Instantiate(locFromMaster.Prefab, center, Quaternion.identity);
        newLocationObject.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);

        //grab our connected script and create a fresh LocationSubject
        LocationObjectScript script = newLocationObject.GetComponent<LocationObjectScript>() as LocationObjectScript;
        LocationSubject newLocSubject = new LocationSubject()
        {
            //now lets set the values to make a new locationSubject card
            Name = "Location " + Time.time,
            Description = "New Location " + Time.time,
            Radius = radius,
            Coordinates = center,
            Layer = 1,

            //add the next id available
            SubjectID = KnowledgeBase.GetNextID()
        };
        script.InitializeFromSubject(newLocSubject);

        //now add our card to the master list
        if (!KnowledgeBase.AddSubject(newLocSubject)) Debug.Log("FAIL ADD");
        //store to the script attached to our new object
        return newLocationObject;
    }

    /// <summary>
    /// Instantiates newSubjectId's prefab at the spawnPoint position.
    /// </summary>
    /// <param name="newSubjectId">The SubjectID of the NPC to spawn.</param>
    /// <param name="spawnPoint">The position to spawn the NPC at.</param>
    public GameObject SpawnObject(int newSubjectId, Vector3 spawnPoint)
    {
        //Use the id to pull the Subject card
        Subject newSubject = KnowledgeBase.GetSubject(newSubjectId);
        if (newSubject != null)
        {
            GameObject newObject = Instantiate(newSubject.Prefab, spawnPoint, Quaternion.identity);
            if (newObject != null)
            {
                SubjectObjectScript script = newObject.GetComponent<SubjectObjectScript>() as SubjectObjectScript;
                script.InitializeFromSubject(newSubject);
                return newObject;
            }
        }
        return null;
    }

    //Button attachments
    public void OnSelectLocation(bool isClicked)
    {
        placeID = KbIds.Location;
        placementStarted = false;
    }

    public void OnSelectBush(bool isClicked)
    {
        placeID = KbIds.Tree;
        placementStarted = false;
    }

    public void OnSelectPlinket()
    {
        placeID = KbIds.Plinkett;
        placementStarted = false;
    }

    public void OnSelectGobber()
    {
        placeID = KbIds.Gobber;
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
