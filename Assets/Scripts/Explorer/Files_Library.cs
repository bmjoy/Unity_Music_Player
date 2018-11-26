using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ####################################################################################################
public enum CustomFileAttributes { folder, hidden, system }

// ####################################################################################################
public class Files_Library : MonoBehaviour {

	private		string			path_current	=	"";
	private		string			path_home		=	"";
	private		ArrayList		list_disks		=	new ArrayList();
	private		ArrayList		list_dirs		=	new ArrayList();
	private		ArrayList		list_files		=	new ArrayList();

	private		bool			vis_folder		=	true;
	private		bool			vis_hidden		=	false;
	private		bool			vis_system		=	false;

	private		int				filter_type		=	0;
	private		string			filter_search	=	"";

	private		ArrayList		history			=	new ArrayList();
	private		int				history_pos		=	-1;

	public		string[]		filetype_doc	=	new string[0];
	public		string[]		filetype_img	=	new string[0];
	public		string[]		filetype_mus	=	new string[0];
	public		string[]		filetype_vid	=	new string[0];

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		ScanDisks();
		path_current	=	list_disks[0].ToString();
		path_current	=	System.Environment.GetFolderPath( System.Environment.SpecialFolder.MyMusic );
		path_home		=	System.Environment.GetFolderPath( System.Environment.SpecialFolder.Personal );
		ScanUpdate();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	isDisks() { return ( path_current == "" ); }
	public	string	getPath() { return path_current; }
	public	bool	setPath( string path_new ) {
		
		string	path_active		=	path_current;
		bool	result			=	false;

		if ( Directory.Exists( path_current ) ) {
			path_current		=	path_new;
			try { ScanUpdate(); } catch( System.Exception ) { path_current = path_active; ScanUpdate(); result = false; }
		}

		ClearHistoryForward( history_pos );
		return result;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	GoBack() {
		bool	result			=	false;
		if ( path_current == "" ) { return result; }
		string	path_active		=	path_current;
		string	path_new		=	Path.GetDirectoryName( path_current );

		if ( Directory.Exists( path_new ) ) {
			path_current		=	path_new;
			result				=	true;
		} else {
			path_current		=	"";
		}

		try { ScanUpdate(); } catch( System.Exception ) { path_current = path_active; ScanUpdate(); result = false; }
		if ( path_current == path_new ) { history_pos++; history.Add( path_active ); }
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	GoForward() {
		if ( history.Count <= 0 || history_pos < 0 ) { return false; }

		bool	result			=	false;
		string	path_new		=	history[history_pos].ToString();
		string	path_active		=	path_current;
		history_pos--;

		if ( Directory.Exists( path_new ) ) {
			path_current		=	path_new;
			result				=	true;
		} else {
			result				=	false;
		}

		try { ScanUpdate(); } catch( System.Exception ) { path_current = path_active; ScanUpdate(); result = false; }
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	Navigate( string dir_name ) {
		string	path_new		=	"";
		string	path_active		=	path_current;
		bool	result			=	false;

		if ( Directory.Exists( path_current + dir_name ) ) { path_new = path_current + dir_name; }
		else if ( Directory.Exists( path_current + "\\" + dir_name ) ) { path_new = path_current + "\\" + dir_name; }
		if ( path_new != "" ) {
			path_current		=	path_new;
			result				=	true;
		}
			
		try { ScanUpdate(); } catch( System.Exception ) { path_current = path_active; ScanUpdate(); result = false; }
		ClearHistoryForward( history_pos );
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	GoHome() {
		string	path_active		=	path_current;
		bool	result			=	false;

		if ( Directory.Exists( path_home ) ) { path_current = path_home; result = true; }
		try { ScanUpdate(); } catch( System.Exception ) { path_current = path_active; ScanUpdate(); result = false; }
		ClearHistoryForward( history_pos );
		return result;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	CreateDirectory( string name ) {
		string	last_char		=	"\\";

		if (!Directory.Exists( path_current + name ) && !Directory.Exists( path_current + "\\" + name )) {
			last_char			=	path_current.Substring( path_current.Length-1, 1 );

			if ( last_char == "\\" ) {
				try { Directory.CreateDirectory( path_current + name ); } catch (System.Exception) { return false; }
			} else {
				try { Directory.CreateDirectory( path_current + "\\" + name ); } catch (System.Exception) { return false; }
			}

			return true;
		} else {
			return false;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	bool	CreateFile( string name ) {
		string	last_char		=	"\\";

		if (!File.Exists( path_current + name ) && !File.Exists( path_current + "\\" + name )) {
			last_char			=	path_current.Substring( path_current.Length-1, 1 );

			if ( last_char == "\\" ) {
				try { File.Create( path_current + name ); } catch (System.Exception) { return false; }
			} else {
				try { File.Create( path_current + "\\" + name ); } catch (System.Exception) { return false; }
			}

			return true;
		} else {
			return false;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	bool	DeleteFile( int i ) {
		bool	result		=	false;

		if ( File.Exists( path_current + getFileName( i ) ) ) {
			File.Delete( path_current + getFileName( i ) );
			result = true;
		} else if ( File.Exists( path_current + "\\" + getFileName( i ) ) ) {
			File.Delete( path_current + "\\" + getFileName( i ) );
			result = true;
		} 

		return result;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	ScanDisks() {
		list_disks.Clear();

		for ( char c = 'A'; c <= 'Z'; c ++ ) {
			if ( Directory.Exists( c+":\\" ) ) { list_disks.Add( c+":\\" ); }
		}
	}

	// ------------------------------------------------------------------------------------------
	private void	ScanDirectories() {
		list_dirs.Clear();

		if ( Directory.Exists( path_current ) && path_current != "" ) {
			string[] array_dirs			=	Directory.GetDirectories( path_current );
			foreach( string str_dir in array_dirs ) {
				string	str_name		=	Path.GetFileName(str_dir);
				if ( !vis_hidden && GetDirAttrib( path_current, str_name, FileAttributes.Hidden ) ) { continue; }
				if ( !vis_system && GetDirAttrib( path_current, str_name, FileAttributes.System ) ) { continue; }
				if ( filter_search != "" && !str_name.ToLower().Contains( filter_search.ToLower() ) ) { continue; }
				list_dirs.Add( str_name );
			}
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	ScanFiles() {
		list_files.Clear();

		if ( Directory.Exists( path_current ) && path_current != "" ) {
			string[]	array_files		=	Directory.GetFiles( path_current );
			foreach ( string str_file in array_files ) {
				string	str_name		=	Path.GetFileName(str_file);
				bool	filter_detect	=	false;

				if ( !vis_hidden && GetFileAttrib( path_current, str_name, FileAttributes.Hidden ) ) { continue; }
				if ( !vis_system && GetFileAttrib( path_current, str_name, FileAttributes.System ) ) { continue; }
				switch( filter_type ) {
					case 0:
						filter_detect		=	true;
						break;
					case 1:
						foreach (string search in filetype_doc) { if ( GetFileExt(path_current, str_name) == search ) { filter_detect = true; break; } }
						break;
					case 2:
						foreach (string search in filetype_img) { if ( GetFileExt(path_current, str_name) == search ) { filter_detect = true; break; } }
						break;
					case 3:
						foreach (string search in filetype_mus) { if ( GetFileExt(path_current, str_name) == search ) { filter_detect = true; break; } }
						break;
					case 4:
						foreach (string search in filetype_vid) { if ( GetFileExt(path_current, str_name) == search ) { filter_detect = true; break; } }
						break;
				}
				if ( !filter_detect ) { continue; }
				if ( filter_search != "" && !str_name.ToLower().Contains( filter_search.ToLower() ) ) { continue; }
				list_files.Add( Path.GetFileName(str_name) );
			}
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	ScanUpdate() {
		ScanDisks();
		ScanDirectories();
		ScanFiles();

		filter_search = "";
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	GetDirAttrib( string path, string name, FileAttributes attrib ) {
		string	path_full	=	"";
		bool	result		=	false;

		if ( Directory.Exists( path + name ) ) { path_full = path + name; }
		else if ( Directory.Exists( path + "\\" + name ) ) { path_full = path + "\\" + name; }
		if ( path_full != "" ) {
			DirectoryInfo	attrib_current	=	new DirectoryInfo( path_full );
			if ((attrib_current.Attributes & attrib) == attrib) { result = true; }
		}

		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	GetFileAttrib( string path, string name, FileAttributes attrib ) {
		string	path_full	=	"";
		bool	result		=	false;

		if ( File.Exists( path + name ) ) { path_full = path + name; }
		else if ( File.Exists( path + "\\" + name ) ) { path_full = path + "\\" + name; }
		if ( path_full != "" ) {
			FileAttributes	attrib_current	=	File.GetAttributes( path_full );
			if ((attrib_current & attrib) == attrib) { result = true; }
		}

		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	string	GetFileExt( string path, string name ) {
		string	path_full	=	"";
		string	return_ext	=	"";

		if ( File.Exists( path + name ) ) { path_full = path + name; }
		else if ( File.Exists( path + "\\" + name ) ) { path_full = path + "\\" + name; }
		if ( path_full != "" ) { return_ext = Path.GetExtension( path_full ); }

		return return_ext.ToLower();
	}

	// ------------------------------------------------------------------------------------------
	public	string	GetFileExt( string path_full ) {
		string	return_ext	=	"";
		if ( File.Exists( path_full ) ) { return_ext = Path.GetExtension( path_full ); }
		return return_ext.ToLower();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	GetAttrib( CustomFileAttributes attrib ) {
		switch( attrib ) {
			case CustomFileAttributes.folder: return vis_folder;
			case CustomFileAttributes.hidden: return vis_hidden;
			case CustomFileAttributes.system: return vis_system;
		}
		return false;
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetAttrib( CustomFileAttributes attrib, bool state ) {
		switch( attrib ) {
			case CustomFileAttributes.folder: vis_folder = state; break;
			case CustomFileAttributes.hidden: vis_hidden = state; break;
			case CustomFileAttributes.system: vis_system = state; break;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetFilterType( int value ) {
		filter_type = value;
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetFilterSearch( string phrase ) {
		filter_search = phrase;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	ArrayList	getDiskList()	{ return list_disks; }
	public	ArrayList	getDirsList()	{ if ( path_current != "" ) { return list_dirs; } else { return list_disks; } }
	public	ArrayList	getFileList()	{ return list_files; }
	public	int			getDiskCount()	{ return list_disks.Count; }
	public	int			getDirsCount()	{ if ( path_current != "" ) { return list_dirs.Count; } else { return getDiskCount(); } }
	public	int			getFileCount()	{ return list_files.Count; }

	// ------------------------------------------------------------------------------------------
	public	string	getDiskName( int i ) {
		if ( i>=0 && i<getDiskCount() ) { return list_disks[i].ToString(); }
		return "";
	}
		
	// ------------------------------------------------------------------------------------------
	public	string	getDirName( int i )	{
		if ( path_current == "" ) { return getDiskName( i ); }
		if ( i>=0 && i<getDirsCount() ) { return list_dirs[i].ToString(); }
		return "";
	}

	// ------------------------------------------------------------------------------------------
	public	string	getFileName( int i )	{
		if ( i>=0 && i<getFileCount() ) { return list_files[i].ToString(); }
		return "";
	}

	// ------------------------------------------------------------------------------------------
	public	string	getFullDirName( int i )	{
		if ( path_current != "" ) { return getDiskName( i ); }
		if ( i>=0 && i<getDirsCount() ) {
			string	path_return		=	"";

			if ( Directory.Exists( path_current + list_dirs[i].ToString() ) ) { path_return = path_current + list_dirs[i].ToString(); }
			else if ( Directory.Exists( path_current + "\\" + list_dirs[i].ToString() ) ) { path_return = path_current + "\\" + list_dirs[i].ToString(); } 
			return path_return;
		}
		return "";
	}

	// ------------------------------------------------------------------------------------------
	public	string	getFullFileName( int i )	{
		if ( i>=0 && i<getFileCount() ) {
			string	path_return		=	"";

			if ( File.Exists( path_current + list_files[i].ToString() ) ) { path_return = path_current + list_files[i].ToString(); }
			else if ( File.Exists( path_current + "\\" + list_files[i].ToString() ) ) { path_return = path_current + "\\" + list_files[i].ToString(); } 
			return path_return;
		}
		return "";
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	isDirectory( int i ) {
		bool	result		=		false;

		if ( i>=0 && i<getDirsCount() ) {
			if ( Directory.Exists( path_current + getDirName(i) ) ) { result = true; }
			else if ( Directory.Exists( path_current + "\\" + getDirName(i) ) ) { result = true; } 
		}

		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	isFile( int i ) {
		bool	result		=		false;

		if ( i>=0 && i<getFileCount() ) {
			if ( File.Exists( path_current + list_files[i].ToString() ) ) { result = true; }
			else if ( File.Exists( path_current + "\\" + list_files[i].ToString() ) ) { result = true; } 
		}

		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	isDirectory( string path, string dir_name ) {
		bool	result		=		false;

		if ( Directory.Exists( path_current + dir_name ) ) { result = true; }
		else if ( Directory.Exists( path_current + "\\" + dir_name ) ) { result = true; } 
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	isFile( string path, string file_name ) {
		bool	result		=		false;

		if ( File.Exists( path_current + file_name ) ) { result = true; }
		else if ( File.Exists( path_current + "\\" + file_name ) ) { result = true; }
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	isDirectory( string path ) {
		bool	result		=		false;

		if ( Directory.Exists( path ) ) { result = true; }
		return result;
	}

	// ------------------------------------------------------------------------------------------
	public	bool	isFile( string path ) {
		bool	result		=		false;

		if ( File.Exists( path ) ) { result = true; }
		return result;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	ClearHistoryForward( int pos ) {
		history.Clear();
		history_pos = -1;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------ 
}
// ####################################################################################################