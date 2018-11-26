using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class Camera_Mouse : MonoBehaviour {

	public		GameObject		ui;
	private		Settings		script_settings;

	private		bool			lockers				=	true;
	private		bool			locker_x			=	false;
	private		bool			locker_y			=	false;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() {
		Init();
	}

	public	void	Init() {
		script_settings		=		ui.GetComponent<Settings>();
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Update() {
		if ( Input.GetKey( KeyCode.LeftShift ) && script_settings.cam_shift ) {
			if ( script_settings.cam_position != 0 ) { script_settings.cam_position = 0; }
			MoveXY();
			MoveZoom();
			Rotate();
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	MoveXY() {
		if ( Input.GetMouseButton(1) ) {
			float	move_x	=	Input.GetAxis("Mouse X");
			float	move_y	=	Input.GetAxis("Mouse Y");

			if ( lockers ) {
				if ( !locker_x && !locker_y ) {
						 if (move_x > 0.1f || move_x < -0.1f ) { locker_x = true; }
					else if (move_y > 0.1f || move_y < -0.1f ) { locker_y = true; }
					else { return; }
				}
				if ( locker_x ) { move_y = 0; }
				if ( locker_y ) { move_x = 0; }
			}

			transform.position	+=	transform.up * move_y;
			transform.position	+=	transform.right * move_x;

			if (transform.position.x > 100) { transform.position = new Vector3( 100, transform.position.y, transform.position.z ); }
			if (transform.position.x < -100) { transform.position = new Vector3( -100, transform.position.y, transform.position.z ); }
			if (transform.position.y > 100) { transform.position = new Vector3( transform.position.x, 100, transform.position.z ); }
			if (transform.position.y < -100) { transform.position = new Vector3( transform.position.x, -100, transform.position.z ); }

			script_settings.cam_positions[0].posX = transform.position.x;
			script_settings.cam_positions[0].posY = transform.position.y;
		}

		if ( Input.GetMouseButtonUp(1) ) {
			locker_x	=	false;
			locker_y	=	false;
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	MoveZoom() {
		float	wheel	=	Input.GetAxis("Mouse ScrollWheel");

		if ( wheel > 0.1f || wheel < -0.1f ) {
			transform.position	+=	transform.forward * wheel;

			if (transform.position.z > 100) { transform.position = new Vector3( transform.position.x, transform.position.y, 100 ); }
			if (transform.position.z < -100) { transform.position = new Vector3( transform.position.x, transform.position.y, -100 ); }

			script_settings.cam_positions[0].posZ = transform.position.z;
		}
	}

	// ------------------------------------------------------------------------------------------
	private	void	Rotate() {
		if ( Input.GetMouseButton(0) ) {
			float	move_x	=	Input.GetAxis("Mouse X");
			float	move_y	=	Input.GetAxis("Mouse Y");

			if ( lockers ) {
				if ( !locker_x && !locker_y ) {
						 if (move_x > 0.1f || move_x < -0.1f ) { locker_x = true; }
					else if (move_y > 0.1f || move_y < -0.1f ) { locker_y = true; }
					else { return; }
				}
				if ( locker_x ) { move_y = 0; }
				if ( locker_y ) { move_x = 0; }
			}

			transform.Rotate( move_y, move_x, 0 );

			script_settings.cam_positions[0].rotX = transform.eulerAngles.x;
			script_settings.cam_positions[0].rotY = transform.eulerAngles.y;
		}

		if ( Input.GetMouseButton(2) ) {
			float	move_x	=	Input.GetAxis("Mouse X");
			transform.Rotate( 0, 0, move_x );

			script_settings.cam_positions[0].rotZ = transform.eulerAngles.z;
		}

		if ( Input.GetMouseButtonUp(0) ) {
			locker_x	=	false;
			locker_y	=	false;
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}

// ####################################################################################################