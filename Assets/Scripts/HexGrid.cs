using UnityEngine;
using UnityEngine.UI;

namespace HexMap
{
    /// <summary>
    /// The component for generating hex cells and manipulates their properties
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// Width of hex grid, along X axis
        /// </summary>
        public int width = 6;
        /// <summary>
        /// Height of hex grid, along Z axis
        /// </summary>
        public int height = 6;

        /// <summary>
        /// Hex cell prefab for generating
        /// </summary>
        public HexCell cellPrefab;
        /// <summary>
        /// Hex cell label prefab for displaying information
        /// </summary>
        public Text cellLabelPrefab;

        /// <summary>
        /// Default hex cell color
        /// </summary>
        public Color defaultColor = Color.white;

        private HexCell[] cells;
        private Canvas gridCanvas;
        private HexMesh hexMesh;

        private void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();
            hexMesh = GetComponentInChildren<HexMesh>();
            cells = new HexCell[height * width];
            for (int z = 0, i = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        private void Start()
        {
            hexMesh.Triangulate(cells);
        }

        /// <summary>
        /// Change the hex cell's color at specified position
        /// </summary>
        /// <param name="position">Hex cell position</param>
        /// <param name="color">Hex cell color</param>
        public void ColorCell(Vector3 position, Color color)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            HexCell cell = cells[index];
            cell.color = color;
            hexMesh.Triangulate(cells);
        }

        /// <summary>
        /// Add a hex cell with specified offset coordinates (x, z), and index i
        /// </summary>
        /// <param name="x">x component of offset coordinates</param>
        /// <param name="z">z component of offset coordinates</param>
        /// <param name="i">index i of cell array</param>
        private void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);

            var cell = cells[i] = Instantiate(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;

            var label = Instantiate(cellLabelPrefab);
            label.transform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
        }
    }
}
