using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

// ####################################################################################################
public enum CustomVisualisationType { none, CirclePeaks, SquarePeaks, SquareGrid }
public enum CustomVisualisationColor { custom, rainbow }

// ####################################################################################################
public class CameraPosition {

	public	float	posX	{ get; set; }
	public	float	posY	{ get; set; }
	public	float	posZ	{ get; set; }
	public	float	rotX	{ get; set; }
	public	float	rotY	{ get; set; }
	public	float	rotZ	{ get; set; }
	public	float	scal	{ get; set; }

	public CameraPosition( float x, float y, float z, float rx, float ry, float rz, float sc ) {
		posX = (float) x;
		posY = (float) y;
		posZ = (float) z;
		rotX = (float) rx;
		rotY = (float) ry;
		rotZ = (float) rz;
		scal = (float) sc;
	}

}

// ####################################################################################################
public class Settings : MonoBehaviour {

	public		float						player_volume	{ get; set; }
	public		int							player_track	{ get; set; }
	public		Color						vis_color		{ get; set; }
	public		CustomVisualisationType		vis_type		{ get; set; }
	public		CustomVisualisationColor	vis_coloration	{ get; set; }
	public		bool						vis_fallowers	{ get; set; }
	public		bool						vis_emission	{ get; set; }
	public		int							cam_position	{ get; set; }
	public		CameraPosition[]			cam_positions	{ get; set; }
	public		bool						cam_shift		{ get; set; }
	public		int							cam_skybox		{ get; set; }
	public		Material[]					cam_skyboxes;
	public		string						save_filepath	{ get; set; }
	public		string						save_playlist	{ get; set; }
	public		bool						auto_playlist	{ get; set; }
	public		bool						auto_play		{ get; set; }
	public		bool						auto_lib		{ get; set; }

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		player_volume		=		0.5f;
		vis_color			=		new Color( 0.12f, 0.61f, 0.91f, 1.00f );
		vis_type			=		CustomVisualisationType.CirclePeaks;
		vis_coloration		=		CustomVisualisationColor.custom;
		vis_fallowers		=		true;
		vis_emission		=		true;
		cam_position		=		1;
		cam_skybox			=		13;
		auto_playlist		=		true;
		auto_play			=		true;
		auto_lib			=		true;

