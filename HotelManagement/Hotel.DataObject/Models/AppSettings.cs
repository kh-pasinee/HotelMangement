using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataObject.Models
{
    public class AppSettings
    {
        public Regexs Regexs { get; set; }
        public Actions Actions { get; set; }
    }
    public class Regexs
    {
        public string Room { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
    }
    public class Actions
    {
        public string Book { get; set; }
        public string BookByFloor { get; set; }
        public string Checkout { get; set; }
        public string CheckoutGuestByFloor { get; set; }
        public string CreateHotel { get; set; }
        public string GetGuestInRoom { get; set; }
        public string ListAvailableRooms { get; set; }
        public string ListGuest { get; set; }
        public string ListGuestByAge { get; set; }
        public string ListGuestByFloor { get; set; }
    }
}
