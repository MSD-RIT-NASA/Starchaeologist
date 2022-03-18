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

    float arrowX = 0f;
    float pauseTimer = 0f;

    int rightWall = 14;
    int leftWall = -2;
    float currentX = 0;
    float arrowSpeed = 16f;

    public AudioSource arrow;

    PlateScript plateReference;

    void Start()
    {
        arrow = GetComponent<AudioSource>();
    }

    public void DataSetup(PlateScript getCurrent)
    {
        plateReference = getCurrent;

        trapping = true;
        starting = true;
        pauseTimer = 0f;

        arrows = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            arrows.Add(null);
            arrows[i] = Instantiate(arrowReference, transform.GetChild(i).transform);
            //adjust the rotation based on the side once the actual arrow model is used
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
            Suspense(1.5f);
            arrow.Play();
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
