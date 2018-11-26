using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class Controller : MonoBehaviour {

	public		GameObject					ui;
	private		CustomVisualisationType		vis_type			=	CustomVisualisationType.none;

	private		Player						script_player;
	private		Settings					script_settings;

	public		GameObject					vis_CirclePeaks;
	public		GameObject					vis_SquarePeaks;
	public		GameObject					vis_SquareGrid;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	void	Start() { Init(); }

	public	void	Init() {
		script_settings		=		ui.GetComponent<Settings>();
		script_player		=		ui.GetComponent<Player>();
	}
		
	// ------------------------------------------------------------------------------------------
	private	void	Update () {
		if ( vis_type != script_settings.vis_type ) {
			DisableVisualisation( vis_type );
			SetVisualisation( script_settings.vis_type );
		}

		if ( vis_type != CustomVisualisationType.none ) {
			WorkVisualisation( vis_type );
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	DisableVisualisation( CustomVisualisationType vis ) {
		switch ( vis ) {
		case CustomVisualisationType.CirclePeaks:	vis_CirclePeaks.GetComponent<CirclePeaks>().Disable();	break;
		case CustomVisualisationType.SquarePeaks:	vis_SquarePeaks.GetComponent<SquarePeaks>().Disable();	break;
		case CustomVisualisationType.SquareGrid:	vis_SquareGrid.GetComponent<SquareGrid>().Disable();	break;
		default:	break;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	SetVisualisation( CustomVisualisationType vis ) {
		vis_type	=	vis;

		switch ( vis_type ) {
		case CustomVisualisationType.CirclePeaks:	vis_CirclePeaks.GetComponent<CirclePeaks>().Init( script_player, script_settings );	break;
		case CustomVisualisationType.SquarePeaks:	vis_SquarePeaks.GetComponent<SquarePeaks>().Init( script_player, script_settings );	break;
		case CustomVisualisationType.SquareGrid:	vis_SquareGrid.GetComponent<SquareGrid>().Init( script_player, script_settings );	break;
		default:	break;
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	WorkVisualisation( CustomVisualisationType vis ) {
		switch ( vis ) {
		case CustomVisualisationType.CirclePeaks:	vis_CirclePeaks.GetComponent<CirclePeaks>().Work();	break;
		case CustomVisualisationType.SquarePeaks:	vis_SquarePeaks.GetComponent<SquarePeaks>().Work();	break;
		case CustomVisualisationType.SquareGrid:	vis_SquareGrid.GetComponent<SquareGrid>().Work();	break;
		default:	break;
		}
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// ####################################################################################################