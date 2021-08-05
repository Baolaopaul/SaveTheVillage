using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTimer : MonoBehaviour
{
    public float maxTime;
    public bool Tick;

    private Image img;
    private float currentTime;


    void Start()
    {
        img = GetComponent<Image>();
        currentTime = maxTime;
    }

  
    void Update()
    {
        Tick = false;
        currentTime -= Time.deltaTime;  
        if (currentTime <= 0)
        {
            Tick = true;
            currentTime = maxTime;
        }

        img.fillAmount = currentTime / maxTime;
    }
}
