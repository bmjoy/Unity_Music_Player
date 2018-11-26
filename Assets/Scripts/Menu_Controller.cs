using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Menu_Controller : MonoBehaviour {

	public		Button			button_menu;
	public		GameObject		menu_panel;
	private		bool			menu_active			=	false;

	public		Button			button_closePL;

	public		Button			button_home;
	public		Button			button_playlist;
	public		Button			button_library;
	public		Button			button_explorer;
	public		Button			button_settings;

	public		GameObject		container_explorer;
	public		GameObject		container_library;
	public		GameObject		container_playlist;
	public		GameObject		container_settings;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		button_closePL.onClick.AddListener( delegate { ResetView(); } );
		button_menu.onClick.AddListener( OnMenuClick );
		button_home.onClick.AddListener( OnHomeClick );
		button_playlist.onClick.AddListener( OnPlayListClick );
		button_library.onClick.AddListener( OnLibraryClick );
		button_explorer.onClick.AddListener( OnExplorerClick );
		button_settings.onClick.AddListener( OnSettingsClick );
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	ResetView() {
		GetComponent<Settings>().SaveData();
		container_explorer.GetComponent<Canvas>().enabled	=	false;
		container_library.GetComponent<Canvas>().enabled	=	false;
		container_playlist.GetComponent<Canvas>().enabled	=	false;
		container_settings.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnMenuClick () {
		if (menu_active) { CloseMenu(); } else { OpenMenu(); }
	}

	// ------------------------------------------------------------------------------------------
	public	void	OpenMenu() {
		menu_active	=	true;
		menu_panel.SetActive( menu_active );
	}

	public	void	CloseMenu() {
		menu_active	=	false;
		menu_panel.SetActive( menu_active );
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnHomeClick () {
		OnMenuClick();
		ResetView();
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnPlayListClick() {
		CloseMenu();
		ResetView();
		container_playlist.GetComponent<Canvas>().enabled	=	true;
		container_library.GetComponent<Library_ListManager>().UpdateList();
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnLibraryClick () {
		CloseMenu();
		ResetView();
		container_library.GetComponent<Canvas>().enabled	=	true;
		container_library.GetComponent<Library_ListManager>().UpdateList();
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnExplorerClick () {
		CloseMenu();
		ResetView();
		container_explorer.GetComponent<Canvas>().enabled	=	true;
		container_explorer.GetComponent<Files_ListManager>().UpdateLists();
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnSettingsClick () {
		CloseMenu();
		ResetView();
		container_settings.GetComponent<Canvas>().enabled	=	true;
		container_settings.GetComponent<Settings_UI>().ControllersUpdate();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}

// ####################################################################################################