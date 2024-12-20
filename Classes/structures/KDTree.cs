﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
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
                        #region Check all keys
                        for (int i = 0; i < dimensions; i++)
                        {
                            if (compareFunc(current.Data[0], point, i) != 0)
                            {
                                isSame = false;
                                break;
                            }
                        }
                        #endregion
                        if (isSame) // all keys are the same, insert into list 
                        {
                            current.Data.Add(point);
                            Count++;
                            return;
                        }
                    }
                    //continue with left son
                    current = current.Left;
                    isLeftChild = true;
                }
                else
                {
                    //continue with right son
                    current = current.Right;
                    isLeftChild = false;
                }

                level++;
            }

            //insert new node
            KDTreeNode<T> newNode = new KDTreeNode<T>(point);
            newNode.Parent = parent;

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
                            else  //remove node
                            {
                                RemoveNode(current,level);

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
        private void RemoveNode(KDTreeNode<T> currentNode, int currentLevel)
        {
            List<(KDTreeNode<T>, int)> nodesToRemove = new() { (currentNode, currentLevel) };
            List<T> reinsertNodes = new();

            while (nodesToRemove != null && nodesToRemove.Count() > 0)
            {
                KDTreeNode<T> current = nodesToRemove[0].Item1;
                int level = nodesToRemove[0].Item2;
                nodesToRemove.RemoveAt(0);

                #region Remove if Node is Leaf
                //check if current is leaf
                if (current.Left == null && current.Right == null)
                {
                    if (current.Parent == null)
                    {
                        //node is root
                        Root = null;
                    }
                    else
                    {
                        if (current.Parent.Left == current)
                        {
                            current.Parent.Left = null;
                        }
                        else
                        {
                            current.Parent.Right = null;
                        }
                    }
                    return;
                }
                #endregion

                KDTreeNode<T> subNode = current;
                int currentDimension = level;

                while (subNode.Left != null || subNode.Right != null)
                {
                    currentDimension = (currentDimension + 1) % dimensions;
                    List<(KDTreeNode<T>, int)> subNodes;
                    if (subNode.Left != null)
                    {
                        FindPredecessorByDimension(subNode.Left, level, ref currentDimension, out subNodes);
                    }
                    else
                    {
                        FindSuccessorsByDimension(subNode.Right!, level, ref currentDimension, out subNodes);
                    }
                    level = currentDimension;

                    if (subNodes != null && subNodes.Count() > 0)
                    {
                        //take first node
                        subNode = subNodes[0].Item1;
                        subNodes.RemoveAt(0);

                        //swap data 
                        var data = subNode.Data;
                        subNode.Data = current.Data;
                        current.Data = data;
                        current = subNode;

                        //Remove points with same key at wanted dimension
                        if (subNodes.Count() > 0)
                        {
                            foreach (var item in subNodes)
                            {
                                nodesToRemove.Add((item));
                                foreach (var d in item.Item1.Data)
                                {
                                    reinsertNodes.Add(d);
                                } }}
                    }
                    else
                    {
                        throw new ApplicationException("No replacement node found.");
                    }
                }
                //final remove
                if (subNode.Parent.Left != null && subNode == subNode.Parent.Left)
                {
                    subNode.Parent.Left = null;
                }
                else
                {
                    subNode.Parent.Right = null;
                }}
            //reinsert points
            if (reinsertNodes != null && reinsertNodes.Count() > 0)
            {
                foreach (var item in reinsertNodes)
                {
                    Insert(item);
                }
            }
            
        }
        /// <summary>
        /// Returns one predecessor based on dimension currentDImension is now dimension of the predecessor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="dimension"></param>
        /// <param name="currentDimension">in-out parameter</param>
        /// <param name="predecessors"></param>
        public void FindPredecessorByDimension(KDTreeNode<T> root, int dimension, ref int currentDimension, out List<(KDTreeNode<T>,int)> predecessors)
        {
            KDTreeNode<T> current = root;
            predecessors = new() { (root,currentDimension) };
            List<(KDTreeNode<T>, int)> nextToProcess = new();
            int currentLevel = currentDimension;

            if (current.Left != null)
            {
                nextToProcess.Add((current.Left, currentLevel));
            }
            if (current.Right != null)
            {
                nextToProcess.Add((current.Right, currentLevel));
            }

            while (nextToProcess.Count() > 0)
            {
                current = nextToProcess.First().Item1;
                currentLevel = (nextToProcess.First().Item2 + 1) % dimensions;
                nextToProcess.RemoveAt(0);

                //update predecessors
                if (compareFunc(current.Data[0], predecessors[0].Item1.Data[0], dimension) > 0)
                {
                    predecessors.Clear();
                    predecessors.Add((current, dimension));
                    currentDimension = currentLevel;
                }

                //desired dimension - take right son if exists
                if (currentLevel == dimension)
                {
                    if (current.Right != null)
                    {
                        nextToProcess.Add((current.Right, currentLevel));
                    }
                }
                else
                {
                    //non-desired dimension need to process both sons
                    if (current.Right != null)
                    {
                        nextToProcess.Add((current.Right, currentLevel));
                    }
                    if (current.Left != null)
                    {
                        nextToProcess.Add((current.Left, currentLevel));
                    }
                }

            }

        }

        /// <summary>
        /// returns list of successors along their dimension
        /// </summary>
        /// <param name="root"></param>
        /// <param name="dimension"></param>
        /// <param name="currentDimension">in-out parameter, dimension of the first successor</param>
        /// <param name="successors"></param>
        public void FindSuccessorsByDimension(KDTreeNode<T> root, int dimension, ref int currentDimension, out List<(KDTreeNode<T>,int)> successors)
        {
            KDTreeNode<T> current = root;
            successors = new() { (root, currentDimension) };

            List<(KDTreeNode<T>, int)> nextToProcess = new();
            int currentLevel = currentDimension;

            if (current.Left != null)
            {
                nextToProcess.Add((current.Left, currentLevel));
            }
            if (current.Right != null)
            {
                nextToProcess.Add((current.Right, currentLevel));
            }

            while (nextToProcess.Count() > 0)
            {
                current = nextToProcess.First().Item1;
                currentLevel = (nextToProcess.First().Item2 + 1) % dimensions;
                nextToProcess.RemoveAt(0);

                //update successors
                if (compareFunc(current.Data[0], successors[0].Item1.Data[0], dimension) == 0)
                {
                    successors.Add((current, currentLevel));
                }
                else if (compareFunc(current.Data[0], successors[0].Item1.Data[0], dimension) < 0)
                {
                    successors.Clear();
                    successors.Add((current,currentLevel));
                    currentDimension = currentLevel;
                }


                //desired dimension - take left son if exists
                if (currentLevel == dimension)
                {
                    if (current.Left != null)
                    {
                        nextToProcess.Add((current.Left, currentLevel));
                    }
                }
                else
                {
                    //non-desired dimension need to process both sons
                    if (current.Right != null)
                    {
                        nextToProcess.Add((current.Right, currentLevel));
                    }
                    if (current.Left != null)
                    {
                        nextToProcess.Add((current.Left, currentLevel));
                    }
                }

            }

        }
        public List<T>? Find(T point)
        {
            int level = 0;
            KDTreeNode<T>? current = GetRoot();

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
                            if (compareFunc(current.Data[0], point, i) != 0)
                            {
                                isSame = false;
                                break;
                            }
                        }
                        if (isSame)
                        {
                            return current.Data;
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
            nextCurrents.Add((current, level));

            while (nextCurrents.Count() != 0)
            {
                (current, level) = nextCurrents.First();
                nextCurrents.RemoveAt(0);

                //check if node fits in range
                bool isInRange = true;
                for (int i = 0; i < dimensions; i++)
                {
                    bool isAboveMin = compareFunc(lowerBound, current.Data[0], i) <= 0;
                    bool isUnderMax = compareFunc(higherBound, current.Data[0], i) >= 0;
                    if (!isAboveMin || !isUnderMax)
                    {
                        isInRange = false;
                        break;
                    }
                }
                if (isInRange)
                {
                    dataList.AddRange(current.Data);
                }
                //decide which son to visit next
                level = level % dimensions;

                int compareResultLower = compareFunc(lowerBound, current.Data[0], level); //-1 :lowerBound < current
                int compareResultHigher = compareFunc(higherBound, current.Data[0], level); //-1 : higherBound < current

                if (compareResultLower <= 0 && current.Left != null)
                {
                    nextCurrents.Add((current.Left, level + 1));
                }

                if (compareResultHigher > 0 && current.Right != null)
                {
                    nextCurrents.Add((current.Right, level + 1));
                }
                current = null;
            }
            return dataList;
        }

        public IEnumerable<T> LevelOrderTraversal()
        {
            if (Root == null)
            {
                yield break;
            }

            Queue<KDTreeNode<T>> queue = new Queue<KDTreeNode<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                KDTreeNode<T> current = queue.Dequeue();

                // Yield each point
                foreach (var data in current.Data)
                {
                    yield return data;
                }

                //queue sons
                if (current.Left != null)
                {
                    queue.Enqueue(current.Left);
                }
                if (current.Right != null)
                {
                    queue.Enqueue(current.Right);
                }
            }
        }

        public KDTreeNode<T>? GetRoot()
        {
            return Root;
        }

        public class KDTreeNode<T>
        {
            public KDTreeNode<T>? Parent { get; set; }
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
