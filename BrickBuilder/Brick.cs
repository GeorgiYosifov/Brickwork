namespace BrickBuilder
{
    public class Brick
    {
        public readonly int number;

        public Brick(Coordinates firstPartCoordinates, Coordinates secondPartCoordinates)
        {
            this.FirstPartCoordinates = firstPartCoordinates;
            this.SecondPartCoordinates = secondPartCoordinates;
        }

        public Brick(int number, Coordinates firstPartCoordinates, Coordinates secondPartCoordinates, BrickType type)
            : this(firstPartCoordinates, secondPartCoordinates)
        {
            this.number = number;
            this.Type = type;
        }

        public Coordinates FirstPartCoordinates { get; set; }

        public Coordinates SecondPartCoordinates { get; set; }

        public BrickType Type { get; set; }
    }
}
