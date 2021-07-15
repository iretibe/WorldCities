using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using WorldCities.Angular.Controllers;
using WorldCities.Angular.Data;
using WorldCities.Angular.Data.Models;
using Xunit;

namespace WorldCities.Tests
{
    public class SeedController_Tests
    {
        //Test the CreateDefaultUsers() method
        [Fact]
        public async void CreateDefaultUsers()
        {
            #region Arrange
            //Create the option instances required by the ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WorldCities")
                .Options;

            var storeOptions = Options.Create(new OperationalStoreOptions());

            //Create a IWebHost environment mock instance
            var mockEnv = new Mock<IWebHostEnvironment>().Object;

            //Define the variables for the users we want to test
            ApplicationUser user_Admin = null;
            ApplicationUser user_User = null;
            ApplicationUser user_NotExisting = null;
            #endregion

            #region Act

            //Create a ApplicationDbContext instance using the in-memory DB
            using (var context = new ApplicationDbContext(options, storeOptions))
            {
                //Create a RoleManager instance
                var roleManager = IdentityHelper.GetRoleManager(
                    new RoleStore<IdentityRole>(context));

                //Create a UserManager instance
                var userManager = IdentityHelper.GetUserManager(
                    new UserStore<ApplicationUser>(context));

                //Create a SeedController instance
                var controller = new SeedController(context, roleManager, userManager, mockEnv);

                //Execute the SeedController's CreateDefaultUsers() method to create the default users (and roles)
                await controller.CreateDefaultUsers();

                //Retrieve the users
                user_Admin = await userManager.FindByEmailAsync("somady12@gmail.com");
                user_User = await userManager.FindByEmailAsync("yessouf2009@yahoo.fr");
                user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");
            }
            #endregion

            #region Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);
            #endregion
        }
    }
}
