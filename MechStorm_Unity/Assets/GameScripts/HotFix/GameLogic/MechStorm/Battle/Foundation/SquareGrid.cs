using System;
using System.Collections.Generic;

namespace MechStorm.Battle.Foundation
{
    public sealed class SquareGrid
    {
        public int Width { get; }
        public int Height { get; }
        
        public SquareGrid(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Grid width must be greater than zero.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Grid height must be greater than zero.");
            }

            Width = width;
            Height = height;
        }
        
        // 四方向邻居偏移：上、下、左、右。
        private static readonly Vector2Int[] _fourDirections =
        {
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 0),
        };

        public bool IsInside(Vector2Int position) => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;

        // 返回只读列表语义，调用方只消费邻居结果，不应修改集合。
        public IReadOnlyList<Vector2Int> GetFourDirectionNeighbors(Vector2Int position)
        {
            var neighbors = new List<Vector2Int>(_fourDirections.Length);
            foreach (var direction in _fourDirections)
            {
                var neighbor = position + direction;
                if (IsInside(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public int GetManhattanDistance(Vector2Int from, Vector2Int to)
        {
            return Math.Abs(to.X - from.X) + Math.Abs(to.Y - from.Y);
        }
    }
}