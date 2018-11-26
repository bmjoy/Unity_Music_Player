using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public enum CustomMessageIconType { Alert, Info, Locked, Stop, }
// ####################################################################################################
public class Msg_Box : MonoBehaviour {

	public		Text			text_title;
	public		Text			text_context;
	public		Button			button_yes;
	public		Button			button_no;
	public		RawImage		image_icon;

	public		delegate void	DelegateYes( object[] args );
	private		DelegateYes		onYesClick;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	Init( string title, string context, CustomMessageIconType icon, DelegateYes function, object[] arguments ) {
		text_title.text			=	title;
		text_context.text		=	context;
		onYesClick				=	function;

		switch ( icon ) {
			case CustomMessageIconType.Alert:
				image_icon.texture	=	Resources.Load( "Icons/White/Actions/64_Alert" ) as Texture;
				break;
			case CustomMessageIconType.Info:
				image_icon.texture	=	Resources.Load( "Icons/White/Actions/64_Help" ) as Texture;
				break;
			case CustomMessageIconType.Locked:
				image_icon.texture	=	Resources.Load( "Icons/White/Actions/64_Lock" ) as Texture;
				break;
			case CustomMessageIconType.Stop:
				image_icon.texture	=	Resources.Load( "Icons/White/Actions/64_Cancel" ) as Texture;
				break;
		}

		button_no.onClick.AddListener( onCancelClick );
		button_yes.onClick.RemoveAllListeners();
		button_yes.onClick.AddListener( delegate { onYesClick( arguments ); } );
	}

	// ------------------------------------------------------------------------------------------
	private	void	onCancelClick() {
		gameObject.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################