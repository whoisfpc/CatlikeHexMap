﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace HexMap
{
    /// <summary>
    /// Hex map component for edit hex map
    /// </summary>
    public class HexMapEditor : MonoBehaviour
    {
        private enum OptionalToggle
        {
            Ignore,
            Yes,
            No
        }

        /// <summary>
        /// Optional color array
        /// </summary>
        public Color[] colors;
        /// <summary>
        /// Hexagon grid to edit
        /// </summary>
        public HexGrid hexGrid;

        /// <summary>
        /// Current active color
        /// </summary>
        private Color activeColor;

        /// <summary>
        /// Current active elevation
        /// </summary>
        private int activeElevation;

        private int activeWaterLevel;

        private int activeUrbanLevel;
        private int activeFarmLevel;
        private int activePlantLevel;

        /// <summary>
        /// flag for should apply color
        /// </summary>
        private bool applyColor;

        /// <summary>
        /// flag for should apply elevation
        /// </summary>
        private bool applyElevation = true;

        private bool applyWaterLevel = true;

        private bool applyUrbanLevel = true;
        private bool applyFarmLevel = true;
        private bool applyPlantLevel = true;

        private int brushSize;

        private OptionalToggle riverMode;
        private OptionalToggle roadMode;
        private OptionalToggle walledMode;

        private bool isDrag;
        private HexDirection dragDirection;
        private HexCell previousCell;

        private void Awake()
        {
            SelectColor(0);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
            else
            {
                previousCell = null;
            }
        }

        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                var currentCell = hexGrid.GetCell(hit.point);
                if (previousCell && previousCell != currentCell)
                {
                    ValidateDrg(currentCell);
                }
                else
                {
                    isDrag = false;
                }
                EditCells(currentCell);
                previousCell = currentCell;
            }
            else
            {
                previousCell = null;
            }
        }

        private void ValidateDrg(HexCell currentCell)
        {
            for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
            {
                if (previousCell.GetNeighbor(dragDirection) == currentCell)
                {
                    isDrag = true;
                    return;
                }
            }
            isDrag = false;
        }

        private void EditCells(HexCell center)
        {
            int centerX = center.coordinates.X;
            int centerZ = center.coordinates.Z;
            for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + brushSize; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
            for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
            {
                for (int x = centerX - brushSize; x <= centerX + r; x++)
                {
                    EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
                }
            }
        }

        private void EditCell(HexCell cell)
        {
            if (cell)
            {
                if (applyColor)
                {
                    cell.Color = activeColor;
                }
                if (applyElevation)
                {
                    cell.Elevation = activeElevation;
                }
                if (applyWaterLevel)
                {
                    cell.WaterLevel = activeWaterLevel;
                }
                if (applyUrbanLevel)
                {
                    cell.UrbanLevel = activeUrbanLevel;
                }
                if (riverMode == OptionalToggle.No)
                {
                    cell.RemoveRiver();
                }
                if (roadMode == OptionalToggle.No)
                {
                    cell.RemoveRoads();
                }
                if (applyFarmLevel)
                {
                    cell.FarmLevel = activeFarmLevel;
                }
                if (applyPlantLevel)
                {
                    cell.PlantLevel = activePlantLevel;
                }
                if (walledMode != OptionalToggle.Ignore)
                {
                    cell.Walled = walledMode == OptionalToggle.Yes;
                }
                if (isDrag)
                {
                    HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                    if (otherCell)
                    {
                        if (riverMode == OptionalToggle.Yes)
                        {
                            otherCell.SetOutgoingRiver(dragDirection);
                        }
                        if (roadMode == OptionalToggle.Yes)
                        {
                            otherCell.AddRoad(dragDirection);
                        }
                    }
                }
            }
        }

        public void SelectColor(int index)
        {
            applyColor = index >= 0;
            if (applyColor)
                activeColor = colors[index];
        }

        public void SetElevation(float elevation)
        {
            activeElevation = (int)elevation;
        }

        public void SetApplyElevation(bool toggle)
        {
            applyElevation = toggle;
        }

        public void SetApplyWaterLevel(bool toggle)
        {
            applyWaterLevel = toggle;
        }

        public void SetApplyUrbanLevel(bool toggle)
        {
            applyUrbanLevel = toggle;
        }

        public void SetWaterLevel(float level)
        {
            activeWaterLevel = (int)level;
        }

        public void SetBrushSize(float size)
        {
            brushSize = (int)size;
        }

        public void SetUrbanLevel(float level)
        {
            activeUrbanLevel = (int)level;
        }

        public void SetApplyFarmLevel(bool toggle)
        {
            applyFarmLevel = toggle;
        }

        public void SetFarmLevel(float level)
        {
            activeFarmLevel = (int)level;
        }

        public void SetApplyPlantLevel(bool toggle)
        {
            applyPlantLevel = toggle;
        }

        public void SetPlantLevel(float level)
        {
            activePlantLevel = (int)level;
        }

        public void ShowUI(bool visible)
        {
            hexGrid.ShowUI(visible);
        }

        public void SetRiverMode(int mode)
        {
            riverMode = (OptionalToggle)mode;
        }

        public void SetRoadMode(int mode)
        {
            roadMode = (OptionalToggle)mode;
        }

        public void SetWalledMode(int mode)
        {
            walledMode = (OptionalToggle)mode;
        }
    }
}
