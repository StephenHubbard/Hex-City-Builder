using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem3D : MonoBehaviour {

    public static GridBuildingSystem3D Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;
    [SerializeField] private GameObject hexVisualPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;

    private GridSystemHex<GridObject> grid;

    private void Awake() {
        Instance = this;

        grid = new GridSystemHex<GridObject>(gridWidth, gridHeight, cellSize, (GridSystemHex<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));


    }

    private void Start() {
        SpawnHexVisuals();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // Debug.Log(grid.GetGridPosition(Mouse3D.GetMouseWorldPosition()));
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

        private GridPosition gridPosition;

        public GridObject(GridSystemHex<GridObject> grid, GridPosition gridPosition) {
            this.grid = grid;
            this.gridPosition = gridPosition;
        }

        public override string ToString() {
            return gridPosition.x + ", " + gridPosition.z;
        }
    }
}
