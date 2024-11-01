using Fisharium.Entities;
using Fisharium.Factories;
using System.Text;

class Program
{
    public static int Width { get; private set; } = 160;
    public static int Height { get; private set; } = 50;
    public static List<Fish> fishes = new List<Fish>();
    public static List<Plant> plants = new List<Plant>();
    public static List<Bubble> bubbles = new List<Bubble>();
    public static Random random = new Random();
    static int fishMoveCounter = 0;
    static (char Char, ConsoleColor Color)[,] buffer;

    static void Main(string[] args)
    {
        Console.SetWindowSize(200, 100);
        Console.SetBufferSize(200, 100);
        Console.CursorVisible = false;

        InitializeAquarium();
        InitializeBuffer();

        while (!Console.KeyAvailable)
        {
            UpdateAquarium();
            DrawAquarium();
            RenderBuffer();
            Thread.Sleep(5);
        }
    }

    static void InitializeAquarium()
    {
        fishes.Add(FishFactory.CreateFishType1(10, 5));
        fishes.Add(FishFactory.CreateFishType2(40, 10));
        fishes.Add(FishFactory.CreateFishType3(60, 15));
        fishes.Add(FishFactory.CreateFishType4(30, 20));

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
            if (bubbles[i].IsOutOfBounds)
            {
                bubbles.RemoveAt(i);
            }
        }
    }

    static void DrawAquarium()
    {
        FillWater();

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

        foreach (var entity in fishes.Concat<Entity>(plants).Concat(bubbles))
        {
            entity.DrawEntity(buffer);
        }
    }

    static void RenderBuffer()
    {
        Console.SetCursorPosition(0, 0);
        ConsoleColor currentColor = ConsoleColor.White;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var cell = buffer[y, x];
                if (currentColor != cell.Color)
                {
                    Console.ForegroundColor = cell.Color;
                    currentColor = cell.Color;
                }
                Console.Write(cell.Char);
            }
            Console.WriteLine();
        }
    }
}
