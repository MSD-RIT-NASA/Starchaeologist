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

    [SerializeField] private InputActionReference rotateLineInputReferenceLeftTrigger;
    [SerializeField] private InputActionReference rotateLineInputReferenceRightTrigger;

    [SerializeField] private InputActionReference rotateLineInputReferenceLeftTrackPad;
    [SerializeField] private InputActionReference rotateLineInputReferenceRightTrackPad;

    void Awake()
    {
        rotateLineInputReferenceLeftTrigger.action.performed += RotLeftTrigger;
        rotateLineInputReferenceRightTrigger.action.performed += RotRightTrigger;

        rotateLineInputReferenceLeftTrackPad.action.performed += RotLeftTrack;
        rotateLineInputReferenceRightTrackPad.action.performed += RotRightTrack;

        //rotateLineInputReferenceLeftTrackPad +=
    }

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

   
    private void RotRightTrigger(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.action.ReadValue<float>() != 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, speed * Time.deltaTime);
        }
    }

    private void RotLeftTrigger(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.action.ReadValue<float>() != 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, -speed * Time.deltaTime);
        }
    }

    private void RotRightTrack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.action.ReadValue<float>() != 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, speed * Time.deltaTime);
        }
    }

    private void RotLeftTrack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.action.ReadValue<float>() != 0 && !entered)
        {
            this.transform.rotation = this.transform.rotation * Quaternion.Euler(0, 0, -speed * Time.deltaTime);
        }
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
