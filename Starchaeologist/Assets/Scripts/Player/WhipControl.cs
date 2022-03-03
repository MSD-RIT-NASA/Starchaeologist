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

    private Quaternion defaultRot;
    private Coroutine uncurlCorout = null;

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
        if (!whipCurledRef || !whipUncurledRef)
        {
            Debug.LogError($"{gameObject.name}'s WhipControl is missing a curled/uncurled whip reference. Double check the inspector.");
        }

        defaultRot = transform.rotation;
        vEstimator.SetEstimationActve(false);
        ToggleWhipCurled(true);

        DebugEntryManager.updateEntry?.Invoke($"({name}) Test", $"<color=#FF0000>X</color>, <color=#00FF00>Y</color>, <color=#0000FF>Z</color>");
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
        if (vEstimator.CurrentAvgVelocity is Vector3 avgVel && avgVel.sqrMagnitude >= minSwingSpeed * minSwingSpeed)
        {
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
    }

    private void ToggleWhipCurled(bool curled)
    {
        whipCurledRef.SetActive(curled);
        whipUncurledRef.SetActive(!curled);
    }
}