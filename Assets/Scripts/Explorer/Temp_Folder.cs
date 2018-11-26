using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Temp_Folder : MonoBehaviour {

	private		bool			clicked			=		false;
	private		float			delay			=		1f;
	private		float			timer;
	private		string			icon_resource;
	private		string			file_name;

	public		GameObject		button_select;

	public		delegate void	DelegateClick(params object[] arguments);

	private		DelegateClick	function_OnClickOnce;
	private		DelegateClick	function_OnClickDouble;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() { Init(); }

	public	void	Init() {
		button_select.GetComponent<Button>().onClick.AddListener( OnClick );
	}

	// ------------------------------------------------------------------------------------------
	private	void	Update() {
		if (clicked) {
			if ((Time.time - timer) > delay) { clicked = false; }
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnClick () {
		string		index_get			=		gameObject.name.ToString();
		string		index_processing	=		"";
		foreach( char c in index_get.ToCharArray() ) { if (char.IsDigit(c)) { index_processing += c; } }

		if (!clicked) {
			clicked		=	true;
			timer		=	Time.time;
			function_OnClickOnce( new object[] { index_processing, icon_resource, file_name, CustomTemplateType.Directory } );
		} else {
			clicked		=	false;
			function_OnClickDouble( new object[] { index_processing, icon_resource, file_name, CustomTemplateType.Directory } );
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	SetOneClick( DelegateClick function ) {
		function_OnClickOnce	=	function;
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetDoubleClick( DelegateClick function ) {
		function_OnClickDouble	=	function;
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetText( string str_name ) {
		file_name			=		str_name;

		var		button		=		transform.GetChild(0).gameObject;
		var		text		=		button.transform.GetChild(1).gameObject;
		text.GetComponent<Text>().text	=	str_name;
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetImage( string res_name ) {
		icon_resource		=		res_name;

		var		button		=		transform.GetChild(0).gameObject;
		var		image		=		button.transform.GetChild(0).gameObject;
		image.GetComponent<RawImage>().texture	=	Resources.Load("Icons/Black/Files/" + res_name) as Texture;
	}
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################