using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createSpheres : MonoBehaviour
{
    public GameObject spherePrimitive;
    // Start is called before the first frame update
    void Start()
    {
        float height = (Camera.main.orthographicSize)-1;
        float width = (Camera.main.aspect * height)-1;
        GameObject spheres = GameObject.Find("Spheres");
        for (int i = 0; i < 10; i++)
        {
            GameObject sphere = Instantiate(spherePrimitive);
            sphere.transform.SetParent(spheres.transform);
            sphere.transform.position = new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
