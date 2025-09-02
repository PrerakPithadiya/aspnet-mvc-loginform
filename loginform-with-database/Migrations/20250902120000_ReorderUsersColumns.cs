using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace loginform_with_database.Migrations
{
    public partial class ReorderUsersColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Keep Id first, then order data columns: FullName, Username, Password
            migrationBuilder.Sql(@"ALTER TABLE `Users` 
                MODIFY COLUMN `FullName` varchar(100) NOT NULL AFTER `Id`,
                MODIFY COLUMN `Username` varchar(50) NOT NULL AFTER `FullName`,
                MODIFY COLUMN `Password` varchar(100) NOT NULL AFTER `Username`;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore original order: Username, Password, FullName (after Id)
            migrationBuilder.Sql(@"ALTER TABLE `Users` 
                MODIFY COLUMN `Username` varchar(50) NOT NULL AFTER `Id`,
                MODIFY COLUMN `Password` varchar(100) NOT NULL AFTER `Username`,
                MODIFY COLUMN `FullName` varchar(100) NOT NULL AFTER `Password`;");
        }
    }
}
