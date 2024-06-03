namespace RTS_Game
{
    public enum Team
    {
        Red,
        Blue
    }

    public class Unit
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Team Team { get; private set; }

        public Unit(int x, int y, Team team)
        {
            X = x;
            Y = y;
            Team = team;
        }
    }
}
