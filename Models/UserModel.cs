using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Text.Json.Serialization;

namespace asp_ecommerce.Models
{
    public enum Role
    {
        USER,
        ADMIN
    }

    [Table("User")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("role")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; } = Role.USER;

        // Relationship: User has many Stores
        // [Reference(typeof(Store))]
        // public List<Store> Stores { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
