
namespace Fisharium.Entities
{
    public class Plant : Entity
    {
        private int swayCounter = 0;
        private int swayDirection = 1;

        public Plant(int x, int y, string appearance, ConsoleColor color)
            : base( x, y, appearance, color)
        {

        }

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
}
