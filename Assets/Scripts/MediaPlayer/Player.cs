using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class Player : MonoBehaviour {

	public		GameObject		audio_container;
	private		AudioSource		mediaplayer;
	private		AudioClip		source;
	private		WWW				importer;
	private		Settings		script_settings;

	private		float			updateTimer			=		0;
	private		bool			startLoading		=		false;
	private		bool			startplay			=		false;

	public		int				spectrum_accuracy	=		127;
	private		float[]			spectrum_array;

	private		int				track_index			=		-1;
	private		string			track_name			=		"";
	private		string			track_fullpath		=		"";

	private		bool			is_pause			=		false;
	private		bool			is_repeat			=		false;
	private		bool			is_shuffle			=		false;
	private		float			player_position		=		0f;
	private		float			player_length		=		0f;
	private		float			player_volume		=		1f;

	public		GameObject		ui_awaiter;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		script_settings		=	GetComponent<Settings>();

		mediaplayer			=	audio_container.GetComponent<AudioSource>();
		setSpectrumDiv( spectrum_accuracy );
	}

	// ------------------------------------------------------------------------------------------
	private	void	Update () {
		if ( mediaplayer.isPlaying ) {
			player_position		=	mediaplayer.time;
			prepareSpectrum();
		}

		if ( startLoading ) {
			AudioDataLoadState	loadResult	=	InitWait();
			if ( loadResult == AudioDataLoadState.Failed ) { startLoading = false; }
			if ( loadResult == AudioDataLoadState.Loaded ) { startLoading = false; InitFinish(); }
			if ( !startLoading ) { ui_awaiter.GetComponent<Canvas>().enabled = false; }
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	InitPlay( int index, string name, string fullpath, bool stpl ) {
		Stop();

		track_index		=	index;
		track_name		=	name;
		track_fullpath	=	fullpath;
		importer		=	new WWW( "file:///" + track_fullpath );
		source			=	importer.GetAudioClip();

		startLoading	=	true;
		startplay		=	stpl;
		ui_awaiter.GetComponent<Canvas>().enabled	=	true;
		ui_awaiter.GetComponent<Msg_Awaiter>().Show( "Loading File...", "Please wait while loading file." );
		ui_awaiter.GetComponent<Msg_Awaiter>().ShowProgressBar( false, 0 );
	}

	// ------------------------------------------------------------------------------------------
	public	AudioDataLoadState	InitWait() {
		updateTimer	+=	Time.deltaTime;
		if ( source.loadState == AudioDataLoadState.Loaded ) { return AudioDataLoadState.Loaded; }
		if ( source.loadState == AudioDataLoadState.Failed ) { return AudioDataLoadState.Failed; }
		if ( updateTimer > 1000 ) { return AudioDataLoadState.Failed; }
		return AudioDataLoadState.Loading;
	}

	// ------------------------------------------------------------------------------------------
	public	void	InitFinish() {
		player_position					=	0;
		player_length					=	source.length;

		mediaplayer.clip				=	source;
		mediaplayer.time				=	0;
		mediaplayer.volume				=	player_volume;

		script_settings.player_track	=	track_index;
		PlayerPrefs.SetInt( "player_track", track_index );
		if ( startplay ) { Play(); }
	}

	// ------------------------------------------------------------------------------------------
	public	void	Play() {
		if ( !mediaplayer.isPlaying ) {
			if ( is_pause ) { mediaplayer.time = player_position; }
			else { mediaplayer.time = 0; }

			mediaplayer.volume	=	player_volume;
			mediaplayer.Play();
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	Pause() {
		if ( mediaplayer.isPlaying ) {
			mediaplayer.Stop();
			is_pause	=	true;
			resetSpectrum();
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	Stop() {
		if ( is_pause || mediaplayer.isPlaying ) {
			mediaplayer.Stop();
			mediaplayer.time	=	0;
			mediaplayer.volume	=	player_volume;
			is_pause			=	false;
			resetSpectrum();
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	setPosition( float pos ) {
		mediaplayer.time	=	pos;
		player_position		=	pos;
	}

	// ------------------------------------------------------------------------------------------
	public	void	setVolume( float vol ) {
		mediaplayer.volume	=	vol;
		player_volume		=	vol;
	}

	// ------------------------------------------------------------------------------------------
	public	void	setRepeat( bool mode ) { is_repeat = mode; }
	public	void	setShuffle( bool mode ) { is_shuffle = mode; }

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	bool	isEmpty()		{ if ( mediaplayer.clip == null ) { return true; } return false; }
	public	int		getIndex()		{ return track_index; }
	public	string	getName()		{ return track_name; }

	public	bool	isPlay()		{ return mediaplayer.isPlaying; }
	public	bool	isPause()		{ return is_pause; }
	public	bool	isRepeat()		{ return is_repeat; }
	public	bool	isShuffle()		{ return is_shuffle; }
	public	float	getPosition()	{ return player_position; }
	public	float	getLength() 	{ return player_length; }
	public	float	getVolume() 	{ return player_volume; }

	// ------------------------------------------------------------------------------------------
	public	string	getTime() {
		int		intMin		=		(int) player_position / 60;
		int		intSec		=		(int) player_position % 60;
		string	strMin		=		intMin.ToString();
		string	strSec		=		intSec.ToString();
		if ( intMin < 10 ) { strMin = "0" + strMin; }
		if ( intSec < 10 ) { strSec = "0" + strSec; }

		return	strMin + ":" + strSec;
	}

	// ------------------------------------------------------------------------------------------
	public	string	getTimeMax() {
		int		intMin		=		(int) player_length / 60;
		int		intSec		=		(int) player_length % 60;
		string	strMin		=		intMin.ToString();
		string	strSec		=		intSec.ToString();
		if ( intMin < 10 ) { strMin = "0" + strMin; }
		if ( intSec < 10 ) { strSec = "0" + strSec; }

		return	strMin + ":" + strSec;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	setSpectrumDiv( int value ) {
		spectrum_accuracy	=	value;
		spectrum_array		=	new float[value];
	}

	// ------------------------------------------------------------------------------------------
	private	void	prepareSpectrum() {
		mediaplayer.GetSpectrumData( spectrum_array, 0, FFTWindow.Blackman );
	}

	// ------------------------------------------------------------------------------------------
	private	void	resetSpectrum() {
		for ( int i = 0; i < spectrum_accuracy; i++ ) {
			spectrum_array[i] = 0;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	float[]	getSpectrum() {
		return spectrum_array;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################