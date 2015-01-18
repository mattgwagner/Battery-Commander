using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System;

namespace BatteryCommander.Common.Services.Users
{
    public class AppUserManager : UserManager<AppUser, int>
    {
        public const String TwoFactorMessageSubject = "Your Security Code";

        public const String TwoFactorMessageFormat = "Use {0} as your login security code.";

        public AppUserManager(IUserStore<AppUser, int> userStore, IIdentityMessageService messageSvc)
            : base(userStore)
        {
            this.UserValidator = new UserValidator<AppUser, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 3;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user

            this.UserTokenProvider = new TotpSecurityStampBasedTokenProvider<AppUser, int> { };

            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<AppUser, int>
            {
                MessageFormat = TwoFactorMessageFormat
            });

            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<AppUser, int>
            {
                Subject = TwoFactorMessageSubject,
                BodyFormat = TwoFactorMessageFormat
            });

            this.EmailService = this.SmsService = messageSvc;
        }
    }
}