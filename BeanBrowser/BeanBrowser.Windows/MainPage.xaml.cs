using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Windows.UI.Xaml.Navigation;
using Windows.Web;
using BeanBrowser.Common;

namespace BeanBrowser
{
	public sealed class LocalUriStreamResolver : IUriToStreamResolver
	{
		private StorageFolder folder; 

		public LocalUriStreamResolver(StorageLocations location)
		{
			if (location == StorageLocations.Local)
				folder = ApplicationData.Current.LocalFolder;
			else
				folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
		}

		public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
		{
			if (uri == null)
			{
				throw new Exception();
			}
			String path = uri.AbsolutePath;

			// Because of the signature of the this method, it can't use await, so we 
			// call into a seperate helper method that can use the C# await pattern.
			return GetContent(path).AsAsyncOperation();
		}

		private async Task<IInputStream> GetContent(String path)
		{
			try
			{
				//var localFolder = ApplicationData.Current.LocalFolder;
				path = "html\\" + path.TrimStart('/', '\\');
				var requestedFile = await folder.GetFileAsync(path.TrimStart('/', '\\').Replace('/', '\\'));
				var stream = await requestedFile.OpenReadAsync();
				return stream;
			}
			catch (Exception)
			{
				throw new Exception("Invalid path");
			}
		}
	}
	
	
	/// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
	    private NavigationHelper navigationHelper;

	    public MainPage()
        {
            InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += LoadState;
			this.navigationHelper.SaveState += SaveState;

        }

	    private void LoadState(object sender, LoadStateEventArgs e)
	    {
	    }

		private void SaveState(object sender, SaveStateEventArgs e)
	    {
	    }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);

			SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
			SettingsService.SettingsChanged += SettingsChanged;
			UpdateBrowser();
		}

		private void SettingsChanged(object sender, EventArgs e)
		{
			UpdateBrowser();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			SettingsService.SettingsChanged -= SettingsChanged;
			SettingsPane.GetForCurrentView().CommandsRequested -= CommandsRequested;

			navigationHelper.OnNavigatedFrom(e);
		}


		private void UpdateBrowser()
		{
			if (SettingsService.StorageLocation != StorageLocations.Remote)
			{
				Uri uri = this.Browser.BuildLocalStreamUri("data", "index.html");
				Browser.NavigateToLocalStreamUri(uri, new LocalUriStreamResolver(SettingsService.StorageLocation.GetValueOrDefault()));
			}
			else
			{
				try
				{
					Browser.Source = new Uri(SettingsService.RootUrl, UriKind.Absolute);
				}
				catch (Exception ex)
				{
					MessageDialog dialog = new MessageDialog("The root url is not valid: " + ex.Message);
					dialog.ShowAsync();
				}
			}
		}



		/// <summary>
		/// Handler for the CommandsRequested event. Add custom SettingsCommands here.
		/// </summary>
		/// <param name="e">Event data that includes a vector of commands (ApplicationCommands)</param>
		void CommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
		{
			SettingsCommand settingsCommand = new SettingsCommand("appsettings", "Settings", handler =>
			{
				AppSettings sf = new AppSettings();
				sf.Show();
			});
			e.Request.ApplicationCommands.Add(settingsCommand);
		}


		private async void SendVote(Int32 vote)
		{
			try
			{
				await Browser.InvokeScriptAsync("msgVote", new[] { vote.ToString() });
			}
			catch
			{

			}
		}

		private void VoteClickA(object sender, RoutedEventArgs e)
		{
			SendVote(0);
		}
		private void VoteClickB(object sender, RoutedEventArgs e)
		{
			SendVote(1);
		}
		private void VoteClickC(object sender, RoutedEventArgs e)
		{
			SendVote(2);
		}
		private void VoteClickD(object sender, RoutedEventArgs e)
		{
			SendVote(3);
		}

		private void SettingsClick(object sender, RoutedEventArgs e)
		{
			AppSettings sf = new AppSettings();
			sf.Show();
		}
    }
}
