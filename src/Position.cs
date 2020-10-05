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
    public class Position : IEquatable<Position>
    {
        public int X { get; }
        public int Y { get; }

        public static Position operator * (int a, Position pos) => new Position(a * pos.X, a * pos.Y);
        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);
        public static bool operator == (Position a, Position b)
        {
            if(ReferenceEquals(a, null))
            {
                if(ReferenceEquals(b, null))
                {
                    return true;
                }

                return false;
            }

            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Position a, Position b) => !(a == b);

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public bool Equals(Position p)
        {
            if(ReferenceEquals(p, null))
            {
                return false;
            }

            if (ReferenceEquals(p, this))
            {
                return true;
            }

            return X == p.X && Y == p.Y;
        }
        //Rough method for converting click position to a cell
        public static Cell GetCellFromClickPosition(Position clickPos, Dictionary<Position, Chunk> chunks)
        {
            Chunk clickedChunk = null;

            foreach(Chunk chunk in chunks.Values)
            {
                Position chunkPos = chunk.GetGlobalPosition();
                if(PositionInSquare(clickPos, chunkPos, Chunk.Width * Cell.Width))
                {
                    clickedChunk = chunk;
                    break;
                }

            }

            if(clickedChunk == null)
            {
                return null;
            }

            Cell clickedCell = null;

            foreach(Cell cell in clickedChunk.GetCells())
            {
                Position cellPos = cell.GetGlobalPosition();
                if(PositionInSquare(clickPos, cellPos, Cell.Width))
                {
                    clickedCell = cell;
                    break;
                }
            }

            return clickedCell;
            
        }

        //Rough method for checking if a position is within a square
        public static bool PositionInSquare(Position checkPos, Position squarePos, int squareWidth)
        {
            return checkPos.X >= squarePos.X && checkPos.Y >= squarePos.Y &&
                checkPos.X < (squarePos.X + squareWidth) && checkPos.Y < (squarePos.Y + squareWidth);
        }
    }
}
