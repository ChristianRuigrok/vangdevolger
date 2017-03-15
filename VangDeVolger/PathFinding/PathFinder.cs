﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VangDeVolger.Elements;
using VangDeVolger.Elements.Blocks;

namespace VangDeVolger.PathFinding
{
    internal class PathFinder
    {
        private readonly PathFinderBlock[,] _grid;
        private readonly List<PathFinderBlock> _openSet = new List<PathFinderBlock>();
        private readonly List<PathFinderBlock> _closedSet = new List<PathFinderBlock>();

        private readonly PathFinderBlock _to;

        /// <summary>
        /// Initialize PathFinder Class
        /// </summary>
        /// <param name="grid">Current Grid of Elements</param>
        /// <param name="from">Position of Enemy Bird</param>
        /// <param name="to">Position of Player Bird</param>
        public PathFinder(Block[,] grid, Element from, Element to)
        {
            
            _grid = _addSiblings(_transformGrid(grid));
            foreach (PathFinderBlock item in _grid)
            {
                if (to.X == item.Block.X && to.Y == item.Block.Y)
                {
                    _to = item;
                }
                else if (from.X == item.Block.X && from.Y == item.Block.Y)
                {
                    _openSet.Add(item);
                }
            }
        }

        /// <summary>
        /// Path Finding using the A* algorithm
        /// </summary>
        /// <returns>List with Optimal Path</returns>
        public List<PathFinderBlock> GetOptimalPath()
        {
            List<PathFinderBlock> path = new List<PathFinderBlock>();

            while (_openSet.Count > 0)
            {
                int winner = 0;
                for (int i = 0; i < _openSet.Count; i++)
                {
                    if (_openSet[i].F < _openSet[winner].F)
                    {
                        winner = i;
                    }
                }

                PathFinderBlock current = _openSet[winner];

                if (current == _to)
                {
                    // DONE
                    PathFinderBlock next = current;
                    path.Add(next);
                    
                    while (next.CameFrom != null)
                    {
                        path.Add(next.CameFrom);
                        next = next.CameFrom;
                    }

                    return path;
                }

                _openSet.Remove(current);
                _closedSet.Add(current);

                foreach (PathFinderBlock sibling in current.SiblingBlocks)
                {
                    if (_closedSet.Contains(sibling) || sibling == null) continue;

                    int tempG = current.G + 1;

                    if (_openSet.Contains(sibling))
                    {
                        if (tempG < sibling.G)
                        {
                            sibling.G = tempG;
                        }
                    }
                    else
                    {
                        sibling.G = tempG;
                        _openSet.Add(sibling);
                    }

                    sibling.H = Heuristic(sibling);
                    sibling.F = sibling.G + sibling.H;
                    sibling.CameFrom = current;
                }
            }
            // No solution
            MessageBox.Show("No solution");
            return path;
        }

        /// <summary>
        /// Transform Blocks to PathFinderBlocks
        /// </summary>
        /// <param name="grid">Block Grid</param>
        /// <returns></returns>
        private static PathFinderBlock[,] _transformGrid(Block[,] grid)
        {
            PathFinderBlock[,] newGrid = new PathFinderBlock[grid.GetLength(0), grid.GetLength(1)];
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++) {
                    newGrid[x, y] = _transformBlock(grid[x, y]);
                }
                
            }
            return newGrid;
        }

        /// <summary>
        /// Transform Block to PathFinderBlock
        /// </summary>
        /// <param name="block">Block to be converted</param>
        /// <returns>PathFinderBlock</returns>
        private static PathFinderBlock _transformBlock(Block block) => new PathFinderBlock(block);

        private PathFinderBlock[,] _addSiblings(PathFinderBlock[,] grid)
        {
            PathFinderBlock[,] newGrid = new PathFinderBlock[grid.GetLength(0), grid.GetLength(1)];
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    PathFinderBlock block = grid[y, x];
                    block = _addSibling(grid, block);
                    newGrid[y, x] = block;
                }
            }
            return newGrid;
        }

        /// <summary>
        /// Adds Sibling to PathFinderBlock
        /// </summary>
        /// <param name="grid">PathFinderBlock grid</param>
        /// <param name="block">Block to add Siblings to</param>
        /// <returns></returns>
        private static PathFinderBlock _addSibling(PathFinderBlock[,] grid, PathFinderBlock block)
        {
            int x = block.Block.X;
            int y = block.Block.Y;

            List<PathFinderBlock> siblings = new List<PathFinderBlock>();

            if (x < grid.GetLength(0) - 1)
            {
                siblings.Add(grid[x + 1, y]);
            }
            if (x > 0)
            {
                siblings.Add(grid[x - 1, y]);
            }
            if (y < grid.GetLength(0) - 1)
            {
                siblings.Add(grid[x, y + 1]);
            }
            if (y > 0)
            {
                siblings.Add(grid[x, y - 1]);
            }

            block.SiblingBlocks = siblings;

            return block;
        }

        /// <summary>
        /// Get Estimated Distance (Heuristic)
        /// </summary>
        /// <param name="from">Block from where to calculate the Heuristic</param>
        /// <returns>Average block distance</returns>
        private int Heuristic(PathFinderBlock from)
        {
            // Manhattan distance
            int x = Math.Abs(from.Block.X - _to.Block.X);
            int y = Math.Abs(from.Block.Y + _to.Block.Y);
            return x + y;
        }
    }
}
