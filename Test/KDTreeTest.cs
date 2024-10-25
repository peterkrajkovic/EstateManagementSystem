using Classes;
using Classes.structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class KDTreeTest
    {
        private static Func<CustomClass, CustomClass, int, int> compareFunc = (CustomClass first, CustomClass second, int level) =>
        {
            switch (level)
            {
                case 0:
                    {
                        int cmp = first.A.CompareTo(second.A);
                        if (cmp == 0)
                        {
                            return first.B.CompareTo(second.B);
                        } else
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
            public double A {  get; set; }
            public string B { get; set; }
            public int C { get; set; }
            public double D { get; set; }
        }

        [DataTestMethod]
        [DataRow(50)]
        public void RandomOperationTest(int operations)
        {
            KDTreeLambda<CustomClass>  tree = new KDTreeLambda<CustomClass>(4, compareFunc);
            Console.WriteLine($"Starting RandomOperationTest with {operations} operations.");

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
                    int currentCount = tree.Count;
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

                    foreach (var item in added)
                    {
                        var list = tree.Find(item);
                        if ((list == null || list.Count() == 0) && !item.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Added item was removed without permission");
                        }
                        else if ((list != null) && item.Equals(added[position]))
                        {
                            Console.Error.WriteLine("Item was not removed properly");
                            tree.Remove(item);
                        }
                    }

                    added.RemoveAt(position);
                }
                else
                {
                    //insert
                   
                    CustomClass newItem = new CustomClass() { A = rnd.NextDouble(), B = "", C = rnd.Next(), D = rnd.NextDouble()};
                    currentId++;
                    Console.WriteLine("Inserting new " + newItem.ToString());
                    int currentCount = tree.Count;
                    tree.Insert(newItem);
                    added.Add(newItem);
                    if (tree.Count == currentCount + 1)
                    {
                        Console.WriteLine("1 item added");
                    }
                    else
                    {
                        Console.Error.WriteLine("1 item was not added");
                    }
                    var list = tree.Find(newItem);
                    if (list == null || list.Count() == 0)
                    {
                        Console.Error.WriteLine("Added item was not found");
                    }

                }
            }

        }
        
    }
}
