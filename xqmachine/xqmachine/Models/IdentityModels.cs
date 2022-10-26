using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace xqmachine.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole,
    CustomUserClaim>
    {

        public DateTime Create_at { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [StringLength(100)]
        public string Create_by { get; set; }

        [StringLength(100)]
        public string Update_by { get; set; }

        public DateTime Update_at { get; set; }

        [StringLength(100)]
        public string Requestcode { get; set; }

        [Column(TypeName = "text")]
        public string Avatar { get; set; }

        [Required]
        [StringLength(256)]
        public string FullName { get; set; }

        //public virtual ICollection<AccountAddress> AccountAddress { get; set; }
        //public virtual ICollection<Feedback> Feedback { get; set; }
        //public virtual ICollection<Order> Order { get; set; }
        //public virtual ICollection<ReplyFeedback> ReplyFeedback { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FullName", this.FullName.ToString()));
            userIdentity.AddClaim(new Claim("Avatar", this.Avatar.ToString()));
            userIdentity.AddClaim(new Claim("ConfirmEmail", this.EmailConfirmed.ToString()));
            bool HasPassword = this.PasswordHash != null;
            userIdentity.AddClaim(new Claim("HasPassword", HasPassword.ToString()));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole,
    int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }



    //Custom model
    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context)
            : base(context)
        {
        }

        public override Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            return base.AddClaimAsync(user, claim);
        }

        public override Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            return base.AddLoginAsync(user, login);
        }

        public override Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            return base.AddToRoleAsync(user, roleName);
        }

        public override Task CreateAsync(ApplicationUser user)
        {
            user.UserName = user.Email;
            user.Create_by = user.Email;
            user.Create_at = DateTime.Now;
            user.Update_at = DateTime.Now;
            user.Update_by = user.Email;
            user.Status = "1";
            user.Avatar = "/Content/Images/logo/logo-banner.png";
            return base.CreateAsync(user);
        }

        public override Task DeleteAsync(ApplicationUser user)
        {
            return base.DeleteAsync(user);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            return base.FindAsync(login);
        }

        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }

        public override Task<ApplicationUser> FindByIdAsync(int userId)
        {
            return base.FindByIdAsync(userId);
        }

        public override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return base.FindByNameAsync(userName);
        }

        public override Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return base.GetAccessFailedCountAsync(user);
        }

        public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            return base.GetClaimsAsync(user);
        }

        public override Task<string> GetEmailAsync(ApplicationUser user)
        {
            return base.GetEmailAsync(user);
        }

        public override Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return base.GetEmailConfirmedAsync(user);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return base.GetLockoutEnabledAsync(user);
        }

        public override Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            return base.GetLockoutEndDateAsync(user);
        }

        public override Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            return base.GetLoginsAsync(user);
        }

        public override Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return base.GetPasswordHashAsync(user);
        }

        public override Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            return base.GetPhoneNumberAsync(user);
        }

        public override Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            return base.GetPhoneNumberConfirmedAsync(user);
        }

        public override Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return base.GetRolesAsync(user);
        }

        public override Task<string> GetSecurityStampAsync(ApplicationUser user)
        {
            return base.GetSecurityStampAsync(user);
        }

        public override Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return base.GetTwoFactorEnabledAsync(user);
        }

        public override Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return base.HasPasswordAsync(user);
        }

        public override Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            return base.IncrementAccessFailedCountAsync(user);
        }

        public override Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            return base.IsInRoleAsync(user, roleName);
        }

        public override Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {
            return base.RemoveClaimAsync(user, claim);
        }

        public override Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            return base.RemoveFromRoleAsync(user, roleName);
        }

        public override Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            return base.RemoveLoginAsync(user, login);
        }

        public override Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            return base.ResetAccessFailedCountAsync(user);
        }

        public override Task SetEmailAsync(ApplicationUser user, string email)
        {
            return base.SetEmailAsync(user, email);
        }

        public override Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            return base.SetEmailConfirmedAsync(user, confirmed);
        }

        public override Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            return base.SetLockoutEnabledAsync(user, enabled);
        }

        public override Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            return base.SetLockoutEndDateAsync(user, lockoutEnd);
        }

        public override Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            return base.SetPasswordHashAsync(user, passwordHash);
        }

        public override Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            return base.SetPhoneNumberAsync(user, phoneNumber);
        }

        public override Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            return base.SetPhoneNumberConfirmedAsync(user, confirmed);
        }

        public override Task SetSecurityStampAsync(ApplicationUser user, string stamp)
        {
            return base.SetSecurityStampAsync(user, stamp);
        }

        public override Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            return base.SetTwoFactorEnabledAsync(user, enabled);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Task UpdateAsync(ApplicationUser user)
        {
            user.Update_at = DateTime.Now;
            return base.UpdateAsync(user);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override Task<ApplicationUser> GetUserAggregateAsync(Expression<Func<ApplicationUser, bool>> filter)
        {
            return base.GetUserAggregateAsync(filter);
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }

        public override Task CreateAsync(CustomRole role)
        {
            return base.CreateAsync(role);
        }

        public override Task DeleteAsync(CustomRole role)
        {
            return base.DeleteAsync(role);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override Task UpdateAsync(CustomRole role)
        {
            return base.UpdateAsync(role);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}