using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (name, description, logo, status) values ('Apple', 'Proin interdum mauris non ligula pellentesque ultrices.', 'Apple.png', 1);insert into Brands (name, description, logo, status) values ('Samsung', 'Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo.', 'Samsung.png', 1);insert into Brands (name, description, logo, status) values ('Oppo', 'Mauris lacinia sapien quis libero.', 'Oppo.png', 1);insert into Brands (name, description, logo, status) values ('Sony', 'Maecenas rhoncus aliquam lacus.', 'Sony.png', 1);insert into Brands (name, description, logo, status) values ('HP', 'Morbi porttitor lorem id ligula.', 'HP.png', 1);insert into Brands (name, description, logo, status) values ('Lenovo', 'Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.', 'Lenovo.png', 1);insert into Brands (name, description, logo, status) values ('Realme', 'Fusce posuere felis sed lacus.', 'Realme.png', 0);insert into Brands (name, description, logo, status) values ('Vivo', 'Phasellus sit amet erat.', 'Vivo.png', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Brands");
        }
    }
}
