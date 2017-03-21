namespace SAP.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SAP.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SAP.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SAP.Models.ApplicationDbContext";
        }

        protected override void Seed(SAP.Models.ApplicationDbContext context)
        {
            /*
             Seed ��������� �� ����� ������ �� �������� � �� �������� ��� update-database.
             ���弝� �� �������� ApplicationDbContext ���� ������� ���� �� ���� ������ (�����, ������) ���������� �� .NET Identity
             */

            //���� ���������� ���� ��� Role 'Admin' ������� �� ����, ������� ���� �� �������
            //��. Id ��������� � ��� ������� ���� ������, ��������� ���������� GUID � ������ ����, ���� ������������
            if (!context.Roles.Any(x => x.Name == "Admin"))
            {
                context.Roles.Add(new IdentityRole { Id = "1", Name = "Admin" });
            }

            //���� ���������� ���� ��� Role 'Interviewer' ������� �� ����, ������� ���� �� �������
            if (!context.Roles.Any(x => x.Name == "Interviewer"))
            {
                context.Roles.Add(new IdentityRole { Id = "2", Name = "Interviewer" });
            }
            
            //
            //���� ������� User �� �� ��� ����� �����
            //��� account �� �� ���� �� ��������� ��� � ����� login
            // Email: admin@sap.com ; Pass:Admin123!
            //if (!context.Users.Any(x => x.Email == "admin@sap.com"))
            //{
            //    var hasher = new PasswordHasher();

            //    var user = new ApplicationUser
            //    {
            //        UserName = "Admin",
            //        PasswordHash = hasher.HashPassword("Admin123!"),
            //        Email = "admin@sap.com",
            //        EmailConfirmed = true,
            //        SecurityStamp = Guid.NewGuid().ToString()
            //    };

            //    user.Roles.Add(new IdentityUserRole { RoleId = "1", UserId = user.Id });

            //    context.Users.Add(user);
            //}

            base.Seed(context);
        }
    }
}
