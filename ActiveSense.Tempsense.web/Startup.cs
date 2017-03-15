
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
            if (!roleManager.RoleExists("Administrador"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Administrador";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "gina";
                user.Email = "gina.ospina@softwareestrategico.com";

                string userPWD = "A@Z200711";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Administrador");

                }

                var user2 = new ApplicationUser();
                user2.UserName = "sistemas";
                user2.Email = "sistemas@mymdiagnostics.com";

                string userPWD1 = "A@Z200711";

                var chkUser2 = UserManager.Create(user2, userPWD1);

                //Add default User to Role Admin   
                if (chkUser2.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user2.Id, "Administrador");

                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Usuario"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Usuario";
                roleManager.Create(role);


                var user1 = new ApplicationUser();
                user1.UserName = "almacen";
                user1.Email = "almacen@mymdiagnostics.com";

                string userPWD1 = "A@Z200711";

                var chkUser1 = UserManager.Create(user1, userPWD1);

                //Add default User to Role Admin   
                if (chkUser1.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user1.Id, "Usuario");

                }

            }

            // creating Creating Employee role    
        }
    }
}
