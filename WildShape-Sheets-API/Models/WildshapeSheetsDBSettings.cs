namespace WildShape_Sheets_API.Models {
    public class WildshapeSheetsDBSettings {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollectionName { get; set; } = null!;
    }
}
