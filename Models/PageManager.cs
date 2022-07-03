using System;
using System.Windows;
using System.Windows.Controls;
using SecurityTest.Views.Pages;

namespace SecurityTest.Models
{
	internal class PageManager
	{
		private static Frame MainFrame { get; set; }
		private static TextBlock Title { get; set; }
		private static Window Window { get; set; }

		public static void SetFrame(Frame frame)
		{
			MainFrame = frame;
		}

		public static Page GetPage()
		{
			return MainFrame.Content as Page;
		}

		public static void Navigate(Page moveToPage)
		{
			MainFrame.Navigate(moveToPage);
		}

		public static void GoBack()
		{
			if (!MainFrame.CanGoBack) return;
			if (MainFrame.Content.ToString().ToLower().Contains("personalcabinet"))
			{
				GC.Collect();
				MainFrame.Navigate(new AuthorizationPage());
			}
			else
			{
				GC.Collect();
				MainFrame.GoBack();
			}
		}

		public static void Clear()
		{

		}

		public static void SetTbTitle(TextBlock textBlock)
		{
			Title = textBlock;
		}

		public static void SetTitle(string title)
		{
			if (Title != null)
				Title.Text = title;
		}

		public static void SetWindow(Window window)
		{
			Window = window;
		}

		public static void Hide()
		{
			Window.Hide();
		}

		public static void Show()
		{
			Window.Show();
		}
	}
}