using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;


//Rotates the line with input and calculates the score


public class rotateLine : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int trials;
    [SerializeField] Canvas canvas;
    [SerializeField] BarGraph barGraph;

    [SerializeField] private InputActionReference rotateLineInputReferenceLeftTrigger;
    [SerializeField] private InputActionReference rotateLineInputReferenceRightTrigger;
    [SerializeField] private InputActionReference instructorContinueTrigger; // Continue to the next round 
    [SerializeField] private InputActionReference instructorResetTrigger;    // Completely reset the game 
    [SerializeField] private MiniSceneLoader miniSceneLoader;

    private ActionBasedController controller;
    private XRBaseInteractor interactor;
    private bool entered = false;
    private List<float> scores;

    private float activationThreshold = 0.2f;

    private bool reloadKeyHold = false;

    void Awake()
    {
        // Initialize controllers  
        rotateLineInputReferenceLeftTrigger.action.performed += RotLeftTrigger;
        rotateLineInputReferenceRightTrigger.action.performed += RotRightTrigger;

    }

    void Start()
    {
        CompletelyResetTest();
    }

    private void Update()
    {
        InstructorCommands();
    }
    
    /// <summary>
    /// Allows the instructor to continue rounds and reset
    /// the test using keyboard keys 
    /// </summary>
    private void InstructorCommands()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.enterKey.isPressed ) // Reload scene 
        {
            miniSceneLoader.ReloadScene();
            
        }
        else if (keyboard.spaceKey.isPressed && reloadKeyHold == false) // Continue to next round 
        {
            SetNextRound();
        }

        reloadKeyHold = keyboard.spaceKey.isPressed;
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

    /// <summary>
    /// Continues to next round if possible. If not creates a graph that
    /// represents the previous rounds of data 
    /// </summary>
    private void SetNextRound()
    {
        // Store current difference of rotation 
        float roundSCore = this.transform.eulerAngles.z;
        if (roundSCore >= 90)
        {
            roundSCore = 180 - roundSCore;
        }
        scores.Add(roundSCore);

        if(scores.Count >= trials)
        {
            // Create bar graph and indicate it is the end of round 
            canvas.gameObject.SetActive(true);
            barGraph.GenerateGraph(scores);
        }
        else
        {
            // Reset this line to a random rotation 
            float z = Random.Range(0, 360);
            this.transform.rotation = Quaternion.Euler(0, 0, z);
        }
    }

    private void CompletelyResetTest()
    {
        // Set this line to a random rotation 
        float z = Random.Range(0, 360);
        this.transform.rotation = Quaternion.Euler(0, 0, z);
        scores = new List<float>();
    }

    private void Score()
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
            //displayGraph();
        }
    }
}
