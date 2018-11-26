using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ####################################################################################################
public class Player_UIAnim : MonoBehaviour {

	private		const int		int_DIFFRENCE		=		136;
	private		bool			animation_enable	=		false;
	private		int				direction			=		-1;
	private		float			animation_speed		=		4f;

	public		GameObject		container_volume;
	private		RectTransform	rect_volume;
	public		GameObject		container_control;
	public		GameObject		container_info;

	public		Button			button_volume;
	public		GameObject		slider_volume;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		rect_volume		=		container_volume.GetComponent<RectTransform>();
		button_volume.onClick.AddListener( OnButtonClick );
	}
	
	// ------------------------------------------------------------------------------------------
	private	void	Update () {
		var		rect_silder				=	slider_volume.GetComponent<RectTransform>();
		var		rect_control			=	container_control.GetComponent<RectTransform>();
		var		rect_info				=	container_info.GetComponent<RectTransform>();

		if ( animation_enable && direction == (-1) && rect_volume.sizeDelta.x < 184 ) {
			Vector2	pos_volume			=	rect_volume.sizeDelta;
			Vector2 pos_silder			=	rect_silder.sizeDelta;
			Vector2 pos_control			=	rect_control.offsetMax;
			Vector2 siz_control			=	rect_control.sizeDelta;
			Vector2 pos_info			=	rect_info.offsetMax;

			if ( rect_volume.sizeDelta.x > 58 ) { slider_volume.SetActive( true ); };
			if ( rect_volume.sizeDelta.x >= 184 ) {
				animation_enable		=	false;
				rect_volume.sizeDelta	=	new Vector2( 184, pos_volume.y );
				return;
			}
			rect_volume.sizeDelta		=	new Vector2( pos_volume.x + animation_speed, pos_volume.y );
			rect_silder.sizeDelta		=	new Vector2( pos_silder.x + animation_speed, pos_silder.y );
			rect_control.offsetMax		=	new Vector2( pos_control.x - animation_speed, pos_control.y );
			rect_control.sizeDelta		=	new Vector2( 320, siz_control.y );
			rect_info.offsetMax			=	new Vector2( pos_info.x - animation_speed, pos_info.y );
		}

		if ( animation_enable && direction == (1) && rect_volume.sizeDelta.x > 48 ) {
			Vector2	pos_volume			=	rect_volume.sizeDelta;
			Vector2 pos_silder			=	rect_silder.sizeDelta;
			Vector2 pos_control			=	rect_control.offsetMax;
			Vector2 siz_control			=	rect_control.sizeDelta;
			Vector2 pos_info			=	rect_info.offsetMax;

			if ( rect_silder.sizeDelta.x < 10 ) { slider_volume.SetActive( false ); };
			if ( rect_volume.sizeDelta.x <= 48 ) {
				animation_enable		=	false;
				rect_volume.sizeDelta	=	new Vector2( 48, pos_volume.y );
				return;
			}
			rect_volume.sizeDelta		=	new Vector2( pos_volume.x - animation_speed, pos_volume.y );
			rect_silder.sizeDelta		=	new Vector2( pos_silder.x - animation_speed, pos_silder.y );
			rect_control.offsetMax		=	new Vector2( pos_control.x + animation_speed, pos_control.y );
			rect_control.sizeDelta		=	new Vector2( 320, siz_control.y );
			rect_info.offsetMax			=	new Vector2( pos_info.x + animation_speed, pos_info.y );
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	OnButtonClick() {
		if ( direction < 0 ) {
			direction			=	1;
			animation_enable	=	true;
			//SetExpand();
		} else if ( direction > 0 ) {
			direction			=	-1;
			animation_enable	=	true;
			//SetCollapse();
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	SetCollapse() {
		var		rect_info		=	container_info.GetComponent<RectTransform>();
		var		rect_control	=	container_control.GetComponent<RectTransform>();

		slider_volume.SetActive( false );
		rect_volume.sizeDelta	=	new Vector2( 48, rect_volume.sizeDelta.y );
		rect_control.offsetMax	=	new Vector2( -72, rect_control.offsetMax.y );
		rect_control.sizeDelta	=	new Vector2( 320, rect_control.sizeDelta.y );
		rect_info.offsetMax		=	new Vector2( -404, rect_info.offsetMax.y );
	}

	private	void	SetExpand() {
		var		rect_info		=	container_info.GetComponent<RectTransform>();
		var		rect_control	=	container_control.GetComponent<RectTransform>();

		slider_volume.SetActive( true );
		rect_volume.sizeDelta	=	new Vector2( 184, rect_volume.sizeDelta.y );
		rect_control.offsetMax	=	new Vector2( -208, rect_control.offsetMax.y );
		rect_control.sizeDelta	=	new Vector2( 320, rect_control.sizeDelta.y );
		rect_info.offsetMax		=	new Vector2( -540, rect_info.offsetMax.y );
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################