
using System;

namespace Fisharium.Entities
{
    public class Fish : Entity
    {
        public int DX { get; private set; }
        public int DY { get; private set; }
        public int MoveSpeed { get; private set; }
        public ConsoleColor[] ColorPattern { get; private set; }

        private int moveCounter = 0;
        private int moveUpdateRate;

        public Fish(int x, int y, string appearance, int dx, int dy, int moveSpeed, ConsoleColor[] colorPattern)
            : base( x, y, appearance, colorPattern[0])
        {
            DX = dx;
            DY = dy;
            MoveSpeed = moveSpeed;
            moveUpdateRate = moveSpeed;
            ColorPattern = colorPattern;
        }

        public override void Update()
        {
            moveCounter++;
            if (moveCounter % moveUpdateRate == 0)
            {
                RandomizeMovement();
            }

            int nextX = X + DX;
            int nextY = Y + DY;
            CheckBounds(ref nextX, ref nextY);
            X = nextX;
            Y = nextY;

            // Boble front of the fish
            if (Program.random.Next(100) < 30)
            {
                int bubbleX = DX > 0 ? X + Appearance.Length - 1 : X;
                if (bubbleX >= 0 && bubbleX < Program.Width)
                {
                    Program.bubbles.Add(new Bubble(bubbleX, Y, ConsoleColor.White));
                }
            }
        }

        private void RandomizeMovement()
        {
            // for fish to not swim backwards
            DX = (DX > 0) ? 1 : -1;

            if (Program.random.Next(2) == 0)
            {
                DY = Program.random.Next(-1, 2);
            }
        }

        private void CheckBounds(ref int nextX, ref int nextY)
        {
            // make fish turn when hit reach the border of the aquarium
            if (nextX <= 0 || nextX >= Program.Width - Appearance.Length)
            {
                DX = -DX;
                ReverseAppearance();
                nextX = X + DX;
            }

            if (nextY <= 1 || nextY >= Program.Height - 1)
            {
                DY = -DY;
                nextY = Y + DY;
            }
        }

        private void ReverseAppearance()
        {
            char[] reversed = new char[Appearance.Length];
            for (int i = 0; i < Appearance.Length; i++)
            {
                char ch = Appearance[Appearance.Length - 1 - i];
                reversed[i] = ch == '>' ? '<' : (ch == '<' ? '>' : ch);
            }
            Appearance = new string(reversed);
        }

        public override void DrawEntity((char Char, ConsoleColor Color)[,] buffer)
        {
            for (int i = 0; i < Appearance.Length; i++)
            {
                if (Y < buffer.GetLength(0) && X + i < buffer.GetLength(1))
                {
                    buffer[Y, X + i] = (Appearance[i], ColorPattern[i % ColorPattern.Length]);
                }
            }
        }
    }
}
