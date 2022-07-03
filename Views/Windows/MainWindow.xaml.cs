using System;
using System.Windows;
using SecurityTest.Models;
using SecurityTest.Views.Pages;

namespace SecurityTest.Views.Windows
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			PageManager.SetTbTitle(TbTitle);
			PageManager.SetFrame(MainFrame);
			PageManager.SetWindow(this);
			PageManager.Navigate(new AuthorizationPage());
		}

		private void MainFrame_OnContentRendered(object sender, EventArgs e)
		{
			
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			PageManager.GoBack();
		}
	}
}
