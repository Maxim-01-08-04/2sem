using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab6
{
    public class Room
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string Name { get; set; }
        public RoomInfo RoomInfo { get; set; }
    }

    public class RoomInfo
    {
        public int RoomId { get; set; }
        public double Area { get; set; }
        public double Temperature { get; set; }
        public bool IsLightOn { get; set; }
    }

    public class Apartment
    {
        public int Id { get; set; }
        public string ApartmentNumber { get; set; }
        public string Description { get; set; }
    }

}
