using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spectre.Console;

class Program
{
    public static int Width { get; private set; } = 80;
    public static int Height { get; private set; } = 24;
    static List<Fish> fishes = new List<Fish>();
    static List<Plant> plants = new List<Plant>();
    static List<Bubble> bubbles = new List<Bubble>();
    static Random random = new Random();
    static int fishMoveCounter = 0;

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        InitializeAquarium();

        while (true)
        {
            UpdateAquarium();
            DrawAquarium();
            Thread.Sleep(80); // Juster dette for å kontrollere hastigheten på animasjonen
        }
    }

    static void InitializeAquarium()
    {
        fishes.Add(new Fish(10, 5, 1, 0, "><>", 2));
        fishes.Add(new Fish(20, 10, -1, 0, "<><", 2));
        fishes.Add(new Fish(40, 15, 1, 0, "><(((°>", 3));

        plants.Add(new Plant(5, Height - 1, "|^|"));
        plants.Add(new Plant(15, Height - 1, "|*|"));
        plants.Add(new Plant(30, Height - 1, "|&|"));
    }

    static void UpdateAquarium()
    {
        fishMoveCounter++;

        foreach (var fish in fishes)
        {
            if (fishMoveCounter % fish.MoveSpeed == 0)
            {
                fish.Move();

                if (random.Next(100) < 10)
                {
                    int bubbleX = fish.DX > 0 ? fish.X + fish.Appearance.Length - 1 : fish.X;
                    if (bubbleX >= 0 && bubbleX < Width)
                    {
                        bubbles.Add(new Bubble(bubbleX, fish.Y));
                    }
                }
            }
        }

        foreach (var plant in plants)
        {
            plant.Sway();
        }

        for (int i = bubbles.Count - 1; i >= 0; i--)
        {
            bubbles[i].Rise();
            if (bubbles[i].Y <= 0)
            {
                bubbles.RemoveAt(i);
            }
        }
    }

    static void DrawAquarium()
    {
        Console.SetCursorPosition(0, 0);

        var canvas = new Canvas(Width, Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                canvas.SetPixel(x, y, Color.Blue);
            }
        }

        for (int x = 0; x < Width; x++)
        {
            canvas.SetPixel(x, 0, Color.White);
            canvas.SetPixel(x, Height - 1, Color.White);
        }
        for (int y = 0; y < Height; y++)
        {
            canvas.SetPixel(0, y, Color.White);
            canvas.SetPixel(Width - 1, y, Color.White);
        }

        foreach (var fish in fishes)
        {
            DrawEntity(fish, canvas, Color.Orange1);
        }

        foreach (var plant in plants)
        {
            DrawEntity(plant, canvas, Color.Green);
        }

        foreach (var bubble in bubbles)
        {
            if (bubble.Y > 0)
            {
                DrawEntity(bubble, canvas, Color.White);
            }
        }

        AnsiConsole.Write(canvas);
    }

    static void DrawEntity(Entity entity, Canvas canvas, Color color)
    {
        int startX = Math.Max(0, entity.X);
        int endX = Math.Min(Width - 1, entity.X + entity.Appearance.Length);
        for (int i = startX, j = 0; i < endX; i++, j++)
        {
            if (entity.Y >= 0 && entity.Y < Height)
            {
                canvas.SetPixel(i, entity.Y, color);
            }
        }
    }

    class Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Appearance { get; set; }

        public Entity(int x, int y, string appearance)
        {
            X = x;
            Y = y;
            Appearance = appearance;
        }
    }

    class Fish : Entity
    {
        public int DX { get; set; }
        public int DY { get; set; }
        public int MoveSpeed { get; set; }

        public Fish(int x, int y, int dx, int dy, string appearance, int moveSpeed) : base(x, y, appearance)
        {
            DX = dx;
            DY = dy;
            MoveSpeed = moveSpeed;
        }

        public void Move()
        {
            X += DX;
            Y += DY;

            if (X <= 0 || X >= Program.Width - Appearance.Length)
            {
                DX = -DX;
                Appearance = new string(Appearance.Reverse().ToArray());
            }

            if (Y <= 1 || Y >= Program.Height - 2)
            {
                DY = -DY;
            }
        }
    }

    class Plant : Entity
    {
        private int swayDirection = 1;
        private int swayCounter = 0;

        public Plant(int x, int y, string appearance) : base(x, y, appearance) { }

        public void Sway()
        {
            swayCounter++;
            if (swayCounter % 10 == 0)
            {
                X += swayDirection;
                swayDirection = -swayDirection;
            }
        }
    }

    class Bubble : Entity
    {
        public Bubble(int x, int y) : base(x, y, "o") { }

        public void Rise()
        {
            Y--;
        }
    }
}
