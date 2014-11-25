using System;
using Windows.Storage;

namespace BeanBrowser.Common
{
	public delegate void SettingsChangeEventHandler(Object sender, EventArgs e);


    public class SettingsService
    {
	    private const String RootUrlKey = "rootUrl";
	    private const String UseLocalStorageKey = "useLocalStorage";

		private static String rootUrl;
		private static Boolean? useLocalStorage;


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

	    public static Boolean? UseLocalStorage
	    {
			get { return TryGetValue<Boolean?>(UseLocalStorageKey) ?? true; }
		    set
		    {
			    if ((useLocalStorage ?? TryGetValue<Boolean?>(UseLocalStorageKey)) == value)
				    return;
			    useLocalStorage = value;
		    }
	    }

		/// <summary>
		/// Discard changes without saving
		/// </summary>
	    public static void Discard()
	    {
		    rootUrl = null;
			useLocalStorage = null;
	    }


		/// <summary>
		/// Save settings
		/// </summary>
	    public static void Save()
		{
			if (rootUrl == null && useLocalStorage == null)
				return;

			SetValue(RootUrlKey, rootUrl);
			rootUrl = null;
			SetValue(UseLocalStorageKey, useLocalStorage);
			useLocalStorage = null;
			OnSettingsChanged();
		    
	    }
    }
}
