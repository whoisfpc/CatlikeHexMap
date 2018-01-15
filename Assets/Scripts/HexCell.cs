using UnityEngine;

namespace HexMap
{
    /// <summary>
    /// HexCell, contains properties and data of a hexagon cell
    /// </summary>
    public class HexCell : MonoBehaviour
    {
        /// <summary>
        /// hex coordinates of this hex cell
        /// </summary>
        public HexCoordinates coordinates;

        /// <summary>
        /// color of this hex cell
        /// </summary>
        public Color color;

        /// <summary>
        /// rect transform of hex cell's ui label 
        /// </summary>
        public RectTransform uiRect;

        /// <summary>
        /// neighbors of this hex cell
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;


        private int elevation;
        /// <summary>
        /// elevation of this hex cell
        /// </summary>
        public int Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
            }
        }

        public Vector3 Position => transform.localPosition;

        /// <summary>
        /// Get the neighbor of this hex cell at specified direction
        /// </summary>
        /// <param name="direction">the direction to obtain hex cell</param>
        /// <returns>the hex cell at specified direction</returns>
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        /// <summary>
        /// Set the neighbor of this hex cell at specified direction
        /// </summary>
        /// <param name="direction">the direction to set neighbor</param>
        /// <param name="cell">the hex cell at specified direction as neighbor</param>
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        /// <summary>
        /// Get HexEdgeType at specified direction
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>HexEdgeType at that direction</returns>
        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }

        /// <summary>
        /// Get HexEdgeType with other hex cell
        /// </summary>
        /// <param name="otherCell">the other hex cell</param>
        /// <returns>HexEdgeType for this hex cell and other hex cell</returns>
        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType( elevation, otherCell.elevation );
        }
    }
}
