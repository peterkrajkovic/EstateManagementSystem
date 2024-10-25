using Classes;
using static Backend.Core;
using Backend;
using Classes.structures;
namespace GUI.Components.Client

{
    public class ClientClass
    {

        public Core Core { get; set; } = new Core();
        public bool InsertParcel(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.InsertParcel(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }
        public void LoadRandomTrees(int parcels, int properties, int coverage)
        {
            Core.LoadRandomTrees(parcels,properties,coverage);
        }
        public bool InsertProperty(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.InsertProperty(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        public void RemoveEstate(Estate p)
        {
            Core.RemoveEstate(p);
        }

        public bool RemoveProperty(Property p)
        {
            return Core.RemoveProperty(p);
        }

        public List<Estate>? FindParcels(double leftBottomHeight, double leftBottomWidth, double rightTopHeight, double rightTopWidth)
        {
            return Core.FindParcels(leftBottomHeight, leftBottomWidth, rightTopHeight, rightTopWidth);
        }

        public List<Estate>? FindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.FindProperties(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        public List<Estate>? FindParcels(double width, double height)
        {
            return Core.RangeFindParcels(width, height);
        }

        public List<Estate>? FindProperties(double width, double height)
        {
            return Core.RangeFindProperties(width, height);
        }

        public List<Estate>? FindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.FindAll(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        public List<Estate>? FindAll(double width, double height)
        {
            return Core.RangeFindAll(width, height);
        }

        public KDTreeVisualizationNode<Estate>? VisualizeAll()
        {
            return Core.VisualizeAll();
        }
        public KDTreeVisualizationNode<Estate>? VisualizeParcels()
        {
            return Core.VisualizeParcels();
        }
        public KDTreeVisualizationNode<Estate>? VisualizeProperties()
        {
            return Core.VisualizeProperties();
        }
        public void  DeleteTrees()
        {
            Core.DeleteTrees();
        }
    }
}
