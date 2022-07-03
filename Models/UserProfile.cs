using SecurityTest.Models.Entities;

namespace SecurityTest.Models
{
	public class UserProfile
	{
		public User User { get; set; }
		public UserInfo UserInfo { get; set; }

		public UserProfile()
		{
			User = new User();
			UserInfo = new UserInfo();
		}
	}
}
