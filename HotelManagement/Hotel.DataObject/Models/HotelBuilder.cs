using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hotel.DataObject.Models
{
    public abstract class HotelBuilder : IHotelBuilder
    {
        public Hotel Hotel { get; set; }

        public bool BookByFloor(int floor, string name, int age, out List<string> roomNumbers, out List<int> keycardNumbers)
        {
            roomNumbers = new List<string>();
            keycardNumbers = new List<int>();
            var isWholeFloorAvailable = Hotel.AvailableRoom.FindAll(room => int.Parse(room.RoomNumber[0].ToString()) == floor).Count() == Hotel.TotalRoom;
            if (isWholeFloorAvailable)
            {
                foreach (var room in (Hotel.AvailableRoom.OrderBy(room => room.RoomNumber)).ToList())
                {
                    if (int.Parse(room.RoomNumber[0].ToString()) == floor)
                    {
                        roomNumbers.Add(room.RoomNumber);
                        Book(room.RoomNumber, name, age, out int keycardNumber, out string guestName);
                        keycardNumbers.Add(keycardNumber);
                    }
                }
            }
            return !Hotel.AvailableRoom.Where(room => int.Parse(room.RoomNumber[0].ToString()) == floor).Any();
        }

        public bool Book(string roomNumber, string name, int age, out int keycardNumber, out string guestName)
        {
            keycardNumber = 0;
            guestName = string.Empty;
            Hotel.AvailableKeyCard.Sort();
            var isBooked = false;
            foreach (var _ in Hotel.AvailableRoom.Where(room => room.RoomNumber == roomNumber).Select(room => new { }))
            {
                var bookRoom = new Room()
                {
                    RoomNumber = roomNumber,
                    Guests = new Guest()
                    {
                        Name = name,
                        Age = age,
                        KeyCardNumber = Hotel.AvailableKeyCard.First()
                    }
                };
                keycardNumber = bookRoom.Guests.KeyCardNumber;
                Hotel.AvailableRoom.RemoveAll(x => x.RoomNumber == roomNumber);
                Hotel.UnavailableRoom.Add(bookRoom);
                Hotel.AvailableKeyCard.RemoveAll(x => x == bookRoom.Guests.KeyCardNumber);
                Hotel.UnavailableKeyCard.Add(bookRoom.Guests.KeyCardNumber);
                return !isBooked;
            }

            foreach (var room in Hotel.UnavailableRoom.Where(room => room.RoomNumber == roomNumber))
            {
                guestName = room.Guests.Name;
                keycardNumber = room.Guests.KeyCardNumber;
                return isBooked;
            }

            return isBooked;
        }

        public bool CheckOut(int keycardNumber, string name, out string roomNumber, out string guestName, out int guestKeyCardNumber)
        {
            var isCheckOut = false;
            guestName = string.Empty;
            roomNumber = string.Empty;
            guestKeyCardNumber = 0;
            foreach (var room in Hotel.UnavailableRoom)
            {
                if (room.Guests.KeyCardNumber == keycardNumber && room.Guests.Name == name)
                {
                    roomNumber = room.RoomNumber;
                    Hotel.UnavailableRoom.RemoveAll(x => x.RoomNumber == room.RoomNumber);
                    Hotel.UnavailableKeyCard.RemoveAll(x => x == keycardNumber);
                    Hotel.AvailableKeyCard.Add(keycardNumber);
                    Hotel.AvailableKeyCard.Sort();
                    Hotel.AvailableRoom.Add(new Room()
                    {
                        RoomNumber = room.RoomNumber,
                        Guests = new Guest(),
                    });
                    return !isCheckOut;
                }
                else if (room.Guests.KeyCardNumber == keycardNumber)
                {
                    guestName = room.Guests.Name;
                    guestKeyCardNumber = room.Guests.KeyCardNumber;
                    return isCheckOut;
                }
            }
            return isCheckOut;
        }

        public bool CheckOutGuestByFloor(int floor, out List<string> roomNumbers)
        {
            roomNumbers = new List<string>();
            foreach (var room in Hotel.UnavailableRoom.ToList())
            {
                if (int.Parse(room.RoomNumber[0].ToString()) == floor)
                {
                    CheckOut(room.Guests.KeyCardNumber, room.Guests.Name, out string roomNumber, out string gn, out int gk);
                    roomNumbers.Add(roomNumber);
                }
            }
            return !Hotel.UnavailableRoom.Where(room => int.Parse(room.RoomNumber[0].ToString()) == floor).Any();
        }

        public void CreateHotel(int floor, int room)
        {
            int keycardNumber = 1;
            Hotel = new Hotel();
            Hotel.TotalRoom = room;
            Hotel.TotalFloor = floor;
            Hotel.TotalKeyCard = floor * room;
            for (int floorNumber = 1; floorNumber <= Hotel.TotalFloor; floorNumber++)
            {
                for (int roomNumber = 1; roomNumber <= Hotel.TotalRoom; roomNumber++)
                {
                    Hotel.AvailableRoom.Add(new Room() { RoomNumber = $"{floorNumber}{roomNumber:00}" });
                    Hotel.AvailableKeyCard.Add(keycardNumber);
                    keycardNumber++;
                }
            }

        }

        public string GetGuestInRoom(string roomNumber)
        {
            return (Hotel.UnavailableRoom.Where(room => room.RoomNumber == roomNumber).Select(room => room.Guests.Name)).ToList().First();
        }

        public List<string> ListAvailableRooms()
        {
            return (Hotel.AvailableRoom.Select(room => room.RoomNumber)).ToList();
        }

        public List<string> ListGuests()
        {
            return (Hotel.UnavailableRoom.Select(room => room.Guests.Name)).ToList();
        }

        public List<string> ListGuestsByAge(string logic, int age)
        {
            return (Hotel.UnavailableRoom.Where(room => Operator(logic, room.Guests.Age, age)).Select(room => room.Guests.Name)).ToList();
        }
        private static Boolean Operator(string logic, int x, int y)
        {
            switch (logic)
            {
                case ">": return x > y;
                case ">=": return x >= y;
                case "<": return x < y;
                case "<=": return x <= y;
                case "==": return x == y;
                case "!=": return x != y;
                default: throw new Exception("invalid logic");
            }
        }

        public List<string> ListGuestsByFloor(int floor)
        {
            return (Hotel.UnavailableRoom.Where(room => int.Parse(room.RoomNumber[0].ToString()) == floor).Select(room => room.Guests.Name)).ToList();
        }
    }
}
