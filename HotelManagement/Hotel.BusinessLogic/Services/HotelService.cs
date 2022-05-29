using Hotel.DataObject;
using Hotel.DataObject.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Hotel.BusinessLogic.Services
{
    public class HotelService
    {

        private readonly ILogger logger;
        private readonly AppSettings appSettings;

        public HotelService(ILogger<HotelService> logger,
                            IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            this.appSettings = appSettings.Value;
        }

        public bool Run(string inputPath, out string outputPath, out string msg)
        {
            msg = "File is not exists or invalid, please use .txt file";
            outputPath = string.Empty;
            var isSuccess = false;
            var index = 1;
            var sb = new StringBuilder();
            var numberValidator = new Validator(appSettings.Regexs.Number);
            var roomValidator = new Validator(appSettings.Regexs.Room);
            var nameValidator = new Validator(appSettings.Regexs.Name);
            try
            {
                LogContext.PushProperty("REQID", Guid.NewGuid().ToString("N"));
                var fileInfo = new FileInfo(inputPath);
                if (fileInfo.Exists && Path.GetExtension(fileInfo.Name).ToLower().Equals(".txt"))
                {
                    using (var sr = new StreamReader(fileInfo.FullName))
                    {
                        var hotel = new HotelCreator(new DefaultHotel());
                        while (!sr.EndOfStream)
                        {
                            var action = sr.ReadLine();
                            var actionList = action.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (actionList[0].Equals(appSettings.Actions.CreateHotel))
                            {                          
                                if (numberValidator.Validate(actionList[1]).IsValid && numberValidator.Validate(actionList[2]).IsValid)
                                {
                                    hotel.CreateHotel(Convert.ToInt16(actionList[1]), Convert.ToInt16(actionList[2]));
                                    sb.AppendLine($"Hotel created with {actionList[1]} floor(s), {actionList[2]} room(s) per floor.");
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.Book))
                            {
                                if (roomValidator.Validate(actionList[1]).IsValid && nameValidator.Validate(actionList[2]).IsValid && numberValidator.Validate(actionList[3]).IsValid)
                                {
                                    if (hotel.Book(actionList[1], actionList[2], Convert.ToInt16(actionList[3]), out int keycardNumber, out string guestName))
                                    {
                                        sb.AppendLine($"Room {actionList[1]} is booked by {actionList[2]} with keycard number {keycardNumber}.");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"Cannot book room {actionList[1]} for {actionList[2]}, The room is currently booked by {guestName}.");
                                    }
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.ListAvailableRooms))
                            {
                                sb.AppendLine($"{String.Join(", ", hotel.ListAvailableRooms().ToArray())}");
                            }
                            else if (actionList[0].Equals(appSettings.Actions.Checkout))
                            {
                                if (numberValidator.Validate(actionList[1]).IsValid && nameValidator.Validate(actionList[2]).IsValid)
                                {
                                    if (hotel.CheckOut(Convert.ToInt16(actionList[1]), actionList[2], out string roomNumber, out string gestName, out int guestKeyCardNumber))
                                    {
                                        sb.AppendLine($"Room {roomNumber} is checkout.");
                                    }
                                    else if (guestKeyCardNumber == 0)
                                    {
                                        sb.AppendLine($"Cannot book room {roomNumber} for {actionList[2]}, The room is currently booked by {gestName}.");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"Only {gestName} can checkout with keycard number {guestKeyCardNumber}.");
                                    }
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.ListGuest))
                            {
                                sb.AppendLine($"{String.Join(", ", hotel.ListGuests().ToArray())}");
                            }
                            else if (actionList[0].Equals(appSettings.Actions.GetGuestInRoom))
                            {
                                if (roomValidator.Validate(actionList[1]).IsValid)
                                {
                                    sb.AppendLine($"{hotel.GetGuestInRoom(actionList[1])}");
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.ListGuestByAge))
                            {
                                if (numberValidator.Validate(actionList[2]).IsValid)
                                {
                                    sb.AppendLine($"{String.Join(", ", hotel.ListGuestsByAge(actionList[1], Convert.ToInt16(actionList[2])).ToArray())}");
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.ListGuestByFloor))
                            {
                                if (numberValidator.Validate(actionList[1]).IsValid)
                                {
                                    sb.AppendLine($"{String.Join(", ", hotel.ListGuestsByFloor(Convert.ToInt16(actionList[1])).ToArray())}");
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.CheckoutGuestByFloor))
                            {
                                if (numberValidator.Validate(actionList[1]).IsValid)
                                {
                                    if (hotel.CheckOutGuestByFloor(Convert.ToInt16(actionList[1]), out List<string> roomNumbers))
                                    {
                                        sb.AppendLine($"Room {String.Join(", ", roomNumbers.ToArray())} are checkout.");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"Can not checkout guest by floor.");
                                    }
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else if (actionList[0].Equals(appSettings.Actions.BookByFloor))
                            {
                                if (numberValidator.Validate(actionList[1]).IsValid && nameValidator.Validate(actionList[2]).IsValid && numberValidator.Validate(actionList[3]).IsValid )
                                {
                                    if (hotel.BookByFloor(Convert.ToInt16(actionList[1]), actionList[2], Convert.ToInt16(actionList[3]), out List<string> roomNumber, out List<int> keycardNumber))
                                    {
                                        sb.AppendLine($"Room {String.Join(", ", roomNumber.ToArray())} are booked with keycard number {String.Join(", ", keycardNumber.ToArray())}");
                                    }
                                    else
                                    {
                                        sb.AppendLine($"Cannot book floor {actionList[1]} for {actionList[2]}.");
                                    }
                                }
                                else
                                {
                                    msg = $"Invalid arguments at line {index}";
                                    return isSuccess;
                                }
                            }
                            else
                            {
                                sb.AppendLine($"Unsupported action at line {index}");
                                continue;
                            }
                            index++;
                        }
                        isSuccess = true;
                    }
                }
                if (isSuccess)
                {
                    outputPath = Path.Combine(fileInfo.Directory.ToString(), "output.txt");
                    if (!File.Exists(outputPath))
                    {
                        File.Create(outputPath);
                        TextWriter tw = new StreamWriter(outputPath);
                        tw.WriteLine(sb.ToString());
                        tw.Close();
                    }
                    else if (File.Exists(outputPath))
                    {
                        using (var tw = new StreamWriter(outputPath, true))
                        {
                            tw.WriteLine(sb.ToString());
                        }
                    }
                }
                return isSuccess;
            }
            catch (Exception ex)
            {
                logger.LogError($"HotelService: {ex.Message}");
                return isSuccess;
            }
        }
    }
}
