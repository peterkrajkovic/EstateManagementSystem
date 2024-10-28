using Classes;
using Classes.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class Visualization
    {

        public static KDTreeVisualizationNode? CreateVisualizationTree( KDTreeLambda<Estate>.KDTreeLambdaNode<Estate> root)
        {
            if (root == null)
            {
                return null;
            }

            // Stack to hold nodes for iterative traversal
            var stack = new Stack<(KDTreeLambda<Estate>.KDTreeLambdaNode<Estate> originalNode, KDTreeVisualizationNode visualNode)>();
            var visualizationRoot = new KDTreeVisualizationNode
            {
                Data = root.Data
            };

            stack.Push((root, visualizationRoot));

            while (stack.Count > 0)
            {
                var (originalNode, visualNode) = stack.Pop();

                // Process the left child
                if (originalNode.Left != null)
                {
                    var leftVisualNode = new KDTreeVisualizationNode
                    {
                        Data = new()
                    };
                    foreach(var item in originalNode.Left.Data)
                    {
                        leftVisualNode.Data.Add(item.Clone());
                    }
                    visualNode.Children.Add(leftVisualNode);
                    stack.Push((originalNode.Left, leftVisualNode));
                }

                // Process the right child
                if (originalNode.Right != null)
                {
                    var rightVisualNode = new KDTreeVisualizationNode
                    {
                        Data = new()
                    };
                    foreach (var item in originalNode.Right.Data)
                    {
                        rightVisualNode.Data.Add(item.Clone());
                    }
                    visualNode.Children.Add(rightVisualNode);
                    stack.Push((originalNode.Right, rightVisualNode));
                }
            }

            return visualizationRoot;
        }
    }
}
