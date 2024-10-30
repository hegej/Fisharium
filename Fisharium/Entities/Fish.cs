
using System;

namespace Fisharium.Entities
{
    public class Fish : Entity
    {
        public int DX { get; private set; }
        public int DY { get; private set; }
        public int MoveSpeed { get; private set; }
        public ConsoleColor[] ColorPattern { get; private set; }

        public Fish(int x, int y, string appearance, int dx, int dy, int moveSpeed, ConsoleColor[] colorPattern)
            : base( x, y, appearance, colorPattern[0])
        {
            DX = dx;
            DY = dy;
            MoveSpeed = moveSpeed;
            ColorPattern = colorPattern;
        }

        public override void Update()

    }
}
