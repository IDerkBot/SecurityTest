using SecurityTest.Models;
using SecurityTest.Models.Entities;
using SecurityTest.Views.Pages.AdminPages;
using SecurityTest.Views.Pages.UserPages;
using System.Linq;
using System.Windows;
using Page = System.Windows.Controls.Page;

namespace SecurityTest.Views.Pages
{
	/// <summary>
	/// Логика взаимодействия для AuthorizationPage.xaml
	/// </summary>
	public partial class AuthorizationPage : Page
	{
		public AuthorizationPage()
		{
			InitializeComponent();
			PageManager.SetTitle(Title);
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var surname = TbSurname.Text;
			var firstname = TbFirstName.Text;
			var patronymic = TbPatronymic.Text;
			var password = TbPassword.Text;

			if (string.IsNullOrWhiteSpace(firstname))
				firstname = null;
			if (string.IsNullOrWhiteSpace(patronymic))
				patronymic = null;
			if (SecurityEntities.GetContext().Users.Any(x => x.Surname == surname && x.Firstname == firstname && x.Patronymic == patronymic))
			{
				var user = SecurityEntities.GetContext().Users.Single(x =>
					x.Surname == surname && x.Firstname == firstname && x.Patronymic == patronymic);
				if (user.Password == password)
				{
					if (user.Access == 0) PageManager.Navigate(new StartTestPage(user));
					else PageManager.Navigate(new PersonalCabinetAdminPage());
				}
				else MessageBox.Show(@"Пароль не верный");
			}
			else
				MessageBox.Show(@"Такого пользователя нет");
		}
	}
}