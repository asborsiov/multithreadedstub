using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RTS_Game
{
    public partial class MainWindow : Window
    {
        private Field field;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            field = new Field(500, 500, 4000, 6000);
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => SimulateTeam(field.RedUnits, true));
            Task.Run(() => SimulateTeam(field.BlueUnits, false));
            DrawField();
        }

        private void DrawField()
        {
            GameCanvas.Children.Clear();

            foreach (var unit in field.RedUnits)
            {
                DrawUnit(unit, Colors.Red);
            }

            foreach (var unit in field.BlueUnits)
            {
                DrawUnit(unit, Colors.Blue);
            }
        }

        private void DrawUnit(Unit unit, Color color)
        {
            Rectangle rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(color)
            };
            Canvas.SetLeft(rect, unit.X);
            Canvas.SetTop(rect, unit.Y);
            GameCanvas.Children.Add(rect);
        }

        private void SimulateTeam(List<Unit> units, bool isChasing)
        {
            Parallel.ForEach(units, unit =>
            {
                var (targetX, targetY) = isChasing ? FindNearestEnemy(unit) : FindFurthestPoint(unit);
                MoveUnitTowards(unit, targetX, targetY);
            });
        }

        private (int, int) FindNearestEnemy(Unit unit)
        {
            var enemies = unit.Team == Team.Red ? field.BlueUnits : field.RedUnits;
            Unit nearestEnemy = null;
            double nearestDistance = double.MaxValue;

            foreach (var enemy in enemies)
            {
                double distance = Math.Sqrt(Math.Pow(unit.X - enemy.X, 2) + Math.Pow(unit.Y - enemy.Y, 2));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            return (nearestEnemy.X, nearestEnemy.Y);
        }

        private (int, int) FindFurthestPoint(Unit unit)
        {
            int furthestX = unit.X, furthestY = unit.Y;
            double furthestDistance = 0;

            foreach (var direction in GetAdjacentPositions(unit.X, unit.Y))
            {
                double distance = Math.Sqrt(Math.Pow(unit.X - direction.Item1, 2) + Math.Pow(unit.Y - direction.Item2, 2));
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    furthestX = direction.Item1;
                    furthestY = direction.Item2;
                }
            }

            return (furthestX, furthestY);
        }

        private void MoveUnitTowards(Unit unit, int targetX, int targetY)
        {
            var directions = GetAdjacentPositions(unit.X, unit.Y)
                .OrderBy(pos => Math.Sqrt(Math.Pow(pos.Item1 - targetX, 2) + Math.Pow(pos.Item2 - targetY, 2)))
                .ToList();

            foreach (var direction in directions)
            {
                if (field.MoveUnit(unit, direction.Item1, direction.Item2))
                {
                    break;
                }
            }
        }

        private List<(int, int)> GetAdjacentPositions(int x, int y)
        {
            var positions = new List<(int, int)>
            {
                (x - 1, y - 1), (x, y - 1), (x + 1, y - 1),
                (x - 1, y),                 (x + 1, y),
                (x - 1, y + 1), (x, y + 1), (x + 1, y + 1)
            };

            return positions.Where(pos => pos.Item1 >= 0 && pos.Item1 < field.Width && pos.Item2 >= 0 && pos.Item2 < field.Height).ToList();
        }
    }
}
