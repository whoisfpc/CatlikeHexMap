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

        private Color color;
        /// <summary>
        /// color of this hex cell
        /// </summary>
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                    return;
                color = value;
                Refresh();
            }
        }

        /// <summary>
        /// rect transform of hex cell's ui label 
        /// </summary>
        public RectTransform uiRect;

        /// <summary>
        /// grid chunk it belongs to
        /// </summary>
        public HexGridChunk chunk;

        /// <summary>
        /// neighbors of this hex cell
        /// </summary>
        [SerializeField]
        private HexCell[] neighbors;


        private int elevation = int.MinValue;
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
                if (elevation == value)
                    return;
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                Refresh();
            }
        }

        public Vector3 Position => transform.localPosition;

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType( elevation, otherCell.elevation );
        }

        private void Refresh()
        {
            if (chunk)
                chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}