		InitSaveDir();
		CameraPositions();
		LoadData();
	}

	public	void	CameraPositions() {
		cam_positions		=		new CameraPosition[4];
		cam_positions[0]	=		new CameraPosition(  0,  0,   0,   0,       0, 0, 60 );
		cam_positions[1]	=		new CameraPosition(-60,  5,  40, -18, 123.75f, 0, 65 );
		cam_positions[2]	=		new CameraPosition( -1,  4,  -8, -18,       0, 0, 90 );
		cam_positions[3]	=		new CameraPosition( -1, 13, -36,   8,       0, 0, 60 );
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	InitSaveDir() {
		save_filepath	=	Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
	
		if (!Directory.Exists( save_filepath + "\\" + "KamilKarpinski" )) { Directory.CreateDirectory( save_filepath + "\\" + "KamilKarpinski" ); }
		if (!Directory.Exists( save_filepath + "\\" + "KamilKarpinski" + "\\" + "UnithraX" )) { Directory.CreateDirectory( save_filepath + "\\" + "KamilKarpinski" + "\\" + "UnithraX" ); }

		save_filepath 	=	save_filepath + "\\" + "KamilKarpinski" + "\\" + "UnithraX";
		save_playlist	=	save_filepath + "\\" + "last_playlist.upl";
	}


	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	SaveData() {
		PlayerPrefs.SetFloat( "player_volume", player_volume );
		PlayerPrefs.SetInt( "player_track", player_track );
		PlayerPrefs.SetFloat( "vis_colorR", vis_color.r );
		PlayerPrefs.SetFloat( "vis_colorG", vis_color.g );
		PlayerPrefs.SetFloat( "vis_colorB", vis_color.b );
		PlayerPrefs.SetFloat( "vis_colorA", vis_color.a );
		switch( vis_type ) {
		case CustomVisualisationType.CirclePeaks: PlayerPrefs.SetInt( "vis_type", 0 );	break;
		case CustomVisualisationType.SquarePeaks: PlayerPrefs.SetInt( "vis_type", 1 );	break;
		case CustomVisualisationType.SquareGrid:  PlayerPrefs.SetInt( "vis_type", 2 );	break;
		}
		switch( vis_coloration ) {
		case CustomVisualisationColor.custom: PlayerPrefs.SetInt( "vis_coloration", 0 );	break;
		case CustomVisualisationColor.rainbow: PlayerPrefs.SetInt( "vis_coloration", 1 );	break;
		}
		BoolPrefsSet( "vis_fallowers", vis_fallowers );
		BoolPrefsSet( "vis_emission", vis_emission );
		PlayerPrefs.SetInt( "cam_position", cam_position );
		PlayerPrefs.SetInt( "cam_skybox", cam_skybox );
		BoolPrefsSet( "cam_shift", cam_shift );
		BoolPrefsSet( "auto_playlist", auto_playlist );
		BoolPrefsSet( "auto_play", auto_play );
		BoolPrefsSet( "auto_lib", auto_lib );
		PlayerPrefs.SetFloat( "cpX", cam_positions[0].posX );
		PlayerPrefs.SetFloat( "cpY", cam_positions[0].posY );
		PlayerPrefs.SetFloat( "cpZ", cam_positions[0].posZ );
		PlayerPrefs.SetFloat( "crX", cam_positions[0].rotX );
		PlayerPrefs.SetFloat( "crY", cam_positions[0].rotY );
		PlayerPrefs.SetFloat( "crZ", cam_positions[0].rotZ );
		PlayerPrefs.SetFloat( "cpS", cam_positions[0].scal );
	}

	private	void	LoadData() {
		try { player_volume = PlayerPrefs.GetFloat( "player_volume" ); } catch ( System.Exception ) { player_volume	= 0.50f; }
		try { player_track = PlayerPrefs.GetInt( "player_track" ); } catch ( System.Exception ) { player_track	= 0; }
		try { vis_color = new Color( PlayerPrefs.GetFloat( "vis_colorR" ), PlayerPrefs.GetFloat( "vis_colorG" ), PlayerPrefs.GetFloat( "vis_colorB" ), PlayerPrefs.GetFloat( "vis_colorA" ) ); } catch ( System.Exception ) { vis_color	=	new Color( 0.12f, 0.61f, 0.91f, 1.00f ); }
		try { int load = PlayerPrefs.GetInt( "vis_type" );
			switch( load ) {
			case 0: vis_type = CustomVisualisationType.CirclePeaks;	break;
			case 1: vis_type = CustomVisualisationType.SquarePeaks;	break;
			case 2: vis_type = CustomVisualisationType.SquareGrid;	break;
			}
		} catch ( System.Exception ) { vis_type	=	CustomVisualisationType.CirclePeaks; }
		try { int load = PlayerPrefs.GetInt( "vis_coloration" );
			switch( load ) {
			case 0: vis_coloration = CustomVisualisationColor.custom;	break;
			case 1: vis_coloration = CustomVisualisationColor.rainbow;	break;
			}
		} catch ( System.Exception ) { vis_coloration	=	CustomVisualisationColor.custom; }
		try { vis_fallowers = BoolPrefsGet( "vis_fallowers" ); } catch ( System.Exception ) { vis_fallowers	=	true; }
		try { vis_emission = BoolPrefsGet( "vis_emission" ); } catch ( System.Exception ) { vis_emission	=	true; }
		try { cam_position = PlayerPrefs.GetInt( "cam_position" ); } catch ( System.Exception ) { cam_position	=	1; }
		try { cam_skybox = PlayerPrefs.GetInt( "cam_skybox" ); } catch ( System.Exception ) { cam_skybox		=	13; }
		try { cam_shift = BoolPrefsGet( "cam_shift" ); } catch ( System.Exception ) { cam_shift	=	true; }
		try { auto_playlist = BoolPrefsGet( "auto_playlist" ); } catch ( System.Exception ) { auto_playlist	=	true; }
		try { auto_play = BoolPrefsGet( "auto_play" ); } catch ( System.Exception ) { auto_play	=	true; }
		try { auto_lib = BoolPrefsGet( "auto_lib" ); } catch ( System.Exception ) { auto_lib	=	true; }

		try {
			cam_positions[0].posX = PlayerPrefs.GetFloat( "cpX" );
			cam_positions[0].posY = PlayerPrefs.GetFloat( "cpY" );
			cam_positions[0].posZ = PlayerPrefs.GetFloat( "cpZ" );
			cam_positions[0].rotX = PlayerPrefs.GetFloat( "crX" );
			cam_positions[0].rotY = PlayerPrefs.GetFloat( "crY" );
			cam_positions[0].rotZ = PlayerPrefs.GetFloat( "crZ" );
			cam_positions[0].scal = PlayerPrefs.GetFloat( "cpS" );
		} catch ( System.Exception ) { cam_positions[0]	=	new CameraPosition( 0, 0, 0, 0, 0, 0, 60 ); }
	}

	private	void	BoolPrefsSet( string key, bool value ) {
		PlayerPrefs.SetInt( key, value ? 1 : 0 );
	}

	private	bool	BoolPrefsGet( string key ) {
		int	value	=	PlayerPrefs.GetInt( key );
		if ( value == 1 ) { return true; }
		if ( value == 0 ) { return false; }
		return false;
	}
	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}

// ####################################################################################################