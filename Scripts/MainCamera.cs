using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainCamera : MonoBehaviour
{
    Camera cam;
    public float width = 750;
    public float height = 1334f;
    public float pexelPerUnit = 100f;
    void Awake()
    {
        float screenW = (float)Screen.width;
        float screenH = (float)Screen.height;
        cam = Camera.main;
        if (screenW / screenH < width / height)
        {
            cam.orthographicSize = screenH / (screenW / (width / pexelPerUnit)) / 2;
        }
        else
        {
            cam.orthographicSize = height / 2 / pexelPerUnit;
        }
    }
}
