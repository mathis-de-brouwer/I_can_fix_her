using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandom : MonoBehaviour
{

    public List<GameObject> propSpawnPoint;
    public List<GameObject> propPrefab;

    void Start()
    {
        SpawnProps();
    }

    void Update()
    {
        
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoint)
        {
            int rand = Random.Range(0, propPrefab.Count);
            GameObject prop = Instantiate(propPrefab[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;
        }

    }
}
