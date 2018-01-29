namespace HexMap
{
    /// <summary>
    /// hex direction enumeration
    /// </summary>
    public enum HexDirection
    {
        /// <summary>
        /// Northeast
        /// </summary>
        NE,
        /// <summary>
        /// East
        /// </summary>
        E,
        /// <summary>
        /// Southeast
        /// </summary>
        SE,
        /// <summary>
        /// Southwest
        /// </summary>
        SW,
        /// <summary>
        /// West
        /// </summary>
        W,
        /// <summary>
        /// Northwest 
        /// </summary>
        NW
    }

    /// <summary>
    /// Extensions methods for HexDirection enumeration
    /// </summary>
    public static class HexDirectionExtensions
    {
        /// <summary>
        /// Obtain opposite direction of specified direction
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>opposite direction of specified direction</returns>
        public static HexDirection Opposite(this HexDirection direction)
        {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }

        /// <summary>
        /// Obtain previous direction of specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>previous direction of specified direction</returns>
        public static HexDirection Previous(this HexDirection direction)
        {
            return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
        }

        /// <summary>
        /// Obtain previous of previous direction of specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>previous direction of specified direction</returns>
        public static HexDirection Previous2(this HexDirection direction)
        {
            direction -= 2;
            return direction >= HexDirection.NE ? direction : (direction + 6);
        }

        /// <summary>
        /// Obtain next direction of specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>next direction of specified direction</returns>
        public static HexDirection Next(this HexDirection direction)
        {
            return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
        }

        /// <summary>
        /// Obtain next of next direction of specified direction(clockwise direction)
        /// </summary>
        /// <param name="direction">the direction</param>
        /// <returns>next direction of specified direction</returns>
        public static HexDirection Next2(this HexDirection direction)
        {
            direction += 2;
            return direction <= HexDirection.NW ? direction : (direction - 6);
        }
    }
}
