using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataObject.Models
{
    public class HotelCreator
    {
        private IHotelBuilder builder;

        public HotelCreator(IHotelBuilder builder)
        {
            this.builder = builder;
        }

        public void CreateHotel(int floor, int room)
        {
            builder.CreateHotel(floor, room);
        }

        public Hotel GetHotel()
        {
            return builder.Hotel;
        }   

        public bool Book(string roomNumber, string name, int age, out int keycardNumber, out string guestName)
        {
            return builder.Book(roomNumber, name, age, out keycardNumber, out guestName);
        }

        public List<string> ListAvailableRooms()
        {
            return builder.ListAvailableRooms();
        }
        public bool CheckOut(int keycardNumber, string name, out string roomNumber, out string guestName, out int guestKeyCardNumber)
        {
            return builder.CheckOut(keycardNumber, name, out roomNumber, out guestName, out guestKeyCardNumber);
        }
        public bool CheckOutGuestByFloor(int floor, out List<string> roomNumbers)
        {
            return builder.CheckOutGuestByFloor(floor, out roomNumbers);
        }
        public List<string> ListGuests()
        {
            return builder.ListGuests();
        }
        public string GetGuestInRoom(string roomNumber)
        {
            return builder.GetGuestInRoom(roomNumber);
        }
        public List<string> ListGuestsByAge(string logic, int age)
        { 
            return builder.ListGuestsByAge(logic, age); 
        }
        public List<string> ListGuestsByFloor(int floor)
        { 
            return builder.ListGuestsByFloor(floor); 
        }
        public bool BookByFloor(int floor, string name, int age, out List<string> roomNumbers, out List<int> keycardNumbers)
        { 
            return builder.BookByFloor(floor, name, age, out roomNumbers, out keycardNumbers); 
        }


    }
}
