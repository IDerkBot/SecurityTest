using SecurityTest.Models.Entities;
using SecurityTest.Views.Pages.AdminPages;
using System.Windows;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для EditQuestionWindow.xaml
	/// </summary>
	public partial class EditQuestionWindow : Window
	{
		private readonly PersonalCabinetAdminPage _currentPage;
		public EditQuestionWindow(Question que, PersonalCabinetAdminPage page)
		{
			InitializeComponent();
			DataContext = que;
			_currentPage = page;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			SecurityEntities.GetContext().SaveChanges();
			_currentPage.UpdateQuestionTable();
			Close();
		}
	}
}
