using System.Collections.Generic;
using UnityEngine;

public class PropRandom : MonoBehaviour
{
    public List<GameObject> propSpawnPoint;
    public List<GameObject> propPrefab;

    private bool _spawned = false;

    private void Start()
    {
        if (_spawned) return;
        _spawned = true;
        SpawnProps();
    }

    private void SpawnProps()
    {
        if (propPrefab == null || propPrefab.Count == 0) return;

        //Debug.Log("[PropRandom] " + name + " starting SpawnProps - " + (propSpawnPoint?.Count ?? 0) + " spawn point(s) found.");

        foreach (GameObject sp in propSpawnPoint)
        {
            if (sp == null)
            {
                //Debug.LogWarning("[PropRandom] " + name + " has a NULL entry in propSpawnPoint list - skipping.");
                continue;
            }

            int existingChildren = sp.transform.childCount;
            if (existingChildren > 0)
                //Debug.LogWarning("[PropRandom] " + name + " spawn point " + sp.name + " already has " + existingChildren + " child(ren) before spawning. Clearing them.");

            // Clear any already-existing prop children (prevents duplicates on re-use)
            for (int i = sp.transform.childCount - 1; i >= 0; i--)
                Destroy(sp.transform.GetChild(i).gameObject);

            int rand = Random.Range(0, propPrefab.Count);
            GameObject prop = Instantiate(propPrefab[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;

            //Debug.Log("[PropRandom] " + name + " spawn point " + sp.name + " at " + sp.transform.position + " spawned " + prop.name + ". Children now: " + sp.transform.childCount);
        }

        //Debug.Log("[PropRandom] " + name + " SpawnProps complete.");
    }
}
