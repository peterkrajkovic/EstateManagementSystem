using Classes;
using Classes.structures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    public class FileHandler
    {
        #region Load
        public static (int Number, string Description, double LeftBottomWidth, double LeftBottomHeight, double RightTopWidth, double RightTopHeight)? ParseCsvLineToEstate(string csvLine)
        {
            var columns = csvLine.Split(';');
            if (columns.Length != 6)
                return null;

            try
            {
                // Parse GPS for LeftBottom
                double leftBottomWidth = double.TryParse(columns[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lbWidth) ? lbWidth : 0;
                double leftBottomHeight = double.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lbHeight) ? lbHeight : 0;

                // Parse GPS for RightTop
                double rightTopWidth = double.TryParse(columns[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double rtWidth) ? rtWidth : 0;
                double rightTopHeight = double.TryParse(columns[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double rtHeight) ? rtHeight : 0;

                // Parse Number and Description
                int number = int.TryParse(columns[4], out int parsedNumber) ? parsedNumber : 0;
                string description = columns[5].Replace('ʘ', ';'); // Convert back from placeholder

                return (number, description, leftBottomWidth, leftBottomHeight, rightTopWidth, rightTopHeight);
            }
            catch
            {
                return null; // Return null if any exception occurs
            }
        }
        #endregion

        #region Save
        public static string SaveFile(KDTree<Estate> parcels, KDTree<Estate> properties)
        {

            return (parcels == null ? "null\n" :SaveTreeToString(parcels)) + (properties == null ? "null\n" : SaveTreeToString(properties));
        }
        public static string SaveTreeToString(KDTree<Estate> tree)
        {
            StringBuilder csvBuilder = new StringBuilder();

            // Write CSV header
            csvBuilder.AppendLine("LeftBottomWidth;LeftBottomHeight;RightTopWidth;RightTopHeight;Number;Description");

            foreach (var estate in tree.LevelOrderTraversal())
            {
                csvBuilder.AppendLine(ConvertEstateToCsv(estate));
            }

            return csvBuilder.ToString();
        }

        private static string ConvertEstateToCsv(Estate estate)
        {
            string leftBottom = $"{estate.LeftBottom?.Width};{estate.LeftBottom?.Height}";
            string rightTop = $"{estate.RightTop?.Width};{estate.RightTop?.Height}";
            string number = estate.Number?.ToString() ?? "";
            string description = estate.Description?.Replace(';', 'ʘ') ?? "";

            return $"{leftBottom};{rightTop};{number};{description}";
        }
        #endregion
    }
}
