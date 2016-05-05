using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityParser2000
{
	/// <summary>
	/// Represents the city map and every tile within, and provides access to individual tiles.
	/// </summary>
	public class CityMap
	{
		/// <summary>
		/// Indicates the number of city tiles along each edge of the city. Cities are always square.
		/// </summary>
		public const int TILES_PER_SIDE = 128;

		public Tile[,] Tiles { get; private set; }

		public CityMap()
		{
			Tiles = new Tile[TILES_PER_SIDE, TILES_PER_SIDE];

            for (int i = 0; i < TILES_PER_SIDE; i++)
            {
                for (int j = 0; j < TILES_PER_SIDE; j++)
                {
                    Tiles[i, j] = new Tile();
                }
            }
		}
	}
}
