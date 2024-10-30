
namespace Fisharium.Entities
{
    public abstract class Entity
    {
        protected int X { get; set; }
        protected int Y { get; set; }
        public string Appearance { get; set; }
        public ConsoleColor Color { get; set; }


        public Entity(int x, int y, string appearance, ConsoleColor color)
        {
            X = x;
            Y = y;
            Appearance = appearance;
            Color = color;
        }

        public virtual void Update() { }

        public virtual void DrawEntity((char Char, ConsoleColor Color)[,] buffer)
        {
            for (int i = 0; i < Appearance.Length; i++)
            {
                if (Y < buffer.GetLength(0) && X + i < buffer.GetLength(1))
                {
                    buffer[Y, X + i] = (Appearance[i], Color);
                }
            }
        }
    }
}
