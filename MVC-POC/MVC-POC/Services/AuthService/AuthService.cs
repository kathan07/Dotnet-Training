using MVC_POC.Models;

namespace MVC_POC.Services.AuthService
{
    public class AuthService
    {
        public Dictionary<string, User> credentials { get; set; }

        public AuthService()
        {
            credentials = new Dictionary<string, User>
            {
                // Admin User
                {
                    "admin@example.com",
                    new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@example.com",
                        Password = "Admin123!",
                        Country = "United States",
                        State = "California",
                        Phone = "1234567890",
                        Role = RoleType.Admin
                    }
                },

                // United States Users (4)
                { "john.doe@example.com", new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "JohnDoe123!", Country = "United States", State = "New York", Phone = "9876543210", Role = RoleType.Member }},
                { "jane.smith@example.com", new User { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Password = "JaneSmith123!", Country = "United States", State = "Texas", Phone = "5551112233", Role = RoleType.Member }},
                { "michael.johnson@example.com", new User { FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com", Password = "MichaelJ123!", Country = "United States", State = "Florida", Phone = "7896541230", Role = RoleType.Member }},
                { "emily.davis@example.com", new User { FirstName = "Emily", LastName = "Davis", Email = "emily.davis@example.com", Password = "EmilyD123!", Country = "United States", State = "Illinois", Phone = "6549873210", Role = RoleType.Member }},

                // United Kingdom Users (4)
                { "mark.taylor@example.com", new User { FirstName = "Mark", LastName = "Taylor", Email = "mark.taylor@example.com", Password = "MarkTaylor123!", Country = "United Kingdom", State = "England", Phone = "1239874560", Role = RoleType.Member }},
                { "emma.anderson@example.com", new User { FirstName = "Emma", LastName = "Anderson", Email = "emma.anderson@example.com", Password = "EmmaAnderson123!", Country = "United Kingdom", State = "Scotland", Phone = "7412589630", Role = RoleType.Member }},
                { "harry.white@example.com", new User { FirstName = "Harry", LastName = "White", Email = "harry.white@example.com", Password = "HarryWhite123!", Country = "United Kingdom", State = "Wales", Phone = "8527419630", Role = RoleType.Member }},
                { "lucy.hall@example.com", new User { FirstName = "Lucy", LastName = "Hall", Email = "lucy.hall@example.com", Password = "LucyHall123!", Country = "United Kingdom", State = "Northern Ireland", Phone = "3698521470", Role = RoleType.Member }},

                // India Users (4)
                { "arjun.sharma@example.com", new User { FirstName = "Arjun", LastName = "Sharma", Email = "arjun.sharma@example.com", Password = "ArjunSharma123!", Country = "India", State = "Maharashtra", Phone = "9876543211", Role = RoleType.Member }},
                { "priya.kapoor@example.com", new User { FirstName = "Priya", LastName = "Kapoor", Email = "priya.kapoor@example.com", Password = "PriyaKapoor123!", Country = "India", State = "Delhi", Phone = "1122334455", Role = RoleType.Member }},
                { "rahul.verma@example.com", new User { FirstName = "Rahul", LastName = "Verma", Email = "rahul.verma@example.com", Password = "RahulVerma123!", Country = "India", State = "Karnataka", Phone = "5566778899", Role = RoleType.Member }},
                { "ananya.mishra@example.com", new User { FirstName = "Ananya", LastName = "Mishra", Email = "ananya.mishra@example.com", Password = "AnanyaM123!", Country = "India", State = "West Bengal", Phone = "2233445566", Role = RoleType.Member }},

                // Germany Users (3)
                { "hans.schmidt@example.com", new User { FirstName = "Hans", LastName = "Schmidt", Email = "hans.schmidt@example.com", Password = "HansSchmidt123!", Country = "Germany", State = "Bavaria", Phone = "1593574862", Role = RoleType.Member }},
                { "anna.muller@example.com", new User { FirstName = "Anna", LastName = "Muller", Email = "anna.muller@example.com", Password = "AnnaMuller123!", Country = "Germany", State = "Berlin", Phone = "7419638520", Role = RoleType.Member }},
                { "lars.krause@example.com", new User { FirstName = "Lars", LastName = "Krause", Email = "lars.krause@example.com", Password = "LarsKrause123!", Country = "Germany", State = "Saxony", Phone = "9638527410", Role = RoleType.Member }},

                // Spain Users (3)
                { "maria.garcia@example.com", new User { FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@example.com", Password = "MariaGarcia123!", Country = "Spain", State = "Madrid", Phone = "7778889990", Role = RoleType.Member }},
                { "carlos.rodriguez@example.com", new User { FirstName = "Carlos", LastName = "Rodriguez", Email = "carlos.rodriguez@example.com", Password = "CarlosRodriguez123!", Country = "Spain", State = "Catalonia", Phone = "6665554443", Role = RoleType.Member }},
                { "sofia.lopez@example.com", new User { FirstName = "Sofia", LastName = "Lopez", Email = "sofia.lopez@example.com", Password = "SofiaLopez123!", Country = "Spain", State = "Valencia", Phone = "5554443332", Role = RoleType.Member }}
            };
        }

        public User ValidateUser(string email, string password)
        {
            if (credentials.TryGetValue(email, out var user) && user.Password == password)
            {
                return new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Country = user.Country,
                    State = user.State,
                    Phone = user.Phone,
                    Role = user.Role
                };
            }
            return null;
        }

        public bool RegisterUser(User user)
        {
            if (credentials.ContainsKey(user.Email))
            {
                return false;
            }
            user.Role = RoleType.Member;
            credentials.Add(user.Email, user);
            return true;
        }
    }
}
