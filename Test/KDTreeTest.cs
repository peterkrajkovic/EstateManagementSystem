using Backend;
using Classes;
using Classes.structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Test
{
    [TestClass]
    public class KDTreeTest
    {
        private static Func<CustomClass, CustomClass, int, int> customCompareFunc = (CustomClass first, CustomClass second, int level) =>
        {
            switch (level)
            {
                case 0:
                    {
                        int cmp = first.A.CompareTo(second.A);
                        if (cmp == 0)
                        {
                            return first.B.CompareTo(second.B);
                        }
                        else
                        {
                            return cmp;
                        }
                    }
                case 1:
                    return first.C.CompareTo(second.C);
                case 2:
                    return first.D.CompareTo(second.D);
                case 3:
                    {
                        int cmp = first.B.CompareTo(second.B);
                        if (cmp == 0)
                        {
                            return first.C.CompareTo(second.C);
                        }
                        else
                        {
                            return cmp;
                        }
                    }
                default:
                    return 0;

            }
        };

        public class CustomClass
        {
            public double A { get; set; }
            public string B { get; set; }
            public int C { get; set; }
            public double D { get; set; }

            public override string ToString()
            {
                return $"Custom Class A: {A}, B: {B}, C: {C}, D: {D}";
            }
        }

        [DataTestMethod]
        [DataRow(10000)]
        public void RandomOperationTest(int operations)
        {
            KDTree<CustomClass> tree = new KDTree<CustomClass>(4, customCompareFunc);
            Console.WriteLine($"Starting RandomOperationTest with {operations} operations.");
            int failed = 0;

            List<CustomClass> added = new();
            Random rnd = new Random();
            long currentId = 0;

            for (int i = 0; i < operations; i++)
            {
                if (rnd.Next(1, 3) < 2 && added.Count() > 0)
                {
                    //remove
                    int position = rnd.Next(0, added.Count());
                    Console.WriteLine("Removing " + added[position].ToString());
                    long currentCount = tree.Count;
                    tree.Remove(added[position]);
                    if (tree.Count == currentCount - 1)
                    {
                        Console.WriteLine("1 item removed");
                    }
                    else if (tree.Count >= currentCount)
                    {
                        Console.Error.WriteLine("1 item was not removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }
                    else
                    {
                        Console.Error.WriteLine("Too many items were removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }

                    //after remove check if item was removed and other items were not
                    foreach (var item in added)
                    {
                        var list = tree.Find(item);
                        if ((list == null || list.Count() == 0) && !item.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Added item was removed without permission");
                            failed++;
                        }
                        else if ((list != null) && item.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Item was not removed properly");
                            tree.Remove(added[position]);
                            failed++;
                        }
                    }
                    added.RemoveAt(position);
                }
                else
                {
                    //insert

                    CustomClass newItem = new CustomClass() { A = rnd.NextDouble(), B = "", C = rnd.Next(), D = rnd.NextDouble() };
                    currentId++;
                    Console.WriteLine("Inserting new " + newItem.ToString());
                    long currentCount = tree.Count;
                    tree.Insert(newItem);
                    added.Add(newItem);

                    //after insert check if item was inserted
                    if (tree.Count == currentCount + 1)
                    {
                        Console.WriteLine("1 item added");
                    }
                    else
                    {
                        Console.Error.WriteLine("1 item was not added");
                        failed++;
                    }
                    var list = tree.Find(newItem);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine("Added item was not found");
                        failed++;
                    }

                }
            }
            Assert.AreEqual(0, failed);
        }


      

        [DataTestMethod]
        [DataRow(50)]
        public void RandomOperationTreeEstates(int operations)
        {
            KDTree<Estate> tree = new(4, Core.compareFunc);
            List<Estate> added = new();
            Random rnd = new Random();
            int failed = 0;
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
                        failed++;
                    }
                    else
                    {
                        Console.Error.WriteLine("Too many items were removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
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
                    var gps = Generator.GenerateRandomGPSLocation();
                    Estate newEstate = rnd.Next(1, 3) < 2 ? new Parcel(currentId, gps[0], gps[1], rnd.Next(1, 11000000), Generator.GenerateRandomName(rnd.Next(4, 7)))
                                                          : new Property(currentId, gps[0], gps[1], rnd.Next(1, 1000000), Generator.GenerateRandomName(rnd.Next(4, 7)));
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
                        failed++;
                    }
                    var list = tree.Find(newEstate);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine("Added item was not found");
                        failed++;
                    }

                }
            }
            Assert.AreEqual(failed, 0);
        }
    }
}
