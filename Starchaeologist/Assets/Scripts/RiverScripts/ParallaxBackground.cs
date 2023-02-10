using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.z;
        length = GetComponent<MeshRenderer>().bounds.size.z;
    }

    // Update is called once per frame
    void Update()
    {
        float relative = (cam.transform.position.z + (float)60.84773) * (1 - parallaxEffect);
        //Debug.Log("Relative: " + relative);
        //how far we have moved in world space
        float distance = ((cam.transform.position.z+ (float)60.84773) * parallaxEffect);
        transform.position = new Vector3(transform.position.x, transform.position.y, startpos + distance);

        if (relative > (startpos + 120 + length))
        {
            Debug.Log("Param" + startpos + " + " + length + " = " + (startpos + length));
            startpos += length;
        }
        else if (relative < (startpos - length + 60))
        {
            Debug.Log("Repeat 2.0");
            startpos -= length;
        }
    }
}
