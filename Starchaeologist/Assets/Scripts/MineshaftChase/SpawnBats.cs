using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBats : MonoBehaviour
{
    [SerializeField]
    private GameObject batPrefab;

    private int numBats;

    private List<GameObject> bats;

    // Start is called before the first frame update
    void Start()
    {
        numBats = 0;
        bats = new List<GameObject>();
        Invoke("BatSpawn", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        //Get bats to move next
    }


    public void RepeatBat()
    {
        //Invoke("BatSpawn", 0.2f);
    }


    public void BatSpawn()
    {
        bats.Add(Instantiate(batPrefab, this.transform));
        bats[numBats].transform.Rotate(0.0f, 5f * numBats, 0.0f, Space.Self);
        numBats++;

        if(numBats > 5)
        {
            CancelInvoke("BatSpawn");
            bats.Clear();
        }
    }
}
