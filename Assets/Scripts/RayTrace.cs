using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class RayTrace : MonoBehaviour {

    Ray ray;
    RaycastHit[] hits;
    GameObject hitObj;
    Vector3 hitPoint;
    GameObject cube;
    GameObject grid;
    public GameObject creation;
    public Material gridMaterial;
    public Material cubeMaterial;
    public Material cubeCurrentMaterial;
    Color cubeColor = Color.white;
    public Image lastColorButton;
    public Image customColor;
    bool enabled = true;

    void Start() {
        DrawGrid();
    }

    // Update is called once per frame
    void Update() {

        if (customColor == lastColorButton)
        {
            cubeColor = customColor.color;
            if (cube != null)
            {
                cube.GetComponent<Renderer>().material.color = cubeColor;
            }
        }

        if (Input.GetButton("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            if (cube != null && enabled)
            {
                cube.GetComponent<Renderer>().material = cubeMaterial;
                cube.GetComponent<Renderer>().material.color = cubeColor;
                cube.GetComponent<Collider>().enabled = true;
                cube.transform.parent = creation.transform;
                cube = null;
            }
        }
        else if (!Input.GetButton("Fire2"))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {

                RaycastHit closest = ClosestHit();
                hitObj = closest.transform.gameObject;
                hitPoint = closest.point;

                if (cube == null)
                {
                    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<Renderer>().material = cubeCurrentMaterial;
                    cube.GetComponent<Renderer>().material.color = new Color(cubeColor.r, cubeColor.g, cubeColor.b, 0.75f);
                    cube.GetComponent<Collider>().enabled = false;
                    cube.name = "cube";
                    cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                } else
                {
                    cube.GetComponent<Renderer>().enabled = true;
                }
                //GetHitPosition();
                cube.transform.position = GetHitPosition();
            }
            else
            {
                if (cube != null)
                {
                    cube.GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }

    private RaycastHit ClosestHit()
    {
        RaycastHit closest = hits[0];
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].distance < closest.distance)
            {
                closest = hits[i];
            }
        }
        return closest;
    }
    private Vector3 GetHitPosition()
    {
        Vector3 localHitPoint = hitObj.transform.InverseTransformPoint(hitPoint);
        if (hitObj.name.Contains("Plane"))
        {

            float x = Mathf.FloorToInt(localHitPoint.x) + 0.5f;

            if (x > localHitPoint.x)
            {
                localHitPoint.x = x - 0.25f;
            } else
            {
                localHitPoint.x = x + 0.25f;
            }
            float z = Mathf.FloorToInt(localHitPoint.z) + 0.5f;

            if (z > localHitPoint.z)
            {
                localHitPoint.z = z - 0.25f;
            }
            else
            {
                localHitPoint.z = z + 0.25f;
            }

            localHitPoint.y += 0.25f;
        } else if (hitObj.name.Contains("cube"))
        {
            Vector3 cubeLocalHit = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                if (localHitPoint[i] == 0.5f || localHitPoint[i] == -0.5f)
                {
                    cubeLocalHit[i] = localHitPoint[i] * 2.0f;
                } else
                {
                    cubeLocalHit[i] = 0.0f;
                }
            }
            localHitPoint = cubeLocalHit;

        }
        return hitObj.transform.TransformPoint(localHitPoint);
    }

    private void DrawGrid()

    {
        grid = new GameObject("grid");

        DrawLine(x: -5.0f);
        DrawLine(x: 5.0f);
        DrawLine(z: -5.0f);
        DrawLine(z: 5.0f);
        for (float x = -4.5f; x < 5.0f; x += 0.5f)
        {
            DrawLine(x: x);
        }
        for (float z = -4.5f; z < 5.0f; z += 0.5f)
        {
            DrawLine(z: z);
        }
    }

    private void DrawLine(float x = float.Epsilon, float z = float.Epsilon)
    {
        float gridOffset = 0.0f;
        GameObject line = new GameObject("line", new System.Type[] { typeof(LineRenderer) });
        line.transform.parent = grid.transform;
        LineRenderer r = line.GetComponent<LineRenderer>();
        r.material = gridMaterial;
        if (x == float.Epsilon)
        {
            r.SetPositions(new Vector3[] { new Vector3(-5.0f, gridOffset, z), new Vector3(5.0f, gridOffset, z) });
        } else
        {
            r.SetPositions(new Vector3[] { new Vector3(x, gridOffset, -5.0f), new Vector3(x, gridOffset, 5.0f) });
        }
        r.SetWidth(0.025f, 0.025f);

    }

    public void SetColor()
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Image image = button.GetComponent<Image>();
        image.fillCenter = !image.fillCenter;

        if (lastColorButton != null && image != lastColorButton) {
            lastColorButton.fillCenter = false;
        }

        //Get the color from the button
        Color newColor;
        if (image == customColor)
        {
            newColor = customColor.color;
        } else
        {
            newColor = col[button.name];
        }

        //Set the cube color
        if (newColor != cubeColor || image != lastColorButton)
        {
            cubeColor = newColor;
        } else
        {
            cubeColor = Color.white;
        }

        lastColorButton = image;

        if (cube != null)
        {
            cube.GetComponent<Renderer>().material.color = cubeColor;
        }
    }

    public void ResetScene()
    {
        for(int i = creation.transform.childCount - 1; i < creation.transform.childCount; i--)
        {
            if (i == -1) { break; }
            Destroy(creation.transform.GetChild(i).gameObject);

        }
    }
    Dictionary<String, Color> col = new Dictionary<String, Color>() { { "Black", Color.black }, { "Red", Color.red }, { "Green", Color.green }, { "Blue", Color.blue } };
    
    public void Enable(bool enable)
    {
        enabled = enable;
    }
    

}
