using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Msg_Awaiter : MonoBehaviour {

	public		Text		text_title;
	public		Text		text_content;
	public		Slider		progress;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	Show( string title, string content ) {
		text_title.text		=	title;
		text_content.text	=	content;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	ShowProgressBar( bool visible, int max ) {
		progress.gameObject.SetActive( visible );
		progress.maxValue	=	max;
		progress.value		=	0;
	}

	// ------------------------------------------------------------------------------------------
	public	void	Increment( int inc ) {
		progress.value	+=	inc;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################