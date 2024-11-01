
namespace Fisharium.Entities
{
    public class Bubble : Entity
    {
        public Bubble(int x, int y, ConsoleColor color)
            : base (x, y, "o", color)
        {

        }

        public bool IsOutOfBounds => Y < 1;

        public override void Update()
        {
            Y--;
        }

        public override void DrawEntity((char Char, ConsoleColor Color)[,] buffer)
        {
            if (Y >= 1 && Y < buffer.GetLength(0) && X >= 0 && X < buffer.GetLength(1))
            {
                buffer[Y, X] = (Appearance[0], Color);
            }
        }
    }
}