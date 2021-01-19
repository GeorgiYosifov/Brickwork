using System;
using System.Collections.Generic;
using System.Linq;

namespace BrickBuilder
{
    class Program
    {
        private static int n;
        private static int m;
        private static IList<Brick> bricks = new List<Brick>();
        private static HashSet<int> usedBrickNumbers = new HashSet<int>();
        private static bool useAdditionalSpaceForStyle = false;

        static void Main()
        {
            try
            {
                InitializeDimensions();
                FetchBricks();
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e.Message);
                return;
            }
            
            ShuffleBricks();
            PrintSecondLayer();
        }

        private static void InitializeDimensions()
        {
            var dimensions = Console.ReadLine().Split().Select(int.Parse).ToArray();

            n = dimensions[0];
            m = dimensions[1];

            if (n % 2 != 0 || m % 2 != 0 || n >= 100 || m >= 100)
            {
                throw new Exception("-1");
            }
        }

        private static void FetchBricks()
        {
            for (int row = 0; row < n; row += 2)
            {
                var upNumbers = Console.ReadLine().Split().Select(int.Parse).ToArray();
                var downNumbers = Console.ReadLine().Split().Select(int.Parse).ToArray();

                var shifter = 0;
                for (int col = 0; col < m; col += shifter)
                {
                    var upNumber = upNumbers[col];
                    var downNumber = downNumbers[col];
                    if (upNumber == downNumber) // check for vertical brick
                    {
                        if (usedBrickNumbers.Contains(upNumber))
                        {
                            throw new Exception("-1");
                        }

                        var upCoordinates = new Coordinates(row, col);
                        var downCoordinates = new Coordinates(row + 1, col);

                        bricks.Add(new Brick(upNumber, upCoordinates, downCoordinates, BrickType.Vertical));
                        usedBrickNumbers.Add(upNumber);
                        shifter = 1;
                    }
                    else if (col != m - 1
                        && upNumbers[col + 1] == upNumber
                        && downNumbers[col + 1] == downNumber) // check for two horizontal bricks
                    {
                        if (usedBrickNumbers.Contains(upNumber) || usedBrickNumbers.Contains(downNumber))
                        {
                            throw new Exception("-1");
                        }

                        var upLeftCoordinates = new Coordinates(row, col);
                        var upRightCoordinates = new Coordinates(row, col + 1);
                        bricks.Add(new Brick(upNumber, upLeftCoordinates, upRightCoordinates, BrickType.Horizontal));
                        usedBrickNumbers.Add(upNumber);

                        var downLeftCoordinates = new Coordinates(row + 1, col);
                        var downRightCoordinates = new Coordinates(row + 1, col + 1);
                        bricks.Add(new Brick(downNumber, downLeftCoordinates, downRightCoordinates, BrickType.Horizontal));
                        usedBrickNumbers.Add(downNumber);
                        shifter = 2;
                    }
                    else
                    {
                        throw new Exception("-1");
                    }
                }
            }
        }

