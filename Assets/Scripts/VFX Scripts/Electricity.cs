using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    private ParticleSystem ps;
    public float hSliderValue = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();   
    }

    // Update is called once per frame
    void Update()
    {
        var main = ps.main;
        main.startSpeed = hSliderValue;
        main.flipRotation = hSliderValue;
    }

    void OnGUI()
    {
        hSliderValue = GUI.HorizontalSlider(new Rect(25, 45, 100, 30), hSliderValue, 0.0F, 10.0F);
    }
}
