using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Msg_Color : MonoBehaviour {

	public		Text			text_title;
	public		Slider			slider_red;
	public		Slider			slider_green;
	public		Slider			slider_blue;
	public		Slider			slider_alpha;
	public		Button			button_ok;
	public		Button			button_cancel;
	public		GameObject		panel_color;

	public		delegate void	DelegateOK( params object[] arguments );
	private		DelegateOK		onOkClick;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnShow( string title, Color init_color, DelegateOK function, object[] args ) {
		button_ok.onClick.RemoveAllListeners();
		button_cancel.onClick.RemoveAllListeners();

		text_title.text		=	title;
		slider_red.value	=	init_color.r;
		slider_green.value	=	init_color.g;
		slider_blue.value	=	init_color.b;
		slider_alpha.value	=	init_color.a;
		onOkClick			=	function;

		button_ok.onClick.AddListener( delegate { onOkClick( args ); } );
		button_cancel.onClick.AddListener( OnCancel );
		slider_red.onValueChanged.AddListener( delegate { OnColorChange(); } );
		slider_green.onValueChanged.AddListener( delegate { OnColorChange(); } );
		slider_blue.onValueChanged.AddListener( delegate { OnColorChange(); } );
		slider_alpha.onValueChanged.AddListener( delegate { OnColorChange(); } );

		Color	color	=	new Color( slider_red.value, slider_green.value, slider_blue.value, slider_alpha.value );
		panel_color.GetComponent<Image>().color		=	color;
	}

	// ------------------------------------------------------------------------------------------
	private	void	OnColorChange() {
		Color	color	=	new Color( slider_red.value, slider_green.value, slider_blue.value, slider_alpha.value );
		panel_color.GetComponent<Image>().color		=	color;
	}

	// ------------------------------------------------------------------------------------------
	public	Color	getColor() {
		return panel_color.GetComponent<Image>().color;
	}
	
	// ------------------------------------------------------------------------------------------
	public	void	OnCancel() {
		gameObject.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################