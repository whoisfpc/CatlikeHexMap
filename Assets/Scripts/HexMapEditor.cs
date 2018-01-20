using UnityEngine;
using UnityEngine.EventSystems;

namespace HexMap
{
    /// <summary>
    /// Hex map component for edit hex map
    /// </summary>
    public class HexMapEditor : MonoBehaviour
    {
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

        /// <summary>
        /// flag for should apply color
        /// </summary>
        private bool applyColor;

        /// <summary>
        /// flag for should apply elevation
        /// </summary>
        private bool applyElevation = true;

        private int brushSize;

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
        }

        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                EditCells(hexGrid.GetCell(hit.point));
            }
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
                    cell.Color = activeColor;
                if (applyElevation)
                    cell.Elevation = activeElevation;
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

        public void SetBrushSize(float size)
        {
            brushSize = (int)size;
        }

        public void ShowUI(bool visible)
        {
            hexGrid.ShowUI(visible);
        }
    }
}
