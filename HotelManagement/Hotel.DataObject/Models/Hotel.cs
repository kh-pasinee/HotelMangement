using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hotel.DataObject.Models
{
    public class Hotel
    {
        public int TotalRoom { get; set; } = 0;
        public int TotalFloor { get; set; } = 0;
        public int TotalKeyCard { get; set; } = 0;

        public List<int> AvailableKeyCard = new List<int>();
        public List<int> UnavailableKeyCard = new List<int>();

        public List<Room> AvailableRoom = new List<Room>();
        public List<Room> UnavailableRoom = new List<Room>();

        public override string ToString()
        {
            var model = this.MemberwiseClone();
            return JsonConvert.SerializeObject(model);
        }
    }

    public class Floor
    { 
        public List<Room> Rooms { get; set; } = new List<Room>();
    }

    public class Room
    {
        public string RoomNumber { get; set; } = string.Empty;
        public Guest Guests { get; set; } = new Guest();
    }

    public class Guest
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public int KeyCardNumber { get; set; } = 0;
    }

}
