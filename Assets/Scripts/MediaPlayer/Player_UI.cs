using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public enum CustomPlayerState { playing, pause, stop }

// ####################################################################################################
public class Player_UI : MonoBehaviour {

	private		GameObject			parent;
	private		Player				script_player;
	private		PlayList_Manager	script_playlistManager;
	private		Settings			script_settings;
	private		CustomPlayerState	state				=		CustomPlayerState.stop;
	private		bool				lock_scroll			=		false;

	public		GameObject			text_container;
	public		Text				file_name;
	public		Text				file_time;
	public		GameObject			panel_repeat;
	public		GameObject			panel_shuffle;
	public		Button				button_playpause;
	public		Button				button_stop;
	public		Button				button_next;
	public		Button				button_back;
	public		Button				button_repeat;
	public		Button				button_shuffle;
	public		Button				button_volume;
	public		Slider				silder_position;
	public		Slider				silder_volume;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		parent					=	transform.parent.gameObject;
		script_player			=	parent.GetComponent<Player>();
		script_playlistManager	=	parent.GetComponent<PlayList_Manager>();
		script_settings			=	parent.GetComponent<Settings>();

		button_playpause.onClick.AddListener( OnPlayPauseClick );
		button_stop.onClick.AddListener( OnStopClick );
		button_next.onClick.AddListener( OnNextClick );
		button_back.onClick.AddListener( OnBackClick );
		button_repeat.onClick.AddListener( OnRepeatClick );
		button_shuffle.onClick.AddListener( OnShuffleClick );
		silder_position.onValueChanged.AddListener( delegate { OnSilderChange(); } );
		silder_volume.onValueChanged.AddListener( delegate { OnVolumeChange(); } );

