namespace WildShape_Sheets_API.Models {
    public class AppSettings {

        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration) {
            _configuration = configuration;
        }

        public string SecretKey => _configuration["APIKeys:SecretKey"];

       
        
    }
}
