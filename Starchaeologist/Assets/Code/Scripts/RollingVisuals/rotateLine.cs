using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


//Rotates the line with input and calculates the score


public class rotateLine : MonoBehaviour
{
    [SerializeField] float speed;

    private ActionBasedController controller;
    private XRBaseInteractor interactor;
    private bool entered = false;
    [SerializeField] Text scoreText;
    [SerializeField] Canvas canvas;
    [SerializeField] Image barGraph;
    [SerializeField] int trials;
    private List<float> scores;

    private float activationThreshold = 0.2f;

    [SerializeField] private InputActionReference rotateLineInputReference;
    [SerializeField] private InputActionReference rotateLineInputReference2;
    // Start is called before the first frame update
    void Start()
    {
        //Vector2 joystickInput = interactor.GetComponent<XRController>().controllerNode == UnityEngine.XR.XRNode.LeftHand;

        controller = GetComponent<ActionBasedController>();
        float z = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, 0, z);
        scores = new List<float>();
        //Debug.Log(transform.rotation.z);

        score();
    }

    // Update is called once per frame
    void Update()
    {
        //inputs();

        // TODO: Test following input code 
        int i = 0;
        /*while (i < 4)
        {
            if (Mathf.Abs(Input.GetAxis("Joy" + i + "X")) > 0.2F || Mathf.Abs(Input.GetAxis("Joy" + i + "Y")) > 0.2F)
                Debug.Log(Input.GetJoystickNames()[i] + " is moved");

            i++;
        }*/

    }
    private void Awake()
    {
        rotateLineInputReference.action.performed += inputs;
    }

    private void inputs(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        /* get the controller
        * on input adjust the rotation of the 
        * line
        */

        //if (Keyboard.current.rightArrowKey.isPressed && !entered)
        //{
        //    this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, -0.1f);
        //}
        //if (Keyboard.current.leftArrowKey.isPressed && !entered)
        //{
        //    this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, 0.1f);
        //}
        //if (Keyboard.current.enterKey.wasPressedThisFrame)
        //{
        //    Debug.Log(trials);
        //    if (entered == true && scores.Count < trials)
        //    {
        //        float z = Random.Range(0, 360);
        //        this.transform.rotation = Quaternion.Euler(0, 0, z);
        //    }
        //    else
        //    {
        //        score();
        //    }
        //    entered = !entered;
        //}
        print(rotateLineInputReference.action.ReadValue<float>());

        if (rotateLineInputReference.action.ReadValue<float>() > 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, -speed * Time.deltaTime);
        }
        if (rotateLineInputReference2.action.ReadValue<float>() < 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, speed * Time.deltaTime);
        }
       /* if (Keyboard.current.enterKey.wasPressedThisFrame || controller.selectActionValue.action.ReadValue<bool>())
        {
            if (entered == true && scores.Count < trials)
            {
                Debug.Log("enter");
                float z = Random.Range(0, 360);
                this.transform.rotation = Quaternion.Euler(0, 0, z);
            }
            else
            {
                score();
            }
            entered = !entered;
        }*/
    }

    private void score()
    {

        //find the distance from quaternion.zero

        float scoreFinal = this.transform.eulerAngles.z;
        if (scoreFinal >= 90)
        {
            scoreFinal = 180 - scoreFinal;
        }

        scores.Add(scoreFinal);
        if (scores.Count >= trials)
        {
            displayGraph();
        }
    }

    private void displayGraph()
    {
        canvas.gameObject.SetActive(true);
        for (int i = 0; i < scores.Count; i++) {
            float thisScore = scores[i];
            Image graph = Instantiate(barGraph, canvas.transform);
            graph.rectTransform.sizeDelta = new Vector2(Mathf.Abs(thisScore* 2.25f), 40);
            float displayScore = 100 - (Mathf.Abs(thisScore) / 1.80f);
            if (thisScore >=0)
            {
                graph.rectTransform.anchoredPosition = new Vector3(452 + (thisScore* 2.25f)/2, -1 * ((i + 1) * 45), 0);
                Text text = Instantiate(scoreText, graph.transform);
                text.rectTransform.anchoredPosition = new Vector3(((thisScore * 2.25f) / 2) + 90, 0, 0);
                text.text = displayScore + "%";
            }
            else
            {
                graph.rectTransform.anchoredPosition = new Vector3(451 + (thisScore * 2.25f)/2, -1 * ((i + 1) * 45), 0);
                Text text = Instantiate(scoreText, graph.transform);
                text.rectTransform.anchoredPosition = new Vector3(((thisScore * 2.25f) / 2), 0, 0);
                text.text = displayScore + "%";
            }

            Debug.Log(thisScore);
        }
    }


}
