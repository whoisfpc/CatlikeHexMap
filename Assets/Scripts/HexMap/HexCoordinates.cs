using UnityEngine;
using System.IO;

namespace HexMap
{
    /// <summary>
    /// Hexagonal coordinates, used to identify a hex cell in a hex grid,
    /// satisfied x + y + z === 0
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {
        [SerializeField]
        private int x, z;

        /// <summary>
        ///  X component of the hex coordinates
        /// </summary>
        public int X => x;

        /// <summary>
        ///  Z component of the hex coordinates
        /// </summary>
        public int Z => z;

        /// <summary>
        ///  Y component of the hex coordinates
        /// </summary>
        public int Y => -X - Z;

        public HexCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(x);
            writer.Write(z);
        }

        public static HexCoordinates Load(BinaryReader reader)
        {
            HexCoordinates c;
            c.x = reader.ReadInt32();
            c.z = reader.ReadInt32();
            return c;
        }

        /// <summary>
        /// Convert offset coordinates to hex coordinates
        /// </summary>
        /// <param name="x">x component of the offset coordinates</param>
        /// <param name="z">z component of the offset coordinates</param>
        /// <returns>A HexCoordinates corresponding specified offset coordinates</returns>
        public static HexCoordinates FromOffsetCoordinates(int x, int z)
        {
            return new HexCoordinates(x - z / 2, z);
        }

        /// <summary>
        /// Convert position to hex coordinates
        /// </summary>
        /// <param name="position">gived local position in Hex Grid transform</param>
        /// <returns>A HexCoordinates corresponding specified position</returns>
        public static HexCoordinates FromPosition(Vector3 position)
        {
            float x = position.x / (HexMetrics.innerRadius * 2f);
            float y = -x;
            float offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);
            if (iX + iY + iZ != 0)
            {
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);

                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }

            return new HexCoordinates(iX, iZ);
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        /// <summary>
        /// A string represent this hex coordinates,
        /// which every component lies on separate lines
        /// </summary>
        /// <returns>A string represent this hex coordinates </returns>
        public string ToStringOnSeparateLines() => $"{X}\n{Y}\n{Z}";

        /// <summary>
        /// Distance to other hex
        /// </summary>
        /// <param name="other">other hex coordinates</param>
        /// <returns>distance to other hex coordinates</returns>
        public int DistanceTo(HexCoordinates other)
        {
            return
                ((x < other.x ? other.x - x : x - other.x) +
                (Y < other.Y ? other.Y - Y : Y - other.Y) +
                (z < other.z ? other.z - z : z - other.z)) / 2;
        }
    }
}
