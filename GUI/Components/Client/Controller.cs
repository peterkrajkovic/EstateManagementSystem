using Classes;
using static Backend.Core;
using Backend;
using Classes.structures;
namespace GUI.Components.Client

{
    public class Controller
    {

        public Core Core { get; set; } = new Core();

        #region Insert
        public bool InsertParcel(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.InsertParcel(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }
        public void LoadRandomTrees(int parcels, int properties, double coverage)
        {
            Core.LoadRandomTrees(parcels,properties,coverage);
        }
        public bool InsertProperty(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.InsertProperty(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        #endregion

        #region Remove
        public void RemoveEstate(Estate p)
        {
            Core.RemoveEstate(p);
        }

        public void RemoveProperty(Property p)
        {
            Core.RemoveProperty(p);
        }
        #endregion

        #region RangeFind
        public List<Estate>? RangeFindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.RangeFindProperties(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }
        public List<Estate>? RangeFindParcels(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.RangeFindParcels(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        public List<Estate>? RangeFindParcels(double width, double height)
        {
            return Core.RangeFindParcels(width, height);
        }

        public List<Estate>? FindProperties(double width, double height)
        {
            return Core.RangeFindProperties(width, height);
        }

        public List<Estate>? RangeFindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.RangeFindAll(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }

        public List<Estate>? RangeFindAll(double width, double height)
        {
            return Core.RangeFindAll(width, height);
        }
        #endregion

        #region Visualization
        public KDTreeVisualizationNode? VisualizeAll()
        {
            return Core.VisualizeAll();
        }
        public KDTreeVisualizationNode? VisualizeParcels()
        {
            return Core.VisualizeParcels();
        }
        public KDTreeVisualizationNode? VisualizeProperties()
        {
            return Core.VisualizeProperties();
        }
        #endregion


        #region Files
        public bool LoadFile(string file)
        {
            return Core.LoadFile(file);
        }

        public string SaveFile()
        {
            return Core.SaveFile();
        }
        #endregion

        public void  DeleteTrees()
        {
            Core.DeleteTrees();
        }
        public bool EditEstate(Estate oldEstate, string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return Core.EditEstate(oldEstate, description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
        }
    }
}
