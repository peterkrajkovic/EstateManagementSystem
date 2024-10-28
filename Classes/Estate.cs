using Classes.structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public  abstract class Estate
    {
        public GPS LeftBottom { get; set; }
        public GPS RightTop { get; set; }
        public int? Number { get; set; }
        public string? Description { get; set; }
        public long Id { get; set; }

        public List<Estate>? References { get; set; } = new();

        public abstract Estate Clone();
        public abstract Estate CloneWithoutReferences();
        public abstract void AddReference(Estate estate);
        public abstract void RemoveReference(Estate estate);
    }

    public class KDTreeVisualizationNode
    {
        public List<Estate> Data { get; set; }
        public List<KDTreeVisualizationNode> Children { get; set; } = new();
    }
}
