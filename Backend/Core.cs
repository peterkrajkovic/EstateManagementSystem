using Classes;
using Classes.structures;
using System.ComponentModel;

namespace Backend
{
    public class Core
    {
        public static Func<Estate, Estate, int, int> compareFunc = (Estate first, Estate second, int level) =>
            {
                switch (level)
                {
                    case 0:
                        return first.LeftBottom.WidthChar.CompareTo(second.LeftBottom.WidthChar);
                    case 1:
                        return first.LeftBottom.Width.CompareTo(second.LeftBottom.Width);
                    case 2:
                        return first.LeftBottom.HeightChar.CompareTo(second.LeftBottom.HeightChar);
                    case 3:
                        return first.LeftBottom.Height.CompareTo(second.LeftBottom.Height);
                    case 4:
                        return first.RightTop.WidthChar.CompareTo(second.RightTop.WidthChar);
                    case 5:
                        return first.RightTop.Width.CompareTo(second.RightTop.Width);
                    case 6:
                        return first.RightTop.HeightChar.CompareTo(second.RightTop.HeightChar);
                    case 7:
                        return first.RightTop.Height.CompareTo(second.RightTop.Height);
                    default:
                        return 0;

                }
            };

        private KDTree<Estate> AllTree { get; set; } = new KDTree<Estate>(8, compareFunc);
        private KDTree<Estate> ParcelTree { get; set; } = new KDTree<Estate>(8, compareFunc);
        private KDTree<Estate> PropertyTree { get; set; } = new KDTree<Estate>(8, compareFunc);
        private long currentId = 0;


        public bool InsertParcel(string description, int number, double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            Parcel p = new Parcel(currentId, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopWidth, rightTopHeight), number, description);
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
            Property p = new Property(currentId, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopWidth, rightTopHeight), number, description);
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
            AllTree = new KDTree<Estate>(8, compareFunc);
            ParcelTree = new KDTree<Estate>(8, compareFunc);
            PropertyTree = new KDTree<Estate>(8, compareFunc);
            currentId = 0;
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
        //public List<Estate>? FindParcels(double leftBottomHeight, double leftBottomWidth, double rightTopHeight, double rightTopWidth)
        //{
        //    Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
        //    return ParcelTree.Find(p);
        //}
        //public List<Estate>? FindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        //{
        //    Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
        //    return PropertyTree.Find(p);
        //}
        //public List<Estate>? FindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        //{
        //    Parcel p = new Parcel(0, new GPS(leftBottomWidth, leftBottomHeight), new GPS(rightTopHeight, rightTopWidth), 0, "");
        //    return AllTree.Find(p);
        //}
        #endregion

