using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Msg_Input : MonoBehaviour {

	public		Text			text_title;
	public		Button			button_ok;
	public		Button			button_cancel;
	public		InputField		edit_input;
	public		Text			edit_inputStatic;
	public		Text			edit_inputInput;

	public		delegate void	DelegateOK();
	private		DelegateOK		onOkClick;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	OnGUI () {
		if ( edit_input.isFocused && Input.GetKey(KeyCode.Return) ) { onOkClick(); }
	}

	// ------------------------------------------------------------------------------------------
	public	void	Init( string title, string input, DelegateOK function ) {
		text_title.text			=	title;
		edit_inputStatic.text	=	input;
		onOkClick				=	function;

		button_cancel.onClick.AddListener( onCancelClick );
		button_ok.onClick.RemoveAllListeners();
		button_ok.onClick.AddListener( delegate { onOkClick(); } );
	}

	// ------------------------------------------------------------------------------------------
	private	void	onCancelClick() {
		gameObject.GetComponent<Canvas>().enabled	=	false;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################