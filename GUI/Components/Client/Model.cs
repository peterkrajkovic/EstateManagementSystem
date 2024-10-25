using Classes.structures;
using Classes;

namespace GUI.Components.Client
{
    public class Model
    {
        public KDTreeVisualizationNode<Estate>? VisualizationNodeAll { get; set; }
        public KDTreeVisualizationNode<Estate>? VisualizationNodeParcels { get; set; }
        public KDTreeVisualizationNode<Estate>? VisualizationNodeProperties { get; set; }
    }
}
