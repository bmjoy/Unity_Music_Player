using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public enum CustomTemplateView { List, Icon }
public enum CustomTemplateType { Directory, File }

// ####################################################################################################
public class Files_ListManager : MonoBehaviour {

	private		CustomTemplateView		view			=	CustomTemplateView.List;
	private		GameObject[]	items_directories		=	new GameObject[0];
	private		GameObject[]	items_files				=	new GameObject[0];

	private		GameObject		parent;
	private		Files_Library	script_files;

	private		const int		itemLIST_HEIGHT			=	32;
	private		const int		itemLIST_SPACING		=	4;

	private		const int		itemICON_HEIGHT			=	64;
	private		const int		itemICON_WIDTH			=	128;
	private		const int		itemICON_SPACING		=	4;

	private		const int		itemSCROLLBAR_WIDTH		=	20;

	public		GameObject		container_directories;	
	public		GameObject		container_files;		
	public		GameObject		template_folderlist;	
	public		GameObject		template_filelist;		
	public		GameObject		template_fileicon;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		parent			=	transform.parent.gameObject;
		script_files	=	parent.GetComponent<Files_Library>();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	CustomTemplateView	getView() { return view; }

	public	void	setView( CustomTemplateView viewType ) {
		DestroyFileLists();
		view	=	viewType;
		GenerateFileLists();
	}

