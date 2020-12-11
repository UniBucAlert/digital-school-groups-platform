using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using DigitalSchoolGroupsPlatform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DigitalSchoolGroups.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<SelectListItem> AllRoles { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [InverseProperty("Users")]
        public virtual ICollection<Group> Groups { get; set; }

        [InverseProperty("Requests")]
        public virtual ICollection<Group> GroupsRequests { get; set; }

      //  public virtual ICollection<Group> AdminOf { get; set; }

        public bool IsInGroup(Group group)
        {
            if (group.GroupAdmin.Id == Id)
                return true;
            foreach (DigitalSchoolGroupsPlatform.Models.Group gr in Groups)
            {
                if (gr.Id == group.Id)
                    return true;
            }
            return false;
        }

        public bool RequestedToJoin(Group group)
        {
            foreach (DigitalSchoolGroupsPlatform.Models.Group gr in GroupsRequests)
            {
                if (gr.Id == group.Id)
                    return true;
            }
            return false;
        }

        public void DeleteJoinRequest(Group group)
        {
            group.Requests.Remove(this);
            GroupsRequests.Remove(group);
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
                DigitalSchoolGroups.Migrations.Configuration>("DefaultConnection"));

        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
