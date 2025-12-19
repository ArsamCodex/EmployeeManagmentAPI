namespace EmployeeManagmentAPI.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        // Primary key for the asset record. Unique identifier.

        public string Name { get; set; }
        // The name of the asset, e.g., "Laptop", "Monitor", "Keyboard".

        public string SerialNumber { get; set; }
        // A unique identifier from the manufacturer to track the asset.

        public DateTime AssignedDate { get; set; }
        // The date when this asset was assigned to the user.

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Links the asset to the specific user (employee) who currently has it.
    }


}
/*
 * Purpose of the Asset Class

Track Company Equipment

Keeps a record of all physical or digital assets assigned to employees.

Assign Assets to Users

The UserId and User properties link an asset to a specific employee.

Monitor Usage and History

AssignedDate shows when the asset was given to the user.

Can be used to check if the asset is overdue for return, maintenance, or replacement.

Inventory Management

Helps the company maintain an inventory of all equipment, including serial numbers for warranty and auditing purposes.
var laptop = new Asset
{
    AssetId = 1,
    Name = "Dell XPS 15 Laptop",
    SerialNumber = "DX15-123456",
    AssignedDate = DateTime.Now,
    UserId = "user123"
};
Explanation:

This creates a record showing that the employee with ID user123 was assigned a Dell XPS 15 Laptop today.
*/