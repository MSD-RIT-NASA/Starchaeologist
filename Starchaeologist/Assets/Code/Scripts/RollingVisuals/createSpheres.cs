using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createSpheres : MonoBehaviour
{
    [SerializeField] private int sphereNum;
    [SerializeField] private GameObject spherePrimitive;
    [SerializeField] public int speedOfRotation;

    // Start is called before the first frame update
    void Start()
    {
        //float height = (Camera.main.orthographicSize)-(float)(2);
        //Debug.Log(height);
        //List<Vector3> spherePos = new List<Vector3>();
        //GameObject spheres = GameObject.Find("Spheres");
        ////make the spheres
        //for (int i = 0; i <= sphereNum; i++)
        //{
        //    Vector3 pos;
        //    bool overlap = true;
        //    GameObject sphere = Instantiate(spherePrimitive);
        //    sphere.transform.SetParent(spheres.transform);
        //    float xpos = Mathf.Sin(i * (360/ (sphereNum)))*height;
        //    float ypos = Mathf.Cos(i * (360/ (sphereNum)))*height;
        //    pos = new Vector3(xpos, ypos, 0);
        //    //pos = new Vector3(Random.Range(-height, height), Random.Range(-height, height), 0);
        //    //make sure none of them overlap
        //    //while (overlap)
        //    //{
        //    //    overlap = false;
        //    //    foreach (Vector3 position in spherePos)
        //    //    {
        //    //        if (position.x - pos.x <= 1 && position.y - pos.y <= 1)
        //    //        {
        //    //            //Debug.Log("overlap");
        //    //            overlap = true;
        //    //            pos = new Vector3(Random.Range(-height, height), Random.Range(-height, height), 0);
        //    //        }
        //    //    }
        //    //}
        //    spherePos.Add(pos);
        //    sphere.transform.position = pos;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.transform.localEulerAngles.z + speedOfRotation * Time.deltaTime);
        //this.transform.RotateAround(Vector3.zero, Vector3.forward, speedOfRotation * Time.deltaTime);
    }
}
