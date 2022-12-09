using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnusSliderController : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider magnusSlider;
    private float _slideSpeed;

    void Start()
    {
        magnusSlider = this.GetComponent<Slider>();
        _slideSpeed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (magnusSlider.value > magnusSlider.minValue) magnusSlider.value -= Time.deltaTime * _slideSpeed;
        }

        if (Input.GetKey(KeyCode.X))
        {
            if (magnusSlider.value < magnusSlider.maxValue) magnusSlider.value += Time.deltaTime * _slideSpeed;
        }
    }
}
