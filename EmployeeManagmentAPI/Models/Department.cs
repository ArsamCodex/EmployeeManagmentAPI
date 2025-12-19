namespace EmployeeManagmentAPI.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        // Primary key for the department. Uniquely identifies each department in the database.

        public string Name { get; set; }
        // Name of the department, e.g., "Human Resources" or "IT".

        public string Description { get; set; }
        // Provides details about the department, like its responsibilities or purpose.

        public ICollection<ApplicationUser> Employees { get; set; }
        // Collection of employees that belong to this department.
        // Each ApplicationUser in this collection represents a user assigned to this department.
    }


}


/*// Create a new department
var itDepartment = new Department
{
    Name = "IT",
    Description = "Handles all technology-related tasks",
    Employees = new List<ApplicationUser>() // Initialize the collection
};

// Create some users
var user1 = new ApplicationUser
{
    UserName = "alice",
    Email = "alice@example.com",
    // Other properties like FirstName, LastName, etc.
};

var user2 = new ApplicationUser
{
    UserName = "bob",
    Email = "bob@example.com",
    // Other properties
};

// Add users to the department
itDepartment.Employees.Add(user1);
itDepartment.Employees.Add(user2);

// Now itDepartment has 2 employees: Alice and Bob


Department: IT
Description: Handles all technology-related tasks
Employees:
  - Username: alice, Email: alice@example.com
  - Username: bob, Email: bob@example.com

*/