namespace asp_ecommerce.Data
{
    public interface ISupabaseService
    {
        Supabase.Client GetClient();
    }

    public class SupabaseService : ISupabaseService
    {
        private readonly Supabase.Client _client;

        public SupabaseService(IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:Key"];

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(url, key, options);
            _client.InitializeAsync().Wait();
        }

        public Supabase.Client GetClient()
        {
            return _client;
        }
    }
}