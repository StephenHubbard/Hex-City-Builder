using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private GameObject hexLandPrefab;

    private List<GridPosition> allHexTiles = new List<GridPosition>();

    void Start()
    {
        SpawnCenterTile();
    }

    private void SpawnCenterTile() {
        for (int x = 0; x < GridBuildingSystem.Instance.gridWidth; x++)
        {
            for (int z = 0; z < GridBuildingSystem.Instance.gridHeight; z++)
            {
                if (x == GridBuildingSystem.Instance.gridWidth / 2 && z == GridBuildingSystem.Instance.gridHeight / 2) {
                    GameObject hexTile = Instantiate(hexLandPrefab, GridBuildingSystem.Instance.grid.GetWorldPosition(x, z), hexLandPrefab.transform.rotation);
                    ChangeTileHeight(hexTile, x, z, GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetTileHeight());
                    SpawnNeighborHexTiles(x, z);
                    ToggleIsOccupiedWithTile(x, z);
                }
            }
        }
    }

    private void SpawnNeighborHexTiles(int x, int z) {
        List<GridPosition> neighborTiles;

        neighborTiles = GridBuildingSystem.Instance.grid.GetNeighborHexTiles(GridBuildingSystem.Instance.grid.GetWorldPosition(x, z));

        foreach (var tile in neighborTiles)
        {
            if (!GridBuildingSystem.Instance.grid.GetGridObject(tile.x, tile.z).GetTileBool()) { 
                GameObject hexTile = Instantiate(hexLandPrefab, GridBuildingSystem.Instance.grid.GetWorldPosition(tile.x, tile.z), hexLandPrefab.transform.rotation);
                ChangeTileHeight(hexTile, tile.x, tile.z, GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetTileHeight());
                ToggleIsOccupiedWithTile(tile.x, tile.z);
            }
        }
    }

    public void ChangeTileHeight(GameObject hexTile, int x, int z, float neighborTileHeight) {
        int randomNum = Random.Range(0, 2);
        float heightChange = 0;
        if (randomNum == 0) {
            heightChange = 20f;
        } else if (randomNum == 1) {
            heightChange = -20f;
        }
        hexTile.transform.localScale += new Vector3(0, neighborTileHeight + heightChange, 0);
        if (hexTile.transform.localScale.y <= 1) {
            hexTile.transform.localScale = new Vector3(hexTile.transform.localScale.x, 5, hexTile.transform.localScale.z);
        }
        GridBuildingSystem.Instance.grid.GetGridObject(x, z).SetTileHeight(hexTile.transform.localScale.y);
    }

    public void ToggleIsOccupiedWithTile(int x, int z) {
        GridBuildingSystem.Instance.grid.GetGridObject(x, z).ToggleHasTile(true);
        GridPosition gridPos = GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition();
    }

    public void SpawnNextLayer() {
        allHexTiles.Clear();

        for (int x = 0; x < GridBuildingSystem.Instance.gridWidth; x++)
        {
            for (int z = 0; z < GridBuildingSystem.Instance.gridHeight; z++)
            {
                if (GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetTileBool()) {
                    if (!allHexTiles.Contains(GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition())) {
                        allHexTiles.Add(GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition());
                    }
                }
            }
        }


        foreach (var tile in allHexTiles)
        {
            SpawnNeighborHexTiles(tile.x, tile.z);
        }

        // StartCoroutine(SpawnNeighborTilesCo());

    }

    private IEnumerator SpawnNeighborTilesCo() {
        foreach (var tile in allHexTiles)
        {
            SpawnNeighborHexTiles(tile.x, tile.z);
            yield return new WaitForSeconds(.5f);
        }
    }
}
