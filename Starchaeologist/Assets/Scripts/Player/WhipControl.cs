using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(VelocityEstimator))]
public class WhipControl : MonoBehaviour
{
    [Tooltip("The estimator we'll be referencing when determining if the player's swinging fast enough. It may or " +
        "may not be on this particular game object.")]
    [SerializeField] private VelocityEstimator vEstimator = null;
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private GameObject whipCurledRef;
    [SerializeField] private GameObject whipUncurledRef;
    [Tooltip("To swing the whip, the velocity of the player's hand should have a magnitude greater than this.")]
    [SerializeField] [Min(0)] private float minSwingSpeed;

    //private Quaternion defaultRot;
    private Coroutine uncurlCorout = null;

    private void Start()
    {
        if (!vEstimator)
        {
            if (!TryGetComponent(out vEstimator))
            {
                Debug.LogError($"{name}'s WhipControl was not given a VelocityEstimator to reference, and GetComponent " +
                    $"failed! Give {name}'s WhipControl a VelocityEstimator reference, or attach one.");
            }
        }
        Debug.Assert(whipCurledRef, $"WhipControl on {name} is missing a curled whip reference. Did you forget to assign one in the inspector?");
        Debug.Assert(whipUncurledRef, $"WhipControl on {name} is missing an uncurled whip reference. Did you forget to assign one in the inspector?");

        //defaultRot = transform.rotation;
        vEstimator.SetEstimationActve(false);
        ToggleWhipCurled(true);
    }

    private void OnEnable()
    {
        controller.activateAction.action.performed += OnControllerPerformed;
    }
    private void OnDisable()
    {
        controller.activateAction.action.performed -= OnControllerPerformed;
    }

    private void OnControllerPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //Only prepare for whip swings if the whip isn't currently being swung, i.e., if it's not uncurled
        if (uncurlCorout == null)
        {
            if (ctx.action.WasPressedThisFrame())
            {
                vEstimator.SetEstimationActve(true);
                //vfx to show you're preparing a swing?
            }
            else if (ctx.action.WasReleasedThisFrame())
            {
                TryDoSwing();
                vEstimator.SetEstimationActve(false);
            }
        }
    }

    private void TryDoSwing()
    {
        if (vEstimator.CurrentAvgVelocity is Vector3 avgVel)
        {
            if (avgVel.sqrMagnitude >= minSwingSpeed * minSwingSpeed)
            {
                DebugEntryManager.updateEntry?.Invoke($"Swing Success", $"True, avgVel (<color=#FF0000>{avgVel.x}</color>, " +
                    $"<color=#00FF00>{avgVel.y}</color>, <color=#0000FF>{avgVel.z}</color>) is valid & fast enough", 3);

                ToggleWhipCurled(false);
                //transform.forward = avgVel.normalized;

                uncurlCorout = Coroutilities.DoAfterDelay(this, () =>
                {
                    ToggleWhipCurled(true);
                    //transform.rotation = defaultRot;
                    uncurlCorout = null;
                },
                0.375f);
            }
            else
            {
                DebugEntryManager.updateEntry?.Invoke($"Swing Success", $"False, avgVel (<color=#FF0000>{avgVel.x}</color>, " +
                    $"<color=#00FF00>{avgVel.y}</color>, <color=#0000FF>{avgVel.z}</color>) is valid but not fast enough", 3);
            }
        }
        else
        {
            DebugEntryManager.updateEntry?.Invoke($"Swing Success", $"False, avgVel is invalid", 3);
        }
    }

    private void ToggleWhipCurled(bool curled)
    {
        whipCurledRef.SetActive(curled);
        whipUncurledRef.SetActive(!curled);
    }
}