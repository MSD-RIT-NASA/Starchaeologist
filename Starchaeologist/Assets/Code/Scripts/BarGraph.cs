using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarGraph : MonoBehaviour
{

    [SerializeField] GameObject barObj;
    [SerializeField] GameObject barValueObj; 
    [SerializeField] int trialCount = 1;

    [SerializeField] float maxValue = 180.0f;
    [SerializeField] float maxBarSize = 10.0f;
    [SerializeField] float vertSpacing;
    [Tooltip("Uses this offset if bar's position is too close")]
    [SerializeField] float minValueOffset;

    public float[] testValues; 

    private RectTransform[] bars; 

    void Start()
    {
        InitGraph();
    }

    void InitGraph()
    {
        bars = new RectTransform[trialCount];
        for (int i = 0; i < trialCount; i++)
        {
            bars[i] = Instantiate(barObj, Vector3.zero, Quaternion.identity, this.transform).GetComponent<RectTransform>();
            bars[i].localPosition = Vector3.up * ( (i + 1) * vertSpacing);

            // Converts bar's value to bar length 
            float l = Mathf.InverseLerp(0.0f, maxValue, Mathf.Abs(testValues[i]));
            bars[i].localScale = new Vector3(l * maxBarSize, 1, 1);

            float offset = bars[i].rect.width * bars[i].localScale.x / 2.0f; // We need to offset our bar locally by its rect 
            bars[i].localPosition += Vector3.right * offset * ((testValues[i] < 0.0f) ? -1.0f : 1.0f);

            // Visualize bar's value on correct side 
            RectTransform valueObject = Instantiate(barValueObj, this.transform).GetComponent<RectTransform>();
            valueObject.localPosition = new Vector3(
                (Mathf.Abs(bars[i].localPosition.x) < minValueOffset) ? minValueOffset * ((testValues[i] < 0.0f) ? -1.0f : 1.0f) : bars[i].localPosition.x,
                bars[i].localPosition.y, 
                0.0f);
            valueObject.GetComponentInChildren<TextMeshProUGUI>().text = testValues[i].ToString();
        }
    }
}
