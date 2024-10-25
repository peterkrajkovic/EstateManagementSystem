﻿using System.ComponentModel;
using System.Net;

namespace Classes
{
    public class Parcel : Estate
    {
        
        public List<Estate>? References { get; set; } = new();
        public Parcel()
        {

        }
        public Parcel(long id, GPS leftBottom, GPS rightTop, int parcelNumber, string parcelDescription) 
        {
            Id = id;
            LeftBottom = leftBottom;
            RightTop = rightTop;
            Description = parcelDescription;
            Number = parcelNumber;
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

        public bool Equals(Parcel other)
        {
            return     RightTop.Height.Equals(other.RightTop.Height)
                    && RightTop.Width.Equals(other.RightTop.Width)
                    && LeftBottom.Height.Equals(other.LeftBottom.Height)
                    && LeftBottom.Width.Equals(other.LeftBottom.Width)
                    && Number.Equals(other.Number)
                    && Description.Equals(other.Description)
                    && Id == other.Id;
        }
    }
}
