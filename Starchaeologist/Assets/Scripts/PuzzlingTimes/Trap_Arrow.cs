using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Arrow : MonoBehaviour
{
    public GameObject arrowReference;
    List<GameObject> arrows;
    public bool trapping = false;
    bool starting = false;
    public bool rightSide;

    float pauseTimer = 0f;

    float rightWall = 13.7f;
    float leftWall = -1.7f;
    float currentX = 0;
    float arrowSpeed = 16f;

    public AudioSource arrowSound;

    PlateScript plateReference;

    void Start()
    {
        arrowSound = GetComponent<AudioSource>();
        if(!rightSide)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        enabled = false;
    }

    public void DataSetup(PlateScript getCurrent)
    {
        plateReference = getCurrent;

        trapping = true;
        starting = true;
        pauseTimer = 0f;

        arrows = new List<GameObject>();
        for (int i = 1; i < transform.childCount; i++)
        {
            arrows.Add(null);
            arrows[i - 1] = Instantiate(arrowReference, transform.GetChild(i).transform);

            //adjust the rotation based on the side
            arrows[i - 1].transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        if(rightSide)
        {
            currentX = rightWall;
        }
        else
        {
            currentX = leftWall;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (trapping)
        {
            ShootArrow();
        }
    }


    void ShootArrow()
    {
        //start of with a pause and an audio cue
        if(starting)
        {
            //pause for suspense
            Suspense(4f);
            arrowSound.Play();
        }
        else
        {
            //check if the arrows have gone far enough to be deleted
            if((rightSide && currentX < leftWall) || (!rightSide && currentX > rightWall))
            {
                //cleanup
                while(arrows.Count != 0)
                {
                    Destroy(arrows[0]);
                    arrows.RemoveAt(0);
                }
                trapping = false;
                plateReference.Reactivate();
                plateReference = null;
                enabled = false;
            }
            else
            {
                //increment time
                if (rightSide)
                {
                    currentX = currentX - (Time.deltaTime * arrowSpeed);
                }
                else
                {
                    currentX = currentX + (Time.deltaTime * arrowSpeed);
                }

                //set each arrow to the new position
                foreach (GameObject arrow in arrows)
                {
                    arrow.transform.position = new Vector3(currentX, arrow.transform.position.y, arrow.transform.position.z);
                }
            }
        }
    }

    //function that pauses for suspense
    void Suspense(float pauseFor)
    {
        if (pauseTimer >= pauseFor)
        {
            starting = false;
        }
        else
        {
            pauseTimer = pauseTimer + Time.deltaTime;
        }
    }
}
