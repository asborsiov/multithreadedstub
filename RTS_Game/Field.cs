using System;
using System.Collections.Generic;

namespace RTS_Game
{
    public class Field
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Unit[,] Grid { get; private set; }
        private Random random;

        public List<Unit> RedUnits { get; private set; }
        public List<Unit> BlueUnits { get; private set; }

        public Field(int width, int height, int redCount, int blueCount)
        {
            Width = width;
            Height = height;
            Grid = new Unit[width, height];
            RedUnits = new List<Unit>();
            BlueUnits = new List<Unit>();
            random = new Random();

            InitializeUnits(redCount, blueCount);
        }

        private void InitializeUnits(int redCount, int blueCount)
        {
            for (int i = 0; i < redCount; i++)
            {
                PlaceUnitRandomly(Team.Red);
            }

            for (int i = 0; i < blueCount; i++)
            {
                PlaceUnitRandomly(Team.Blue);
            }
        }

        private void PlaceUnitRandomly(Team team)
        {
            int x, y;
            do
            {
                x = random.Next(Width);
                y = random.Next(Height);
            } while (Grid[x, y] != null);

            Unit unit = new Unit(x, y, team);
            Grid[x, y] = unit;

            if (team == Team.Red)
                RedUnits.Add(unit);
            else
                BlueUnits.Add(unit);
        }

        public bool MoveUnit(Unit unit, int newX, int newY)
        {
            if (newX >= 0 && newX < Width && newY >= 0 && newY < Height && Grid[newX, newY] == null)
            {
                Grid[unit.X, unit.Y] = null;
                unit.X = newX;
                unit.Y = newY;
                Grid[newX, newY] = unit;
                return true;
            }
            return false;
        }
    }
}
