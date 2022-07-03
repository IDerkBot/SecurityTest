using SecurityTest.Models;
using SecurityTest.Models.Entities;
using SecurityTest.Views.Pages.AdminPages;
using System;
using System.Web.Security;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для AddUserWindow.xaml
	/// </summary>
	public partial class AddUserWindow : Window
	{
		#region Variables

		private readonly UserProfile _currentProfile;

		#endregion

		#region Load

		public AddUserWindow()
		{
			InitializeComponent();
			_currentProfile = new UserProfile();
			DataContext = _currentProfile;
		}

		#endregion

		#region Buttons

		private void BtnUploadImage_OnClick(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog{ Filter = @"Image File (*.jpg, *.png)|*.jpg;*.png" };
			if(ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			var image = ImageManager.CroppedToBitmapImage(ofd.FileName);
			_currentProfile.UserInfo.Image = ImageManager.CroppedToBytes(image);
			MainImage.Source = image;
		}

		private void BtnAddUser_OnClick(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_currentProfile.User.Surname) ||
			    string.IsNullOrWhiteSpace(_currentProfile.User.Surname) ||
			    string.IsNullOrWhiteSpace(_currentProfile.User.Surname) ||
			    string.IsNullOrWhiteSpace(_currentProfile.User.Surname) ||
			    _currentProfile.UserInfo.DateBirth == null ||
			    string.IsNullOrWhiteSpace(_currentProfile.UserInfo.PassportSeries) ||
			    string.IsNullOrWhiteSpace(_currentProfile.UserInfo.PassportNumber) ||
			    string.IsNullOrWhiteSpace(_currentProfile.UserInfo.PassportIssuedBy) ||
			    _currentProfile.UserInfo.PassportDate == null)
			{
				MessageBox.Show("Вы не заполнили все данные!", "Внимание");
				return;
			}

			SecurityEntities.GetContext().Users.Add(_currentProfile.User);
			_currentProfile.UserInfo.User = _currentProfile.User;
			SecurityEntities.GetContext().UserInfoes.Add(_currentProfile.UserInfo);

			SecurityEntities.GetContext().SaveChanges();
			MessageBox.Show("Пользователь добавлен!");
			(PageManager.GetPage() as PersonalCabinetAdminPage)?.UpdateUsersTable();
			Close();
		}
		private void BtnPasswordGenerate_OnClick(object sender, RoutedEventArgs e)
		{
			TbPassword.Text = Membership.GeneratePassword(6, 1);
		}

		#endregion

		#region Events

		//EventArgs On Close
		private void AddUserWindow_OnClosed(object sender, EventArgs e)
		{
			PageManager.Show();
		}

		#endregion
	}
}