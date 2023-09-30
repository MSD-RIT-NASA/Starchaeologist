using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceOverlayUIVR : MonoBehaviour
{
    private const string shaderTestMode = "unity_GUIZTestMode";
    [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always;
    [SerializeField] Graphic[] uiElementsToApplyEffectTo;

    private Dictionary<Material,Material> materialMapping = new Dictionary<Material,Material>();

    void Start()
    {
        foreach(var graphic in uiElementsToApplyEffectTo)
        {
            Material material = graphic.materialForRendering;

            if(material == null)
            {
                Debug.Log("Target Material does not have a rendering component");
                continue;
            }

            if(materialMapping.TryGetValue(material,out Material materialCopy) == false)
            {
                materialCopy = new Material(material);
                materialMapping.Add(material,materialCopy);
            }

            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            graphic.material = materialCopy;
        }
    }
}