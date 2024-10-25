using Classes;
using Classes.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class Generator
    {
        public static KDTreeLambda<Estate> GenerateRandomLambdaTree(int parcelCount, int propertyCount)
        {
            Func<Estate, Estate, int, int> compare = (Estate first, Estate second, int level) =>
            {
                switch (level)
                {
                    case 0:
                        return first.RightTop.Width.CompareTo(second.RightTop.Width);
                    case 1:
                        return first.RightTop.Height.CompareTo(second.RightTop.Height);
                    case 2:
                        return first.LeftBottom.Width.CompareTo(second.LeftBottom.Width);
                    case 3:
                        return first.LeftBottom.Height.CompareTo(second.LeftBottom.Height);
                    default:
                        return 0;

                }
            };

            KDTreeLambda<Estate> newKDTree = new KDTreeLambda<Estate>(2, compare);
            Random random = new Random();
            for (int i = 0; i < parcelCount; i++)
            {
                List<GPS> gps = GenerateRandomGPSLocation();
                Parcel p = new Parcel(gps[0], gps[1], random.Next(), GenerateRandomName(random.Next(4, 10)));
                newKDTree.Insert(p);
            }

            for (int i = 0; i < propertyCount; i++)
            {
                List<GPS> gps = GenerateRandomGPSLocation();
                newKDTree.Insert(new Property(gps[0], gps[1], random.Next(), GenerateRandomName(random.Next(4, 10))));
            }

            return newKDTree;
        }
        public static KDTree<double, Estate> GenerateRandomTree(int parcelCount, int propertyCount, int coverage)
        {
            List<GPS> usedGPSParcel = new();
            List<GPS> usedGPSProperty = new();
            KDTree<double, Estate> newKDTree = new KDTree<double, Estate>(4);

            Random random = new Random();
            for (int i = 0; i < parcelCount; i++)
            {
                List<GPS> gps = GenerateRandomGPSLocation();
                Parcel p = new Parcel(gps[0], gps[1], random.Next(), GenerateRandomName(random.Next(4, 10)));
                newKDTree.Insert(new double[] { p.LeftBottom.Width, p.LeftBottom.Height, p.RightTop.Width, p.RightTop.Height }, p);
            }

            for (int i = 0; i < propertyCount; i++)
            {
                List<GPS> gps = GenerateRandomGPSLocation();
                Property p = new Property(gps[0], gps[1], random.Next(), GenerateRandomName(random.Next(4, 10)));
                newKDTree.Insert(new double[] { gps[0].Width, gps[0].Height, gps[1].Width, gps[1].Height }, p);
            }

            return newKDTree;
        }
        public static List<GPS> GenerateRandomGPSLocation()
        {
            Random random = new Random();
            char[] widthDirections = { 'W', 'E' };  // E = East, W = West
            char[] heightDirections = { 'N', 'S' }; // N = North, S = South

            double width1 = random.NextDouble() * 90;
            double height1 = random.NextDouble() * 180;
            double width2 = random.NextDouble() * 90;
            double height2 = random.NextDouble() * 180;

            char widthChar1 = widthDirections[random.Next(widthDirections.Length)];
            char heightChar1 = heightDirections[random.Next(heightDirections.Length)];
            char widthChar2 = widthDirections[random.Next(widthDirections.Length)];
            char heightChar2 = heightDirections[random.Next(heightDirections.Length)];

            // Adjust width and height based on direction
            if (widthChar1 == 'W') width1 = -width1;    // West makes width negative
            if (heightChar1 == 'S') height1 = -height1; // South makes height negative
            if (widthChar2 == 'W') width2 = -width2;
            if (heightChar2 == 'S') height2 = -height2;

            return new List<GPS>
            {
                new GPS(){ WidthChar = widthChar1, Width = width1, HeightChar = heightChar1, Height = height1 },
                new GPS(){ WidthChar = widthChar2, Width = width2, HeightChar = heightChar2, Height = height2 }
            };
        }
        public static string GenerateRandomName(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomName = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomName[i] = chars[random.Next(chars.Length)];
            }
            return new string(randomName);
        }


        /// <summary>
        /// Function repeatedly adds/removes points of the inserted tree
        /// </summary>
        /// <param name="tree">KDTree</param>
        /// <param name="operations">number of operations to be performed</param>
        /// <returns>points that were added to the tree</returns>
        public static (List<double>, List<int[]>) RandomTreeOperation(ref KDTree<int, double> tree, int dimensions, int operations)
        {
            Random rnd = new Random();
            List<double> addedDoubles = new List<double>();
            List<int[]> addedKeys = new List<int[]>();

            for (int i = 0; i < operations; i++)
            {
                if (rnd.Next(1, 3) < 2)
                {
                    //remove
                    if (addedDoubles.Count() > 0)
                    {
                        int position = rnd.Next(0, addedDoubles.Count());
                        tree.Remove(addedKeys[position], addedDoubles[position]);
                        addedDoubles.RemoveAt(position);
                        addedKeys.RemoveAt(position);
                    }
                }
                else
                {
                    //insert
                    int[] keys = new int[dimensions];
                    for (int j = 0; j < dimensions; j++)
                    {
                        keys[j] = rnd.Next(1, 200);
                    }
                    tree.Insert(keys, rnd.NextDouble());
                }
            }
            return (addedDoubles, addedKeys);
        }
    }
}
