using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private GameObject hexLandPrefab;


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
                    GridBuildingSystem.Instance.grid.GetGridObject(x, z).SetTileHeight(hexTile.transform.localScale.y);
                    ToggleIsOccupiedWithTile(x, z);
                }
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

        if (hexTile.transform.localScale.y <= 5) {
            hexTile.transform.localScale = new Vector3(hexTile.transform.localScale.x, 5, hexTile.transform.localScale.z);
        }
        GridBuildingSystem.Instance.grid.GetGridObject(x, z).SetTileHeight(hexTile.transform.localScale.y);
    }

    public void ToggleIsOccupiedWithTile(int x, int z) {
        GridBuildingSystem.Instance.grid.GetGridObject(x, z).ToggleHasTile(true);
        GridPosition gridPos = GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition();
    }

    public void SpawnNextLayer() {
        List<GridPosition> vacantAndAvailableToSpawnHexTiles = new List<GridPosition>();

        List<GridPosition> currentHexTiles = new List<GridPosition>();

        for (int x = 0; x < GridBuildingSystem.Instance.gridWidth; x++)
        {
            for (int z = 0; z < GridBuildingSystem.Instance.gridHeight; z++)
            {
                if (GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetTileBool()) {
                    currentHexTiles.Add(GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition());
                }
            }
        }

        foreach (var tile in currentHexTiles)
        {
            List<GridPosition> neighbourGridPositionList = GridBuildingSystem.Instance.grid.GetNeighborHexTiles(GridBuildingSystem.Instance.grid.GetGridObject(tile.x, tile.z).GetGridPosition());

            foreach (var neighborTile in neighbourGridPositionList)
            {
                if (!GridBuildingSystem.Instance.grid.GetGridObject(neighborTile.x, neighborTile.z).GetTileBool()) {
                    if (!vacantAndAvailableToSpawnHexTiles.Contains(GridBuildingSystem.Instance.grid.GetGridObject(neighborTile.x, neighborTile.z).GetGridPosition())) {
                        GridBuildingSystem.Instance.grid.GetGridObject(neighborTile.x, neighborTile.z).SetInheritedParentTileHeight(GridBuildingSystem.Instance.grid.GetGridObject(tile.x, tile.z).GetTileHeight());
                        vacantAndAvailableToSpawnHexTiles.Add(GridBuildingSystem.Instance.grid.GetGridObject(neighborTile.x, neighborTile.z).GetGridPosition());
                    } 
                } 
            }
        }

        // Debug.Log(vacantAndAvailableToSpawnHexTiles.Count);

        foreach (var vacantAndAvailableTile in vacantAndAvailableToSpawnHexTiles)
        {
            GameObject hexTile = Instantiate(hexLandPrefab, GridBuildingSystem.Instance.grid.GetWorldPosition(vacantAndAvailableTile.x, vacantAndAvailableTile.z), hexLandPrefab.transform.rotation);
            ChangeTileHeight(hexTile, vacantAndAvailableTile.x, vacantAndAvailableTile.z, GridBuildingSystem.Instance.grid.GetGridObject(vacantAndAvailableTile.x, vacantAndAvailableTile.z).GetInheritedParentTileHeight());

            ToggleIsOccupiedWithTile(vacantAndAvailableTile.x, vacantAndAvailableTile.z);
            GridBuildingSystem.Instance.grid.GetGridObject(vacantAndAvailableTile.x, vacantAndAvailableTile.z).SetTileHeight(hexTile.transform.localScale.y);
        }
        
    }
}
