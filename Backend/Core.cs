using Classes;
using Classes.structures;
using System.ComponentModel;

namespace Backend
{
    public class Core
    {
        private static Func<Estate, Estate, int, int> compareFunc = (Estate first, Estate second, int level) =>
            {
                switch (level)
                {
                    case 0:
                        return first.LeftBottom.Width.CompareTo(second.LeftBottom.Width);
                    case 1:
                        return first.LeftBottom.Height.CompareTo(second.LeftBottom.Height);
                    case 2:
                        return first.RightTop.Width.CompareTo(second.RightTop.Width);
                    case 3:
                        return first.RightTop.Height.CompareTo(second.RightTop.Height);
                    default:
                        return 0;

                }
            };

        private KDTreeLambda<Estate> AllTree { get; set; } = new KDTreeLambda<Estate>(4, compareFunc);
        private KDTreeLambda<Estate> ParcelTree { get; set; } = new KDTreeLambda<Estate>(4, compareFunc);
        private KDTreeLambda<Estate> PropertyTree { get; set; } = new KDTreeLambda<Estate>(4, compareFunc);
        private long currentId = 0;


        public bool InsertParcel(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Parcel p = new Parcel(currentId, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), number, description);
            currentId++;
            ParcelTree.Insert(p);
            AllTree.Insert(p);
            p.References = RangeFindProperties(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
            return true;
        }

        public bool InsertProperty(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Property p = new Property(currentId, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), number, description);
            currentId++;
            PropertyTree.Insert(p);
            AllTree.Insert(p);
            p.References = RangeFindParcels(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
            return true;
        }

        #region Remove
        public bool RemoveParcel(Parcel p)
        {
            AllTree.Remove(p);
            ParcelTree.Remove(p);
            return true;
        }
        public bool RemoveProperty(Property p)
        {
            AllTree.Remove(p);
            ParcelTree.Remove(p);
            return true;
        }
        public void RemoveEstate(Estate e)
        {
            if (e is Parcel)
            {
                RemoveParcel((Parcel)e);
            }
            else
            {
                RemoveProperty((Property)e);
            }
        }
        public void DeleteTrees()
        {
            AllTree = new KDTreeLambda<Estate>(4, compareFunc);
            ParcelTree = new KDTreeLambda<Estate>(4, compareFunc);
            PropertyTree = new KDTreeLambda<Estate>(4, compareFunc);
        }
        #endregion