		silder_position.maxValue	=	0.0f;
		silder_position.value		=	0.0f;
		silder_volume.maxValue		=	1.0f;
		silder_volume.value			=	script_settings.player_volume;
	}

	// ------------------------------------------------------------------------------------------
	private	bool	CheckState() {
		CustomPlayerState	new_state;	

		if ( script_player.isPlay() ) { new_state	=	CustomPlayerState.playing; }
		else if ( script_player.isPause() ) { new_state	=	CustomPlayerState.pause; }
		else { new_state	=	CustomPlayerState.stop; }

		if ( new_state	!=	state ) {
			var		buttonPP	=	button_playpause.gameObject;
			var		icon		=	buttonPP.transform.GetChild(0).gameObject;

			if ( new_state	==	CustomPlayerState.playing ) {
				icon.GetComponent<RawImage>().texture	=	Resources.Load("Icons/White/Player/64_Pause") as Texture;
			}
			if ( new_state	==	CustomPlayerState.pause ) {
				icon.GetComponent<RawImage>().texture	=	Resources.Load("Icons/White/Player/64_Play") as Texture;
			}
			if ( new_state	==	CustomPlayerState.stop ) {
				icon.GetComponent<RawImage>().texture	=	Resources.Load("Icons/White/Player/64_Play") as Texture;
			}

			state	=	new_state;
			return true;
		}
		return false;
	}

	// ------------------------------------------------------------------------------------------
	private	void 	Update() {
		//file_name.text	=	script_player.getName();
		file_name.text	=	CropText( text_container, file_name, script_player.getName() );
		file_time.text	=	script_player.getTime() + " / " + script_player.getTimeMax();

		if ( script_player.isPlay() ) {
			lock_scroll				=	true;
			if ( silder_position.maxValue != script_player.getLength() ) {
				silder_position.maxValue	=	script_player.getLength();
			}
			silder_position.value	=	script_player.getPosition();
			lock_scroll				=	false;
		}

		if( script_player.isEmpty() ) { return; }

		if ( !script_player.isPause() && !script_player.isPlay() ) {
			if ( ((int) script_player.getPosition()) >= ((int) script_player.getLength()) ) {
				if ( script_player.isRepeat() ) {
					script_player.setPosition( 0 );
					script_player.Play();
				} else {
					OnNextClick();
				}
			}
		}

		CheckState();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	string	CropText( GameObject container, Text output, string str ) {
		if ( str == "" ) { return ""; }

		int 			textLeght	=	0;
		float			charLeght	=	0;
		int				areaChars	=	0;
		float			area		=	CalculateWidth( container ) - (52*2);
		Font			fontType	=	output.font;
		CharacterInfo	charInfo	=	new CharacterInfo();
		char[]			char_str	=	str.ToCharArray();

		foreach ( char c in char_str ) {
			fontType.GetCharacterInfo(c, out charInfo, output.fontSize );
			textLeght += charInfo.advance;
		}

		charLeght	=	textLeght / str.Length;
		areaChars	=	(int) (area / charLeght);
		if ( str.Length < areaChars ) { areaChars = str.Length; }
		return str.Substring( 0, areaChars );
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
	// ------------------------------------------------------------------------------------------
	public	void	SetFile( int index, string name, string fullpath ) {
		script_player.InitPlay( index, name, fullpath, true );
		silder_position.maxValue	=	script_player.getLength();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	OnPlayPauseClick() {
		if ( !script_player.isPlay() ) { script_player.Play(); }
		else { script_player.Pause(); }
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnStopClick() {
		if ( script_player.isPlay() ) { script_player.Stop(); }
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnNextClick() {
		int	active_index		=	script_player.getIndex();

		if ( script_player.isShuffle() ) {	
			int		new_int		=	Random.Range( 0, script_playlistManager.GetSize()-1 );
			string	name		=	script_playlistManager.GetName( new_int );
			string	path		=	script_playlistManager.GetFile( new_int );
			script_player.Stop();
			script_player.InitPlay( new_int, name, path, true );

		} else if ( active_index + 1 < script_playlistManager.GetSize() ) {
			string	name		=	script_playlistManager.GetName( active_index + 1 );
			string	path		=	script_playlistManager.GetFile( active_index + 1 );
			script_player.Stop();
			script_player.InitPlay( active_index + 1, name, path, true );
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnBackClick() {
		if ( script_player.getPosition() < 10 ) {
			int	active_index	=	script_player.getIndex();
			if ( active_index - 1 >= 0 ) {
				string	name	=	script_playlistManager.GetName( active_index - 1 );
				string	path	=	script_playlistManager.GetFile( active_index - 1 );
				script_player.Stop();
				script_player.InitPlay( active_index - 1, name, path, true );
			}
		} else {
			script_player.Stop();
			script_player.setPosition( 0 );
			script_player.Play();
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	LaunchTrack( int index, bool stpl ) {
		if ( index >= 0 && index < script_playlistManager.GetSize() ) {
			string	name		=	script_playlistManager.GetName( index );
			string	path		=	script_playlistManager.GetFile( index );
			script_player.Stop();
			script_player.InitPlay( index, name, path, stpl );
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	LaunchLast() {
		if ( script_playlistManager.GetSize() >= 1 ) {
			int		index		=	script_playlistManager.GetSize() - 1;
			string	name		=	script_playlistManager.GetName( index );
			string	path		=	script_playlistManager.GetFile( index );
			script_player.Stop();
			script_player.InitPlay( index, name, path, true );
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnRepeatClick() { 
		script_player.setRepeat( !script_player.isRepeat() );
		panel_repeat.SetActive( script_player.isRepeat() );
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnShuffleClick() {
		script_player.setShuffle( !script_player.isShuffle() );
		panel_shuffle.SetActive( script_player.isShuffle() );
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnSilderChange() {
		if (lock_scroll) { return; }
		script_player.setPosition( silder_position.value );
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnVolumeChange() { 
		var		buttonV		=	button_volume.gameObject;
		var		icon		=	buttonV.transform.GetChild(0).gameObject;

		script_player.setVolume( silder_volume.value );
		script_settings.player_volume	=	silder_volume.value;
		PlayerPrefs.SetFloat( "player_volume", silder_volume.value );
		if ( silder_volume.value <= 0.05f ) {
			icon.GetComponent<RawImage>().texture = Resources.Load("Icons/White/Player/64_Mute") as Texture;
			return;
		} else if ( silder_volume.value > 0.75f ) {
			icon.GetComponent<RawImage>().texture = Resources.Load("Icons/White/Player/64_VolumeUp") as Texture;
			return;
		} else if ( silder_volume.value > 0.25f  ) {
			icon.GetComponent<RawImage>().texture = Resources.Load("Icons/White/Player/64_Volume") as Texture;
			return;
		} else {
			icon.GetComponent<RawImage>().texture = Resources.Load("Icons/White/Player/64_VolumeDown") as Texture;
			return;
		}

	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################