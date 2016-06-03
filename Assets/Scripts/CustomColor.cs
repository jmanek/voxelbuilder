using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomColor : MonoBehaviour {

    Image image;
    public Slider red;
    public Slider green;
    public Slider blue;
    // Use this for initialization
    void Start () {
        image = gameObject.GetComponent<Image>();
	}

    public void UpdateColor()
    {
        image.color = new Color(red.value / 255.0f, green.value / 255.0f, blue.value / 255.0f);
    }
}
