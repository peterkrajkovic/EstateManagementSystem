using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Estate
    {
        public int EstateNumber { get; set; }
        public string EstateDescription { get; set; }
        public GPS LeftBottom { get; set; }
        public GPS RightTop { get; set; }

    }
}
