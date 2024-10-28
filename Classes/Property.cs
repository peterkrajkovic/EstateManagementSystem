using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Property : Estate
    {

        public Property(long id, GPS leftBottom, GPS rightTop, int propertyNumber, string propertyDescription)
        {
            Id = id;
            LeftBottom = leftBottom;
            RightTop = rightTop;
            Description = propertyDescription;
            Number = propertyNumber;
            References = new();
        }
        public override Estate CloneWithoutReferences()
        {
            var copy = (Property)this.MemberwiseClone();
            copy.References = null;
            return copy;
        }
        public override Estate Clone()
        {
            var copy = (Property)this.MemberwiseClone();
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
                }
            }
            for (int i = 0; i < refsToRemove.Count; i++)
            {
                References.Remove(refsToRemove[i]);
            }
        }

        public override bool Equals(Object? other)
        {
            return  other is Property && other != null
                    && RightTop.Height.Equals(((Property)other).RightTop.Height)
                    && RightTop.Width.Equals(((Property)other).RightTop.Width)
                    && LeftBottom.Height.Equals(((Property)other).LeftBottom.Height)
                    && LeftBottom.Width.Equals(((Property)other).LeftBottom.Width)
                    && Description.Equals(((Property)other).Description)
                    && Number.Equals(((Property)other).Number)
                    && Id == ((Property)other).Id;
        }
    }
}
