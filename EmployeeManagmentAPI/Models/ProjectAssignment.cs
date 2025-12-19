namespace EmployeeManagmentAPI.Models
{
    public class ProjectAssignment
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        // Navigation property to the Project this assignment belongs to.

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Navigation property to the user assigned to the project.

        public string RoleInProject { get; set; }
        // The role of the user in this project (e.g., "Developer", "QA", "Manager").

        public DateTime AssignedDate { get; set; }
        // The date when the user was assigned to the project.
    }

}
/*Purpose

Represents the many-to-many relationship between Project and ApplicationUser.

Allows storing extra information about the assignment, like the user's role and assignment date.

EF Core uses ProjectId and UserId as foreign keys to link to Project and ApplicationUser.



// Users
var alice = new ApplicationUser { Id = "u1", FirstName = "Alice" };
var bob = new ApplicationUser { Id = "u2", FirstName = "Bob" };

// Project
var project = new Project
{
    Name = "Mobile App",
    Description = "Develop a new mobile application",
    StartDate = DateTime.Now
};

// Assign Alice as Developer
var assignment1 = new ProjectAssignment
{
    Project = project,
    ProjectId = project.ProjectId,
    User = alice,
    UserId = alice.Id,
    RoleInProject = "Developer",
    AssignedDate = DateTime.Now
};

// Assign Bob as QA
var assignment2 = new ProjectAssignment
{
    Project = project,
    ProjectId = project.ProjectId,
    User = bob,
    UserId = bob.Id,
    RoleInProject = "QA",
    AssignedDate = DateTime.Now
};

// Add assignments to project
project.ProjectAssignments.Add(assignment1);
project.ProjectAssignments.Add(assignment2);

// Print assignments
foreach (var a in project.ProjectAssignments)
{
    Console.WriteLine($"{a.User.FirstName} is assigned as {a.RoleInProject} on {a.AssignedDate}");
}


Alice is assigned as Developer on 12/15/2025
Bob is assigned as QA on 12/15/2025

*/