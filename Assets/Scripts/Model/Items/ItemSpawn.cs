using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public GameObject[] items;
    public int numberOfItemsPerLevel;

    public void Start()
    {
        for (int i = 0; i < numberOfItemsPerLevel; i++)
        {
            Instantiate(items[Random.Range(0, items.Length)], new Vector3(-14, 3 * i, 0),Quaternion.identity);
        }
    }
}
