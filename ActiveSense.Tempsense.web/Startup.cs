
using ActiveSense.Tempsense.web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ActiveSense.Tempsense.web.Startup))]
namespace ActiveSense.Tempsense.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            //createRolesandUsers();
        }

        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Administrator"))
            {

                // first we create Admin role   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "fbenton";
                user.Email = "frank.benton2020@gmail.com";

                string userPWD = "Zaq2#ert";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Administrator");

                }

                var user2 = new ApplicationUser();
                user2.UserName = "systems";
                user2.Email = "systems@mymdiagnostics.com";

                string userPWD1 = "A@Z200711";

                var chkUser2 = UserManager.Create(user2, userPWD1);

                //Add default User to Role Admin   
                if (chkUser2.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user2.Id, "Administrator");

                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);


                var user1 = new ApplicationUser();
                user1.UserName = "compterroom";
                user1.Email = "compterroom@mymdiagnostics.com";

                string userPWD1 = "A@Z200711";

                var chkUser1 = UserManager.Create(user1, userPWD1);

                //Add default User to Role Admin   
                if (chkUser1.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user1.Id, "User");

                }

            }

            // creating Creating Employee role    
        }
    }
}
