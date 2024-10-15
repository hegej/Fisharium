using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    public static int Width { get; private set; } = 80;
    public static int Height { get; private set; } = 24;
    public static List<Fish> fishes = new List<Fish>();
    public static List<Plant> plants = new List<Plant>();
    public static List<Bubble> bubbles = new List<Bubble>();
    public static Random random = new Random();
    static int fishMoveCounter = 0;
    static (char Char, ConsoleColor Color)[,] buffer;

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        InitializeAquarium();
        InitializeBuffer();

        while (!Console.KeyAvailable)
        {
            UpdateAquarium();
            DrawAquarium();
            RenderBuffer();
            Thread.Sleep(80);
        }
    }

    static void InitializeAquarium()
    {
        fishes.Add(new Fish(10, 5, 1, 0, "><>", 2, ConsoleColor.Yellow));
        fishes.Add(new Fish(20, 10, -1, 0, "<><", 2, ConsoleColor.Cyan));
        fishes.Add(new Fish(40, 15, 1, 0, "><(((*>", 3, ConsoleColor.Magenta));

        plants.Add(new Plant(5, Height - 2, "|^|", ConsoleColor.Green));
        plants.Add(new Plant(15, Height - 2, "|*|", ConsoleColor.Green));
        plants.Add(new Plant(30, Height - 2, "|&|", ConsoleColor.Green));
    }

    static void InitializeBuffer()
    {
        buffer = new (char, ConsoleColor)[Height, Width];
        FillWater();
    }

    static void FillWater()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y == 1 || y == 2)
                {
                    buffer[y, x] = ('~', ConsoleColor.Cyan);
                }
                else if (y < Height / 2)
                {
                    buffer[y, x] = ('≈', ConsoleColor.Blue);
                }
                else
                {
                    buffer[y, x] = ('≈', ConsoleColor.DarkBlue);
                }
            }
        }
    }

    static void UpdateAquarium()
    {
        fishMoveCounter++;

        foreach (var fish in fishes)
        {
            if (fishMoveCounter % fish.MoveSpeed == 0)
            {
                fish.Update();
            }
        }

        foreach (var plant in plants)
        {
            plant.Update();
        }

        for (int i = bubbles.Count - 1; i >= 0; i--)
        {
            bubbles[i].Update();
            if (bubbles[i].Y <= 2)
            {
                bubbles.RemoveAt(i);
            }
        }
    }

    static void DrawAquarium()
    {
        FillWater();

        // Draw frame
        for (int x = 0; x < Width; x++)
        {
            buffer[0, x] = ('-', ConsoleColor.White);
            buffer[Height - 1, x] = ('-', ConsoleColor.White);
        }
        for (int y = 1; y < Height - 1; y++)
        {
            buffer[y, 0] = ('|', ConsoleColor.White);
            buffer[y, Width - 1] = ('|', ConsoleColor.White);
        }

        // Draw entities
        foreach (var fish in fishes)
        {
            DrawEntity(fish);
        }

        foreach (var plant in plants)
        {
            DrawEntity(plant);
        }

        foreach (var bubble in bubbles)
        {
            DrawEntity(bubble);
        }
    }

    static void DrawEntity(Entity entity)
    {
        if (entity.Y >= 0 && entity.Y < Height && entity.X >= 0 && entity.X < Width - entity.Appearance.Length)
        {
            for (int i = 0; i < entity.Appearance.Length; i++)
            {
                buffer[entity.Y, entity.X + i] = (entity.Appearance[i], entity.Color);
            }
        }
    }

    static void RenderBuffer()
    {
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.ForegroundColor = buffer[y, x].Color;
                Console.Write(buffer[y, x].Char);
            }
            Console.WriteLine();
        }
    }
}


class Entity
{
    public int X { get; set; }
    public int Y { get; set; }
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
}

class Fish : Entity
{
    public int DX { get; set; }
    public int DY { get; set; }
    public int MoveSpeed { get; set; }

    public Fish(int x, int y, int dx, int dy, string appearance, int moveSpeed, ConsoleColor color)
        : base(x, y, appearance, color)
    {
        DX = dx;
        DY = dy;
        MoveSpeed = moveSpeed;
    }

    public override void Update()
    {
        X += DX;
        Y += DY;

        // Snu fisken når den treffer kanten av akvariet
        if (X <= 0 || X >= Program.Width - Appearance.Length)
        {
            DX = -DX;
            ReverseAppearance();
        }

        if (Y <= 1 || Y >= Program.Height - 2)
        {
            DY = -DY;
        }

        // Generer bobler
        if (Program.random.Next(100) < 10)
        {
            int bubbleX = DX > 0 ? X + Appearance.Length : X - 1;
            if (bubbleX >= 0 && bubbleX < Program.Width)
            {
                Program.bubbles.Add(new Bubble(bubbleX, Y - 1, ConsoleColor.White));
            }
        }
    }

    private void ReverseAppearance()
    {
        char[] reversed = new char[Appearance.Length];
        for (int i = 0; i < Appearance.Length; i++)
        {
            char ch = Appearance[Appearance.Length - 1 - i];
            switch (ch)
            {
                case '>':
                    reversed[i] = '<';
                    break;
                case '<':
                    reversed[i] = '>';
                    break;
                default:
                    reversed[i] = ch;
                    break;
            }
        }
        Appearance = new string(reversed);
    }
}


class Plant : Entity
{
    private int swayDirection = 1;
    private int swayCounter = 0;

    public Plant(int x, int y, string appearance, ConsoleColor color)
        : base(x, y, appearance, color) { }

    public override void Update()
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
    public Bubble(int x, int y, ConsoleColor color)
        : base(x, y, "o", color) { }

    public override void Update()
    {
        Y--;
    }
}