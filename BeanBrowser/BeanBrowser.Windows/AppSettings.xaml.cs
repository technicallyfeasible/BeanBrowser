using System;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using BeanBrowser.Common;

namespace BeanBrowser
{
	public sealed partial class AppSettings : SettingsFlyout
	{
		public AppSettings()
		{
			InitializeComponent();
			Loaded += PageLoaded;
		}

		private void PageLoaded(object sender, RoutedEventArgs e)
		{
			LocalDataPath.Text = ApplicationData.Current.LocalFolder.Path;
			RootUrl.Text = SettingsService.RootUrl;
			Boolean useLocal = SettingsService.UseLocalStorage.GetValueOrDefault();
			ButtonRemote.IsChecked = !useLocal;
			ButtonLocal.IsChecked = useLocal;
		}


		private async void SaveClick(object sender, RoutedEventArgs e)
		{
			SettingsService.UseLocalStorage = ButtonLocal.IsChecked;
			SettingsService.RootUrl = RootUrl.Text;

			// if a remote url is used it must be valid
			if (!SettingsService.UseLocalStorage.GetValueOrDefault())
			{
				String error = null;
				try
				{
					Uri uri = new Uri(SettingsService.RootUrl, UriKind.Absolute);
				}
				catch (Exception ex)
				{
					error = ex.Message;
				}
				if (!String.IsNullOrEmpty(error))
				{
					SettingsService.Discard();
					MessageDialog dialog = new MessageDialog(error);
					await dialog.ShowAsync();
					return;
				}
			}

			SettingsService.Save();
			Hide();
		}
	}
}
