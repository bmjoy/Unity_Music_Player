using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Files_UI : MonoBehaviour {

	private		GameObject			parent;
	private		Files_Library		script_files;
	private		PlayList_Manager	script_playlist;
	private		Settings			script_settings;

	public		GameObject			ui_player;

	private		bool				addAllFiles_status			=	false;
	private		int					addAllFiles_counter			=	0;
	private		int					addAllFiles_max				=	0;

	public		Button				button_Exit;

	public		Button				button_Back;
	public		Button				button_Forward;
	public		Button				button_Home;
	public		GameObject			edit_path;
	public		Text				edit_pathStatic;
	public		Text				edit_pathInput;
	public		Button				button_Navigate;

	public		Button				button_Menu;
	public		GameObject			edit_search;
	public		Text				edit_searchStatic;
	public		Text				edit_searchInput;
	public		Button				button_Search;
	public		Dropdown			combobox_Filter;
	public		Button				button_Attributes;

	public		GameObject			flyout_Menu;
	private		bool				flyout_menuActive			=	false;
	public		Button				button_CreateFile;
	public		Button				button_CreateFolder;

	public		GameObject			flyout_Attributes;
	private		bool				flyout_attributesActive		=	false;
	public		Toggle				checkbox_Folder;
	public		Toggle				checkbox_Hidden;
	public		Toggle				checkbox_System;

	public		RawImage			image_previewIcon;
	public		Text				text_previewName;
	public		Button				button_addAll;
	public		Slider				slider_Sizer;

	public		GameObject			ui_library;
	public		GameObject			input_manager;
	public		GameObject			message_manager;
	public		GameObject			ui_await;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		parent				=	transform.parent.gameObject;
		script_files		=	parent.GetComponent<Files_Library>();
		script_playlist		=	parent.GetComponent<PlayList_Manager>();
		script_settings		=	parent.GetComponent<Settings>();

		button_Exit.onClick.AddListener( OnButtonExit );
		button_Back.onClick.AddListener( OnBackClick );
		button_Forward.onClick.AddListener( OnForwardClick );
		button_Home.onClick.AddListener( OnHomeClick );
		button_Navigate.onClick.AddListener( OnNavigateClick );

		button_Menu.onClick.AddListener( OnMenuClick );
		button_CreateFile.onClick.AddListener( OnNewFileClick );
		button_CreateFolder.onClick.AddListener( OnNewFolderClick );
		button_Search.onClick.AddListener( OnSearchClick );
		combobox_Filter.onValueChanged.AddListener( delegate { OnFilterChange( combobox_Filter.value ); } );
		button_Attributes.onClick.AddListener( OnAttributesClick );
		checkbox_Folder.onValueChanged.AddListener( delegate { OnAttributesChange( CustomFileAttributes.folder, checkbox_Folder.isOn ); } );
		checkbox_Hidden.onValueChanged.AddListener( delegate { OnAttributesChange( CustomFileAttributes.hidden, checkbox_Hidden.isOn ); } );
		checkbox_System.onValueChanged.AddListener( delegate { OnAttributesChange( CustomFileAttributes.system, checkbox_System.isOn ); } );
		button_addAll.onClick.AddListener( OnAddAllClick );
		slider_Sizer.onValueChanged.AddListener( delegate { OnSizeChange( slider_Sizer.value ); } );

		script_files.Init();
		GetComponent<Files_ListManager>().Init();
		UpdateUI();
	}
	
	// ------------------------------------------------------------------------------------------
	private	void	OnGUI() {
		if ( edit_path.GetComponent<InputField>().isFocused && Input.GetKey(KeyCode.Return) ) {
			OnNavigateClick();
		}

		if ( edit_search.GetComponent<InputField>().isFocused && Input.GetKey(KeyCode.Return) ) {
			OnSearchClick();
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	Update() {
		if ( addAllFiles_status ) { AddAllFilesWork(); }
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	OnButtonExit() {
		gameObject.GetComponent<Canvas>().enabled	=	false;
		parent.GetComponent<Menu_Controller>().ResetView();
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnBackClick() {
		script_files.GoBack();
		UpdateUI();
	}

	private	void	OnForwardClick() {
		script_files.GoForward();
		UpdateUI();
	}

	private	void	OnHomeClick() {
		script_files.GoHome();
		UpdateUI();
	}

	private	void	OnNavigateClick() {
		script_files.setPath( edit_pathInput.text );
		edit_pathInput.text = "";
		UpdateUI();
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnMenuClick() {
		flyout_menuActive = !flyout_menuActive;
		flyout_Menu.SetActive( flyout_menuActive );
	}

	private	void	OnSearchClick() {
		script_files.SetFilterSearch( edit_searchInput.text );
		edit_searchInput.text = "";
		script_files.ScanUpdate();
		UpdateUI();
	}

	private	void	OnFilterChange( int value ) {
		script_files.SetFilterType( value );
		script_files.ScanUpdate();
		UpdateUI();
	}

	private	void	OnAttributesClick() {
		flyout_attributesActive = !flyout_attributesActive;
		flyout_Attributes.SetActive( flyout_attributesActive );
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnNewFileClick() {
		input_manager.GetComponent<Msg_Input>().Init( "Creating new File", "NewFile", InputManager_OnFileOKClick );
		input_manager.GetComponent<Canvas>().enabled	=	true;
		flyout_Menu.SetActive( false );
	}

	private	void	OnNewFolderClick() {
		input_manager.GetComponent<Msg_Input>().Init( "Creating new Folder", "NewFolder", InputManager_OnDirectoryOKClick );
		input_manager.GetComponent<Canvas>().enabled	=	true;
		flyout_Menu.SetActive( false );
	}
	
	public	void	InputManager_OnFileOKClick() {
		string	file_name	=	input_manager.GetComponent<Msg_Input>().edit_inputInput.text;
		if ( script_files.CreateFile( file_name ) ) { flyout_menuActive = false; input_manager.GetComponent<Canvas>().enabled = flyout_menuActive; }
		script_files.ScanUpdate();
		UpdateUI();
	}

	public	void	InputManager_OnDirectoryOKClick() {
		string	file_name	=	input_manager.GetComponent<Msg_Input>().edit_inputInput.text;
		if ( script_files.CreateDirectory( file_name ) ) { flyout_menuActive = false; input_manager.GetComponent<Canvas>().enabled = flyout_menuActive; }
		script_files.ScanUpdate();
		UpdateUI();
	}
		
	private	void	OnAttributesChange( CustomFileAttributes attribute, bool value ) {
		flyout_attributesActive = false;
		flyout_Attributes.SetActive( flyout_attributesActive );
		script_files.SetAttrib( attribute, value );
		script_files.ScanUpdate();
		UpdateUI();
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnSizeChange( float position ) {
		if ( position <= 0.5f && GetComponent<Files_ListManager>().getView() == CustomTemplateView.Icon ) {
			GetComponent<Files_ListManager>().setView( CustomTemplateView.List ); UpdateUI();
		} else if ( position > 0.5f && GetComponent<Files_ListManager>().getView() == CustomTemplateView.List ) {
			GetComponent<Files_ListManager>().setView( CustomTemplateView.Icon ); UpdateUI();
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnOneClick( object[] args ) {
		image_previewIcon.texture	=	Resources.Load( "Icons/White/Files/" + args[1].ToString() ) as Texture;
		text_previewName.text		=	args[2].ToString();
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnDoubleClick( object[] args ) {
		int					index	=	int.Parse( args[0].ToString() );
		CustomTemplateType	type	=	(CustomTemplateType) args[3];

		image_previewIcon.texture	=	Resources.Load( "Icons/64_Empty" ) as Texture;;
		text_previewName.text		=	"";

		if ( script_files.isDirectory( index ) && type == CustomTemplateType.Directory ) {
			script_files.Navigate( script_files.getDirName( index ) );
			UpdateUI();
		} else if ( script_files.isFile( index ) && type == CustomTemplateType.File ) {
			OnAddClick( args );
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnAddClick( object[] args ) {
		int					index		=	int.Parse( args[0].ToString() );
		CustomTemplateType	type		=	(CustomTemplateType) args[3];

		if ( script_files.isFile( index ) && type == CustomTemplateType.File ) {
			if ( script_files.GetFileExt(script_files.getFullFileName( index )) == ".upl" ) {
				ui_library.GetComponent<Library_UI>().OnLoadPlayList( args[1].ToString(), script_files.getFileName( index ), script_files.getFullFileName( index ) );
				return;
			}

			string			ext			=	script_files.GetFileExt(script_files.getFullFileName( index )).ToLower();
			if (  ext == ".ogg" || ext == ".wav" ) {
				ui_library.GetComponent<Library_UI>().Add( args[1].ToString(), script_files.getFileName( index ), script_files.getFullFileName( index ) );
				if ( script_settings.auto_play ) { ui_player.GetComponent<Player_UI>().LaunchLast(); }
			} else {
				string		title		=	"Unknown " + ext + " file";
				string		context		=	"File with this extension canno't be loaded becouse is not supported by Unity import library.";
				message_manager.GetComponent<Canvas>().enabled	=	true;
				message_manager.GetComponent<Msg_Box>().Init( title, context, CustomMessageIconType.Locked, OnAddClick_Error, args );
			}
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnAddClick_Error( object[] args ) {
		message_manager.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnAddAllClick( ) {
		AddAllFilesInit();
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddAllFilesInit() {
		addAllFiles_counter		=	0;
		addAllFiles_max			=	script_files.getFileCount();

		if ( script_files.GetAttrib( CustomFileAttributes.folder ) ) {
			addAllFiles_counter		=	script_files.getDirsCount();
			addAllFiles_max			=	addAllFiles_max + addAllFiles_counter;
		}

		ui_await.GetComponent<Canvas>().enabled		=	true;
		ui_await.GetComponent<Msg_Awaiter>().Show( "Importing Files...", "Please white until loading files it's over." );
		ui_await.GetComponent<Msg_Awaiter>().ShowProgressBar( true, addAllFiles_max );
		addAllFiles_status			=	true;
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddAllFilesWork() {
		if ( addAllFiles_counter == addAllFiles_max ) {
			addAllFiles_status	=	false;
			AddAllFilesFinish();
			return;
		}

		try {
			GameObject	item	=	GetComponent<Files_ListManager>().GetFileItem( addAllFiles_counter );
			if ( item != null ) {
				int					index		=	item.GetComponent<Temp_File>().getIndex();
				CustomTemplateType	type		=	item.GetComponent<Temp_File>().getType();
				string				icon_res	=	item.GetComponent<Temp_File>().getIcon();
				string				file_name	=	item.GetComponent<Temp_File>().getName();

				string				title		=	"Importing Files...";
				string				counter		=	addAllFiles_counter.ToString() + " / " + addAllFiles_max.ToString();
				string				content		=	"Please white until loading files it's over. \n" + file_name + " " + counter;
				ui_await.GetComponent<Msg_Awaiter>().Show( title, content  );
				if ( script_files.isFile( index ) && type == CustomTemplateType.File ) {
					ui_library.GetComponent<Library_UI>().AddNoUpdate( icon_res, file_name, script_files.getFullFileName( index ) );
				}
			}

			addAllFiles_counter++;
			ui_await.GetComponent<Msg_Awaiter>().Increment( 1 );
		
		} catch ( System.Exception ) {
			AddAllFilesFinish();
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddAllFilesFinish() {
		addAllFiles_status		=	false;
		ui_await.GetComponent<Msg_Awaiter>().ShowProgressBar( false, 0 );
		ui_await.GetComponent<Canvas>().enabled		=	false;
		ui_library.GetComponent<Library_UI>().UpdateUI();

		if ( script_settings.auto_play ) { ui_player.GetComponent<Player_UI>().LaunchLast(); }
		if ( script_settings.auto_lib ) {
			this.parent.GetComponent<Menu_Controller>().ResetView();
			this.parent.GetComponent<Menu_Controller>().OnLibraryClick();
			this.parent.GetComponent<Menu_Controller>().menu_panel.SetActive(false);
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnRemoveClick( object[] args ) {
		string		title		=	"Deleting file";
		string		context		=	"Do you want to delete " + args[1].ToString() + " file?";

		message_manager.GetComponent<Canvas>().enabled	=	true;
		message_manager.GetComponent<Msg_Box>().Init( title, context, CustomMessageIconType.Stop, MessageManager_OnRemoveYesClick, args );
	}

	// ------------------------------------------------------------------------------------------
	public	void	MessageManager_OnRemoveYesClick( object[] args ) {
		int				index		=	int.Parse( args[0].ToString() );
		CustomTemplateType	type	=	(CustomTemplateType) args[2];

		message_manager.GetComponent<Canvas>().enabled	=	false;
		if ( script_files.isFile( index ) && type == CustomTemplateType.File ) {
			script_files.DeleteFile( index );
			script_files.ScanUpdate();
			UpdateUI();
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	UpdateUI() {
		edit_pathStatic.text	=	script_files.getPath();
		GetComponent<Files_ListManager>().UpdateLists();
	}
	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################