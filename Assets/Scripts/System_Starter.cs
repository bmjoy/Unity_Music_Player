using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class System_Starter : MonoBehaviour {

	public		GameObject		ui_explorer;
	public		GameObject		ui_library;
	public		GameObject		ui_settings;
	public		GameObject		ui_playlist;
	public		GameObject		ui_mediaplayer;
	public		GameObject		ui_mainemnu;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() { Init(); }

	public	void	Init () {
		var	platform = Application.platform;
		Debug.Log( "Running " + platform.ToString() );

		switch( platform ) {
			case RuntimePlatform.WindowsEditor: return;
			case RuntimePlatform.WindowsPlayer: return;
			default:
				Application.Quit();
				break;
		}

		//GetComponent<Settings>().Init();
		//GetComponent<Files_Library>().Init();
		//GetComponent<PlayList_Manager>().Init();
		//GetComponent<Player>().Init();
		//GetComponent<Menu_Controller>().Init();

		//ui_settings.GetComponent<Settings_UI>().Init();
		//ui_mediaplayer.GetComponent<Player_UIAnim>().Init();
		//ui_mediaplayer.GetComponent<Player_UI>().Init();
		//ui_explorer.GetComponent<Files_ListManager>().Init();
		//ui_explorer.GetComponent<Files_UI>().Init();
		//ui_library.GetComponent<Library_ListManager>().Init();
		//ui_library.GetComponent<Library_UI>().Init();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################