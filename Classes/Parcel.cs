using System.ComponentModel;
using System.Net;

namespace Classes
{
    public class Parcel : Estate
    {
        public Parcel()
        {

        }

        public override Estate Clone()
        {
            var copy = (Parcel)this.MemberwiseClone();
            var newRefs = new List<Estate>();
            if (copy.References != null)
            {
                foreach (var x in copy.References)
                {
                    newRefs.Add(x.CloneWithoutReferences());
                }
            }
            return copy;
        }

        public override Estate CloneWithoutReferences()
        {
            var copy = (Parcel)this.MemberwiseClone();
            copy.References = null;
            return copy;
        }

        public Parcel(long id, GPS leftBottom, GPS rightTop, int parcelNumber, string parcelDescription) 
        {
            Id = id;
            LeftBottom = leftBottom;
            RightTop = rightTop;
            Description = parcelDescription;
            Number = parcelNumber;
            References = [];
        }

      

        public override void AddReference(Estate estate)
        {
            if (References == null) References = new List<Estate>();
            References.Add(estate);
        }

        public override void RemoveReference(Estate estate)
        {
            var refsToRemove = new List<Estate>();
            for (int i = 0; i < References.Count; i++)
            {
                if (References[i].Equals(estate))
                {
                    refsToRemove.Add(estate);
                    break;
                }
            }
            for (int i = 0; i < refsToRemove.Count; i++)
            {
                References.Remove(refsToRemove[i]);
            }
        }

        public override bool Equals(Object? other)
        {
            return  other is Estate && other != null
                    && RightTop.Height.Equals(((Estate)other).RightTop.Height)
                    && RightTop.Width.Equals(((Estate)other).RightTop.Width)
                    && LeftBottom.Height.Equals(((Estate)other).LeftBottom.Height)
                    && LeftBottom.Width.Equals(((Estate)other).LeftBottom.Width)
                    && Number.Equals(((Estate)other).Number)
                    && Description.Equals(((Estate)other).Description)
                    && Id == ((Estate)other).Id;
        }
    }
}
