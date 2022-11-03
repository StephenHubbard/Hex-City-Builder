using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileBase : MonoBehaviour
{
    [SerializeField] private GameObject hexTileTop;
    [SerializeField] private Transform hexTileTopSpawnPoint;

    void Start()
    {
        GameObject hexTileTopGO = Instantiate(hexTileTop, hexTileTopSpawnPoint.position, hexTileTop.transform.rotation);
        hexTileTopGO.transform.SetParent(this.transform);
        // hexTileTop.transform.position = Vector3.zero;
    }

    
}
