//using HotelGlob.RMS.Identity.Data;
//using HotelGlob.RMS.Identity.Data.Models;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;

//namespace HotelGlob.RMS.Api.App_Start
//{
//    public static class IdentityConfig
//    {
//        public static void Configure(ApplicationUserManager manager, IdentityFactoryOptions<ApplicationUserManager> options)
//        {
//            // Configure validation logic for usernames
//            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
//            {
//                AllowOnlyAlphanumericUserNames = false,
//                RequireUniqueEmail = true
//            };
//            // Configure validation logic for passwords
//            manager.PasswordValidator = new PasswordValidator
//            {
//                RequiredLength = 6,
//                RequireNonLetterOrDigit = true,
//                RequireDigit = true,
//                RequireLowercase = true,
//                RequireUppercase = true,
//            };
//            var dataProtectionProvider = options.DataProtectionProvider;
//            if (dataProtectionProvider != null)
//            {
//                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
//            }
//        }
//    }
//}