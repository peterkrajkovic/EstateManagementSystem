using Classes;
using Classes.structures;
using System;
using System.Linq.Expressions;

namespace Backend
{
    public class Generator
    {

        public static List<GPS> GenerateRandomGPSLocation()
        {
            Random random = new Random();
            char[] widthDirections = { 'W', 'E' };  // E = East, W = West
            char[] heightDirections = { 'N', 'S' }; // N = North, S = South

            double width1, height1, width2, height2 = 1;

            char widthChar1 = widthDirections[random.Next(widthDirections.Length)];
            char heightChar1 = heightDirections[random.Next(heightDirections.Length)];
            char widthChar2 = widthDirections[random.Next(widthDirections.Length)];
            char heightChar2 = heightDirections[random.Next(heightDirections.Length)];

            // Adjust width and height based on direction
            if (widthChar1 == 'W') width1 = -1;    // West makes width negative
            if (heightChar1 == 'S') height1 = -1; // South makes height negative
            if (widthChar2 == 'W') width2 = -1;
            if (heightChar2 == 'S') height2 = -1;

            width1 = random.NextDouble() * 90;
            height1 = random.NextDouble() * 180;
            width2 = width1 + random.NextDouble() * 90;
            height2 = height1 + random.NextDouble() * 180;

            if (height2 > 0) heightChar2 = 'N';
            if (width2 > 0) widthChar2 = 'E';

            return new List<GPS>
            {
                new GPS(){ WidthChar = widthChar1, Width = width1, HeightChar = heightChar1, Height = height1 },
                new GPS(){ WidthChar = widthChar2, Width = width2, HeightChar = heightChar2, Height = height2 }
            };
        }
        public static string GenerateRandomName(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPRSTUVWYZ";
            var randomName = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomName[i] = chars[random.Next(chars.Length)];
            }
            return new string(randomName);
        }

        public static List<Estate> RandomLambdaTreeOperation(ref KDTree<Estate> tree, int operations)
        {
            List<Estate> added = new();
            Random rnd = new Random();
            long currentId = 0;

            for (int i = 0; i < operations; i++)
            {
                if (rnd.Next(1, 3) < 2 && added.Count() > 0)
                {
                    //remove
                    int position = rnd.Next(0, added.Count());
                    Console.WriteLine("Removing " + added[position].GetType());
                    long currentCount = tree.Count;
                    tree.Remove(added[position]);
                    if (tree.Count == currentCount - 1)
                    {
                        Console.WriteLine("1 item removed");
                    }
                    else if (tree.Count >= currentCount)
                    {
                        Console.Error.WriteLine("1 item was not removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                    }
                    else
                    {
                        Console.Error.WriteLine("Too many items were removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                    }

                    foreach (var estate in added)
                    {
                        var list = tree.Find(estate);
                        if ((list == null || list.Count() == 0) && !estate.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Added item was removed without permission");
                        }
                        else if ((list != null) && estate.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Item was not removed properly");
                            tree.Remove(estate);
                        }
                    }
                    
                    added.RemoveAt(position);
                }
                else
                {
                    //insert
                    var gps = GenerateRandomGPSLocation();
                    Estate newEstate = rnd.Next(1, 3) < 2 ? new Parcel(currentId, gps[0], gps[1], rnd.Next(1, 11000000), GenerateRandomName(rnd.Next(4, 7)))
                                                          : new Property(currentId, gps[0], gps[1], rnd.Next(1, 1000000), GenerateRandomName(rnd.Next(4, 7)));
                    currentId++;
                    Console.WriteLine("Inserting new " + newEstate.GetType());
                    long currentCount = tree.Count;
                    tree.Insert(newEstate);
                    added.Add(newEstate);
                    if (tree.Count == currentCount + 1)
                    {
                        Console.WriteLine("1 item added");
                    }
                    else
                    {
                        Console.Error.WriteLine("1 item was not added");
                    }
                    var list = tree.Find(newEstate);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine("Added item was not found");
                    }

                }
            }
            return added;
        }

       
    }
}
