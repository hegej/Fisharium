using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

class Program
{
    public static int Width { get; private set; } = 80;
    public static int Height { get; private set; } = 24;
    static char[,] screen = new char[Height, Width];
    static List<Fish> fishes = new List<Fish>();
    static List<Plant> plants = new List<Plant>();

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        InitializeAquarium();

        while (true)
        {
            UpdateAquarium();
            DrawAquarium();
            Thread.Sleep(100); // Oppdater hvert 100ms
        }
    }

    static void InitializeAquarium()
    {
        // Legg til fisker
        fishes.Add(new Fish(10, 5, 1, 0, "><>"));
        fishes.Add(new Fish(20, 10, -1, 1, "<><"));

        // Legg til planter
        plants.Add(new Plant(5, Height - 1, "|^|"));
        plants.Add(new Plant(15, Height - 1, "|*|"));
    }

    static void UpdateAquarium()
    {
        foreach (var fish in fishes)
        {
            fish.Move();
        }

        foreach (var plant in plants)
        {
            plant.Sway();
        }
    }

    static void DrawAquarium()
    {
        // Tøm skjermen
        Array.Clear(screen, 0, screen.Length);

        // Tegn ramme
        for (int i = 0; i < Width; i++)
        {
            screen[0, i] = '-';
            screen[Height - 1, i] = '-';
        }
        for (int i = 0; i < Height; i++)
        {
            screen[i, 0] = '|';
            screen[i, Width - 1] = '|';
        }

        // Tegn fisker
        foreach (var fish in fishes)
        {
            DrawEntity(fish);
        }

        // Tegn planter
        foreach (var plant in plants)
        {
            DrawEntity(plant);
        }

        // Vis akvariet
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Console.Write(screen[y, x] == '\0' ? ' ' : screen[y, x]);
            }
            Console.WriteLine();
        }
    }

    static void DrawEntity(Entity entity)
    {
        int startX = Math.Max(0, entity.X);
        int endX = Math.Min(Width - 1, entity.X + entity.Appearance.Length);
        for (int i = startX, j = 0; i < endX; i++, j++)
        {
            if (entity.Y >= 0 && entity.Y < Height)
            {
                screen[entity.Y, i] = entity.Appearance[j];
            }
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

    public Fish(int x, int y, int dx, int dy, string appearance) : base(x, y, appearance)
    {
        DX = dx;
        DY = dy;
    }

    public void Move()
    {
        X += DX;
        Y += DY;

        // Snu når fisken treffer kanten
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
        if (swayCounter % 5 == 0)
        {
            X += swayDirection;
            swayDirection = -swayDirection;
        }
    }
}