        #region Find
        public List<Estate>? FindParcels(double leftBottomHeight, double leftBottomWidth, double rightTopHeight, double rightTopWidth)
        {
            Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
            return ParcelTree.Find(p);
        }
        public List<Estate>? FindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
            return PropertyTree.Find(p);
        }
        public List<Estate>? FindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
            return AllTree.Find(p);
        }
        #endregion

        #region RangeFind
        public List<Estate>? RangeFindParcels(double leftBottomHeight, double leftBottomWidth, double rightTopHeight, double rightTopWidth)
        {
            List<Estate> estates = new();

            //1.st scenario - estates cross border on right top side of existing estate
            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(leftBottomWidth, leftBottomHeight), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            var newEstates = ParcelTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = ParcelTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            return estates.Distinct().ToList();
        }
        public List<Estate>? RangeFindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            List<Estate> estates = new();

            //1.st scenario - estates cross border on right top side of existing estate
            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(leftBottomWidth, leftBottomHeight), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            var newEstates = PropertyTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = PropertyTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            return estates.Distinct().ToList();
        }
        public List<Estate>? RangeFindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            List<Estate> estates = new();

            //1.st scenario - estates cross border on right top side of existing estate
            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(leftBottomWidth, leftBottomHeight), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            var newEstates = AllTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = AllTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                estates.AddRange(newEstates);
            }

            return estates.Distinct().ToList();
        }
        public List<Estate>? RangeFindParcels(double width, double height)
        {
            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");
            return ParcelTree.RangeFind(lowerParcel, higherParcel);
        }
        public List<Estate>? RangeFindProperties(double width, double height)
        {
            Property lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Property higherProperty = new Property(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");
            return PropertyTree.RangeFind(lowerProperty, higherProperty);
        }
        public List<Estate>? RangeFindAll(double width, double height)
        {
            Estate lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Estate higherProperty = new Property(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");
            return AllTree.RangeFind(lowerProperty, higherProperty);
        }
        #endregion

        #region Visualization
        public KDTreeVisualizationNode<Estate>? VisualizeAll()
        {
            return AllTree.CreateVisualizationTree();
        }
        public KDTreeVisualizationNode<Estate>? VisualizeParcels()
        {
            return ParcelTree.CreateVisualizationTree();
        }
        public KDTreeVisualizationNode<Estate>? VisualizeProperties()
        {
            return PropertyTree.CreateVisualizationTree();
        }
        #endregion

        #region Files

        #endregion

        #region Loader
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parcelCount"></param>
        /// <param name="propertyCount"></param>
        /// <param name="coverage"> which n-item in order uses previously used GPS</param>
        public void LoadRandomTrees(int parcelCount, int propertyCount, int coverage)
        {
            DeleteTrees();

            List<Property> usedProperties = new();
            List<Parcel> usedParcels = new();
            Random random = new Random();
            long currentId = 0;

            while (usedParcels.Count < parcelCount || usedProperties.Count < propertyCount)
            {
                List<GPS> gps = Generator.GenerateRandomGPSLocation();
                currentId++;

                if (usedProperties.Count == propertyCount || (usedParcels.Count < parcelCount && (random.Next(1, 3) < 2)))
                {
                    CreateParcel(ref currentId, gps, usedProperties, usedParcels, random, coverage);
                }
                else
                {
                    CreateProperty(ref currentId, gps, usedProperties, usedParcels, random, coverage);
                }
            }
        }

        private void CreateParcel(ref long currentId, List<GPS> gps, List<Property> usedProperties, List<Parcel> usedParcels, Random random, int coverage)
        {
            UpdateGPSForParcel(gps, usedProperties, usedParcels, random, coverage);
            Parcel parcel = new Parcel(currentId, gps[0], gps[1], random.Next(), Generator.GenerateRandomName(random.Next(4, 10)));
            ParcelTree.Insert(parcel);
            AllTree.Insert(parcel);
            parcel.References = RangeFindProperties(gps[0].Width, gps[0].Height, gps[1].Width, gps[1].Height);
            usedParcels.Add(parcel);
        }

        private void CreateProperty(ref long currentId, List<GPS> gps, List<Property> usedProperties, List<Parcel> usedParcels, Random random, int coverage)
        {
            UpdateGPSForProperty(gps, usedProperties, usedParcels, random, coverage);
            Property property = new Property(currentId, gps[0], gps[1], random.Next(), Generator.GenerateRandomName(random.Next(4, 10)));
            PropertyTree.Insert(property);
            AllTree.Insert(property);
            property.References = RangeFindParcels(gps[0].Width, gps[0].Height, gps[1].Width, gps[1].Height);
            usedProperties.Add(property);
        }

        private void UpdateGPSForParcel(List<GPS> gps, List<Property> usedProperties, List<Parcel> usedParcels, Random random, int coverage)
        {
            if (usedParcels.Count % coverage == 0 && usedProperties.Count > 0)
            {
                int pos = random.Next(0, usedProperties.Count);
                gps[0] = new GPS(usedProperties[pos].LeftBottom);
                gps[1] = new GPS(usedProperties[pos].RightTop);
            }
        }

        private void UpdateGPSForProperty(List<GPS> gps, List<Property> usedProperties, List<Parcel> usedParcels, Random random, int coverage)
        {
            if (usedProperties.Count % coverage == 0 && usedParcels.Count > 0)
            {
                int pos = random.Next(0, usedParcels.Count);
                gps[0] = new GPS(usedParcels[pos].LeftBottom);
                gps[1] = new GPS(usedParcels[pos].RightTop);
            }
        }
        #endregion
    }
}

