
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
        private double actualX; // For smooth movement
        private double actualY;
        private double verticalOffset = 0; // For bølgebevegelse
        private const double MOVEMENT_SMOOTHING = 0.2;  // Redusert fra 0.8 for raskere akselerasjon
        private const double SPEED_MULTIPLIER = 4.0;    // Ny konstant for å øke basishastigheten
        private const double WAVE_SPEED = 0.05;         // Justert for bedre bølgebevegelse
        private const double WAVE_AMPLITUDE = 0.3;
        private double velocityX = 0;
        private double velocityY = 0;
        private Random random = new Random();
        private bool IsMultiLine => Appearance?.Contains("\n") ?? false;

        public Fish(int x, int y, string appearance, int dx, int dy, int moveSpeed, ConsoleColor[] colorPattern)
            : base(x, y, appearance, colorPattern[0])
        {
            DX = dx;
            DY = dy;
            MoveSpeed = moveSpeed;
            moveUpdateRate = 10;
            ColorPattern = colorPattern;
            actualX = x;
            actualY = y;
        }

        public override void Update()
        {
            moveCounter++;

            if (moveCounter % moveUpdateRate == 0)
            {
                RandomizeMovement();
            }

            double targetVelocityX = DX * (MoveSpeed * SPEED_MULTIPLIER);
            double targetVelocityY = DY * (MoveSpeed * SPEED_MULTIPLIER * 0.5);

            velocityX = velocityX * MOVEMENT_SMOOTHING + targetVelocityX * (1 - MOVEMENT_SMOOTHING);
            velocityY = velocityY * MOVEMENT_SMOOTHING + targetVelocityY * (1 - MOVEMENT_SMOOTHING);

            verticalOffset += WAVE_SPEED;
            double waveMotion = Math.Sin(verticalOffset) * WAVE_AMPLITUDE * MoveSpeed;

            actualX += velocityX * 0.3;
            actualY += (velocityY + waveMotion) * 0.3;

            X = (int)Math.Round(actualX);
            Y = (int)Math.Round(actualY);

            CheckBounds(ref actualX, ref actualY);

            if (random.Next(100) < 20)
            {
                int bubbleX;
                if (IsMultiLine)
                {
                    string[] lines = Appearance.Split('\n');
                    int middleLineIndex = lines.Length / 2;
                    string middleLine = lines[middleLineIndex].TrimEnd();

                    bubbleX = DX > 0 ? X + middleLine.Length - 1 : X;
                    if (bubbleX >= 0 && bubbleX < Program.Width)
                    {
                        Program.bubbles.Add(new Bubble(bubbleX, Y + middleLineIndex, ConsoleColor.White));
                    }
                }
                else
                {
                    bubbleX = DX > 0 ? X + Appearance.Length - 1 : X;
                    if (bubbleX >= 0 && bubbleX < Program.Width)
                    {
                        Program.bubbles.Add(new Bubble(bubbleX, Y, ConsoleColor.White));
                    }
                }
            }
        }

        public ConsoleColor GetColorForChar(char character)
        {
            return character switch
            {
                'o' => ConsoleColor.Green,
                ',' => ConsoleColor.Green,
                '/' => ConsoleColor.Magenta,
                '>' => ConsoleColor.Red,
                ':' => ConsoleColor.Magenta,
                ';' => ConsoleColor.DarkYellow,
                _ => Color
            };
        }

        public override void DrawEntity((char Char, ConsoleColor Color)[,] buffer)
        {
            string[] lines = Appearance.Split('\n');
            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y].TrimEnd();
                int colorStart = DX > 0 ? 0 : ColorPattern.Length - 1;
                int colorDirection = DX > 0 ? 1 : -1;

                for (int x = 0; x < line.Length; x++)
                {
                    if (Y + y >= 0 && Y + y < buffer.GetLength(0) &&
                        X + x >= 0 && X + x < buffer.GetLength(1))
                    {
                        char currentChar = line[x];
                        if (char.IsWhiteSpace(currentChar))
                        {
                            if (Y + y < buffer.GetLength(0) / 2)
                            {
                                buffer[Y + y, X + x] = ('≈', ConsoleColor.Blue);
                            }
                            else
                            {
                                buffer[Y + y, X + x] = ('≈', ConsoleColor.DarkBlue);
                            }
                        }
                        else
                        {
                            ConsoleColor charColor = GetColorForChar(currentChar);
                            if (charColor == Color)
                            {
                                int colorIndex = (colorStart + (x * colorDirection)) % ColorPattern.Length;
                                if (colorIndex < 0) colorIndex += ColorPattern.Length;
                                charColor = ColorPattern[colorIndex];
                            }
                            buffer[Y + y, X + x] = (currentChar, charColor);
                        }
                    }
                }
            }
        }

        private void ReverseAppearance()
        {
            if (IsMultiLine)
            {
                string[] lines = Appearance.Split('\n');
                string[] reversedLines = new string[lines.Length];
                int maxLength = lines.Max(line => line.TrimEnd().Length);

                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].TrimEnd();
                    string paddedLine = trimmedLine.PadRight(maxLength);

                    char[] reversed = new char[maxLength];
                    for (int j = 0; j < maxLength; j++)
                    {
                        char ch = paddedLine[maxLength - 1 - j];
                        reversed[j] = ch switch
                        {
                            '>' => '<',
                            '<' => '>',
                            '/' => '\\',
                            '\\' => '/',
                            ')' => '(',
                            '(' => ')',
                            '[' => ']',
                            ']' => '[',
                            '{' => '}',
                            '}' => '{',
                            ',' => ',',
                            '`' => '´',
                            _ => ch
                        };
                    }

                    reversedLines[i] = new string(reversed).TrimEnd();
                }


                Appearance = string.Join("\n", reversedLines);
            }
            else
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

        private void RandomizeMovement()
        {

            if (random.Next(15) == 0) 
            {
                DX = (DX > 0) ? 1 : -1;
            }

            if (random.Next(8) == 0) 
            {
                DY = Math.Sign(DY + (random.NextDouble() - 0.5) * 0.8);
            }
        }

        private void CheckBounds(ref double nextX, ref double nextY)
        {
            int maxWidth = IsMultiLine ?
                Appearance.Split('\n').Max(line => line.Length) :
                Appearance.Length;
            int height = IsMultiLine ?
                Appearance.Split('\n').Length :
                1;


            if (nextX <= 2)
            {
                DX = 1;
                ReverseAppearance();
                nextX = 2;
            }
            else if (nextX >= Program.Width - maxWidth - 2)
            {
                DX = -1;
                ReverseAppearance();
                nextX = Program.Width - maxWidth - 2;
            }

            if (nextY <= 2)
            {
                DY = Math.Abs(DY);
                nextY = 2;
            }
            else if (nextY >= Program.Height - height - 2)
            {
                DY = -Math.Abs(DY);
                nextY = Program.Height - height - 2;
            }
        }
    }
}
