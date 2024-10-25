using Classes.structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public  abstract class Estate : IComparable<Estate>//, IKDTreeNodeClass<Estate>
    {
        public GPS LeftBottom { get; set; }
        public GPS RightTop { get; set; }
        public int? Number { get; set; }
        public string? Description { get; set; }
        public long Id { get; set; }

        public  abstract string? GetDescription();
        public abstract int? GetNumber();
        public abstract void SetDescription(string description);

        public abstract void SetNumber(int number);



        public int CompareTo(Estate? other)
        {
            return LeftBottom.Width == other.LeftBottom.Width && LeftBottom.Height == other.LeftBottom.Height ? 0 : -1;
        }

        public int CompareTo(Estate other, int level)
        {
            throw new NotImplementedException();
        }

        public abstract void AddReference(ref Estate estate);
        public abstract void RemoveReference(ref Estate estate);

    }
}
