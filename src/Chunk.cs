/***
 * Code for game of life
 * Author: Leonardo Rezza <lrezza@kth.se>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Chunk
    {
        public static int Width;
        public static Dictionary<Position, Chunk> Chunks;

        public bool needsDrawUpdate = true;

        private Position localPosition;
        private Cell[,] cells;

        public Chunk(Position localPosition)
        {
            this.localPosition = localPosition;
            cells = new Cell[Width, Width];
        }

        //Populates the chunk with Width ^ 2 cells 
        public void Populate()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    cells[x, y] = new Cell(new Position(x, y), false, this);
                }
            }
        }

        //Returns linear array of the cells in this chunk
        public Cell[,] GetCells()
        {
            return cells;
        }

        //Returns the global pixel-position of this chunk relative to the canvas
        public Position GetGlobalPosition()
        {
            return Width * Cell.Width * localPosition;
        }

        //Returns the index-position of this chunk relative to other chunks
        public Position GetLocalPosition()
        {
            return localPosition;
        }
    }
}
