using System.Collections.Generic;

namespace Hotel.DataObject.Models
{
    public interface IHotelBuilder
    {
        Hotel Hotel { get; set; }
        void CreateHotel(int floor, int room);
        bool Book(string roomNumber, string name, int age, out int keycardNumber, out string guestName);
        List<string> ListAvailableRooms();
        bool CheckOut(int keycardNumber, string name, out string roomNumber, out string guestName, out int guestKeyCardNumber);
        bool CheckOutGuestByFloor(int floor, out List<string> roomNumbers);
        List<string> ListGuests();
        string GetGuestInRoom(string roomNumber);
        List<string> ListGuestsByAge(string logic, int age);
        List<string> ListGuestsByFloor(int floor);
        bool BookByFloor(int floor, string name, int age, out List<string> roomNumbers, out List<int> keycardNumbers);

    }
}