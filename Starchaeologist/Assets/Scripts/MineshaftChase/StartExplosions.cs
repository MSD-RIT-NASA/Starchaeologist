using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExplosions : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> explosions;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < explosions.Count; i++)
        {
            explosions[i].Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            for(int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Play();
            }
        }
    }
}
