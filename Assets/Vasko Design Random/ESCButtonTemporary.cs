using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCButtonTemporary : MonoBehaviour
{

    public GameObject wardrobeCanvas;
    public GameObject wardrobeCamera;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            wardrobeCamera.SetActive(true);
            wardrobeCanvas.SetActive(true);
        }
        
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            wardrobeCamera.SetActive(false);
            wardrobeCanvas.SetActive(false);
        }
    }
}
