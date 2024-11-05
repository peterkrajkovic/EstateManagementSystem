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

        #region Structs for RandomOperationTest
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
        #endregion

        [DataTestMethod]
        [DataRow(1000000)]
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
                    Console.WriteLine($"Operation {i + 1}. Removing " + added[position].ToString());
                    long currentCount = tree.Count;
                    tree.Remove(added[position]);
                    if (tree.Count == currentCount - 1)
                    {
                        Console.WriteLine("1 item removed");
                    }
                    else if (tree.Count >= currentCount)
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. 1 item was not removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }
                    else
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. Too many items were removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }

                    //after remove check if item was removed and other items were not
                    foreach (var item in added)
                    {
                        var list = tree.Find(item);
                        bool notFoundInList = true;
                        if (list != null)
                        {
                            foreach (var it in list)
                            {
                                if (it.Equals(item))
                                {
                                    notFoundInList = false;
                                }
                            }
                        }

                        if ((list == null || notFoundInList) && !item.Equals(added[position]))
                        {
                            Console.Error.WriteLine($"Operation {i + 1}. Added item {item.ToString()} was removed without permission");
                            var l = tree.Find(item);
                            failed++;
                        }
                        else if ((list != null) && item.Equals(added[position]))
                        {
                            Console.Error.WriteLine($"Operation {i + 1}. Item was not removed properly");
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
                    Console.WriteLine($"Operation {i+1}. Inserting " + newItem.ToString());
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
                        Console.Error.WriteLine($"Operation {i+1}. 1 item was not added");
                        failed++;
                    }
                    var list = tree.Find(newItem);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. Added item was not found");
                        failed++;
                    }

                }
            }
            Assert.AreEqual(0, failed);
        }

        #region Structs for 2D random operation test
        private static Func<Class2D, Class2D, int, int> compareFunc2D = (Class2D first, Class2D second, int level) =>
        {
            switch (level)
            {
                case 0:
                    {
                        int cmp = first.Primary.CompareTo(second.Primary);
                        if (cmp == 0)
                        {
                            return first.X.CompareTo(second.X);
                        }
                        else
                        {
                            return cmp;
                        }
                    }
                case 1:
                    {
                        int cmp = first.Primary.CompareTo(second.Primary);
                        if (cmp == 0)
                        {
                            return first.Y.CompareTo(second.Y);
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

        public class Class2D
        {
            public string Primary {  get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public override string ToString()
            {
                return $"2D Class Primary: {Primary}, X: {X}, Y: {Y}";
            }
        }
        public string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPRSTUVWYZ";
            Random random = new Random();
            string s = "";
            for (int i = 0; i < 10; i++) {
                s += chars[random.Next(chars.Length)];
            }
            return s;

        }

        #endregion
        [DataTestMethod]
        [DataRow(1000)]
        public void RandomOperationTest2D(int operations)
        {
            KDTree<Class2D> tree = new KDTree<Class2D>(2, compareFunc2D);
            Console.WriteLine($"Starting RandomOperationTest with {operations} operations.");
            int failed = 0;

            List<Class2D> added = new();
            Random rnd = new Random();
            long currentId = 0;

            for (int i = 0; i < 20000; i++)
            {
                Class2D newItem = new Class2D() { Primary = RandomString(), X = rnd.Next(0, 50), Y = rnd.Next(0, 50) };
                currentId++;
                Console.WriteLine($"Operation {i + 1}. Inserting " + newItem.ToString());
                long currentCount = tree.Count;
                tree.Insert(newItem);
                added.Add(newItem);
            }

            for (int i = 0; i < operations; i++)
            {
                if (rnd.Next(1, 3) < 2 && added.Count() > 0)
                {
                    //remove
                    int position = rnd.Next(0, added.Count());
                    Console.WriteLine($"Operation {i + 1}. Removing " + added[position].ToString());
                    long currentCount = tree.Count;
                    tree.Remove(added[position]);
                    if (tree.Count == currentCount - 1)
                    {
                        Console.WriteLine("1 item removed");
                    }
                    else if (tree.Count >= currentCount)
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. 1 item was not removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }
                    else
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. Too many items were removed. Previous count: " + currentCount + ", current count: " + tree.Count);
                        failed++;
                    }

                    //after remove check if item was removed and other items were not
                    foreach (var item in added)
                    {
                        var list = tree.Find(item);
                        bool notFoundInList = true;
                        if (list != null)
                        {
                            foreach (var it in list)
                            {
                                if (it.Equals(item))
                                {
                                    notFoundInList = false;
                                }
                            }
                        }

                        if ((list == null || notFoundInList) && !item.Equals(added[position]))
                        {
                            Console.Error.WriteLine($"Operation {i + 1}. Added item {item.ToString()} was removed without permission");
                            var l = tree.Find(item);
                            failed++;
                        }
                        else if ((list != null) && item.Equals(added[position]))
                        {
                            Console.Error.WriteLine($"Operation {i + 1}. Item was not removed properly");
                            failed++;
                        }
                    }
                    added.RemoveAt(position);
                }
                else
                {
                    //insert

                    Class2D newItem = new Class2D() { Primary = RandomString(), X = rnd.Next(0,50), Y = rnd.Next(0,50) };
                    currentId++;
                    Console.WriteLine($"Operation {i + 1}. Inserting " + newItem.ToString());
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
                        Console.Error.WriteLine($"Operation {i + 1}. 1 item was not added");
                        failed++;
                    }
                    var list = tree.Find(newItem);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine($"Operation {i + 1}. Added item was not found");
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


        #region Structs For RemoveTest
        class RemoveClass

        {
            public int X;
            public int Y;
        }
        Func<RemoveClass, RemoveClass, int, int> compareFuncRemove = (RemoveClass first, RemoveClass second, int level) =>
        {
            switch (level)
            {
                case 0:
                    return first.X.CompareTo(second.X);
                case 1:
                    return first.Y.CompareTo(second.Y);
                default:
                    return 0;
            }
        };
        #endregion

        [TestMethod]
        public void RemoveTest()
        {
            var tree = new KDTree<RemoveClass>(2, compareFuncRemove);
            var x1 = new RemoveClass() { X = 10, Y = 10 };
            var x2 = new RemoveClass() { X = 8, Y = 10 };
            var x3 = new RemoveClass() { X = 10, Y = 20 };
            var x4 = new RemoveClass() { X = 15, Y = 12 };
            var x5 = new RemoveClass() { X = 5, Y = 7 };
            var x6 = new RemoveClass() { X = 9, Y = 12 };
            var x7 = new RemoveClass() { X = 10, Y = 20 };
            var x8 = new RemoveClass() { X = 12, Y = 13 };
            var x9 = new RemoveClass() { X = 11, Y = 13 };
            var x10 = new RemoveClass() { X = 11, Y = 14 };
            var x11 = new RemoveClass() { X = 15, Y = 10 };
            var x12 = new RemoveClass() { X = 17, Y = 11 };
            var x13 = new RemoveClass() { X = 12, Y = 14 };
            var x14 = new RemoveClass() { X = 15, Y = 15 };


            tree.Insert(x1);
            tree.Insert(x2);
            tree.Insert(x3);
            tree.Insert(x4);
            tree.Insert(x5);
            tree.Insert(x6);
            tree.Insert(x7);
            tree.Insert(x8);
            tree.Insert(x9);
            tree.Insert(x10);
            tree.Insert(x11);
            tree.Insert(x12);
            tree.Insert(x13);
            tree.Insert(x14);

            Console.WriteLine(tree.GetRoot().Right.Right.Left.Data[0].X + ", "+ tree.GetRoot().Right.Right.Left.Data[0].Y);
            tree.Remove(x4);
            var l = tree.Find(x1);
            l = tree.Find(x2);
            l = tree.Find(x3);
            l = tree.Find(x5);
            l = tree.Find(x6);
            l = tree.Find(x7);
            l = tree.Find(x8);
            l = tree.Find(x9);
            l = tree.Find(x10);
            l = tree.Find(x11);
            l = tree.Find(x12);
            l = tree.Find(x13);
            l = tree.Find(x14);

            foreach (var item in tree.LevelOrderTraversal())
            {
                Console.WriteLine(item.X + ", " + item.Y);
            }
            tree.Remove(x1);
            foreach (var item in tree.LevelOrderTraversal())
            {
                Console.WriteLine(item.X + ", " + item.Y);
            }
            Console.WriteLine("-----------------");
            tree.Remove(x3);
            foreach (var item in tree.LevelOrderTraversal())
            {
                Console.WriteLine(item.X + ", " + item.Y);
            }
        }
    }
}
