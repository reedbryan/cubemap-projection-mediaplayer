using UnityEngine;
using UnityEngine.UI;

public class ResetSlider : MonoBehaviour
{
    [SerializeField]private Slider slider;

    void Start(){
        slider = GetComponent<Slider>();
    }

    // Call this method to reset the slider
    public void ResetToMiddle()
    {
        slider.value = (slider.minValue + slider.maxValue) / 2f;
    }
}
