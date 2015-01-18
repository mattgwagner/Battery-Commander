using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BatteryCommander.Common.Services.Users
{
    public class UserService :
        IUserStore<AppUser, int>,
        IUserPasswordStore<AppUser, int>,
        IUserLockoutStore<AppUser, int>,
        IUserTwoFactorStore<AppUser, int>,
        IUserEmailStore<AppUser, int>,
        IUserSecurityStampStore<AppUser, int>,
        IUserPhoneNumberStore<AppUser, int>
    // IUserClaimStore<AppUser, int>,
    // IUserRoleStore<AppUser, int>,
    // IQueryableUserStore<AppUser, int>
    {
        // Sign-in Flow
        // FindByNameAsync
        // GetLockoutEnabledAsync
        // GetLockoutEndDateAsync
        // GetPasswordHashAsync
        // GetTwoFactorEnabledAsync

        private readonly DataContext _db;

        public UserService(DataContext db)
        {
            _db = db;
        }

        //     Finds a user
        public async Task<AppUser> FindByIdAsync(int userId)
        {
            if (userId < 1) throw new ArgumentOutOfRangeException("Invalid userId");

            // This is very simplisitic for now, but in the future may hit the cache and/or eager load certain values

            var user = await GetUserAggregateAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("Invalid userId");
            }

            return user;
        }

        //     Get the user password hash
        public Task<string> GetPasswordHashAsync(AppUser user)
        {
            return Task.FromResult(user.Password);
        }

        //     Returns true if a user has a password set
        public Task<bool> HasPasswordAsync(AppUser user)
        {
            return Task.FromResult(!String.IsNullOrWhiteSpace(user.Password));
        }

        //     Set the user password hash
        public Task SetPasswordHashAsync(AppUser user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(0);
        }

        //     Insert a new user
        public Task CreateAsync(AppUser user)
        {
            throw new NotImplementedException("Not allowed via UserService");
        }

        //     Delete a user
        public Task DeleteAsync(AppUser user)
        {
            throw new NotImplementedException("Not allowed via UserService");
        }

        //     Find a user by name
        public Task<AppUser> FindByNameAsync(string userName)
        {
            return GetUserAggregateAsync(user => user.UserName.ToUpper() == userName.ToUpper());
        }

        //     Update a user
        public async Task UpdateAsync(AppUser user)
        {
            await _db.SaveChangesAsync();
        }

        //     Returns the current number of failed access attempts. This number usually
        //     will be reset whenever the password is verified or the account is locked
        //     out.
        public Task<int> GetAccessFailedCountAsync(AppUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        //     Returns whether the user can be locked out.
        public Task<bool> GetLockoutEnabledAsync(AppUser user)
        {
            return Task.FromResult(true);
        }

        //     Returns the DateTimeOffset that represents the end of a user's lockout, any
        //     time in the past should be considered not locked out.
        public Task<DateTimeOffset> GetLockoutEndDateAsync(AppUser user)
        {
            return Task.FromResult(new DateTimeOffset(user.LockoutEndDate ?? DateTime.MinValue));
        }

        //     Used to record when an attempt to access the user has failed
        public Task<int> IncrementAccessFailedCountAsync(AppUser user)
        {
            user.AccessFailedCount += 1;
            return Task.FromResult(user.AccessFailedCount);
        }

        //     Used to reset the access failed count, typically after the account is successfully accessed
        public Task ResetAccessFailedCountAsync(AppUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        //     Sets whether the user can be locked out.
        public Task SetLockoutEnabledAsync(AppUser user, bool enabled)
        {
            // This is always true for us
            return Task.FromResult(0);
        }

        //     Locks a user out until the specified end date (set to a past date, to unlock a user)
        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDate = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        //     Returns whether two factor authentication is enabled for the user
        public Task<bool> GetTwoFactorEnabledAsync(AppUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        //     Sets whether two factor authentication is enabled for the user
        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        //     Returns the user associated with this email
        public Task<AppUser> FindByEmailAsync(string email)
        {
            return GetUserAggregateAsync(user => user.UserName.ToUpper() == email.ToUpper());
        }

        //     Get the user email
        public Task<string> GetEmailAsync(AppUser user)
        {
            return Task.FromResult(user.UserName);
        }

        //     Returns true if the user email is confirmed
        public Task<bool> GetEmailConfirmedAsync(AppUser user)
        {
            return Task.FromResult(user.EmailAddressConfirmed);
        }

        //     Set the user email
        public Task SetEmailAsync(AppUser user, string email)
        {
            throw new NotImplementedException("Not allowed via UserService");
        }

        //     Sets whether the user email is confirmed
        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed)
        {
            user.EmailAddressConfirmed = confirmed;
            return Task.FromResult(0);
        }

        //     Get the user security stamp
        public Task<string> GetSecurityStampAsync(AppUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        //     Set the security stamp for the user
        public Task SetSecurityStampAsync(AppUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        //     Get the user phone number
        public Task<String> GetPhoneNumberAsync(AppUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        //     Returns true if the user phone number is confirmed
        public Task<Boolean> GetPhoneNumberConfirmedAsync(AppUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        //     Set the user's phone number
        public Task SetPhoneNumberAsync(AppUser user, String phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        //     Sets whether the user phone number is confirmed
        public Task SetPhoneNumberConfirmedAsync(AppUser user, Boolean confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
            // TODO
        }

        private Task<AppUser> GetUserAggregateAsync(Expression<Func<AppUser, bool>> filter)
        {
            return _db.Users
                .SingleOrDefaultAsync(filter);
        }
    }
}