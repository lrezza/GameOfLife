/***
 * Code for game of life
 * Author: Leonardo Rezza <lrezza@kth.se>
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GameOfLife
{
    public partial class MainForm : Form
    {
        private Dictionary<Position, Chunk> chunks;
        private bool gameRunning = false;
        private Timer timer;
        
        public MainForm()
        {
            DoubleBuffered = true;
            InitializeComponent();
            SetupVariables();
            GenerateChunks();
            SetCellNeighbours();
            
        }

        //Seperate method for setting up stuff
        private void SetupVariables()
        {
            Cell.Width = 20;
            Chunk.Width = 5;
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += OnTick;
        }

        private void SetCellNeighbours()
        {
            foreach(Chunk chunk in chunks.Values)
            {
                Cell[,] cells = chunk.GetCells();
                for (int x = 0; x < Chunk.Width; x++)
                {
                    for (int y = 0; y < Chunk.Width; y++)
                    {
                        cells[x, y].GetNeighbours();
                    }
                }
            }
        }

        //Generates chunks and populates the chunks with cells
        private void GenerateChunks()
        {
            chunks = new Dictionary<Position, Chunk>();

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Position localPosition = new Position(x, y);
                    Chunk chunk = new Chunk(localPosition);
                    chunk.Populate(); 
                    chunks.Add(localPosition, chunk);
                }
            }

            Chunk.Chunks = chunks;
        }

        //Draws the cells 
        private void DrawCells(Graphics g)
        {
            foreach(Chunk chunk in chunks.Values)
            {
                Cell[,] cells = chunk.GetCells();
                for (int x = 0; x < Chunk.Width; x++)
                {
                    for (int y = 0; y < Chunk.Width; y++)
                    {
                        if (cells[x, y].Alive)
                        {
                            Position pos = cells[x, y].GetGlobalPosition();
                            //Adding 1 and subtracting 2 to make sure that this rectangle doesn't overlap with the grid too much
                            g.FillRectangle(Brushes.LightGray, pos.X + 1, pos.Y + 1, Cell.Width - 2, Cell.Width - 2);
                        }
                    }
                }
                
            }
        }

        //Draws a grid
        private void DrawGrid(Graphics g)
        {
            Pen gridPen = new Pen(Brushes.White);
            gridPen.Width = 2;

            for (int i = 0; i < Chunk.Width * 6 * 5; i++)
            {
                int step = Cell.Width * i;
                int canvasLength = Chunk.Width * 5 * Cell.Width;

                g.DrawLine(gridPen, step, 0, step, canvasLength);
                g.DrawLine(gridPen, 0, step, canvasLength, step);
            }

        }

        //On paint event
        private void Canvas_paint(object sender, PaintEventArgs e)
        {
            DrawGrid(e.Graphics);
            DrawCells(e.Graphics);
        }

        //Event call for mouseclick on canvas, if a cell is clicked then invert its lifestate
        private void Canvas_click(object sender, MouseEventArgs e)
        {
            if(gameRunning)
            {
                return;
            }

            Position clickPos = new Position(e.Location.X, e.Location.Y);
            Cell clickedCell = Position.GetCellFromClickPosition(clickPos, chunks);
            if (clickedCell != null)
            {
                clickedCell.Alive = !clickedCell.Alive;
                clickedCell.Parent.needsDrawUpdate = true;
                canvas.Invalidate();
            }
        }

        //Runs the game
        private void RunBtn_Click(object sender, EventArgs e)
        {
            if(!gameRunning)
            {
                gameRunning = true;
                timer.Enabled = true;
                RunBtn.Text = "Stop";
            }
            else
            {
                gameRunning = false;
                timer.Enabled = false;
                RunBtn.Text = "Start";
            }
        }

        //Event for timer tick
        private void OnTick(object sender, EventArgs e)
        {
            ApplyCellRules();
            canvas.Refresh();
        }

        //Keeps track of rules for cell death and reproduction
        private void ApplyCellRules()
        {
            List<Cell> cellsToChange = new List<Cell>();

            foreach (Chunk chunk in chunks.Values)
            {
                for (int x = 0; x < Chunk.Width; x++)
                {
                    for (int y = 0; y < Chunk.Width; y++)
                    {
                        Cell cell = chunk.GetCells()[x, y];
                        int livingNeighbours = cell.GetAliveNeighbours();

                        if(cell.Alive)
                        {
                            if(livingNeighbours < 2 || livingNeighbours > 3) // Cells will keep living only if they have 2 or 3 living neighbours
                            {
                                cellsToChange.Add(cell);
                            }
                        }
                        else if(livingNeighbours == 3) //Dead cells become alive if they have 3 living neighbours
                        {
                            cellsToChange.Add(cell);
                        }
                    }
                }
            }

            //Inverts the cellstates in the list
            foreach(Cell cell in cellsToChange)
            {
                cell.Alive = !cell.Alive;
            }
            
        }
        
        //Clears all cells
        private void ClearBtn_Click(object sender, EventArgs e)
        {
            if(!gameRunning)
            {
                foreach (Chunk chunk in chunks.Values)
                {
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        for (int y = 0; y < Chunk.Width; y++)
                        {
                            Cell cell = chunk.GetCells()[x, y];
                            cell.Alive = false;
                        }
                    }
                }
            }

            canvas.Refresh();
        }
    }
}
