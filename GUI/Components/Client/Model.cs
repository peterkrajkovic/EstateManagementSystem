using Classes.structures;
using Classes;

namespace GUI.Components.Client
{
    public class Model
    {
        public KDTreeVisualizationNode? VisualizationNodeAll { get; set; }
        public KDTreeVisualizationNode? VisualizationNodeParcels { get; set; }
        public KDTreeVisualizationNode? VisualizationNodeProperties { get; set; }


        public class EstateModel
        {
            public double X1 { get; set; }
            public double X2 { get; set; }
            public double Y1 { get; set; }
            public double Y2 { get; set; }
            public string Description { get; set; }
            public int Number { get; set; }
            public int EstateType { get; set; }
        }
    }
}
