using System.Net;

namespace Classes
{
    public class Parcel
    {
        public int ParcelNumber {  get; set; }  
        public string ParcelDescription { get; set; }
        public GPS LeftBottom {  get; set; }
        public GPS RightTop { get; set; }
    }
}
