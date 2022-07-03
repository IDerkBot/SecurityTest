using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SecurityTest.Models;
using SecurityTest.Models.Entities;
using SecurityTest.Views.Windows;

namespace SecurityTest.Views.Pages.AdminPages
{
	/// <summary>
	/// Логика взаимодействия для PersonalCabinetAdminPage.xaml
	/// </summary>
	public partial class PersonalCabinetAdminPage : Page
	{
		#region Load

		public PersonalCabinetAdminPage()
		{
			InitializeComponent();
			PageManager.SetTitle(Title);
		}

		private void PersonalCabinetAdminPage_OnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateUsersTable();
		}

		#endregion

		#region Tab1

		private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var search = (sender as TextBox)?.Text.ToLower();
			var list = SecurityEntities.GetContext().UserInfoes.ToList();
			var cultureRu = new CultureInfo("ru-Ru");
			var searchResult =
				list.Where(x =>
				{
					DateTime date;
					if (x.DateBirth != null)
						date = (DateTime)x.DateBirth;
					else
						date = DateTime.Now;
					return x.User.Fullname.ToLower().Contains(search) ||
					       x.ID.ToString().Contains(search) ||
					       date.ToString(cultureRu.DateTimeFormat.ShortDatePattern).Contains(search);
				}).ToList();
			DgUsers.ItemsSource = searchResult;
		}

		private void BtnClearSearch_OnClick(object sender, RoutedEventArgs e)
		{
			TbSearch.Text = "";
		}

		private void BtnAddUser_OnClick(object sender, RoutedEventArgs e)
		{
			var window = new AddUserWindow();
			window.Show();
			PageManager.Hide();
		}

		private void DgUsers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedUser = DgUsers.SelectedItem as UserInfo;
			PageManager.Navigate(new UserCardPage(selectedUser));
		}

		#endregion

		#region Tab2

		private void RbTestSelected_OnChecked(object sender, RoutedEventArgs e)
		{
			var obj = sender as RadioButton;
			DgThemes.ItemsSource = SecurityEntities.GetContext().Themes.Where(x => x.Test.Title == obj.Content.ToString()).ToList();
		}

		private void DgThemes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DgThemes.SelectedItem == null)
			{
				DgQuestions.ItemsSource = null;
				DgAnswers.ItemsSource = null;
			}
			else if (DgThemes.SelectedItem is Theme selectedTheme)
			{
				DgQuestions.ItemsSource = SecurityEntities.GetContext().Questions.Where(x => x.Theme.ID == selectedTheme.ID).ToList();
				DgAnswers.ItemsSource = null;
			}
		}

		private void LvQuestions_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DgQuestions.SelectedItem is Question selectedQue)
				DgAnswers.ItemsSource = SecurityEntities.GetContext().Answers.Where(x => x.Question.ID == selectedQue.ID).ToList();
		}

		private void BtnEditQue_OnClick(object sender, RoutedEventArgs e)
		{
			var queEdit = new EditQuestionWindow(DgQuestions.SelectedItem as Question, this);
			queEdit.Show();
		}

		private void BtnEditAnswer_OnClick(object sender, RoutedEventArgs e)
		{
			var ansEdit = new EditAnswerWindow(DgAnswers.SelectedItem as Answer, this);
			ansEdit.Show();
		}

		private void BtnAddQue_OnClick(object sender, RoutedEventArgs e)
		{
			var form = new AddQuestionWindow(DgThemes.SelectedItem as Theme, this);
			form.Show();
		}
		private void BtnAddAns_OnClick(object sender, RoutedEventArgs e)
		{
			var form = new AddAnswerWindow(DgQuestions.SelectedItem as Question, this);
			form.Show();
		}
		private void BtnDeleteQue_OnClick(object sender, RoutedEventArgs e)
		{
			if (!(DgQuestions.SelectedItem is Question question)) return;
			var answers = question.Answers.ToList();
			SecurityEntities.GetContext().Answers.RemoveRange(answers);
			SecurityEntities.GetContext().Questions.Remove(question);
			SecurityEntities.GetContext().SaveChanges();
			UpdateQuestionTable();

		}

		private void BtnDeleteAnswer_OnClick(object sender, RoutedEventArgs e)
		{
			if (DgAnswers.SelectedItem is not Answer answer) return;
			SecurityEntities.GetContext().Answers.Remove(answer);
			SecurityEntities.GetContext().SaveChanges();
			UpdateAnswersTable();
		}

		#endregion

		#region Methods

		public void DeleteUser(UserProfile deletedProfile)
		{
			SecurityEntities.GetContext().Users.Remove(deletedProfile.User);
			SecurityEntities.GetContext().UserInfoes.Remove(deletedProfile.UserInfo);
			SecurityEntities.GetContext().SaveChanges();
			UpdateUsersTable();
		}

		public void UpdateUsersTable()
		{
			DgUsers.ItemsSource = SecurityEntities.GetContext().UserInfoes.ToList();
		}

		public void UpdateThemeTable()
		{

		}

		public void UpdateQuestionTable()
		{
			var theme = DgThemes.SelectedItem as Theme;
			DgQuestions.ItemsSource = SecurityEntities.GetContext().Questions.Where(x => x.Theme.ID == theme.ID).ToList();
		}

		public void UpdateAnswersTable()
		{
			if (DgQuestions.SelectedItem is Question selectedQue)
				DgAnswers.ItemsSource = SecurityEntities.GetContext().Answers.Where(x => x.Question.ID == selectedQue.ID).ToList();
		}

		#endregion

		private void BtnImageLoad_OnClick(object sender, RoutedEventArgs e)
		{
			var userInfoes = SecurityEntities.GetContext().UserInfoes.ToList();
			foreach (var userInfo in userInfoes)
			{
				if(userInfo.Image == null) continue;
				if (!Directory.Exists($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\Фотографии\"))
				{
					Directory.CreateDirectory(
						$@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\Фотографии\");
				}
				File.WriteAllBytes($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Экзаменационные листы\Фотографии\{userInfo.IDUser}.jpg", userInfo.Image);
			}
		}
	}
}