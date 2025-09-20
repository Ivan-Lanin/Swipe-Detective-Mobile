using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RotationSlider : MonoBehaviour
{
    public Slider RotationSliderComponent { get; private set; }


    void Awake()
    {
        RotationSliderComponent = GetComponent<Slider>(); 
    }

    public void ResetSliderValue()
    {
        RotationSliderComponent.value = 0;
    }
}
