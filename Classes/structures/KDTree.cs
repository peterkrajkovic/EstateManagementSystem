using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Classes.structures
{
    public class KDTree<T>
    {
        private int dimensions;
        private KDTreeNode<T>? Root { get; set; }

        private Func<T, T, int, int> compareFunc;

        public long Count { get; set; } = 0;
        public KDTree(int dimensions, Func<T, T, int, int> compare)
        {
            this.dimensions = dimensions;
            this.compareFunc = compare;
        }

        public void Insert(T point)
        {
            int level = 0;
            KDTreeNode<T>? current = GetRoot();
            KDTreeNode<T>? parent = null;
            bool isLeftChild = true;

            while (current != null)
            {
                parent = current;
                level = level % dimensions;

                int compareResult = compareFunc(point, current.Data[0], level);

                if (compareResult <= 0)
                {
                    if (compareResult == 0)
                    {
                        //check all keys
                        bool isSame = true;
                        for (int i = 0; i < dimensions; i++)
                        {
                            if (compareFunc(current.Data[0], point, i) != 0)
                            {
                                isSame = false;
                                break;
                            }
                        }
                        if (isSame)
                        {
                            current.Data.Add(point);
                            return;
                        }
                    }
                    current = current.Left;
                    isLeftChild = true;
                }
                else
                {
                    current = current.Right;
                    isLeftChild = false;
                }

                level++;
            }

            KDTreeNode<T> newNode = new KDTreeNode<T>(point);

            if (parent == null)
            {
                this.Root = newNode;
            }
            else if (isLeftChild)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }
            Count++;
        }

        public void Remove(T point)
        {
            int level = 0;
            KDTreeNode<T>? current = Root;
            KDTreeNode<T>? parent = null;
            bool isLeftChild = false;

            while (current != null)
            {
                level = level % dimensions;

                //find node with  same keys
                int compareResult = compareFunc(point, current.Data[0], level);
                if (compareResult <= 0)
                {
                    if (compareResult == 0)
                    {
                        //check all keys
                        bool isSame = true;
                        for (int i = 0; i < dimensions; i++)
                        {
                            if (compareFunc(point, current.Data[0], i) != 0)
                            {
                                isSame = false;
                                break;
                            }
                        }
                        if (isSame)
                        {
                            //removing point
                            Count--;

                            if (current.Data.Count() > 1)
                            {
                                //no need to remove node
                                for (int i = 0; i < current.Data.Count(); i++)
                                {
                                    if (current.Data[i].Equals(point))
                                    {
                                        current.Data.RemoveAt(i);
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                //remove node
                                
                                //check if current is leaf
                                if (current.Left == null && current.Right == null)
                                {
                                    if (parent == null)
                                    {
                                        //node is root
                                        Root = null;
                                    }
                                    else
                                    {
                                        if (isLeftChild)
                                        {
                                            parent.Left = null;
                                        }
                                        else
                                        {
                                            parent.Right = null;
                                        }
                                    }
                                    return;
                                }
                                #region RemoveBySuccessorOrPredecessor
                                //KDTreeNode<T> subNode = current.Left == null ? current.Right : current.Left;
                                //KDTreeNode<T>? subNodeParent = current;
                                //int currentDimension = (level + 1) % dimensions;
                                //List<KDTreeNode<T>> reinsertNodes = new();


                                //while (subNode.Left != null || subNode.Right != null)
                                //{
                                //    //continue
                                //    current = subNode;
                                //    if (current.Left != null)
                                //    {
                                //        FindPredecessorsByDimension(current.Left, level, ref currentDimension, out subNode, out subNodeParent);
                                //    } else
                                //    {
                                //        List<KDTreeNode<T>> successors = null;
                                //        FindSuccessorsByDimension(current.Right!, level, ref currentDimension, out successors, out subNodeParent);

                                //        if (successors != null && successors.Count() > 0)
                                //        {
                                //            subNode = successors[0];
                                //            successors.RemoveAt(0);

                                //            //Remove points with same key at wanted dimension
                                //            if (successors.Count() > 1)
                                //            {
                                //                foreach(var item in successors)
                                //                {
                                //                    foreach (var d in item.Data)
                                //                    {
                                //                        Remove(d);
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //    //swap data 
                                //    var data = subNode.Data;
                                //    subNode.Data = current.Data;
                                //    current.Data = data;
                                //}

                                //if (subNodeParent == null)
                                //{
                                //    subNodeParent = current;
                                //}
                                ////final remove
                                //if(subNodeParent.Left != null && subNode == subNodeParent.Left)
                                //{
                                //    subNodeParent.Left = null;
                                //} else
                                //{
                                //    subNodeParent.Right = null;
                                //}

                                ////reinsert points
                                //if (reinsertNodes != null && reinsertNodes.Count() > 0)
                                //{
                                //    foreach (var item in reinsertNodes)
                                //    {
                                //        foreach (var d in item.Data)
                                //        {
                                //            Remove(d);
                                //        }
                                //    }
                                //}
                                #endregion

                                #region delete subtree
                                //reinsert items below current deleted node
                                if (parent == null)
                                {
                                    //node is root
                                    Root = null;
                                }
                                else
                                {
                                    if (isLeftChild)
                                    {
                                        parent.Left = null;
                                    }
                                    else
                                    {
                                        parent.Right = null;
                                    }
                                }
                                List<KDTreeNode<T>> objectsToInsert = new();
                                if (current.Left != null)
                                {
                                    objectsToInsert.Add(current.Left);
                                }
                                if (current.Right != null)
                                {
                                    objectsToInsert.Add(current.Right);
                                }
                                current.Left = null;
                                current.Right = null;
                                while (objectsToInsert.Count() > 0)
                                {
                                    var currentNode = objectsToInsert.Last();
                                    objectsToInsert.RemoveAt(objectsToInsert.Count() - 1);

                                    for (int i = 0; i < currentNode.Data.Count(); i++)
                                    {
                                        Count--;
                                        Insert(currentNode.Data[i]);
                                    }

                                    if (currentNode.Left != null)
                                    {
                                        objectsToInsert.Add(currentNode.Left);
                                    }
                                    if (currentNode.Right != null)
                                    {
                                        objectsToInsert.Add(currentNode.Right);
                                    }
                                }
                                #endregion
                                return;
                            }
                        }
                    }
                    parent = current;
                    isLeftChild = true;
                    current = current.Left;
                    level++;
                }
                else
                {
                    parent = current;
                    isLeftChild = false;
                    current = current.Right;
                    level++;
                }

            }
        }

        public void FindPredecessorsByDimension(KDTreeNode<T> root, int dimension, ref int currentDimension, out KDTreeNode<T> predecessor, out KDTreeNode<T>? predecessorParent)
        {
            KDTreeNode<T> current = root;
            KDTreeNode<T>? currentParent = null;
            predecessor = root;
            predecessorParent = null;
            List<(KDTreeNode<T>,int, KDTreeNode<T>?)> nextToProces = new();
            int currentLevel = currentDimension;

            if (current.Left != null) {
                nextToProces.Add((current.Left, currentLevel, currentParent));
            }
            if (current.Right != null) {
                nextToProces.Add((current.Right, currentLevel, currentParent));
            }

            while (nextToProces.Count() > 0)
            {
                current = nextToProces.Last().Item1;
                currentLevel = (nextToProces.Last().Item2 + 1) % dimensions;
                currentParent = nextToProces.Last().Item3;
                nextToProces.RemoveAt(nextToProces.Count() - 1);

                if (compareFunc(current.Data[0], predecessor.Data[0], dimension) >= 0)
                {
                    predecessor = current;
                    currentDimension = currentLevel;
                    predecessorParent = currentParent;
                }

                //desired dimension - take right son if exists
                if (currentLevel == dimension)
                {
                    if (current.Right != null)
                    {
                        nextToProces.Add((current.Right, currentLevel, currentParent));
                    }
                } else
                {
                    //non-desired dimension need to process both sons
                    if (current.Right != null)
                    {
                        nextToProces.Add((current.Right, currentLevel, currentParent));
                    }
                    if (current.Left != null)
                    {
                        nextToProces.Add((current.Left, currentLevel, currentParent));
                    }
                }

            }

        }


        public void FindSuccessorsByDimension(KDTreeNode<T> root, int dimension, ref int currentDimension, out List<KDTreeNode<T>> successors, out KDTreeNode<T>? predecessorParent)
        {
            KDTreeNode<T> current = root;
            KDTreeNode<T>? currentParent = null;
            successors = new();
            predecessorParent = null;
            List<(KDTreeNode<T>, int, KDTreeNode<T>?)> nextToProcess = new();
            int currentLevel = currentDimension;

            if (current.Left != null)
            {
                nextToProcess.Add((current.Left, currentLevel, currentParent));
            }
            if (current.Right != null)
            {
                nextToProcess.Add((current.Right, currentLevel, currentParent));
            }

            while (nextToProcess.Count() > 0)
            {
                current = nextToProcess.Last().Item1;
                currentLevel = (nextToProcess.Last().Item2 + 1) % dimensions;
                currentParent = nextToProcess.Last().Item3;
                nextToProcess.RemoveAt(nextToProcess.Count() - 1);

                if (compareFunc(current.Data[0], successors[0].Data[0], dimension) == 0)
                {
                    successors.Add(current);
                    currentDimension = currentLevel;
                    predecessorParent = currentParent;
                }
                else if (compareFunc(current.Data[0], successors[0].Data[0], dimension) < 0);
                {
                    successors.Clear();
                    successors.Add(current);
                }

                //desired dimension - take left son if exists
                if (currentLevel == dimension)
                {
                    if (current.Left != null)
                    {
                        nextToProcess.Add((current.Left, currentLevel, currentParent));
                    }
                }
                else
                {
                    //non-desired dimension need to process both sons
                    if (current.Right != null)
                    {
                        nextToProcess.Add((current.Right, currentLevel, currentParent));
                    }
                    if (current.Left != null)
                    {
                        nextToProcess.Add((current.Left, currentLevel, currentParent));
                    }
                }

            }

        }
        public List<T>? Find(T point)
        {
            int level = 0;
            KDTreeNode<T>? current = GetRoot();
            List<T> foundPoints = new();

            while (current != null)
            {
                level = level % dimensions;

                int compareResult = compareFunc(point, current.Data[0], level);

                if (compareResult <= 0)
                {
                    //check all dimensions
                    if (compareResult == 0)
                    {
                        bool isSame = true;
                        for (int i = 0; i < dimensions; i++)
                        {
                            if (compareFunc(current.Data[0], point, level) != 0)
                            {
                                isSame = false;
                                break;
                            }
                        }
                        if (isSame)
                        {
                            for (int i = 0; i < current.Data.Count(); i++)
                            {
                                if (current.Data[i].Equals(point))
                                {
                                    foundPoints.Add(current.Data[i]);
                                }
                            }
                            return foundPoints;
                        }
                    }

                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
                level++;
            }
            return null;
        }

        public List<T> RangeFind(T lowerBound, T higherBound)
        {
            int level = 0;
            List<T> dataList = new List<T>();
            KDTreeNode<T>? current = Root;

            if (current == null)
            {
                return null;
            }

            KDTreeNode<T>? parent = null;

            // Initialize nextCurrents as a list of tuples (node, level)
            List<(KDTreeNode<T> Node, int Level)> nextCurrents = new();
            nextCurrents.Add((current,level));

            while (nextCurrents.Count() != 0)
            {
                (current, level) = nextCurrents.First();
                nextCurrents.RemoveAt(0);

                //check if node fits in range
                bool isInRange = true;
                for (int i = 0; i < dimensions; i++)
                {
                    bool isAboveMin = compareFunc(lowerBound, current.Data[0], i) < 0;
                    bool isUnderMax = compareFunc(higherBound, current.Data[0], i) > 0;
                    if (!isAboveMin || !isUnderMax)
                    {
                        isInRange = false;
                    }
                }

                if (isInRange)
                {
                    dataList.AddRange(current.Data);
                }

                //decide which son to visit next
                level = level % dimensions;

                
                int compareResultLower = compareFunc( lowerBound, current.Data[0], level); //-1 :lowerBound < current
                int compareResultHigher = compareFunc( higherBound, current.Data[0], level); //-1 : higherBound < current

                if (compareResultLower <= 0 && current.Left != null)
                {
                    nextCurrents.Add((current.Left,level + 1));
                }

                if (compareResultHigher > 0 && current.Right != null)
                {
                    nextCurrents.Add((current.Right,level + 1));
                }
                current = null;
            }
            return dataList;
        }
        

        public KDTreeNode<T>? GetRoot()
        {
            return Root;
        }

        public class KDTreeNode<T>
        {
            public KDTreeNode<T>? Left { get; set; }
            public KDTreeNode<T>? Right { get; set; }

            public List<T> Data { get; set; }
            public KDTreeNode(T data)
            {
                Data = new List<T> { data };
            }
        }
    }
}
