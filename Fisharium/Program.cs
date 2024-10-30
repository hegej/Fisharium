using System;
using System.Collections.Generic;
using System.Threading;

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
        //int windowWidth = Width + 200;
        //int windowHeight = Height + 100; 

        Console.SetWindowSize(200, 100);
        Console.SetBufferSize(200, 100);

        //Console.SetWindowSize(windowWidth, windowHeight);
        //Console.SetBufferSize(windowWidth, windowHeight);
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
        fishes.Add(new Fish(10, 5, 1, 0, @"
         ,       
      .:/    
   ,,///;,   ,;/ 
 o)::::::;;///
>::::::::;;\\\ 
  ''\\\\\'"" ';\ 
     ';", 2, ConsoleColor.Gray));

        fishes.Add(new Fish(20, 10, -1, 0, "<><", 2, ConsoleColor.Cyan));
        fishes.Add(new Fish(40, 15, 1, 0, "><(((*>", 3, ConsoleColor.Magenta));
        fishes.Add(new Fish(50, 7, -1, 0, "<°((><", 3, ConsoleColor.Green));
        fishes.Add(new Fish(60, 12, 1, 0, "><>", 4, ConsoleColor.Red));
        fishes.Add(new Fish(30, 20, 1, 0, "<><><>", 2, ConsoleColor.Blue));

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
        int startY = entity.Y;
        string[] lines = entity.Appearance.Split('\n');

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                char currentChar = line[x];
                if (currentChar != ' ') 
                {
                    ConsoleColor color = entity.GetColorForChar(currentChar);
                    if (entity.Y + y < Height && entity.X + x < Width)
                    {
                        buffer[entity.Y + y, entity.X + x] = (currentChar, color);
                    }
                }
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


public class Entity
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

public class Fish : Entity
{
    public int DX { get; set; }
    public int DY { get; set; }
    public int MoveSpeed { get; set; }

    private int moveCounter = 0;
    private int moveUpdateRate;

    public Fish(int x, int y, int dx, int dy, string appearance, int moveSpeed, ConsoleColor color)
        : base(x, y, appearance, color)
    {
        DX = dx;
        DY = dy;
        MoveSpeed = moveSpeed;
        moveUpdateRate = moveSpeed;
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

        if (Program.random.Next(100) < 30)
        {
            int bubbleX = DX > 0 ? X + Appearance.Length - 1 : X;
            if (bubbleX >= 0 && bubbleX < Program.Width)
            {
                Program.bubbles.Add(new Bubble(bubbleX, Y - 1, ConsoleColor.White));
            }
        }
    }

    private void RandomizeMovement()
    {
        // Fisk skal aldri svømme baklengs, så DX må alltid være 1 eller -1
        if (Program.random.Next(2) == 0)
        {
            DX = (DX > 0) ? 1 : -1; 
        }

        // Fisk kan bevege seg vertikalt opp, ned eller ingen vertikal bevegelse
        if (Program.random.Next(2) == 0)
        {
            DY = Program.random.Next(-1, 2); 
        }
    }

    private void CheckBounds(ref int nextX, ref int nextY)
    {
        // Snu fisken før den treffer kanten på akvariet
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
