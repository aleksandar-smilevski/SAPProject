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
             Seed функцијава ја полни базата со податоци и се повикува при update-database.
             Бидејќи го користам ApplicationDbContext имам пристап само до оние модели (класи, табели) искреирани од .NET Identity
             */

            //Овде проверувам дали има Role 'Admin' запишан во база, доколку нема го креирам
            //пс. Id вредноста е број запишан како стринг, претходно генерирање GUID и личеше грдо, вака поедноставно
            if (!context.Roles.Any(x => x.Name == "Admin"))
            {
                context.Roles.Add(new IdentityRole { Id = "1", Name = "Admin" });
            }

            //Овде проверувам дали има Role 'Interviewer' запишан во база, доколку нема го креирам
            if (!context.Roles.Any(x => x.Name == "Interviewer"))
            {
                context.Roles.Add(new IdentityRole { Id = "2", Name = "Interviewer" });
            }
            
            //
            //Овде креирам User кој ќе има админ улога
            //Овој account ќе ни биде за тестирање има и лесен login
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
