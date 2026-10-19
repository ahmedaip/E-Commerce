using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (name, description, status) values ('Mobiles', 'Phasellus id sapien in sapien iaculis congue.', 0);insert into Categories (name, description, status) values ('Tablets', 'Proin risus.', 1);insert into Categories (name, description, status) values ('PCs', 'Quisque id justo sit amet sapien dignissim vestibulum.', 1);insert into Categories (name, description, status) values ('Laptops', 'Phasellus id sapien in sapien iaculis congue.', 1);insert into Categories (name, description, status) values ('Cameras', 'Duis bibendum.', 1);insert into Categories (name, description, status) values ('Accessories', 'Duis ac nibh.', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Categories");
        }
    }
}
