using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ManajemenPerpus.Core.Models;

namespace ManajemenPerpus.CLI.Service
{
    public class NotifikasiService
    {
        private readonly string _jsonFilePath;
        private List<Notifikasi> _notifikasiList;

        public NotifikasiService()
        {
            _jsonFilePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataNotifikasi.json");
            _notifikasiList = new List<Notifikasi>();
            LoadAllData();
        }

        #region Data Loading Methods
        private List<Notifikasi> LoadAllData()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    string json = File.ReadAllText(_jsonFilePath);
                    _notifikasiList = JsonSerializer.Deserialize<List<Notifikasi>>(json) ?? new List<Notifikasi>();
                    return _notifikasiList;
                }

                // Create file if doesn't exist
                SaveAllData(new List<Notifikasi>());
                return new List<Notifikasi>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return new List<Notifikasi>();
            }
        }

        public List<Notifikasi> LoadDataForUser(string idPengguna)
        {
            var allData = LoadAllData();
            return allData.Where(n => n.IdPengguna == idPengguna).ToList();
        }
        #endregion

        #region Data Saving Methods
        public bool SaveAllData(List<Notifikasi> notifications)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonData = JsonSerializer.Serialize(notifications, options);
                File.WriteAllText(_jsonFilePath, jsonData);
                _notifikasiList = notifications;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving all notification data: {ex.Message}");
                return false;
            }
        }

        public bool SaveDataForUser(string idPengguna, List<Notifikasi> userNotifications)
        {
            try
            {
                var allNotifications = LoadAllData();
                allNotifications.RemoveAll(n => n.IdPengguna == idPengguna);
                allNotifications.AddRange(userNotifications);
                return SaveAllData(allNotifications);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user notification data: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region CRUD Operations
        public List<Notifikasi> GetAllNotifikasi()
        {
            return LoadAllData();
        }

        public List<Notifikasi> GetNotifikasiByPengguna(string idPengguna)
        {
            return LoadDataForUser(idPengguna);
        }

        public Notifikasi GetNotifikasiById(string id)
        {
            return LoadAllData().FirstOrDefault(n => n.IdNotifikasi == id);
        }

        public Notifikasi AddNotifikasi(Notifikasi newNotifikasi)
        {
            try
            {
                if (string.IsNullOrEmpty(newNotifikasi.IdNotifikasi))
                {
                    newNotifikasi.IdNotifikasi = Guid.NewGuid().ToString();
                }
                newNotifikasi.TanggalNotifikasi = DateTime.Now;

                var allNotifications = LoadAllData();
                allNotifications.Add(newNotifikasi);

                if (SaveAllData(allNotifications))
                {
                    return newNotifikasi;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding notification: {ex.Message}");
                return null;
            }
        }

        public Notifikasi UpdateNotifikasi(Notifikasi updatedNotifikasi)
        {
            try
            {
                var allNotifications = LoadAllData();
                var existing = allNotifications.FirstOrDefault(n => n.IdNotifikasi == updatedNotifikasi.IdNotifikasi);

                if (existing != null)
                {
                    existing.IdPengguna = updatedNotifikasi.IdPengguna;
                    existing.IsiNotifikasi = updatedNotifikasi.IsiNotifikasi;
                    existing.TanggalNotifikasi = updatedNotifikasi.TanggalNotifikasi;

                    if (SaveAllData(allNotifications))
                    {
                        return existing;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification: {ex.Message}");
                return null;
            }
        }

        public bool DeleteNotifikasi(string id)
        {
            try
            {
                var allNotifications = LoadAllData();
                var toRemove = allNotifications.FirstOrDefault(n => n.IdNotifikasi == id);

                if (toRemove != null)
                {
                    allNotifications.Remove(toRemove);
                    return SaveAllData(allNotifications);
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting notification: {ex.Message}");
                return false;
            }
        }

        public Notifikasi SendNotifikasi(string idPengguna, string message)
        {
            var newNotif = new Notifikasi(
                idNotifikasi: Guid.NewGuid().ToString(),
                idPengguna: idPengguna,
                isiNotifikasi: message,
                tanggalNotifikasi: DateTime.Now
            );
            return AddNotifikasi(newNotif);
        }
        #endregion
    }
}
