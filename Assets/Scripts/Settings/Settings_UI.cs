using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public enum CustomButtonColorType { max, mid, min }

// ####################################################################################################
public class Settings_UI : MonoBehaviour {

	private			GameObject		parent;
	private			Settings		script_settings;
	public			Camera			cam;

	public			Dropdown		combobox_vis;
	public			Dropdown		combobox_visColor;
	public			GameObject		panel_visColor;
	public			Button			button_visColor;
	public			Toggle			toggle_fallowers;
	public			Toggle			toggle_emission;

	public			Dropdown		combobox_cam;
	public			Dropdown		combobox_skybox;
	public			Toggle			toggle_camshift;
	public			Text			text_camposx;
	public			Text			text_camposy;
	public			Text			text_camposz;
	public			Text			text_camrotx;
	public			Text			text_camroty;
	public			Text			text_camrotz;
	public			Text			text_camscal;
	public			Slider			slider_camposx;
	public			Slider			slider_camposy;
	public			Slider			slider_camposz;
	public			Slider			slider_camrotx;
	public			Slider			slider_camroty;
	public			Slider			slider_camrotz;
	public			Slider			slider_camscal;

	public			Toggle			toggle_autoplaylist;
	public			Toggle			toggle_autoplay;
	public			Toggle			toggle_autolib;

	public			Button			button_close;
	public			GameObject		ui_menu;
	public			GameObject		ui_visualisation;
	public			GameObject		ui_camera;
	public			GameObject		ui_player;
	public			GameObject		ui_informations;
	public			Button			button_uiVisualisation;
	public			Button			button_uiCamera;
	public			Button			button_player;
	public			Button			button_informations;

	public			GameObject		ui_colorbox;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		parent				=	transform.parent.gameObject;
		script_settings		=	parent.GetComponent<Settings>();

		button_close.onClick.AddListener( OnCloseClick );
		button_uiVisualisation.onClick.AddListener( OnVisualisationSelect );
		button_uiCamera.onClick.AddListener( OnCameraSelect );
		button_player.onClick.AddListener( OnPlayerSelect );
		button_informations.onClick.AddListener( OnInfoSelect );

		ControllersUpdate();

		combobox_vis.onValueChanged.AddListener( delegate { OnComboboxVisChange(combobox_vis.value); } );
		combobox_visColor.onValueChanged.AddListener( delegate { OnCoboboxVisColorChange(combobox_visColor.value); } );
		button_visColor.onClick.AddListener( OnButtonClickColorChange_Init );
		toggle_fallowers.onValueChanged.AddListener( delegate { script_settings.vis_fallowers = toggle_fallowers.isOn; } );
		toggle_emission.onValueChanged.AddListener( delegate { script_settings.vis_emission = toggle_emission.isOn; } );

		combobox_cam.onValueChanged.AddListener( delegate { OnComboBoxCameraChange( combobox_cam.value ); } );
		combobox_skybox.onValueChanged.AddListener( delegate { OnComboBoxSkyboxChange( combobox_skybox.value ); } );
		toggle_camshift.onValueChanged.AddListener( delegate { script_settings.cam_shift = toggle_camshift.isOn; } );
		slider_camposx.onValueChanged.AddListener( delegate { OnCameraSilderX( slider_camposx.value ); } );
		slider_camposy.onValueChanged.AddListener( delegate { OnCameraSilderY( slider_camposy.value ); } );
		slider_camposz.onValueChanged.AddListener( delegate { OnCameraSilderZ( slider_camposz.value ); } );
		slider_camrotx.onValueChanged.AddListener( delegate { OnCameraSilderrX( slider_camrotx.value ); } );
		slider_camroty.onValueChanged.AddListener( delegate { OnCameraSilderrY( slider_camroty.value ); } );
		slider_camrotz.onValueChanged.AddListener( delegate { OnCameraSilderrZ( slider_camrotz.value ); } );
		slider_camscal.onValueChanged.AddListener( delegate { OnCameraSildersC( slider_camscal.value ); } );

