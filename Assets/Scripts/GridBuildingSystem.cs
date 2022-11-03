using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour {

    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;
    [SerializeField] private GameObject hexVisualPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;
    [SerializeField] private bool showHexVisuals = true;

    public GridSystemHex<GridObject> grid;

    private void Awake() {
        Instance = this;

        grid = new GridSystemHex<GridObject>(gridWidth, gridHeight, cellSize, (GridSystemHex<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
    }

    private void Start() {
        if (showHexVisuals) {
            SpawnHexVisuals();
        }

    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            GridPosition gridPos = grid.GetGridPosition(Mouse3D.GetMouseWorldPosition());
            // Debug.Log(GridBuildingSystem.Instance.grid.GetGridObject(gridPos.x, gridPos.z).GetTileBool());
            // Debug.Log(GridBuildingSystem.Instance.grid.GetGridObject(gridPos.x, gridPos.z).GetGridPosition());
            // Debug.Log(GridBuildingSystem.Instance.grid.GetGridObject(gridPos.x, gridPos.z).GetTileHeight());

            // List<GridPosition> neighbourGridPositionList = GridBuildingSystem.Instance.grid.GetNeighborHexTiles(GridBuildingSystem.Instance.grid.GetGridObject(gridPos.x, gridPos.z).GetGridPosition());

            // foreach (var tile in neighbourGridPositionList)
            // {
            //     Debug.Log(GridBuildingSystem.Instance.grid.GetGridObject(tile.x, tile.z).GetGridPosition());
            // }

            WorldGeneration.instance.FindAverageHeightOfSurroundingTiles(gridPos.x, gridPos.z);
        }
    }


    private void SpawnHexVisuals() {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Instantiate(hexVisualPrefab, grid.GetWorldPosition(x, z), hexVisualPrefab.transform.rotation);
            }
        }
    }

    public class GridObject {

        private GridSystemHex<GridObject> grid;
        private bool hasTile = false;
        private GridPosition gridPosition;
        private float tileHeight;
        private float inheritedParentTileHeight;

        public GridObject(GridSystemHex<GridObject> grid, GridPosition gridPosition) {
            this.grid = grid;
            this.gridPosition = gridPosition;
        }

        public override string ToString() {
            return gridPosition.x + ", " + gridPosition.z;
        }

        public void ToggleHasTile(bool hasTile) {
            this.hasTile = hasTile;
        }

        public bool GetTileBool() {
            return hasTile;
        }

        public GridPosition GetGridPosition() {
            return gridPosition;
        }

        public void SetTileHeight(float tileHeight) {
            this.tileHeight = tileHeight;
        }

        public float GetTileHeight() {
            return tileHeight;
        }

        public float GetInheritedParentTileHeight() {
            return inheritedParentTileHeight;
        }

        public void SetInheritedParentTileHeight(float inheritedParentTileHeight) {
            this.inheritedParentTileHeight = inheritedParentTileHeight;
        }
    }
}