        private static void ShuffleBricks() // Transform segments of bricks
        {
            var shift = 0;
            for (int i = 0; i < bricks.Count; i += shift)
            {
                var tempBrick = new Brick(bricks[i].FirstPartCoordinates, bricks[i].SecondPartCoordinates);

                if (bricks[i].Type == BrickType.Vertical)
                {
                    if (bricks[i + 1].Type == BrickType.Vertical)
                    {
                        // 1 2 => 1 1
                        // 1 2    2 2
                        bricks[i].SecondPartCoordinates = bricks[i + 1].FirstPartCoordinates;
                        bricks[i].Type = BrickType.Horizontal;
                        bricks[i + 1].FirstPartCoordinates = tempBrick.SecondPartCoordinates;
                        bricks[i + 1].Type = BrickType.Horizontal;
                        shift = 2;
                    }
                    else if (bricks[i + 1].Type == BrickType.Horizontal)
                    {
                        if (i + 3 < bricks.Count && bricks[i + 3].Type == BrickType.Vertical)
                        {
                            // 1 2 2 4 => 1 1 3 3
                            // 1 3 3 4    2 2 4 4
                            var tempDownHorizontalBrick = new Brick(bricks[i + 2].FirstPartCoordinates, bricks[i + 2].SecondPartCoordinates);
                            bricks[i].SecondPartCoordinates = bricks[i + 1].FirstPartCoordinates;
                            bricks[i].Type = BrickType.Horizontal;
                            bricks[i + 1].FirstPartCoordinates = bricks[i + 3].FirstPartCoordinates;
                            bricks[i + 2].SecondPartCoordinates = tempBrick.SecondPartCoordinates;
                            bricks[i + 3].FirstPartCoordinates = tempDownHorizontalBrick.SecondPartCoordinates;
                            bricks[i + 3].Type = BrickType.Horizontal;
                            shift = 4;
                        }
                        else
                        {
                            // 3 1 1 => 1 1 3
                            // 3 2 2    1 2 3
                            bricks[i].FirstPartCoordinates = bricks[i + 1].SecondPartCoordinates;
                            bricks[i].SecondPartCoordinates = bricks[i + 2].SecondPartCoordinates;

                            bricks[i + 1].SecondPartCoordinates = tempBrick.FirstPartCoordinates;
                            bricks[i + 2].SecondPartCoordinates = tempBrick.SecondPartCoordinates;

                            // h -> horizontal brick
                            // v -> vertical brick
                            // Change order of bricks in their collection, for example: v h h => h h v
                            var tempUpHorizontal = bricks[i + 1];
                            var tempDownHorizontal = bricks[i + 2];
                            bricks[i + 2] = bricks[i];
                            bricks[i] = tempUpHorizontal;
                            bricks[i + 1] = tempDownHorizontal;

                            shift = 3;
                        }
                    }
                }
                else if (bricks[i].Type == BrickType.Horizontal)
                {
                    if (i + 2 < bricks.Count && bricks[i + 2].Type == BrickType.Vertical)
                    {
                        // 1 1 3 => 3 1 1
                        // 2 2 3    3 2 2
                        var tempVerticalBrick = new Brick(bricks[i + 2].FirstPartCoordinates, bricks[i + 2].SecondPartCoordinates);

                        bricks[i + 2].FirstPartCoordinates = bricks[i].FirstPartCoordinates;
                        bricks[i + 2].SecondPartCoordinates = bricks[i + 1].FirstPartCoordinates;

                        bricks[i].FirstPartCoordinates = tempVerticalBrick.FirstPartCoordinates;
                        bricks[i + 1].FirstPartCoordinates = tempVerticalBrick.SecondPartCoordinates;

                        // h -> horizontal brick
                        // v -> vertical brick
                        // Change order of bricks in their collection, for example: h h v => v h h
                        var tempUpHorizontal = bricks[i];
                        var tempDownHorizontal = bricks[i + 1];
                        bricks[i] = bricks[i + 2];
                        bricks[i + 1] = tempUpHorizontal;
                        bricks[i + 2] = tempDownHorizontal;

                        shift = 3;
                    }
                    else
                    {
                        // 1 1 => 1 2
                        // 2 2    1 2
                        bricks[i].SecondPartCoordinates = bricks[i + 1].FirstPartCoordinates;
                        bricks[i].Type = BrickType.Vertical;
                        bricks[i + 1].FirstPartCoordinates = tempBrick.SecondPartCoordinates;
                        bricks[i + 1].Type = BrickType.Vertical;
                        shift = 2;
                    }
                }
            }
        }

        private static void PrintSecondLayer()
        {
            Console.WriteLine();

            useAdditionalSpaceForStyle = bricks.FirstOrDefault(b => b.number.ToString().Length > 1) != null;
            var shifter = 0;
            var upRow = new List<string>();
            var downRow = new List<string>();
            var counter = 0;

            for (int i = 0; i <= bricks.Count; i += shifter)
            {
                if (counter >= m)
                {
                    Console.Write("|");
                    foreach (var value in upRow)
                    {
                        Console.Write(value);
                    }
                    Console.WriteLine();

                    Console.Write("|");
                    foreach (var value in downRow)
                    {
                        Console.Write(value);
                    }
                    Console.WriteLine();
                    
                    upRow = new List<string>();
                    downRow = new List<string>();
                    counter = 0;
                }

                if (i >= bricks.Count)
                {
                    break;
                }

                var numberToString = bricks[i].number.ToString();
                if (useAdditionalSpaceForStyle && numberToString.Length == 1)
                {                    
                    numberToString += " ";
                }

                if (bricks[i].Type == BrickType.Vertical)
                {
                    upRow.Add($"{numberToString}|");
                    downRow.Add($"{numberToString}|");

                    shifter = 1;
                    counter++;
                }
                else if (bricks[i].Type == BrickType.Horizontal)
                {
                    var downNumberToString = bricks[i + 1].number.ToString();
                    if (useAdditionalSpaceForStyle && downNumberToString.Length == 1)
                    {
                        downNumberToString += " ";
                    }

                    upRow.Add($"{numberToString} {numberToString}|");
                    downRow.Add($"{downNumberToString} {downNumberToString}|");

                    shifter = 2;
                    counter += 2; 
                }
            }
        }
    }   
}
