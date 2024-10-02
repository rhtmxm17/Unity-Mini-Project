using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [SerializeField] RectTransform canvas;
    [SerializeField] Slider hpSlider;
    [SerializeField] Image fillImage;

    public float MaxHP { set { hpSlider.maxValue = value; } }
    public float CurHP { set { hpSlider.value = value; } }
    public Color HP_SliderColor { set { fillImage.color = value; } }

    private void LateUpdate()
    {
        canvas.rotation = Camera.main.transform.rotation;
    }
}
