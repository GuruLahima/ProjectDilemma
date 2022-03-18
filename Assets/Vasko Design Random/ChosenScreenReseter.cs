using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenScreenReseter : MonoBehaviour
{

    public GameObject infoIcon;
    public GameObject gJ2;
    public GameObject gJ3;
    public GameObject gJ4;
    public GameObject gJ5;
    public GameObject gJ6;



    void OnEnable()
    {
        infoIcon.SetActive(true);
        gJ2.SetActive(false);
        gJ3.SetActive(false);
        gJ4.SetActive(false);
        gJ5.SetActive(false);
        gJ6.SetActive(false);
        Debug.Log("PrintOnEnable: script was enabled");
    }

    
}