        #region RangeFind
        public List<Estate>? RangeFindParcels(double leftBottomWidth, double leftBottomHeight, double rightTopHeight, double rightTopWidth)
        {
            List<Estate> estates = new();
            char leftWidthChar = leftBottomWidth >= 0 ? 'E' : 'W';
            char leftHeightChar = leftBottomHeight >= 0 ? 'N' : 'S';
            char rightWidthChar = rightTopWidth >= 0 ? 'E' : 'W';
            char rightHeightChar = rightTopHeight >= 0 ? 'N' : 'S';

            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue, 'E', 'N'), new GPS(leftBottomWidth, leftBottomHeight, 'E','N'), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(rightTopWidth, rightTopHeight, 'W','S'), new GPS(double.MaxValue, double.MaxValue, 'W', 'S'), 0, "");

            var newEstates = ParcelTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }


            return estates.Distinct().ToList();
        }
        public List<Estate>? RangeFindProperties(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            List<Estate> estates = new();
            char leftWidthChar = leftBottomWidth >= 0 ? 'E' : 'W';
            char leftHeightChar = leftBottomHeight >= 0 ? 'N' : 'S';
            char rightWidthChar = rightTopWidth >= 0 ? 'E' : 'W';
            char rightHeightChar = rightTopHeight >= 0 ? 'N' : 'S';

            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue, 'E', 'N'), new GPS(leftBottomWidth, leftBottomHeight, 'E','N'), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(rightTopWidth, rightTopHeight, 'W', 'S'), new GPS(double.MaxValue, double.MaxValue, 'W', 'S'), 0, "");

            var newEstates = PropertyTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            return estates.ToList();
        }
        public List<Estate>? RangeFindAll(double leftBottomWidth, double leftBottomHeight, double rightTopWidth, double rightTopHeight)
        {
            List<Estate> estates = new();
            char leftWidthChar = leftBottomWidth >= 0 ? 'E' : 'W';
            char leftHeightChar = leftBottomHeight >= 0 ? 'N' : 'S';
            char rightWidthChar = rightTopWidth >= 0 ? 'E' : 'W';
            char rightHeightChar = rightTopHeight >= 0 ? 'N' : 'S';

            Estate lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue, 'E', 'N'), new GPS(leftBottomWidth, leftBottomHeight, 'E', 'N'), 0, "");
            Estate higherParcel = new Parcel(0, new GPS(rightTopWidth, rightTopHeight, 'W','S'), new GPS(double.MaxValue, double.MaxValue, 'W', 'S'), 0, "");
            var newEstates = AllTree.RangeFind(lowerParcel, higherParcel);
            if (newEstates != null && newEstates.Count() > 0)
            {
                foreach (var item in newEstates)
                {
                    estates.Add(item);
                }
            }

            return estates.ToList();
        }
        public List<Estate>? RangeFindParcels(double width, double height)
        {
            char widthChar = width >= 0 ? 'E' : 'W';
            char heightChar = height >= 0 ? 'N' : 'S';
            Parcel lowerParcel = new Parcel(0, new GPS(double.MinValue, double.MinValue, 'E', 'N'), new GPS(width, height, 'E','N'), 0, "");
            Parcel higherParcel = new Parcel(0, new GPS(width, height, 'W','S'), new GPS(double.MaxValue, double.MaxValue, 'W', 'S'), 0, "");


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
            char widthChar = width >= 0 ? 'E' : 'W';
            char heightChar = height >= 0 ? 'N' : 'S';
            Property lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue,'E', 'N'), new GPS(width, height,'E','N'), 0, "");
            Property higherProperty = new Property(0, new GPS(width, height, 'W','S'), new GPS(double.MaxValue, double.MaxValue,'W','S'), 0, "");

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
            char widthChar = width >= 0 ? 'E' : 'W';
            char heightChar = height >= 0 ? 'N' : 'S';
            Property lowerProperty = new Property(0, new GPS(double.MinValue, double.MinValue, 'E', 'N'), new GPS(width, height, 'E','N'), 0, "");
            Property higherProperty = new Property(0, new GPS(width, height, 'W','S'), new GPS(double.MaxValue, double.MaxValue, 'W', 'S'), 0, "");

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
        public string SaveFile()
        {
            return FileHandler.SaveFile(ParcelTree, PropertyTree);
        }

        public bool LoadFile(string csvFile)
        {
            DeleteTrees();
            bool readingParcels = true;
            using (StringReader reader = new StringReader(csvFile))
            {
                string line;
                bool firstLine = true;
                while ((line = reader.ReadLine()) != null && line != string.Empty)
                {

                    if (line.Contains("null")) //handle null values
                    {
                        if (readingParcels)
                        {
                            ParcelTree = new KDTree<Estate>(4, compareFunc);
                            readingParcels = false;
                        }
                        else
                        {
                            PropertyTree = new KDTree<Estate>(4, compareFunc);
                            return true;
                        }
                    }
                    else if (line.Contains("LeftBottom")) //handle headers
                    {
                        if (firstLine)
                        {
                            firstLine = false;
                        }
                        else
                        {
                            readingParcels = false;
                        }
                    }
                    else //handle estate 
                    {
                        var estateData = FileHandler.ParseCsvLineToEstate(line);
                        if (estateData != null && estateData.HasValue)
                        {
                            var (number, description, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight) = estateData.Value;

                            if (readingParcels)
                            {
                                InsertParcel(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
                            }
                            else
                            {
                                InsertProperty(description, number, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
                            }
                        }
                        else
                        {
                            //incorrect line
                            DeleteTrees();
                            return false;
                        }
                    }
                }

                return true;
            }
        }
        #endregion

        #region Loader
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parcelCount"></param>
        /// <param name="propertyCount"></param>
        /// <param name="coverage"> how many estates should overlap 0-100</param>
        public void LoadRandomTrees(int parcelCount, int propertyCount, double coverage)
        {
            DeleteTrees();

            List<Estate> usedProperties = new();
            List<Estate> usedParcels = new();
            Random random = new Random();
            long currentId = 0;

            if (parcelCount < propertyCount)
            {

                for (int i = 0; i < parcelCount; i++)
                {
                    GPS[] gps = Generator.GenerateRandomGPSLocation();
                    CreateParcel(currentId, gps, usedProperties, usedParcels, 0);
                }
                for (int i = 0; i < propertyCount; i++)
                {
                    GPS[] gps = Generator.GenerateRandomGPSLocation();
                    CreateProperty(currentId, gps, usedProperties, usedParcels, coverage);
                }
            } else
            {
                for (int i = 0; i < propertyCount; i++)
                {
                    GPS[] gps = Generator.GenerateRandomGPSLocation();
                    CreateProperty(currentId, gps, usedProperties, usedParcels, 0);
                }
                for (int i = 0; i < parcelCount; i++)
                {
                    GPS[] gps = Generator.GenerateRandomGPSLocation();
                    CreateParcel(currentId, gps, usedProperties, usedParcels, coverage);
                }
            }
        }

        public Parcel CreateParcel(long currentId, GPS[] gps, List<Estate>? usedProperties, List<Estate>? usedParcels, double coverage)
        {
            if (coverage > 0)
            {
                UpdateGPS(gps, usedProperties, coverage);
            }

            Random random = new Random();
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
            if (usedParcels != null)
            {
                usedParcels.Add(p);
            }

            currentId++;
            return p;
        }

        private Property CreateProperty(long currentId, GPS[] gps, List<Estate> usedProperties, List<Estate> usedParcels, double coverage)
        {
            if (coverage > 0)
            {
                UpdateGPS(gps, usedParcels, coverage);
            }

            Random random = new Random();
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
            if (usedProperties != null)
            {
                usedProperties.Add(p);
            }

            currentId++;
            return p;
        }


        private void UpdateGPS(GPS[] gps, List<Estate>? usedEstatesOthers, double coverage)
        {
            Random random = new Random();
            double doubleCoverage = coverage / 100;

            if (usedEstatesOthers != null && random.NextDouble() < doubleCoverage && usedEstatesOthers.Count > 0)
            {
                int pos = random.Next(0, usedEstatesOthers.Count);
                gps[0] = new GPS(usedEstatesOthers[pos].LeftBottom);
                gps[1] = new GPS(usedEstatesOthers[pos].RightTop);
            }
        }
        #endregion
    }
}

