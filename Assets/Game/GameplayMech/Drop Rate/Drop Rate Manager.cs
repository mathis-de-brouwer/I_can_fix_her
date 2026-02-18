using System.Collections.Generic;
using UnityEngine;


public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        [Range(0f, 100f)] public float dropRate;
    }

    public List<Drops> drops;

    /// <summary>
    /// Called by EnemyStats.Kill() when the enemy actually dies during gameplay.
    /// Do NOT use OnDestroy for spawning — it also fires on scene unload.
    /// </summary>
    public void TriggerDrop()
    {
        if (drops == null || drops.Count == 0)
            return;

        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
                possibleDrops.Add(rate);
        }

        if (possibleDrops.Count > 0)
        {
            Drops chosen = possibleDrops[Random.Range(0, possibleDrops.Count)];
            Instantiate(chosen.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
