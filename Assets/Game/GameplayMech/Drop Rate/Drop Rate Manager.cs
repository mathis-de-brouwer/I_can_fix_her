using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops>drops;


    void OnDestroy()
    {
        float randomNumber = UnityEngine.Random.Range(0f,100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach(Drops rate in drops)
        {
            if(randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);

                 
            }
        }
        //Checks if there are multiple drops that have a 100%/very high chance of dropping and is picking one of them to drop
        if(possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0, possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        } 

         
    }
}
