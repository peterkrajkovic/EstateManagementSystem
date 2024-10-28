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
    public class KDTreeLambda<T>
    {
        private int dimensions;
        private KDTreeLambdaNode<T>? Root { get; set; }

        private Func<T, T, int, int> compareFunc;

        public int Count { get; set; }
        public KDTreeLambda(int dimensions, Func<T, T, int, int> compare)
        {
            this.dimensions = dimensions;
            this.compareFunc = compare;
        }

        public void Insert(T point)
        {
            int level = 0;
            KDTreeLambdaNode<T>? current = GetRoot();
            KDTreeLambdaNode<T>? parent = null;
            bool isLeftChild = true;

            while (current != null)
            {
                parent = current;
                level = level % dimensions;

                int compareResult = compareFunc(current.Data[0], point, level);

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

            KDTreeLambdaNode<T> newNode = new KDTreeLambdaNode<T>(point);

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
            KDTreeLambdaNode<T>? current = Root;
            KDTreeLambdaNode<T>? parent = null;
            bool isLeftChild = false;

            while (current != null)
            {
                level = level % dimensions;

                //find node with  same keys
                int compareResult = compareFunc(current.Data[0], point, level);
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
                                List<KDTreeLambdaNode<T>> objectsToInsert = new();

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

                                //reinsert items below current deleted node
                                if (current.Left != null)
                                {
                                    objectsToInsert.Add(current.Left);
                                }
                                if (current.Right != null)
                                {
                                    objectsToInsert.Add(current.Right);
                                }

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
        /// <summary>
        /// Helper method for Remove node
        /// <param name="root">starting point</param>
        /// <param name="dimension"> dimension of the wanted node</param>
        /// <param name="currentDimension"> dimension of the starting node if root passed dimension should be 0</param>
        /// <returns>leaf node- inorder predescessor by dimension, its parent and if node is its parent left child</returns>
        //public void FindPredecessorsByDimension(ref KDTreeLambdaNode<T> root, bool isLeftChild, int dimension, int currentDimension, out List<KDTreeLambdaNode<T>> predecessors, out bool isPredecessorLeftChild)
        //{
        //    ref KDTreeLambdaNode<T> current = ref root;
        //    KDTreeLambdaNode<T> parent = null;
        //    predecessors = new();
        //    int currentLevel = currentDimension;
        //    while (current != null)
        //    {
        //        if (currentLevel == dimension)
        //        {
        //            if (current.Right != null)
        //            {
        //                parent = current;
        //                current = current.Right;
        //                isLeftChild = false;

        //                predecessors.Clear();
        //                predecessors.Add(current.Right);
        //            }
        //            else
        //            {
        //                if (compareFunc(current.Left.Data[0], predecessors[0].Data[0], dimension) == 0)
        //                {
        //                    predecessors.Add(current.Left);
        //                    parent = current;
        //                    current = current.Left;
        //                    isLeftChild = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (compareFunc(current.Data[0], predecessors[0].Data[0], dimension) == 0)
        //            {
        //                predecessors.Add(current.Left);
        //                parent = current;
        //                current = current.Left;
        //                isLeftChild = true;
        //            }
        //            #region Finds node with the highest value of dimension from current node children
        //            if (current.Left == null && current.Right == null)
        //            {
        //                break; //reached leaf
        //            }
        //            bool dimensionSkipped = false;
        //            if (current.Left != null)
        //            {
        //                if (current.Left.Left != null && compareFunc(current.Left.Left.Data[0], current.Data[0], dimension) == 1)
        //                {
        //                    current = current.Left.Left;
        //                    parent = current.Left;
        //                    isLeftChild = true;
        //                    dimensionSkipped = true;
        //                }
        //                if (current.Left.Right != null && current.Left.Right.Keys[dimension].CompareTo(current.Keys[dimension]) == 1)
        //                {
        //                    current = current.Left.Right;
        //                    parent = current.Left;
        //                    isLeftChild = false;
        //                    dimensionSkipped = true;
        //                }
        //            }
        //            if (current.Right != null)
        //            {
        //                if (current.Right.Left != null && current.Right.Left.Keys[dimension].CompareTo(current.Keys[dimension]) == 1)
        //                {
        //                    current = current.Right.Left;
        //                    parent = current.Right;
        //                    isLeftChild = true;
        //                    dimensionSkipped = true;
        //                }
        //                if (current.Right.Right != null && current.Right.Right.Keys[dimension].CompareTo(current.Keys[dimension]) == 1)
        //                {
        //                    current = current.Right.Right;
        //                    parent = current.Right;
        //                    isLeftChild = false;
        //                    dimensionSkipped = true;
        //                }
        //            }
        //            if (dimensionSkipped)
        //            {
        //                currentLevel = (currentLevel + 1) % this.dimensions;
        //            }
        //            #endregion
        //        }

        //        currentLevel = (currentLevel + 1) % this.dimensions;
        //    }
        //    predecessor = current;
        //    predecessorParent = parent;
        //    isPredecessorLeftChild = isLeftChild;

        //}
        public List<T>? Find(T point)
        {
            int level = 0;
            KDTreeLambdaNode<T>? current = GetRoot();
            List<T> foundPoints = new();

            while (current != null)
            {
                level = level % dimensions;

                int compareResult = compareFunc(current.Data[0], point, level);

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
            KDTreeLambdaNode<T>? current = Root;

            if (current == null)
            {
                return null;
            }

            KDTreeLambdaNode<T>? parent = null;

            // Initialize nextCurrents as a list of tuples (node, level)
            List<(KDTreeLambdaNode<T> Node, int Level)> nextCurrents = new();
            nextCurrents.Add((current,level));

            while (nextCurrents.Count() != 0)
            {
                (current, level) = nextCurrents.First();
                nextCurrents.RemoveAt(0);

                //check if node fits in range
                bool isInRange = true;
                for (int i = 0; i < dimensions; i++)
                {
                    if (compareFunc(current.Data[0], lowerBound, i) < 0 || compareFunc(current.Data[0], higherBound, i) > 0)
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

                
                int compareResultLower = compareFunc(current.Data[0], lowerBound, level); //-1 :current < lowerBound
                int compareResultHigher = compareFunc(current.Data[0], higherBound, level); //-1 : current < higherBound

                if (compareResultLower >= 0 && current.Left != null)
                {
                    nextCurrents.Add((current.Left,level + 1));
                }

                if (compareResultHigher <= 0 && current.Right != null)
                {
                    nextCurrents.Add((current.Right,level + 1));
                }
                current = null;
            }
            return dataList;
        }
        

        public KDTreeLambdaNode<T>? GetRoot()
        {
            return Root;
        }

        public class KDTreeLambdaNode<T>
        {
            public KDTreeLambdaNode<T>? Left { get; set; }
            public KDTreeLambdaNode<T>? Right { get; set; }

            public List<T> Data { get; set; }
            public KDTreeLambdaNode(T data)
            {
                Data = new List<T> { data };
            }
        }
    }
}
