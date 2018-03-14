using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace HexMap
{
    /// <summary>
    /// The component for generating hex cells and manipulates their properties
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        /// <summary>
        /// hex map cell count
        /// </summary>
        public int cellCountX = 20, cellCountZ = 15;

        public HexCell cellPrefab;

        public Text cellLabelPrefab;

        public HexGridChunk chunkPrefab;

        public Texture2D noiseSource;

        /// <summary>
        /// Hash Grid random seed
        /// </summary>
        public int seed;

        private HexCell[] cells;
        private HexGridChunk[] chunks;
        private int chunkCountX, chunkCountZ;

        private void Awake()
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            CreateMap(cellCountX, cellCountZ);
        }

        public bool CreateMap(int x, int z)
        {
            bool negativeSize = x <= 0 || z <= 0;
            bool multipleChunkSize = x % HexMetrics.chunkSizeX == 0 && z % HexMetrics.chunkSizeZ == 0;
            if (negativeSize || !multipleChunkSize)
            {
                Debug.LogError("Unsupported map size.");
                return false;
            }
            if (chunks != null)
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    Destroy(chunks[i].gameObject);
                }
            }
            cellCountX = x;
            cellCountZ = z;
            chunkCountX = cellCountX / HexMetrics.chunkSizeX;
            chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
            CreateChunks();
            CreateCells();
            return true;
        }

        private void OnEnable()
        {
            if (!HexMetrics.noiseSource)
            {
                HexMetrics.noiseSource = noiseSource;
                HexMetrics.InitializeHashGrid(seed);
            }
        }

        public void ShowUI(bool visible)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].ShowUI(visible);
            }
        }

        private void CreateChunks()
        {
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            for (int z = 0, i = 0; z < chunkCountZ; z++)
            {
                for (int x = 0; x < chunkCountX; x++)
                {
                    HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                    chunk.transform.SetParent(transform);
                }
            }
        }

        private void CreateCells()
        {
            cells = new HexCell[cellCountZ * cellCountX];
            for (int z = 0, i = 0; z < cellCountZ; z++)
            {
                for (int x = 0; x < cellCountX; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(cellCountX);
            writer.Write(cellCountZ);

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Save(writer);
            }
        }

        public void Load(BinaryReader reader, int header)
        {
            int x = 20, z = 15;
            if (header >= 1)
            {
                x = reader.ReadInt32();
                z = reader.ReadInt32();
            }
            if (x != cellCountX || z != cellCountZ)
            {
                if (!CreateMap(x, z))
                {
                    return;
                }
            }

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Load(reader);
            }
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].Refresh();
            }
        }

        /// <summary>
        /// Obtain hex cell that at position
        /// </summary>
        /// <param name="position">Hex cell position</param>
        /// <returns>the hex cell at specified position</returns>
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            return cells[index];
        }

        /// <summary>
        /// Obtain hex cell according to hex coordinates
        /// </summary>
        /// <param name="coordinates">Hex cell coordinates</param>
        /// <returns>the hex cell at specified coordinates</returns>
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= cellCountZ)
            {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX)
            {
                return null;
            }
            return cells[x + z * cellCountX];
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
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {
                if ((z & 1 ) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                    }
                }
            }

            var label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            cell.uiRect = label.rectTransform;
            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            int chunkX = x / HexMetrics.chunkSizeX;
            int chunkZ = z / HexMetrics.chunkSizeZ;
            HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

            int localX = x - chunkX * HexMetrics.chunkSizeX;
            int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }
    }
}