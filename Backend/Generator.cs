using Classes;
using Classes.structures;
using System;
using System.Linq.Expressions;

namespace Backend
{
    public class Generator
    {

        public static GPS[] GenerateRandomGPSLocation()
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

            width1 = random.NextDouble() * 100000;
            height1 = random.NextDouble() * 100000;
            width2 = width1 + 1 +random.NextDouble() * 99;
            height2 = height1 + 1 + random.NextDouble() * 99;

            if (height2 > 0) heightChar2 = 'N';
            if (width2 > 0) widthChar2 = 'E';

            return new GPS[]
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

       
    }
}
