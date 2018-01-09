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

        /// <summary>
        /// Handle mouse left button down, change the color of the corresponding cell
        /// </summary>
        private void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                hexGrid.ColorCell(hit.point, activeColor);
            }
        }

        /// <summary>
        /// Change the active color
        /// </summary>
        /// <param name="index">Color index</param>
        public void SelectColor(int index)
        {
            activeColor = colors[index];
        }
    }
}
