using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipGrabbableItem : MonoBehaviour
{
    [Tooltip("How long it will take for this grabbable object to fly towards the player, when grabbed.")]
    [SerializeField] [Min(0)] private float flyDuration;
    [Tooltip("This object will fly toward a supplied transform's position, plus this vector.")]
    [SerializeField] private Vector3 destinationOffset;

    private Coroutine flyToPlayerCorout;

    public void FlyToGrabber(Transform grabberTform)
    {
        //If this is called when the coroutine is not null, that means the coroutine is already running. No
        //need to run it a second time.
        if (flyToPlayerCorout == null)
        {
            //"lerp" over to the thing that grabbed this object in flyDuration seconds.
            float progress = 0;
            flyToPlayerCorout = Coroutilities.DoUntil
            (
                this,
                () =>
                {
                    progress += Time.deltaTime / flyDuration;
                    transform.position = Vector3.Lerp(transform.position, grabberTform.position + destinationOffset, progress);
                },
                () => progress >= 1
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Object ${gameObject.name} was just collected!");
            Coroutilities.TryStopCoroutine(this, ref flyToPlayerCorout);
            //TODO: Give player their well deserved points
            Destroy(gameObject);
        }
    }
}
