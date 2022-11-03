using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private GameObject hexLandPrefab;
    [SerializeField] private int hexLayers = 2;
    [SerializeField] private int testButtonLayers = 20;

    public static WorldGeneration instance;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        SpawnCenterTile();
        SpawnStartingLayers();
    }

    private void SpawnCenterTile() {
        for (int x = 0; x < GridBuildingSystem.Instance.gridWidth; x++)
        {
            for (int z = 0; z < GridBuildingSystem.Instance.gridHeight; z++)
            {
                if (x == GridBuildingSystem.Instance.gridWidth / 2 && z == GridBuildingSystem.Instance.gridHeight / 2) {
                    GameObject hexTile = Instantiate(hexLandPrefab, GridBuildingSystem.Instance.grid.GetWorldPosition(x, z), hexLandPrefab.transform.rotation);
                    // ChangeTileHeight(hexTile, x, z, GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetTileHeight());
                    GridBuildingSystem.Instance.grid.GetGridObject(x, z).SetTileHeight(hexTile.transform.localScale.y);
                    ToggleIsOccupiedWithTile(x, z);
                }
            }
        }
    }

    private void SpawnStartingLayers() {
        for (int i = 0; i < hexLayers; i++)
        {
            SpawnNextLayer();
        }
    }

    public void ChangeTileHeight(GameObject hexTile, int x, int z, float neighborTileHeight) {
        float baseHeight = 0;
        int findRandomHeightFlip = Random.Range(0, 2);
        if (findRandomHeightFlip == 0) {
            baseHeight = FindAverageHeightOfSurroundingTiles(x, z);
        } else {
            baseHeight = neighborTileHeight;
        }

        int randomNum = Random.Range(0, 2);
        float heightChange = 0;
        if (randomNum == 0) {
            heightChange = 20f;
        } else if (randomNum == 1) {
            heightChange = -20f;
        } 

        baseHeight = (RoundToMultipleOf(20, Mathf.RoundToInt(baseHeight)));

        hexTile.transform.localScale += new Vector3(0, baseHeight + heightChange, 0);

        if (hexTile.transform.localScale.y <= 5) {
            hexTile.transform.localScale = new Vector3(hexTile.transform.localScale.x, 10, hexTile.transform.localScale.z);
        }
        GridBuildingSystem.Instance.grid.GetGridObject(x, z).SetTileHeight(hexTile.transform.localScale.y);
    }

    private int RoundToMultipleOf(int multipleOf, int numberToRound) {
        int rem = numberToRound % multipleOf;
        return rem >= 5 ? (numberToRound - rem + multipleOf) : (numberToRound - rem);
    }

    public float FindAverageHeightOfSurroundingTiles(int x, int z) {
        float averageHeight = 0;

        List<GridPosition> neighbourGridPositionListWithActiveHexTile = new List<GridPosition>();

        foreach (var neighborTile in GridBuildingSystem.Instance.grid.GetNeighborHexTiles(GridBuildingSystem.Instance.grid.GetGridObject(x, z).GetGridPosition()))
        {
            neighbourGridPositionListWithActiveHexTile.Add(GridBuildingSystem.Instance.grid.GetGridObject(neighborTile.x, neighborTile.z).GetGridPosition());
        }


        foreach (var validNeighboreTile in neighbourGridPositionListWithActiveHexTile)
        {
            averageHeight += GridBuildingSystem.Instance.grid.GetGridObject(validNeighboreTile.x, validNeighboreTile.z).GetTileHeight();
        }

        averageHeight = averageHeight / neighbourGridPositionListWithActiveHexTile.Count;
        
        return averageHeight;
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

        foreach (var vacantAndAvailableTile in vacantAndAvailableToSpawnHexTiles)
        {
            GameObject hexTile = Instantiate(hexLandPrefab, GridBuildingSystem.Instance.grid.GetWorldPosition(vacantAndAvailableTile.x, vacantAndAvailableTile.z), hexLandPrefab.transform.rotation);
            ChangeTileHeight(hexTile, vacantAndAvailableTile.x, vacantAndAvailableTile.z, GridBuildingSystem.Instance.grid.GetGridObject(vacantAndAvailableTile.x, vacantAndAvailableTile.z).GetInheritedParentTileHeight());

            ToggleIsOccupiedWithTile(vacantAndAvailableTile.x, vacantAndAvailableTile.z);
            GridBuildingSystem.Instance.grid.GetGridObject(vacantAndAvailableTile.x, vacantAndAvailableTile.z).SetTileHeight(hexTile.transform.localScale.y);
        }
    }

    
}
