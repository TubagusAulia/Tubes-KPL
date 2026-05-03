using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ManajemenPerpus.Core.Models;

namespace ManajemenPerpus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenggunaController : ControllerBase
    {
        private static List<Pengguna> users = new List<Pengguna>();
        private string filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataPengguna.json");

        private void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(users, options);
            System.IO.File.WriteAllText(filePath, jsonString);
        }

        private void EnsureDefaultUsers()
        {
            bool changed = false;
            if (!users.Any(u => u.Username == "admin"))
            {
                users.Add(new Pengguna(
                    idPengguna: "A001",
                    username: "admin",
                    password: "admin",
                    role: Pengguna.ROLEPENGGUNA.admin,
                    fullname: "System Administrator",
                    email: "admin@perpus.com",
                    phone: "000000000000",
                    address: "Admin Office"
                ));
                changed = true;
            }

            if (!users.Any(u => u.Username == "user"))
            {
                users.Add(new Pengguna(
                    idPengguna: "U001",
                    username: "user",
                    password: "user",
                    role: Pengguna.ROLEPENGGUNA.anggota,
                    fullname: "Default User",
                    email: "user@perpus.com",
                    phone: "111111111111",
                    address: "User Address"
                ));
                changed = true;
            }

            if (changed)
            {
                SaveToFile();
            }
        }

        [HttpGet]
        public ActionResult<List<Pengguna>> GetAllUsers()
        {
            // Gunakan path absolut relatif terhadap direktori kerja aplikasi
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    string jsonData = System.IO.File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<List<Pengguna>>(jsonData);
                    users = data ?? new List<Pengguna>();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error reading or parsing the file: {ex.Message}");
                }
            }
            else
            {
                users = new List<Pengguna>();
            }

            EnsureDefaultUsers();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public ActionResult<Pengguna> GetUserById(string id)
        {
            var user = users.FirstOrDefault(u => u.IdPengguna == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<Pengguna> CreateUser([FromBody] Pengguna newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid user data.");
            }
            // Generate ID for the new user
            newUser.IdPengguna = newUser.Role == Pengguna.ROLEPENGGUNA.admin ? "A" : "P";
            users.Add(newUser);
            SaveToFile();
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.IdPengguna }, newUser);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(string id, [FromBody] Pengguna updatedUser)
        {
            if (updatedUser == null || updatedUser.IdPengguna != id)
            {
                return BadRequest("Invalid user data.");
            }
            var user = users.FirstOrDefault(u => u.IdPengguna == id);
            if (user == null)
            {
                return NotFound();
            }
            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.Fullname = updatedUser.Fullname;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Address = updatedUser.Address;
            SaveToFile();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(string id)
        {
            var user = users.FirstOrDefault(u => u.IdPengguna == id);
            if (user == null)
            {
                return NotFound();
            }
            users.Remove(user);
            SaveToFile();
            return NoContent();
        }

        [HttpPost("login")]
        public ActionResult<Pengguna> Login([FromBody] LoginRequest request)
        {
            // Ensure users are loaded from JSON
            if (users == null || users.Count == 0)
            {
                GetAllUsers();
            }
            
            var user = users!.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized("Username atau password salah.");
            }
            return Ok(user);
        }

        public class LoginRequest 
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
