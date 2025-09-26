using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace lab6
{
    public class DatabaseManager
    {
        public string ConnectionString { get; set; }

        public bool IsConnected()
        {
            return !string.IsNullOrEmpty(ConnectionString);
        }

        public List<Apartment> GetApartments()
        {
            var apartments = new List<Apartment>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM Apartments", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        apartments.Add(new Apartment
                        {
                            Id = reader.GetInt32(0),
                            ApartmentNumber = reader.GetString(1),
                            Description = reader.GetString(2)
                        });
                    }
                }
            }
            return apartments;
        }

        public List<Room> GetRoomsWithInfo()
        {
            var rooms = new List<Room>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqliteCommand(
                    "SELECT r.*, ri.Area, ri.Temperature, ri.IsLightOn FROM Rooms r JOIN RoomInfo ri ON r.Id = ri.RoomId",
                    connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rooms.Add(new Room
                        {
                            Id = reader.GetInt32(0),
                            ApartmentId = reader.GetInt32(1),
                            Name = reader.GetString(2),
                            RoomInfo = new RoomInfo
                            {
                                RoomId = reader.GetInt32(0),
                                Area = reader.GetDouble(3),
                                Temperature = reader.GetDouble(4),
                                IsLightOn = reader.GetInt32(5) == 1
                            }
                        });
                    }
                }
            }
            return rooms;
        }

        public void SaveApartments(List<Apartment> apartments)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var apartment in apartments)
                {
                    if (apartment.Id == 0)
                    {
                        var command = new SqliteCommand(
                            "INSERT INTO Apartments (ApartmentNumber, Description) VALUES (@number, @desc)",
                            connection);
                        command.Parameters.AddWithValue("@number", apartment.ApartmentNumber);
                        command.Parameters.AddWithValue("@desc", apartment.Description);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        var command = new SqliteCommand(
                            "UPDATE Apartments SET ApartmentNumber = @number, Description = @desc WHERE Id = @id",
                            connection);
                        command.Parameters.AddWithValue("@number", apartment.ApartmentNumber);
                        command.Parameters.AddWithValue("@desc", apartment.Description);
                        command.Parameters.AddWithValue("@id", apartment.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SaveRooms(List<Room> rooms)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var room in rooms)
                {
                    if (room.Id == 0)
                    {
                        var command = new SqliteCommand(
                            "INSERT INTO Rooms (ApartmentId, Name) VALUES (@aid, @name)",
                            connection);
                        command.Parameters.AddWithValue("@aid", room.ApartmentId);
                        command.Parameters.AddWithValue("@name", room.Name);
                        command.ExecuteNonQuery();

                        command = new SqliteCommand("SELECT last_insert_rowid()", connection);
                        var newId = (long)command.ExecuteScalar();

                        command = new SqliteCommand(
                            "INSERT INTO RoomInfo (RoomId, Area, Temperature, IsLightOn) VALUES (@rid, @area, @temp, @light)",
                            connection);
                        command.Parameters.AddWithValue("@rid", newId);
                        command.Parameters.AddWithValue("@area", room.RoomInfo.Area);
                        command.Parameters.AddWithValue("@temp", room.RoomInfo.Temperature);
                        command.Parameters.AddWithValue("@light", room.RoomInfo.IsLightOn ? 1 : 0);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        var command = new SqliteCommand(
                            "UPDATE Rooms SET ApartmentId = @aid, Name = @name WHERE Id = @id",
                            connection);
                        command.Parameters.AddWithValue("@aid", room.ApartmentId);
                        command.Parameters.AddWithValue("@name", room.Name);
                        command.Parameters.AddWithValue("@id", room.Id);
                        command.ExecuteNonQuery();

                        command = new SqliteCommand(
                            "UPDATE RoomInfo SET Area = @area, Temperature = @temp, IsLightOn = @light WHERE RoomId = @rid",
                            connection);
                        command.Parameters.AddWithValue("@area", room.RoomInfo.Area);
                        command.Parameters.AddWithValue("@temp", room.RoomInfo.Temperature);
                        command.Parameters.AddWithValue("@light", room.RoomInfo.IsLightOn ? 1 : 0);
                        command.Parameters.AddWithValue("@rid", room.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void DeleteApartment(int id)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqliteCommand("DELETE FROM Apartments WHERE Id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }

        public void AddTestData()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
            }
        }

        public void DeleteRoom(int id)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqliteCommand("DELETE FROM Rooms WHERE Id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
