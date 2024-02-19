using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveForward : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float time =(float)Time.deltaTime;
        Vector3 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, pos.z-time);
    }
}
