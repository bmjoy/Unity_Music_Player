﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class CirclePeaks : MonoBehaviour {

	public		GameObject		object_peak;
	public		GameObject		object_fallow;
	private		GameObject[]	array_peak;
	private		GameObject[]	array_fallow;
	private		Player			script_player;
	private		Settings		script_settings;

	private		int				accuracy;
	private		float[]			spectrum_oryginal;
	private		float[]			spectrum_modified;
	private		float[]			spectrum_fallower;
	private		bool			show_fallowers;

	private		int[]			normalizer_condition	=	new int[7] { 60, 250, 500, 2000, 4000, 6000, 20000 };
	private		float[]			normalizer_modifiers	=	new float[7] { 0.002f, 0.007f, 0.013f, 0.062f, 0.150f, 0.250f, 0.650f };

	public		int				radius					=	50;
	public		float			speed_peak				=	0.5f;
	public		float			speed_fallow			=	0.1f;
	public		int				height_max				=	75;
	public		int				height_upgrade			=	2500;

	private		float			color_alpha				=	1.0f;
	private		bool			color_rainbow			=	false;
	private		float			color_adder				=	0.0234375f;
	private		float			color_speed				=	0.02f;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	Init( Player player_script, Settings settings_script ) {
		script_player		=	player_script;
		accuracy			=	player_script.spectrum_accuracy-1;
		script_settings		=	settings_script;

		array_peak			=	new GameObject[accuracy];
		array_fallow		=	new GameObject[accuracy];
		spectrum_oryginal	=	new float[accuracy];
		spectrum_modified	=	new float[accuracy];
		spectrum_fallower	=	new float[accuracy];
		Generate();
	}

	// ------------------------------------------------------------------------------------------
	public	void	Generate() {
		for ( int i = 0; i < accuracy; i++ ) {
			// PEAKS
			GameObject	new_peak				=	Instantiate( object_peak, gameObject.transform );
			new_peak.transform.position			=	transform.position;
			new_peak.name						=	"Peak_" + i.ToString();

			float	alpha						=	((2*3.14f) / accuracy) * i;
			float	position_x					=	radius * Mathf.Sin(alpha);
			float	position_z					=	radius * Mathf.Cos(alpha);

			new_peak.transform.eulerAngles		=	new Vector3( 0, alpha, 0 );
			new_peak.transform.localScale		=	new Vector3( 1, 0.25f, 1 );
			new_peak.transform.position			=	new Vector3( position_x, 0.25f, position_z );
			array_peak[i]						=	new_peak;

			// FALLOWERS
			GameObject	new_fallow				=	Instantiate( object_fallow, gameObject.transform );
			new_fallow.transform.position		=	transform.position;
			new_fallow.name						=	"Fallow_" + i.ToString();
			new_fallow.transform.eulerAngles	=	new Vector3( 0, alpha, 0 );
			new_fallow.transform.localScale		=	new Vector3( 1, 0.1f, 1 );
			new_fallow.transform.position		=	new Vector3( position_x, 0.06f, position_z );
			array_fallow[i]						=	new_fallow;
		}
	}
		
	// ------------------------------------------------------------------------------------------
	public	void	Work() {
		spectrum_oryginal		=	script_player.getSpectrum();

		if ( show_fallowers != script_settings.vis_fallowers ) {
			show_fallowers = script_settings.vis_fallowers;
			EnableFallowers( show_fallowers );
		}

		for ( int i = 0; i < accuracy; i++ ) {
			if ( array_peak[i] != null ) {
				// PEAKS
				var		local_peak		=	array_peak[i].transform;
				var		render_peak		=	array_peak[i].GetComponent<Renderer>();
				var		material_peak	=	render_peak.material;
				float	height			=	spectrum_oryginal[i] * height_upgrade * Normalize(i);

				if ( spectrum_modified[i] < height ) { spectrum_modified[i] = height; }
				else { spectrum_modified[i] = spectrum_modified[i] - speed_peak; }
				if ( spectrum_modified[i] > height_max ) { spectrum_modified[i] = height_max; }
				if ( spectrum_modified[i] < 0.25f ) { spectrum_modified[i] = 0.25f; }

				float	peak_pos				=	spectrum_modified[i];
				float	peak_hei				=	spectrum_modified[i];
				render_peak.material.color		=	SetColor( render_peak.material.color, peak_hei );
				local_peak.localScale			=	new Vector3( 1, peak_hei, 1 );
				local_peak.position				=	new Vector3( local_peak.position.x, peak_pos, local_peak.position.z );

				var		local_fallow			=	array_fallow[i].transform;
				var		render_fallow			=	array_fallow[i].GetComponent<Renderer>();
				var		material_fallow			=	render_fallow.material;

				// FALLOWERS
				if (show_fallowers) {

					if ( spectrum_fallower[i] < height*2f ) { spectrum_fallower[i] = height*2f; }
					else { spectrum_fallower[i] = spectrum_fallower[i] - speed_fallow; }
					if ( spectrum_fallower[i] > height_max+0.6f ) { spectrum_fallower[i] = height_max+0.6f; }
					if ( spectrum_fallower[i] < 0.6f )	 { spectrum_fallower[i] = 0.6f; }

					float	fallow_pos				=	spectrum_fallower[i];
					render_fallow.material.color	=	render_peak.material.color;
					local_fallow.position			=	new Vector3( local_fallow.position.x, fallow_pos, local_fallow.position.z );
				}

				// EMISSION
				if ( script_settings.vis_emission ) {
					Color		color				=	material_peak.color;
					float		probeP				=	peak_hei / height_max;
					float		probeF				=	spectrum_fallower[i] / height_max;
					Color		emission_colorP		=	new Color( color.r * probeP, color.g * probeP, color.b * probeP );
					Color		emission_colorF		=	new Color( color.r * probeF, color.g * probeF, color.b * probeF );
					material_peak.SetColor( "_EmissionColor", emission_colorP );
					material_fallow.SetColor( "_EmissionColor", emission_colorF );
				} else {
					material_peak.SetColor( "_EmissionColor", new Color( 0.1f, 0.1f, 0.1f ) );
					material_fallow.SetColor( "_EmissionColor", new Color( 0.1f, 0.1f, 0.1f ) );
				}

			}
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	Disable() {
		for ( int i = 0; i < accuracy; i++ ) {
			Destroy( array_peak[i] );
			Destroy( array_fallow[i] );
		}
		array_peak			=	new GameObject[0];
		array_fallow		=	new GameObject[0];
		spectrum_oryginal	=	new float[0];
		spectrum_modified	=	new float[0];
		spectrum_fallower	=	new float[0];
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	EnableFallowers( bool value ) {
		for ( int i = 0; i < accuracy; i++ ) { array_fallow[i].SetActive( value ); }
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	float	Normalize( int position ) { 
		int	finder	=	( position * 20000 ) / accuracy;

		for ( int i = 0; i < 7; i++ ) {
			if ( finder < normalizer_condition[i] ) { return (float) normalizer_modifiers[i]; }
		}
		return 1.0f;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	Color	SetColor( Color color, float spec_height ) {

		if ( script_settings.vis_coloration == CustomVisualisationColor.custom ) {
			color_rainbow	=	false;
			return ColorNormal();
		}

		if ( script_settings.vis_coloration == CustomVisualisationColor.rainbow ) {
			if ( !color_rainbow ) {
				Color color_generator	=	new Color( 1.0f, 0.0f, 0.0f, color_alpha );
				GenerateColorRainbow( color_generator );
				color_rainbow			=	true;
				return color_generator;
			}
			return ColorRainbow( color );
		}

		return color;
	}

	// ------------------------------------------------------------------------------------------
	private	Color	ColorNormal() {
		Color	new_color	=	new Color(
			script_settings.vis_color.r,
			script_settings.vis_color.g,
			script_settings.vis_color.b,
			color_alpha
		);

		return new_color;
	}

	// ------------------------------------------------------------------------------------------
	private	void	GenerateColorRainbow( Color color_generator ) {
		for ( int i = 0; i < accuracy; i++ ) {
			if ( array_peak[i] != null ) {
				var	render				=	array_peak[i].GetComponent<Renderer>();
				color_generator			=	ColorRainbow( color_generator );
				render.material.color	=	color_generator;
			}
		}
	}

	// ------------------------------------------------------------------------------------------
	private	Color	ColorRainbow( Color color ) {
		Color	new_color	=	Color.white;

		if 	( color_rainbow == false ) { color_adder = 6.0f / accuracy; }
		else { color_adder = color_speed; }

		// ------------------------------ Up Green ------------------------------
		if ( (color.r >= 1.0f) && (color.g < 1.0f) && (color.b <= 0.0f) ) {
			new_color	=	new Color( color.r, color.g + color_adder, color.b, color_alpha );
			if ( new_color.g >= 1.0f ) {
				new_color		=	new Color( new_color.r, 1.0f, new_color.b, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ Down Red ------------------------------
		if ( (color.r > 0.0f) && (color.g >= 1.0f) && (color.b <= 0.0f) ) {
			new_color	=	new Color( color.r - color_adder, color.g, color.b, color_alpha );
			if ( new_color.r <= 0.0f ) {
				new_color		=	new Color( 0.0f, new_color.g, new_color.b, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ Up Blue ------------------------------
		if ( (color.r <= 0.0f) && (color.g >= 1.0f) && (color.b < 1.0f) ) {
			new_color	=	new Color( color.r, color.g, color.b + color_adder, color_alpha );
			if ( new_color.b >= 1.0f ) {
				new_color		=	new Color( new_color.r, new_color.g, 1.0f, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ Down Green ------------------------------
		if ( (color.r <= 0.0f) && (color.g > 0.0f) && (color.b >= 1.0f) ) {
			new_color	=	new Color( color.r, color.g - color_adder, color.b, color_alpha );
			if ( new_color.g <= 0.0f ) {
				new_color		=	new Color( new_color.r, 0.0f, new_color.b, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ Up Red ------------------------------
		if ( (color.r < 1.0f) && (color.g <= 0.0f) && (color.b >= 1.0f) ) {
			new_color	=	new Color( color.r + color_adder, color.g, color.b, color_alpha );
			if ( new_color.r >= 1.0f ) {
				new_color		=	new Color( 1.0f, new_color.g, new_color.b, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ Down Blue ------------------------------
		if ( (color.r >= 1.0f) && (color.g <= 0.0f) && (color.b > 0.0f) ) {
			new_color	=	new Color( color.r, color.g, color.b - color_adder, color_alpha );
			if ( new_color.b <= 0.0f ) {
				new_color		=	new Color( new_color.r, new_color.g, 0.0f, color_alpha );
			}
			return new_color;
		}

		// ------------------------------ No Color ------------------------------
		return color;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
}
// #################################################################################################### 
