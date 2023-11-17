using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [SerializeField] List<Vector2> trappedPlates;
    [SerializeField] Vector3 hitBoxPos;
    [SerializeField] Vector3 hitBoxSize;

    public List<Vector2> PossiblePlates { get { return trappedPlates; } }
    public Vector3 HitBoxOffset { get {return hitBoxPos; } }
    public Vector3 HitBoxSize { get {return hitBoxSize; } }


    public abstract void ActivateTrap();


/*    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        

        for(int i = 0; i < trappedPlates.Count; i++)
        {
            Gizmos.DrawWireCube(trappedPlates[i].transform.position + hitBoxPos, hitBoxSize);
        }
    }*/


}
