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
			InstallDirDataPath.Text = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
			RootUrl.Text = SettingsService.RootUrl;
			StorageLocations location = SettingsService.StorageLocation.GetValueOrDefault();
			ButtonRemote.IsChecked = (location == StorageLocations.Remote);
			ButtonInstallDir.IsChecked = (location == StorageLocations.InstallDir);
			ButtonLocal.IsChecked = (location == StorageLocations.Local);
		}


		private async void SaveClick(object sender, RoutedEventArgs e)
		{
			StorageLocations location = StorageLocations.InstallDir;
			if(ButtonLocal.IsChecked.GetValueOrDefault())
				location = StorageLocations.Local;
			else if (ButtonRemote.IsChecked.GetValueOrDefault())
				location = StorageLocations.Remote;

			SettingsService.StorageLocation = location;
			SettingsService.RootUrl = RootUrl.Text;

			// if a remote url is used it must be valid
			if (location == StorageLocations.Remote)
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
