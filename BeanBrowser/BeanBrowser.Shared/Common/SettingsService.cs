using System;
using Windows.Storage;

namespace BeanBrowser.Common
{
	public delegate void SettingsChangeEventHandler(Object sender, EventArgs e);

	public enum StorageLocations
	{
		InstallDir,
		Local,
		Remote
	}

    public class SettingsService
    {
	    private const String RootUrlKey = "rootUrl";
	    private const String StorageLocationKey = "storageLocation";

		private static String rootUrl;
		private static StorageLocations? storageLocation;


	    public static event SettingsChangeEventHandler SettingsChanged;
	    protected static void OnSettingsChanged()
	    {
		    SettingsChangeEventHandler handler = SettingsChanged;
		    if (handler != null) handler(null, EventArgs.Empty);
	    }


	    private static T TryGetValue<T>(String key)
	    {
		    Object val;
		    ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out val);
		    return (T) val;
	    }
	    private static void SetValue(String key, Object value)
	    {
			ApplicationData.Current.LocalSettings.Values[key] = value;
	    }


	    public static String RootUrl
	    {
			get { return TryGetValue<String>(RootUrlKey) ?? "http://localhost:8080/index.html"; }
		    set
		    {
			    if ((rootUrl ?? TryGetValue<String>(RootUrlKey)) == value)
				    return;
			    rootUrl = value;
		    }
		}

	    public static StorageLocations? StorageLocation
	    {
			get { return (StorageLocations?) TryGetValue<Int32?>(StorageLocationKey).GetValueOrDefault(); }
		    set
		    {
				if ((storageLocation ?? (StorageLocations?) TryGetValue<Int32?>(StorageLocationKey)) == value)
				    return;
			    storageLocation = value;
		    }
	    }

		/// <summary>
		/// Discard changes without saving
		/// </summary>
	    public static void Discard()
	    {
		    rootUrl = null;
			storageLocation = null;
	    }


		/// <summary>
		/// Save settings
		/// </summary>
	    public static void Save()
		{
			if (rootUrl == null && storageLocation == null)
				return;

			SetValue(RootUrlKey, rootUrl);
			rootUrl = null;
			SetValue(StorageLocationKey, (Int32) storageLocation.GetValueOrDefault());
			storageLocation = null;
			OnSettingsChanged();
		    
	    }
    }
}
