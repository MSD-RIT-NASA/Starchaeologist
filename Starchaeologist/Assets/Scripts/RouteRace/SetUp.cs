using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : MonoBehaviour
{
    [SerializeField] GameObject playerRef;
    [SerializeField] RoadBuilder roadBuilderScript;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(rules());
        roadBuilderScript.BuildRoad();
    }

    /*
    //Show controls and objective
    IEnumerator rules()
    {
        yield return new WaitForSeconds(8);
        playerRef.transform.GetChild(2).gameObject.SetActive(false);
    }
    */
}
