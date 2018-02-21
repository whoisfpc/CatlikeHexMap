using UnityEngine;
using System.IO;

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

        private int specialIndex;
        /// <summary>
        /// index of special features
        /// </summary>
        public int SpecialIndex
        {
            get
            {
                return specialIndex;
            }
            set
            {
                if (specialIndex != value && !HasRiver)
                {
                    specialIndex = value;
                    RemoveRoads();
                    RefreshSelfOnly();
                }
            }
        }

        public bool IsSpecial => specialIndex > 0;

        private int farmLevel;
        /// <summary>
        /// Farm feature likeihood level
        /// </summary>
        public int FarmLevel
        {
            get
            {
                return farmLevel;
            }
            set
            {
                if (farmLevel != value)
                {
                    farmLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        private int plantLevel;
        /// <summary>
        /// Plant feature likeihood
        /// </summary>
        public int PlantLevel
        {
            get
            {
                return plantLevel;
            }
            set
            {
                if (plantLevel != value)
                {
                    plantLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        private int urbanLevel;
        /// <summary>
        /// urban feature likeihood level
        /// </summary>
        public int UrbanLevel
        {
            get
            {
                return urbanLevel;
            }
            set
            {
                if (urbanLevel != value)
                {
                    urbanLevel = value;
                    RefreshSelfOnly();
                }
            }
        }

        private bool walled;
        /// <summary>
        /// wall feature
        /// </summary>
        public bool Walled
        {
            get
            {
                return walled;
            }
            set
            {
                if (walled != value)
                {
                    walled = value;
                    Refresh();
                }
            }
        }

        private int terrainTypeIndex;
        public int TerrainTypeIndex
        {
            get
            {
                return terrainTypeIndex;
            }
            set
            {
                if (terrainTypeIndex != value)
                {
                    terrainTypeIndex = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// color of this hex cell
        /// </summary>
        public Color Color => HexMetrics.colors[terrainTypeIndex];

        private bool hasIncomingRiver;
        public bool HasIncomingRiver => hasIncomingRiver;

        private bool hasOutgoingRiver;
        public bool HasOutgoingRiver => hasOutgoingRiver;

        private HexDirection incomingRiver;
        public HexDirection IncomingRiver => incomingRiver;

        private HexDirection outgoingRiver;
        public HexDirection OutgoingRiver => outgoingRiver;

        public bool HasRiver => hasIncomingRiver || hasOutgoingRiver;

        public bool HasRiverBeginOrEnd => hasIncomingRiver != hasOutgoingRiver;

        public HexDirection RiverBeginOrEndDirection => hasIncomingRiver ? incomingRiver : outgoingRiver;

        private int waterLevel;
        public int WaterLevel
        {
            get
            {
                return waterLevel;
            }
            set
            {
                if (waterLevel == value)
                {
                    return;
                }
                waterLevel = value;
                ValidateRivers();
                Refresh();
            }
        }

        public bool IsUnderwater => waterLevel > elevation;

        /// <summary>
        /// Stream bed y position
        /// </summary>
        public float StreamBedY => (elevation + HexMetrics.streamBedElevationOffset) * HexMetrics.elevationStep;

        /// <summary>
        /// River surface y position
        /// </summary>
        public float RiverSurfaceY => (elevation + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;

        /// <summary>
        /// Water surface y position
        /// </summary>
        public float WaterSurfaceY => (waterLevel + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;

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

        [SerializeField]
        private bool[] roads;

        public bool HasRoads
        {
            get
            {
                for (int i = 0; i < roads.Length; i++)
                {
                    if (roads[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }


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
                RefreshPosition();
                ValidateRivers();
                for (int i = 0; i < roads.Length; i++)
                {
                    if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                    {
                        SetRoad(i, false);
                    }
                }
                Refresh();
            }
        }

        public Vector3 Position => transform.localPosition;

        private void RefreshPosition()
        {
            Vector3 position = transform.localPosition;
            position.y = elevation * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(terrainTypeIndex);
            writer.Write(elevation);
            writer.Write(waterLevel);
            writer.Write(urbanLevel);
            writer.Write(farmLevel);
            writer.Write(plantLevel);
            writer.Write(specialIndex);
            writer.Write(walled);

            writer.Write(hasIncomingRiver);
            writer.Write((int)incomingRiver);
            writer.Write(hasOutgoingRiver);
            writer.Write((int)outgoingRiver);

            for (int i = 0; i < roads.Length; i++)
            {
                writer.Write(roads[i]);
            }
        }

        public void Load(BinaryReader reader)
        {
            terrainTypeIndex = reader.ReadInt32();
            elevation = reader.ReadInt32();
            RefreshPosition();
            waterLevel = reader.ReadInt32();
            urbanLevel = reader.ReadInt32();
            farmLevel = reader.ReadInt32();
            plantLevel = reader.ReadInt32();
            specialIndex = reader.ReadInt32();
            walled = reader.ReadBoolean();

            hasIncomingRiver = reader.ReadBoolean();
            incomingRiver = (HexDirection)reader.ReadInt32();

            hasOutgoingRiver = reader.ReadBoolean();
            outgoingRiver = (HexDirection)reader.ReadInt32();

            for (int i = 0; i < roads.Length; i++)
            {
                roads[i] = reader.ReadBoolean();
            }
        }

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

        public bool HasRiverThroughEdge(HexDirection direction)
        {
            return
                hasIncomingRiver && incomingRiver == direction ||
                hasOutgoingRiver && outgoingRiver == direction;
        }

        public void RemoveOutgoingRiver()
        {
            if (!hasOutgoingRiver)
            {
                return;
            }
            hasOutgoingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(outgoingRiver);
            neighbor.hasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveIncomingRiver()
        {
            if (!hasIncomingRiver)
            {
                return;
            }
            hasIncomingRiver = false;
            RefreshSelfOnly();

            HexCell neighbor = GetNeighbor(incomingRiver);
            neighbor.hasOutgoingRiver = false;
            neighbor.RefreshSelfOnly();
        }

        public void RemoveRiver()
        {
            RemoveOutgoingRiver();
            RemoveIncomingRiver();
        }

        public void SetOutgoingRiver(HexDirection direction)
        {
            if (hasOutgoingRiver && outgoingRiver == direction)
            {
                return;
            }
            HexCell neighbor = GetNeighbor(direction);
            if (!IsValidRiverDestination(neighbor))
            {
                return;
            }
            RemoveOutgoingRiver();
            if (hasIncomingRiver && incomingRiver == direction)
            {
                RemoveIncomingRiver();
            }
            hasOutgoingRiver = true;
            outgoingRiver = direction;
            specialIndex = 0;
            neighbor.RemoveIncomingRiver();
            neighbor.hasIncomingRiver = true;
            neighbor.incomingRiver = direction.Opposite();
            neighbor.specialIndex = 0;
            SetRoad((int)direction, false);
        }

        public bool HasRoadThroughEdge(HexDirection direction)
        {
            return roads[(int)direction];
        }

        public void AddRoad(HexDirection direction)
        {
            bool notHasSpecial = !IsSpecial && !GetNeighbor(direction).IsSpecial;
            if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1 && notHasSpecial)
            {
                SetRoad((int)direction, true);
            }
        }

        public void RemoveRoads()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (roads[i])
                {
                    SetRoad(i, false);
                }
            }
        }

        public int GetElevationDifference(HexDirection direction)
        {
            int difference = elevation - GetNeighbor(direction).elevation;
            return difference >= 0 ? difference : -difference;
        }

        private void SetRoad(int index, bool state)
        {
            roads[index] = state;
            neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
            neighbors[index].RefreshSelfOnly();
            RefreshSelfOnly();
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

        private void RefreshSelfOnly()
        {
            chunk.Refresh();
        }

        private bool IsValidRiverDestination(HexCell neighbor)
        {
            return neighbor && (elevation >= neighbor.elevation || waterLevel == neighbor.elevation);
        }

        private void ValidateRivers()
        {
            if (hasOutgoingRiver && !IsValidRiverDestination(GetNeighbor(outgoingRiver)))
            {
                RemoveOutgoingRiver();
            }
            if (hasIncomingRiver && !GetNeighbor(incomingRiver).IsValidRiverDestination(this))
            {
                RemoveIncomingRiver();
            }
        }
    }
}
