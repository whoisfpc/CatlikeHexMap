﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace HexMap.Gameplay
{
    public class HexGameUI : MonoBehaviour
    {
        public HexGrid grid;

        private HexCell currentCell;
        private HexUnit selectedUnit;

        public void SetEditMode(bool toggle)
        {
            enabled = !toggle;
            grid.ShowUI(!toggle);
            grid.ClearPath();
        }

        private void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    DoSelection();
                }
                else if (selectedUnit)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        DoMove();
                    }
                    else
                    {
                        DoPathfinding();
                    };
                }
            }
        }

        private void DoMove()
        {
            if (grid.HasPath)
            {
                selectedUnit.Travel(grid.GetPath());
                grid.ClearPath();
            }
        }

        private void DoPathfinding()
        {
            if (UpdateCurrentCell())
            {
                if (currentCell && selectedUnit.IsValidDestination(currentCell))
                {
                    grid.FindPath(selectedUnit.Location, currentCell, 24);
                }
                else
                {
                    grid.ClearPath();
                }
            }
        }

        private void DoSelection()
        {
            grid.ClearPath();
            UpdateCurrentCell();
            if (currentCell)
            {
                selectedUnit = currentCell.Unit;
            }
        }

        private bool UpdateCurrentCell()
        {
            HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (cell != currentCell)
            {
                currentCell = cell;
                return true;
            }
            return false;
        }
    }
}
