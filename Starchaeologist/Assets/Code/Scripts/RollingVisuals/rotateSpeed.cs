using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateSpeed : MonoBehaviour
{
    private int velocity;
    private float angles;
    // Start is called before the first frame update
    void Start()
    {
        velocity = this.GetComponentInParent<createSpheres>().speedOfRotation;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        angles = velocity/(Vector2.Distance(position, Vector2.zero));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Vector3.zero, Vector3.forward, velocity * Time.deltaTime);
    }
}
