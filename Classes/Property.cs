using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Property : Estate
    {
        public List<Estate>? References { get; set; } = new();

        public Property(long id, GPS leftBottom, GPS rightTop, int propertyNumber, string propertyDescription)
        {
            Id = id;
            LeftBottom = leftBottom;
            RightTop = rightTop;
            Description = propertyDescription;
            Number = propertyNumber;
        }

        public override string? GetDescription()
        {
            return Description;
        }

        public override int? GetNumber()
        {
            return Number;
        }

        public override void SetDescription(string description)
        {
            Description = description;
        }

        public override void SetNumber(int number)
        {
            Number = number;
        }

        public override void AddReference(ref Estate estate)
        {
            References.Add(estate);
        }
        public override void RemoveReference(ref Estate estate)
        {
            References.Remove(estate);
        }

        public bool Equals(Property other)
        {
            return RightTop.Height.Equals(other.RightTop.Height)
                    && RightTop.Width.Equals(other.RightTop.Width)
                    && LeftBottom.Height.Equals(other.LeftBottom.Height)
                    && LeftBottom.Width.Equals(other.LeftBottom.Width)
                    && Description.Equals(other.Description)
                    && Number.Equals(other.Number)
                    && Id == other.Id;
        }
    }
}
