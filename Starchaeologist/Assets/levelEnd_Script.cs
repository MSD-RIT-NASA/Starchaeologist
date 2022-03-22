using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelEnd_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        //when the player first comes into contact with the end block, show the score screen
        if (other.gameObject.CompareTag("PlayerBody"))
        {

        }
    }
}
