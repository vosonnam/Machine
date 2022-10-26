using System.Security.Claims;
using System.Security.Principal;

namespace xqmachine.Models.Helpers
{
    public static class AuthHelper
	{
		public static string GetFullName(this IIdentity identity)
		{
			var claim = ((ClaimsIdentity)identity).FindFirst("FullName");
			return (claim != null) ? claim.Value : string.Empty;
		}
		public static string GetAvatar(this IIdentity identity)
		{
			var claim = ((ClaimsIdentity)identity).FindFirst("Avatar");
			return (claim != null) ? claim.Value : string.Empty;
		}

		public static bool IsConfirmEmail(this IIdentity identity)
		{
			var claim = ((ClaimsIdentity)identity).FindFirst("ConfirmEmail");
			bool rs = bool.Parse(claim.Value);
			return rs;
		}
		public static bool HasPassword(this IIdentity identity)
		{
			var claim = ((ClaimsIdentity)identity).FindFirst("HasPassword");
			bool rs = bool.Parse(claim.Value);
			return rs;
		}
	}
}