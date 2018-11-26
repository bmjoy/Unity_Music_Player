using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// #############################################################################
//  X   X   XXXXX   XXXXX   X   X   XXXX     XXX    XXXX    XXXXX
//	X   X     X     X       X   X   X   X   X   X   X   X     X  
//	X   X     X     XXX     X   X   XXXX    X   X   XXXX      X  
//	 X X      X     X        X X    X       X   X   X   X     X  
//	  X     XXXXX   XXXXX     X     X        XXX    X   X     X  
//
//	 XXXX   X   X   X   X    XXX 
//	X        X X    XX  X   X   X
//	 XXX      X     X X X   X    
//	    X     X     X  XX   X   X
//	XXXX      X     X   X    XXX 
// #############################################################################

public class ViewportSync : MonoBehaviour {

	void Start() {
		UpdateRect();
		//UpdateScrollbar();
	}

	// ----------------------------------------------------------------------
	/// <summary> Resetuje pozycję kontenera list UI </summary>

	public void UpdateRect() {
		GetComponent<RectTransform>().anchorMin		=	new Vector2( 0.0f, 0.0f );
		GetComponent<RectTransform>().anchorMax		=	new Vector2( 1.0f, 1.0f );
		GetComponent<RectTransform>().pivot			=	new Vector2( 0.5f, 0.5f );
	}

	// ----------------------------------------------------------------------
	public void UpdateScrollbar() {
		for ( int i = 0; i < transform.childCount; i++ ) {
			Scrollbar	sc	=	transform.GetChild(i).GetComponent<Scrollbar>();

			if ( sc != null ) {
				sc.value	=	0.0f;
			}
		}
	}

}

// #############################################################################