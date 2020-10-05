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
    public class Cell
    {
        public static int Width;

        public bool Alive { get; set; } = false;
        public Chunk Parent { get; }

        private Position localPosition;
 
        private Cell[] neighbours;

        //Enum for handling directions in neighbour lookup
        private enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft };
        
        public Cell(Position localPosition, bool alive, Chunk chunkParent)
        {
            Alive = alive;
            this.localPosition = localPosition;
            Parent = chunkParent;
        }

        //Returns the global pixel-position of this cell relative to the canvas
        public Position GetGlobalPosition()
        {
            return Parent.GetGlobalPosition() + Width * localPosition;
        }

        //Returns local index-position of this cell relative to parent chunk
        public Position GetLocalPosition()
        {
            return localPosition;
        }

        //Gets neighbours in all 8 directions
        public void GetNeighbours()
        {
            neighbours = new Cell[8];
            for (int i = 0; i < 8; i++)
            {
                neighbours[i] = GetNeighbourInDirection((Direction)i); 
            }
        }

        //Very ugly way to handle this but it's easy to implement
        private Cell GetNeighbourInDirection(Direction dir)
        {
            Position neighbourPos = null;

            //Don't look at this switch please
            switch(dir)
            {
                case Direction.Up:
                    neighbourPos = new Position(localPosition.X, localPosition.Y - 1);
                    break;
                case Direction.UpRight:
                    neighbourPos = new Position(localPosition.X + 1, localPosition.Y - 1);
                    break;
                case Direction.Right:
                    neighbourPos = new Position(localPosition.X + 1, localPosition.Y);
                    break;
                case Direction.DownRight:
                    neighbourPos = new Position(localPosition.X + 1, localPosition.Y + 1);
                    break;
                case Direction.Down:
                    neighbourPos = new Position(localPosition.X, localPosition.Y + 1);
                    break;
                case Direction.DownLeft:
                    neighbourPos = new Position(localPosition.X - 1, localPosition.Y + 1);
                    break;
                case Direction.Left:
                    neighbourPos = new Position(localPosition.X - 1, localPosition.Y);
                    break;
                case Direction.UpLeft:
                    neighbourPos = new Position(localPosition.X - 1, localPosition.Y - 1);
                    break;
                default:
                    neighbourPos = null;
                    break;
            }

            //Checks if neighbourPos is in the current chunk, else checks in the neighbour chunk
            if(InChunkRange(neighbourPos.X) && InChunkRange(neighbourPos.Y))
            {
                return Parent.GetCells()[neighbourPos.X, neighbourPos.Y];
            }
            else
            {
                
                //xDir and yDir builds the neighbourChunkDirection which decides which direction the neighbour chunk is in
                int xDir = 0, yDir = 0;
                if(!InChunkRange(neighbourPos.X))
                {
                    xDir = neighbourPos.X < 0 ? -1 : 1;
                }
                if(!InChunkRange(neighbourPos.Y))
                {
                    yDir = neighbourPos.Y < 0 ? -1 : 1;
                }

                Position neighbourChunkDirection = new Position(xDir, yDir);
               
                Position neighbourChunkPos = Parent.GetLocalPosition() + neighbourChunkDirection;
                Chunk neighbourChunk = null;
                Chunk.Chunks.TryGetValue(neighbourChunkPos, out neighbourChunk);
                
                if(neighbourChunk == null)
                {
                    return null;
                }
                else
                {
                    
                    int xPos = (neighbourPos.X + Chunk.Width) % Chunk.Width; //These operations "convert" the index-positions into neighbour local index-position relative to its parent
                    int yPos = (neighbourPos.Y + Chunk.Width) % Chunk.Width; //Example if Chunk.Width is 5 then index-range is 0-4 where a 5 would be converted to 0 and -1 would be converted to 4

                    return neighbourChunk.GetCells()[xPos, yPos];
                }
            }
    
        }

        //Checks if value is in range chunk index range
        private bool InChunkRange(int value)
        {
            return value >= 0 && value < Chunk.Width;
        }

        //Returns amount of alive neighbours
        public int GetAliveNeighbours()
        {
            int amount = 0;
            for (int i = 0; i < 8; i++)
            {
                if(neighbours[i] != null)
                {
                    amount += neighbours[i].Alive ? 1 : 0;
                }
            }

            return amount;
        }
    }
}
