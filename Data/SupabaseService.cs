using Supabase;
using asp_ecommerce.Models;

namespace asp_ecommerce.Services
{
    public class SupabaseAuthService
    {
        private readonly Supabase.Client _client;
        private readonly IConfiguration _configuration;

        public SupabaseAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:Key"];

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(url, key, options);
            _client.InitializeAsync().Wait();
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAsync(string name, string email, string password)
        {
            try
            {
                var existingUser = await _client
                    .From<User>()
                    .Where(x => x.Email == email)
                    .Get();

                if (existingUser.Models.Any())
                {
                    return (false, "Email already registered", null);
                }

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = "USER",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var insertedUser = await _client.From<User>().Insert(user);
                var newUser = insertedUser.Models.FirstOrDefault();

                if (newUser == null)
                {
                    return (false, "Registration failed. Please try again.", null);
                }

                return (true, "Registration successful!", newUser);
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
        {
            try
            {
                var userResponse = await _client
                    .From<User>()
                    .Where(x => x.Email == email)
                    .Single();

                if (userResponse == null)
                {
                    return (false, "Invalid email or password", null);
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, userResponse.Password);

                if (!isPasswordValid)
                {
                    return (false, "Invalid email or password", null);
                }

                userResponse.UpdatedAt = DateTime.UtcNow;
                await _client.From<User>().Update(userResponse);

                return (true, "Login successful", userResponse);
            }
            catch (Exception ex)
            {
                return (false, "Invalid email or password", null);
            }
        }

        public async Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var userExists = await _client
                    .From<User>()
                    .Where(x => x.Email == email)
                    .Get();

                if (!userExists.Models.Any())
                {
                    return (false, "No account found with this email address");
                }

                // In a real application, you would:
                // 1. Generate a unique reset token
                // 2. Store it in the database with an expiration time
                // 3. Send an email with the reset link

                return (true, "Password reset instructions have been sent to your email");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send reset email: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                var userResponse = await _client
                    .From<User>()
                    .Where(x => x.Email == email)
                    .Single();

                if (userResponse == null)
                {
                    return (false, "User not found");
                }

                userResponse.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                userResponse.UpdatedAt = DateTime.UtcNow;
                await _client.From<User>().Update(userResponse);

                return (true, "Password has been reset successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to reset password: {ex.Message}");
            }
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _client
                    .From<User>()
                    .Where(x => x.Id == userId)
                    .Single();
            }
            catch
            {
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _client
                    .From<User>()
                    .Where(x => x.Email == email)
                    .Single();
            }
            catch
            {
                return null;
            }
        }
    }
}