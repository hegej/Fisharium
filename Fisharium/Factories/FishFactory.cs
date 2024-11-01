using Fisharium.Entities;


namespace Fisharium.Factories
{
    public class FishFactory
    {
        public static Fish CreateFishType1(int x, int y)
        {
            string appearance = @"
         ,       
      .:/    
   ,,///;,   ,;/ 
 o)::::::;;///
>::::::::;;\\\
  ''\\\\\'"" ';\ 
     '";

            ConsoleColor[] colorPattern = {
                ConsoleColor.Green,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.Yellow,
                ConsoleColor.Red
            };

            return new Fish(x, y, appearance, -1, 0, 4, colorPattern);
        }

        public static Fish CreateFishType2(int x, int y)
        {
            string appearance = "<><";
            ConsoleColor[] colorPattern = { ConsoleColor.Cyan, ConsoleColor.White };
            return new Fish(x, y, appearance, -1, 2, 5, colorPattern);
        }

        public static Fish CreateFishType3(int x, int y)
        {
            string appearance = "><(((*>";
            ConsoleColor[] colorPattern = { ConsoleColor.Magenta, ConsoleColor.Gray };
            return new Fish(x, y, appearance, 1, 0, 3, colorPattern);
        }
    }
}
