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
            if (p.References != null)
            {
                for (int i = 0; i < p.References.Count(); i++)
                {
                    p.References[i].AddReference(p);
                }
            }
            return true;
        }

        public bool InsertProperty(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Property p = new Property(currentId, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), number, description);
            currentId++;
            PropertyTree.Insert(p);
            AllTree.Insert(p);
            p.References = RangeFindParcels(leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
            if (p.References != null)
            {
                for (int i = 0; i < p.References.Count(); i++)
                {
                    p.References[i].AddReference(p);
                }
            }
            return true;
        }

        #region Remove
        public void RemoveParcel(Parcel p)
        {
            AllTree.Remove(p);
            ParcelTree.Remove(p);
            var referenced = RangeFindProperties(p.LeftBottom.Width, p.LeftBottom.Height, p.RightTop.Width, p.RightTop.Height);
            if (referenced == null)
            {
                return;
            }
            for (int i = 0;i < referenced.Count(); i++)
            {
                referenced[i].RemoveReference(p);
            }
        }
        public void RemoveProperty(Property p)
        {
            AllTree.Remove(p);
            PropertyTree.Remove(p);
            var referenced = RangeFindParcels(p.LeftBottom.Width, p.LeftBottom.Height, p.RightTop.Width, p.RightTop.Height);
            if (referenced == null)
            {
                return;
            }
            for (int i = 0; i < referenced.Count(); i++)
            {
                referenced[i].RemoveReference(p);
            }
        }
        public void RemoveEstate(Estate e)
        {
            if (e is Parcel)
            {
                var foundParcel = ParcelTree.Find((Parcel)e);
                if (foundParcel != null && foundParcel.Count() > 0)
                {
                    RemoveParcel((Parcel)foundParcel[0]);
                }
            }
            else
            {
                var foundProperty = PropertyTree.Find((Property)e);
                if (foundProperty != null && foundProperty.Count() > 0)
                {
                    RemoveProperty((Property)foundProperty[0]);
                }
            }
        }
        public void DeleteTrees()
        {
            AllTree = new KDTreeLambda<Estate>(4, compareFunc);
            ParcelTree = new KDTreeLambda<Estate>(4, compareFunc);
            PropertyTree = new KDTreeLambda<Estate>(4, compareFunc);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldParcel"></param>
        /// <param name="newParcel"></param>
        /// <returns>structures changed or not</returns>
        public bool EditEstate(Estate oldEstate, string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            bool gpsChanged = IsGPSChanged(oldEstate, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);

            if (oldEstate is Parcel)
            {

                var result = ParcelTree.Find(oldEstate);
                if (result != null && result.Count() > 0)
                {
                    if (gpsChanged)
                    {
                        //GPS changed, remove estate and reinsert
                        RemoveParcel((Parcel)result[0]);
                        InsertParcel(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
                    }
                    else
                    {
                        result[0].Description = description;
                        result[0].Number = number;
                    }
                }
            }
            else
            {
                var result = PropertyTree.Find(oldEstate);
                if (result != null && result.Count() > 0)
                {
                    if (gpsChanged)
                    {
                        //GPS changed, remove estate and reinsert
                        RemoveProperty((Property)result[0]);
                        InsertProperty(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
                    }
                    else
                    {
                        result[0].Description = description;
                        result[0].Number = number;
                    }
                }
            }
            return gpsChanged;
        }

        private bool IsGPSChanged(Estate oldEstate, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            return !(oldEstate.LeftBottom.Width.Equals(leftBottomWidth) 
                    && oldEstate.LeftBottom.Height.Equals(leftBottomHeight) 
                    && oldEstate.RightTop.Width.Equals(rightTopWidth) 
                    && oldEstate.RightTop.Height.Equals(rightTopHeight));
        }
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
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = ParcelTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach(var item in newEstates)
                {
                    estates.Add(item);
                }
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
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = PropertyTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
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
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            //2.nd scenario estates cross border on left bottom side of existing estate
            Estate lowerParcel2 = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(rightTopWidth, rightTopHeight), 0, "");
            Estate higherParcel2 = new Parcel(0, new GPS(rightTopWidth, rightTopHeight), new GPS(double.MaxValue, double.MaxValue), 0, "");
            newEstates = AllTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            return estates.Distinct().ToList();
        }
        public List<Estate>? RangeFindParcels(double width, double height)
        {
            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");

            var estates = new List<Estate>();
            var newEstates = ParcelTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item.Clone());
                }
            }
            return estates;
        }
        public List<Estate>? RangeFindProperties(double width, double height)
        {
            Property lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Property higherProperty = new Property(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");

            var estates = new List<Estate>();
            var newEstates = PropertyTree.RangeFind(lowerProperty, higherProperty);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item.Clone());
                }
            }
            return estates;
        }
        public List<Estate>? RangeFindAll(double width, double height)
        {
            Estate lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue), new GPS(width, height), 0, "");
            Estate higherProperty = new Property(0, new GPS(width, height), new GPS(double.MaxValue, double.MaxValue), 0, "");

            var estates = new List<Estate>();
            var newEstates = AllTree.RangeFind(lowerProperty, higherProperty);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item.Clone());
                }
            }
            return estates;
        }
        #endregion

        #region Visualization
        public KDTreeVisualizationNode? VisualizeAll()
        {
            return Visualization.CreateVisualizationTree(AllTree.GetRoot());
        }
        public KDTreeVisualizationNode? VisualizeParcels()
        {
            return Visualization.CreateVisualizationTree(ParcelTree.GetRoot());
        }
        public KDTreeVisualizationNode? VisualizeProperties()
        {
            return Visualization.CreateVisualizationTree(PropertyTree.GetRoot());
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
            Parcel p = new Parcel(currentId, gps[0], gps[1], random.Next(1,999999), Generator.GenerateRandomName(random.Next(3, 7)));
            ParcelTree.Insert(p);
            AllTree.Insert(p);
            p.References = RangeFindProperties(gps[0].Width, gps[0].Height, gps[1].Width, gps[1].Height);
            if (p.References != null)
            {
                for (int i = 0; i < p.References.Count(); i++)
                {
                    p.References[i].AddReference(p);
                }
            }
            usedParcels.Add(p);
        }

        private void CreateProperty(ref long currentId, List<GPS> gps, List<Property> usedProperties, List<Parcel> usedParcels, Random random, int coverage)
        {
            UpdateGPSForProperty(gps, usedProperties, usedParcels, random, coverage);
            Property p = new Property(currentId, gps[0], gps[1], random.Next(1,99999), Generator.GenerateRandomName(random.Next(3, 7)));
            PropertyTree.Insert(p);
            AllTree.Insert(p);
            p.References = RangeFindParcels(gps[0].Width, gps[0].Height, gps[1].Width, gps[1].Height);
            if (p.References != null)
            {
                for (int i = 0; i < p.References.Count(); i++)
                {
                    p.References[i].AddReference(p);
                }
            }
            usedProperties.Add(p);
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