	// ------------------------------------------------------------------------------------------
	public	void	UpdateLists() {
		DestroyFolderList();
		DestroyFileLists();
		GenerateFolderList();
		GenerateFileLists();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	GenerateFolderList() {
		var		container		=	container_directories.GetComponent<RectTransform>();
		int		count_folders	=	script_files.getDirsCount();

		items_directories		=	new GameObject[count_folders];
		for ( int index = 0; index < count_folders; index ++ ) { AddObjectFolderItem( index ); }

		int		size_y			=	(itemLIST_HEIGHT * count_folders) + (itemICON_SPACING * count_folders+1);
		container.sizeDelta		=	new Vector2( 0, size_y );
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddObjectFolderItem( int index ) {
		var		item			=	Instantiate( template_folderlist, container_directories.transform );
		var		container		=	item.GetComponent<RectTransform>();
		var		button			=	item.transform.GetChild(0).gameObject;
		var		image			=	button.transform.GetChild(0).gameObject;
		var		text			=	button.transform.GetChild(1).gameObject;
		int		posX			=	itemLIST_SPACING;
		int		posZ			=	(itemLIST_HEIGHT * index) + (itemLIST_SPACING * (index+1));

		item.name				=	"ItemFolder_" + index.ToString();
		container.offsetMin		=	new Vector2( posX, itemLIST_HEIGHT );
		container.offsetMax		=	new Vector2( -posX, -posZ );
		container.sizeDelta		=	new Vector2( container.sizeDelta.x, itemLIST_HEIGHT );

		item.GetComponent<Temp_Folder>().SetOneClick( GetComponent<Files_UI>().OnOneClick );
		item.GetComponent<Temp_Folder>().SetDoubleClick( GetComponent<Files_UI>().OnDoubleClick );
		item.GetComponent<Temp_Folder>().SetText( script_files.getDirName( index ) );

		if ( script_files.isDisks() ) {
			item.GetComponent<Temp_Folder>().SetImage( "64_HDD" );
		} else {
			item.GetComponent<Temp_Folder>().SetImage( "64_FolderOpen" );
		}

		item.SetActive( true );
		items_directories[index]				=	item;
	}

	// ------------------------------------------------------------------------------------------
	private	void	DestroyFolderList() {
		for ( int i = 0; i<items_directories.Length; i++ ) { Destroy( items_directories[i] ); }
		items_directories	=	new GameObject[0];
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	GenerateFileLists() {
		switch ( view ) {
			case CustomTemplateView.Icon: 	GenerateFileIcons();	break;
			case CustomTemplateView.List:	GenerateFileList();		break;
		}
	}

	private	void	DestroyFileLists() {
		switch ( view ) {
		case CustomTemplateView.Icon: 	DestroyFileIcons();		break;
		case CustomTemplateView.List:	DestroyFileList();		break;
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	GenerateFileList() {
		var		container		=	container_files.GetComponent<RectTransform>();
		int		count_dirs		=	script_files.getDirsCount();
		int		count_files		=	script_files.getFileCount();
		int		i_dirs			=	0;
		int		i_files			=	0;

		items_files				=	new GameObject[count_dirs + count_files];

		if ( script_files.GetAttrib( CustomFileAttributes.folder ) ) {
			for ( i_dirs = 0; i_dirs < count_dirs; i_dirs ++ ) {
				AddObjectFolderFileItem( i_dirs, i_dirs );
			}
		}

		for ( i_files = 0; i_files < count_files; i_files ++ ) {
			AddObjectFileItem( i_files, i_dirs+i_files );
		}

		int		size_y			=	(itemLIST_HEIGHT * (i_dirs + i_files)) + (itemICON_SPACING * (i_dirs + i_files + 1));
		container.sizeDelta		=	new Vector2( 0, size_y );
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddObjectFolderFileItem( int index, int position ) {
		var		item			=	Instantiate( template_filelist, container_files.transform );
		var		container		=	item.GetComponent<RectTransform>();
		var		button_select	=	item.transform.GetChild(0).gameObject;
		var		button_add		=	item.transform.GetChild(1).gameObject;
		var		button_remove	=	item.transform.GetChild(2).gameObject;
		var		image			=	button_select.transform.GetChild(0).gameObject;
		var		text			=	button_select.transform.GetChild(1).gameObject;
		int		posX			=	itemLIST_SPACING;
		int		posZ			=	(itemLIST_HEIGHT * index) + (itemLIST_SPACING * (index+1));

		item.name				=	"ItemFolder_" + index.ToString();
		container.offsetMin		=	new Vector2( posX, itemLIST_HEIGHT );
		container.offsetMax		=	new Vector2( -posX, -posZ );
		container.sizeDelta		=	new Vector2( container.sizeDelta.x, itemLIST_HEIGHT );

		item.GetComponent<Temp_File>().SetOneClick( GetComponent<Files_UI>().OnOneClick );
		item.GetComponent<Temp_File>().SetDoubleClick( GetComponent<Files_UI>().OnDoubleClick );
		item.GetComponent<Temp_File>().SetAddClick( GetComponent<Files_UI>().OnAddClick );
		item.GetComponent<Temp_File>().SetRemoveClick( GetComponent<Files_UI>().OnRemoveClick );
		item.GetComponent<Temp_File>().SetText( script_files.getDirName( index ) );
		item.GetComponent<Temp_File>().SetType( CustomTemplateType.Directory );

		if ( script_files.isDisks() ) {
			SetIcon( item, "folder-hdd" );
		} else {
			switch ( script_files.getDirName( index ).ToLower() ) {
			case "documents":	SetIcon( item, "folder-doc" );		break;
			case "links":		SetIcon( item, "folder-net" );		break;
			case "music":		SetIcon( item, "folder-music" );	break;
			case "pictures":	SetIcon( item, "folder-img" );		break;
			case "videos":		SetIcon( item, "folder-vid" );		break;
			default:			SetIcon( item, "folder-closed" );	break;
			}
		}

		item.SetActive( true );
		items_files[position]				=	item;
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddObjectFileItem( int index, int position ) {
		var		item			=	Instantiate( template_filelist, container_files.transform );
		var		container		=	item.GetComponent<RectTransform>();
		var		button_select	=	item.transform.GetChild(0).gameObject;
		var		button_add		=	item.transform.GetChild(1).gameObject;
		var		button_remove	=	item.transform.GetChild(2).gameObject;
		var		image			=	button_select.transform.GetChild(0).gameObject;
		var		text			=	button_select.transform.GetChild(1).gameObject;
		int		posX			=	itemLIST_SPACING;
		int		posZ			=	(itemLIST_HEIGHT * position) + (itemLIST_SPACING * (position+1));

		item.name				=	"ItemFile_" + index.ToString();
		container.offsetMin		=	new Vector2( posX, itemLIST_HEIGHT );
		container.offsetMax		=	new Vector2( -posX, -posZ );
		container.sizeDelta		=	new Vector2( container.sizeDelta.x, itemLIST_HEIGHT );

		item.GetComponent<Temp_File>().SetOneClick( GetComponent<Files_UI>().OnOneClick );
		item.GetComponent<Temp_File>().SetDoubleClick( GetComponent<Files_UI>().OnDoubleClick );
		item.GetComponent<Temp_File>().SetAddClick( GetComponent<Files_UI>().OnAddClick );
		item.GetComponent<Temp_File>().SetRemoveClick( GetComponent<Files_UI>().OnRemoveClick );
		item.GetComponent<Temp_File>().SetText( script_files.getFileName( index ) );
		item.GetComponent<Temp_File>().SetType( CustomTemplateType.File );

		string	ext		=	script_files.GetFileExt( script_files.getPath(), script_files.getFileName(index) ).ToLower();	
		SetIcon( item, ext );

		item.SetActive( true );
		items_files[position]				=	item;
	}

	// ------------------------------------------------------------------------------------------
	private	void	DestroyFileList() {
		for ( int i = 0; i<items_files.Length; i++ ) {
			Destroy( items_files[i] );
		}
		items_files		=	new GameObject[0];
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private void 	CalculateFileIcon( int width, ref int posX, ref int posZ ) {
		if ( posX + (itemICON_WIDTH*2) + itemICON_SPACING < width ) {
			posX = posX + itemICON_WIDTH + itemICON_SPACING;
		} else {
			posX = itemICON_SPACING;
			posZ = posZ + itemICON_HEIGHT + itemICON_SPACING;
		}
	}

	// ------------------------------------------------------------------------------------------
	private	int		CalculateWidth( GameObject container ) {
		//	X 0 Left		X 0.5 Center		X 1 Right
		//	Y 0 Down		Y 0.5 Center		Y 1 Up
		//	Min 0 Max 1 Stretch
		var		parent	=	container;
		var		rect	=	container.GetComponent<RectTransform>();

		int		y_left	=	0;
		int		y_right	=	0;

		while ( parent.name != "UI" ) {
			y_left		=	y_left + (int) rect.offsetMin.x;
			y_right		=	y_right + Mathf.Abs((int) rect.offsetMax.x);
			parent		=	parent.transform.parent.gameObject;
			rect		=	parent.GetComponent<RectTransform>();
		}

		return (int) rect.sizeDelta.x - (y_left + y_right);
	}

	// ------------------------------------------------------------------------------------------
	private	void	GenerateFileIcons() {
		var		container			=	container_files.GetComponent<RectTransform>();
		int		container_width		=	CalculateWidth( container_files );
		int		count_dirs			=	script_files.getDirsCount();
		int		count_files			=	script_files.getFileCount();
		int		i_dirs				=	0;
		int		i_files				=	0;
		int		posX				=	itemICON_SPACING;
		int		posZ				=	itemICON_SPACING;

		items_files		=	new GameObject[count_dirs + count_files];

		if ( script_files.GetAttrib( CustomFileAttributes.folder ) ) {
			for ( i_dirs = 0; i_dirs < count_dirs; i_dirs ++ ) {
				AddObjectFolderFileIcon( i_dirs, i_dirs, posX, posZ );
				CalculateFileIcon( container_width, ref posX, ref posZ );
			}
		}

		if ( posX != itemICON_SPACING ) {
			posZ = posZ + itemICON_HEIGHT + itemICON_SPACING;
			posX = itemICON_SPACING;
		}

		for ( i_files = 0; i_files < count_files; i_files ++ ) {
			AddObjectFileIcon( i_files, i_files + i_dirs, posX, posZ );
			CalculateFileIcon( container_width, ref posX, ref posZ );
		}

		posZ = posZ + itemICON_HEIGHT + itemICON_SPACING;
		container.sizeDelta		=	new Vector2( 0, posZ  );
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddObjectFolderFileIcon( int index, int position, int posX, int posZ ) {
		var		item			=	Instantiate( template_fileicon, container_files.transform );
		var		container		=	item.GetComponent<RectTransform>();
		var		button_select	=	item.transform.GetChild(0).gameObject;
		var		button_add		=	item.transform.GetChild(1).gameObject;
		var		button_remove	=	item.transform.GetChild(2).gameObject;
		var		image			=	button_select.transform.GetChild(0).gameObject;
		var		text			=	button_select.transform.GetChild(1).gameObject;

		item.name				=	"ItemFolder_" + index.ToString();
		container.offsetMin		=	new Vector2( posX, itemICON_HEIGHT );
		container.offsetMax		=	new Vector2( itemICON_WIDTH, -posZ );
		container.sizeDelta		=	new Vector2( itemICON_WIDTH, itemICON_HEIGHT );

		item.GetComponent<Temp_File>().SetOneClick( GetComponent<Files_UI>().OnOneClick );
		item.GetComponent<Temp_File>().SetDoubleClick( GetComponent<Files_UI>().OnDoubleClick );
		item.GetComponent<Temp_File>().SetAddClick( GetComponent<Files_UI>().OnAddClick );
		item.GetComponent<Temp_File>().SetRemoveClick( GetComponent<Files_UI>().OnRemoveClick );
		item.GetComponent<Temp_File>().SetText( script_files.getDirName( index ) );
		item.GetComponent<Temp_File>().SetType( CustomTemplateType.Directory );

		if ( script_files.isDisks() ) {
			SetIcon( item, "folder-hdd" );
		} else {
			switch ( script_files.getDirName( index ).ToLower() ) {
			case "documents":	SetIcon( item, "folder-doc" );		break;
			case "links":		SetIcon( item, "folder-net" );		break;
			case "music":		SetIcon( item, "folder-music" );	break;
			case "pictures":	SetIcon( item, "folder-img" );		break;
			case "videos":		SetIcon( item, "folder-vid" );		break;
			default:			SetIcon( item, "folder-closed" );	break;
			}
		}

		item.SetActive( true );
		items_files[position]				=	item;
	}

	// ------------------------------------------------------------------------------------------
	private	void	AddObjectFileIcon( int index, int position, int posX, int posZ ) {
		var		item			=	Instantiate( template_fileicon, container_files.transform );
		var		container		=	item.GetComponent<RectTransform>();
		var		button_select	=	item.transform.GetChild(0).gameObject;
		var		button_add		=	item.transform.GetChild(1).gameObject;
		var		button_remove	=	item.transform.GetChild(2).gameObject;
		var		image			=	button_select.transform.GetChild(0).gameObject;
		var		text			=	button_select.transform.GetChild(1).gameObject;

		item.name				=	"ItemFile_" + index.ToString();
		container.offsetMin		=	new Vector2( posX, itemICON_HEIGHT );
		container.offsetMax		=	new Vector2( itemICON_WIDTH, -posZ );
		container.sizeDelta		=	new Vector2( itemICON_WIDTH, itemICON_HEIGHT );

		item.GetComponent<Temp_File>().SetOneClick( GetComponent<Files_UI>().OnOneClick );
		item.GetComponent<Temp_File>().SetDoubleClick( GetComponent<Files_UI>().OnDoubleClick );
		item.GetComponent<Temp_File>().SetAddClick( GetComponent<Files_UI>().OnAddClick );
		item.GetComponent<Temp_File>().SetRemoveClick( GetComponent<Files_UI>().OnRemoveClick );
		item.GetComponent<Temp_File>().SetText( script_files.getFileName( index ) );
		item.GetComponent<Temp_File>().SetType( CustomTemplateType.File );

		string	ext		=	script_files.GetFileExt( script_files.getPath(), script_files.getFileName(index) ).ToLower();	
		SetIcon( item, ext );

		item.SetActive( true );
		items_files[position]				=	item;
	}

	// ------------------------------------------------------------------------------------------
	private	void	DestroyFileIcons() {
		for ( int i = 0; i<items_files.Length; i++ ) {
			Destroy( items_files[i] );
		}
		items_files		=	new GameObject[0];
	}
		
	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	GameObject	GetFileItem( int index ) {
		if ( index >= 0 && index < items_files.Length ) {
			return items_files[index];
		}
		return null;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	SetIcon( GameObject item, string ext ) {
		switch ( ext ) {
			// Audio
		case ".acc":	item.GetComponent<Temp_File>().SetImage( "64_FileAudio" );			break;
		case ".ogg":	item.GetComponent<Temp_File>().SetImage( "64_OGG" );				break;
		case ".mp3":	item.GetComponent<Temp_File>().SetImage( "64_FileAudio" );			break;
		case ".wav":	item.GetComponent<Temp_File>().SetImage( "64_FileAudio" );			break;
		case ".wma":	item.GetComponent<Temp_File>().SetImage( "64_FileAudio" );			break;
			// Images
		case ".bmp":	item.GetComponent<Temp_File>().SetImage( "64_FileImage" );			break;
		case ".gif":	item.GetComponent<Temp_File>().SetImage( "64_GIF" );				break;
		case ".ico":	item.GetComponent<Temp_File>().SetImage( "64_FileImage" );			break;
		case ".jpg":	item.GetComponent<Temp_File>().SetImage( "64_JPEG" );				break;
		case ".jpeg":	item.GetComponent<Temp_File>().SetImage( "64_JPEG" );				break;
		case ".png":	item.GetComponent<Temp_File>().SetImage( "64_FileImage" );			break;
		case ".tiff":	item.GetComponent<Temp_File>().SetImage( "64_FileImage" );			break;
			// Documents
		case ".txt":	item.GetComponent<Temp_File>().SetImage( "64_FileDocument" );		break;
		case ".pdf":	item.GetComponent<Temp_File>().SetImage( "64_PDF" );				break;
		case ".doc":	item.GetComponent<Temp_File>().SetImage( "64_FileDocument" );		break;
		case ".docx":	item.GetComponent<Temp_File>().SetImage( "64_FileDocument" );		break;
			// Video
		case ".avi":	item.GetComponent<Temp_File>().SetImage( "64_AVI" );				break;
		case ".mp4":	item.GetComponent<Temp_File>().SetImage( "64_FileVideo" );			break;
		case ".mpg":	item.GetComponent<Temp_File>().SetImage( "64_FileVideo" );			break;
		case ".mpeg":	item.GetComponent<Temp_File>().SetImage( "64_FileVideo" );			break;
		case ".wmv":	item.GetComponent<Temp_File>().SetImage( "64_FileVideo" );			break;
			// Others
		case ".exe":	item.GetComponent<Temp_File>().SetImage( "64_EXE" );						break;
			// Folders
		case "folder-hdd":		item.GetComponent<Temp_File>().SetImage( "64_HDD" );				break;
		case "folder-closed":	item.GetComponent<Temp_File>().SetImage( "64_Folder" );				break;
		case "folder-opened":	item.GetComponent<Temp_File>().SetImage( "64_FolderOpen" );			break;
		case "folder-doc":		item.GetComponent<Temp_File>().SetImage( "64_FolderDocuments" );	break;
		case "folder-net":		item.GetComponent<Temp_File>().SetImage( "64_FolderNetwork" );		break;
		case "folder-music":	item.GetComponent<Temp_File>().SetImage( "64_FolderMusic" );		break;
		case "folder-img":		item.GetComponent<Temp_File>().SetImage( "64_FolderPictures" );		break;
		case "folder-vid":		item.GetComponent<Temp_File>().SetImage( "64_Folder" );				break;
		default:				item.GetComponent<Temp_File>().SetImage( "64_File" );				break;
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################