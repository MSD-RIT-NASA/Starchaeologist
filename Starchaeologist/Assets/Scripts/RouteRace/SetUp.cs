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
    //Wait to connect to server for balance board controls 
    IEnumerator connect()
    {
        yield return new WaitForSeconds(8);
    }
    */
}
