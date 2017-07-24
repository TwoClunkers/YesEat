using UnityEngine;
using System.Collections;

public class LocationObjectScript : MonoBehaviour
{
    public LocationSubject locationSubject;
    private bool isChanged = false;

    // Use this for initialization
    void Start()
    {
        locationSubject = new LocationSubject();
    }

    // Update is called once per frame
    void Update()
    {
        if (isChanged)
        {
            //resize and reposition if we have a valid location subject
            if (locationSubject != null)
            {
                gameObject.transform.localScale = new Vector3((float)locationSubject.Radius, 0.1f, (float)locationSubject.Radius);
                gameObject.transform.position = locationSubject.Coordinates;
            }
            isChanged = false;
        }

        //look for objects withing this location
    }
}
