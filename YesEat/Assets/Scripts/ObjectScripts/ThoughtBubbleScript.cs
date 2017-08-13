using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubbleScript : MonoBehaviour
{

    public string displayText;
    public float displayTime;

    public Color imageColor;
    public Color textColor;
    public Color startImageColor;
    public Color startTextColor;
    public Color endImageColor;
    public Color endTextColor;

    public Color[] colorList;

    // Use this for initialization
    void Start()
    {
        colorList = new Color[10];
        displayText = "default text";
        displayTime = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        displayTime -= Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + (20 * Time.deltaTime));
        UpdateColor();
        transform.GetComponent<Renderer>().material.color = imageColor;
        transform.GetComponentInChildren<Image>().color = imageColor;
        transform.GetComponentInChildren<Text>().color = textColor;
        if (displayTime < 0) Destroy(gameObject);
    }

    void UpdateColor()
    {
        imageColor = Color.Lerp(startImageColor, endImageColor, Time.deltaTime / displayTime);
        textColor = Color.Lerp(startTextColor, endTextColor, Time.deltaTime / displayTime);
    }

    public void PopMessage(string message, int colorSelect)
    {
        if (colorSelect >= colorList.Length) colorSelect = 0;
        startImageColor = colorList[colorSelect];
        transform.GetComponentInChildren<Text>().text = message;
        startTextColor = Color.black;
        displayTime = 1.0f;
    }
}