		toggle_autoplaylist.onValueChanged.AddListener( delegate { script_settings.auto_playlist = toggle_autoplaylist.isOn; } );
		toggle_autoplay.onValueChanged.AddListener( delegate { script_settings.auto_play = toggle_autoplay.isOn; } );
		toggle_autolib.onValueChanged.AddListener( delegate { script_settings.auto_lib = toggle_autolib.isOn; } );
	}

	// ------------------------------------------------------------------------------------------
	public	void	ControllersUpdate() {
		switch ( script_settings.vis_type ) {
		case CustomVisualisationType.CirclePeaks:	combobox_vis.value = 0;	break;
		case CustomVisualisationType.SquarePeaks:	combobox_vis.value = 1;	break;
		case CustomVisualisationType.SquareGrid:	combobox_vis.value = 2;	break;
		}
		switch ( script_settings.vis_coloration ) {
		case CustomVisualisationColor.custom:	combobox_visColor.value = 0;	break;
		case CustomVisualisationColor.rainbow:	combobox_visColor.value = 1;	break;
		}
		panel_visColor.GetComponent<Image>().color		=	script_settings.vis_color;
		toggle_fallowers.isOn							=	script_settings.vis_fallowers;
		toggle_emission.isOn							=	script_settings.vis_emission;

		combobox_cam.value								=	script_settings.cam_position;
		combobox_skybox.value							=	script_settings.cam_skybox;
		toggle_camshift.isOn							=	script_settings.cam_shift;
		EnabledCustomSliders();
		slider_camposx.value							=	script_settings.cam_positions[0].posX;
		slider_camposy.value							=	script_settings.cam_positions[0].posY;
		slider_camposz.value							=	script_settings.cam_positions[0].posZ;
		slider_camrotx.value							=	script_settings.cam_positions[0].rotX;
		slider_camroty.value							=	script_settings.cam_positions[0].rotY;
		slider_camrotz.value							=	script_settings.cam_positions[0].rotZ;
		slider_camscal.value							=	script_settings.cam_positions[0].scal;
		SelectCamera( combobox_cam.value );
		SelectSkybox( combobox_skybox.value );

		toggle_autoplaylist.isOn						=	script_settings.auto_playlist;
		toggle_autoplay.isOn							=	script_settings.auto_play;
		toggle_autolib.isOn								=	script_settings.auto_lib;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	OnCloseClick() {
		gameObject.GetComponent<Canvas>().enabled = false;
		parent.GetComponent<Menu_Controller>().ResetView();
		script_settings.SaveData();
	}

	public	void	CloaseTabs() {
		ui_visualisation.SetActive( false );
		ui_camera.SetActive( false );
		ui_player.SetActive( false );
		ui_informations.SetActive( false );
		script_settings.SaveData();
	}

	public	void	OnVisualisationSelect() {
		CloaseTabs();
		ui_visualisation.SetActive( true );
		switch ( script_settings.vis_type ) {
		case CustomVisualisationType.CirclePeaks:	combobox_vis.value = 0;	break;
		case CustomVisualisationType.SquarePeaks:	combobox_vis.value = 1;	break;
		case CustomVisualisationType.SquareGrid:	combobox_vis.value = 2;	break;
		}
		switch ( script_settings.vis_coloration ) {
		case CustomVisualisationColor.custom:	combobox_visColor.value = 0;	break;
		case CustomVisualisationColor.rainbow:	combobox_visColor.value = 1;	break;
		}
		panel_visColor.GetComponent<Image>().color		=	script_settings.vis_color;
		toggle_fallowers.isOn							=	script_settings.vis_fallowers;
		toggle_emission.isOn							=	script_settings.vis_emission;
	}

	public	void	OnCameraSelect() {
		CloaseTabs();
		ui_camera.SetActive( true );
		combobox_cam.value								=	script_settings.cam_position;
		combobox_skybox.value							=	script_settings.cam_skybox;
		toggle_camshift.isOn							=	script_settings.cam_shift;
		EnabledCustomSliders();
		slider_camposx.value							=	script_settings.cam_positions[0].posX;
		slider_camposy.value							=	script_settings.cam_positions[0].posY;
		slider_camposz.value							=	script_settings.cam_positions[0].posZ;
		slider_camrotx.value							=	script_settings.cam_positions[0].rotX;
		slider_camroty.value							=	script_settings.cam_positions[0].rotY;
		slider_camrotz.value							=	script_settings.cam_positions[0].rotZ;
		slider_camscal.value							=	script_settings.cam_positions[0].scal;
	}

	public	void	OnPlayerSelect() {
		CloaseTabs();
		ui_player.SetActive( true );
		toggle_autoplaylist.isOn						=	script_settings.auto_playlist;
		toggle_autoplay.isOn							=	script_settings.auto_play;
		toggle_autolib.isOn								=	script_settings.auto_lib;
	}

	public	void	OnInfoSelect() {
		CloaseTabs();
		ui_informations.SetActive( true );
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnComboboxVisChange( int value ) {
		switch (value) {
		case 0:		script_settings.vis_type	=	CustomVisualisationType.CirclePeaks;	break;
		case 1:		script_settings.vis_type	=	CustomVisualisationType.SquarePeaks;	break;
		case 2:		script_settings.vis_type	=	CustomVisualisationType.SquareGrid;		break;
		default:	script_settings.vis_type	=	CustomVisualisationType.CirclePeaks;	break;
		}
		script_settings.cam_position	=	value+1;
		combobox_cam.value	=	value+1;
		OnComboBoxCameraChange( value+1 );
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnCoboboxVisColorChange( int value ) {
		switch (value) {
		case 0:		script_settings.vis_coloration	=	CustomVisualisationColor.custom;	break;
		case 1:		script_settings.vis_coloration	=	CustomVisualisationColor.rainbow;	break;
		default:	script_settings.vis_coloration	=	CustomVisualisationColor.custom;	break;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnButtonClickColorChange_Init() {
		ui_colorbox.GetComponent<Canvas>().enabled	=	true;
		ui_colorbox.GetComponent<Msg_Color>().OnShow( "Visualisation Color", script_settings.vis_color, OnButtonClickColorChange, null );
	}

	public	void	OnButtonClickColorChange( object[] args ) {
		panel_visColor.GetComponent<Image>().color	=	ui_colorbox.GetComponent<Msg_Color>().getColor();
		script_settings.vis_color					=	ui_colorbox.GetComponent<Msg_Color>().getColor();
		ui_colorbox.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnComboBoxCameraChange( int value ) {
		script_settings.cam_position = value;
		SelectCamera( value );
		EnabledCustomSliders();
	}

	// ------------------------------------------------------------------------------------------
	public	void	EnabledCustomSliders() {
		if ( combobox_cam.value == 0 ) {
			slider_camposx.interactable					=	true;
			slider_camposy.interactable					=	true;
			slider_camposz.interactable					=	true;
			slider_camrotx.interactable					=	true;
			slider_camroty.interactable					=	true;
			slider_camrotz.interactable					=	true;
			slider_camscal.interactable					=	true;
		} else {
			slider_camposx.interactable					=	false;
			slider_camposy.interactable					=	false;
			slider_camposz.interactable					=	false;
			slider_camrotx.interactable					=	false;
			slider_camroty.interactable					=	false;
			slider_camrotz.interactable					=	false;
			slider_camscal.interactable					=	false;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnComboBoxSkyboxChange( int value ) {
		script_settings.cam_skybox = value;
		SelectSkybox( value );
	}

	// ------------------------------------------------------------------------------------------
	private	void	SelectCamera( int value ) {
		Vector3	cam_pos	=	new Vector3( script_settings.cam_positions[value].posX, script_settings.cam_positions[value].posY, script_settings.cam_positions[value].posZ );
		Vector3	cam_rot	=	new Vector3( script_settings.cam_positions[value].rotX, script_settings.cam_positions[value].rotY, script_settings.cam_positions[value].rotZ );

		cam.transform.position		=	cam_pos;
		cam.transform.eulerAngles	=	cam_rot;
		cam.fieldOfView				=	script_settings.cam_positions[value].scal;
	}

	// ------------------------------------------------------------------------------------------
	private	void	SelectSkybox( int value ) {
		cam.GetComponent<Skybox>().material		=	script_settings.cam_skyboxes[value];
	}

	// ------------------------------------------------------------------------------------------
	public	void	OnCameraSilderX( float value )  {
		script_settings.cam_positions[0].posX = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camposx.text = value.ToString();
	}

	public	void	OnCameraSilderY( float value )  {
		script_settings.cam_positions[0].posY = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camposy.text = value.ToString();
	}

	public	void	OnCameraSilderZ( float value )  {
		script_settings.cam_positions[0].posZ = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camposz.text = value.ToString();
	}

	public	void	OnCameraSilderrX( float value ) {
		script_settings.cam_positions[0].rotX = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camrotx.text = value.ToString() + "\xB0";
	}

	public	void	OnCameraSilderrY( float value ) {
		script_settings.cam_positions[0].rotY = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camroty.text = value.ToString() + "\xB0";
	}

	public	void	OnCameraSilderrZ( float value ) {
		script_settings.cam_positions[0].rotZ = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camrotz.text = value.ToString() + "\xB0";
	}

	public	void	OnCameraSildersC( float value ) {
		Debug.Log( "OnCameraSliders " + value );
		script_settings.cam_positions[0].scal = value;
		if ( script_settings.cam_position == 0 ) { SelectCamera(0); }
		text_camscal.text = value.ToString();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}

// ####################################################################################################