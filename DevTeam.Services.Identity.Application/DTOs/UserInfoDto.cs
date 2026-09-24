namespace DevTeam.Services.Identity.Application.DTOs
{
    public class UserInfoDto
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public List<string> Permissions { get; set; }
    }
}
//{
//"token": "eyJhbGciOiJIUzI1NiIs...",
//  "user": {
//    "id": 1,
//    "username": "admin",
//    "permissions": [
//      "Shipment.Read",
//      "Shipment.Create"
//    ]
//  }
//}