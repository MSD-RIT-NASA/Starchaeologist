using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(VelocityEstimator))]
public class WhipControl : MonoBehaviour
{
    [Tooltip("The estimator we'll be referencing when determining if the player's swining fast enough. It may or " +
        "may not be on this particular game object.")]
    [SerializeField] private VelocityEstimator vEstimator = null;
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private WhipGrabTrigger grabTrigger;
    [Tooltip("To swing the whip, the velocity of the player's hand should have a magnitude greater than this.")]
    [SerializeField] private float minSwingSpeed;


    private void Start()
    {
        if (!vEstimator)
        {
            if (!TryGetComponent(out vEstimator))
            {
                Debug.LogError($"{gameObject.name}'s WhipControl was not given a VelocityEstimator to reference, and GetComponent " +
                    $"failed! Give {gameObject.name}'s WhipControl a VelocityEstimator reference, or attach one.");
            }
        }
        if (!grabTrigger)
        {
            if (!TryGetComponent(out grabTrigger))
            {
                Debug.LogError($"{gameObject.name}'s WhipControl was not given a WhipGrabTrigger to use when swinging, and GetComponent " +
                    $"failed! Give {gameObject.name}'s WhipControl a WhipGrabTrigger reference, or attach one.");
            }
        }
        vEstimator.SetEstimationActve(false);
        grabTrigger.enabled = false;
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

    private void TryDoSwing()
    {
        if (vEstimator.CurrentAvgVelocity.sqrMagnitude >= minSwingSpeed * minSwingSpeed)
        {
            grabTrigger.enabled = true;
            Coroutilities.DoAfterDelay(this, () => grabTrigger.enabled = false, 3);
        }
    }
}