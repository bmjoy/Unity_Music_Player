using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ####################################################################################################
public class SquareGrid : MonoBehaviour {

	public		GameObject		object_peak;
	public		GameObject		object_fallow;
	private		GameObject[,]	array_peak;
	private		Player			script_player;
	private		Settings		script_settings;

	private		const int		const_SIZE				=	2;
	private		const float		const_SPACE				=	0.25f;

	private		int				accuracy				=	16;
	private		float[]			spectrum_oryginal;
	private		float[]			spectrum_modified;

	private		int[]			normalizer_condition	=	new int[7] { 60, 250, 500, 2000, 4000, 6000, 20000 };
	private		float[]			normalizer_modifiers	=	new float[7] { 0.002f, 0.007f, 0.013f, 0.062f, 0.150f, 0.250f, 0.650f };

	public		float			speed_peak				=	0.5f;
	public		int				height_max				=	20;
	public		int				height_upgrade			=	250;

	private		float			color_alpha				=	1.0f;
	private		bool			color_rainbow			=	false;
	private		float			color_adder				=	0.0234375f;
	private		float			color_speed				=	0.02f;

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	public	void	Init( Player player_script, Settings settings_script ) {
		script_player		=	player_script;
		script_settings		=	settings_script;

		array_peak			=	new GameObject[accuracy,accuracy];
		spectrum_oryginal	=	new float[accuracy];
		spectrum_modified	=	new float[accuracy];
		Generate();
	}

	// ------------------------------------------------------------------------------------------
	public	void	Generate() {
		float	position_x	=	-(((const_SIZE * accuracy) + (const_SPACE * (accuracy-1)))/2);
		float	position_z	=	-(((const_SIZE * accuracy) + (const_SPACE * (accuracy-1)))/2);

		for ( int line = 0; line < accuracy; line++ ) {
			for ( int i = 0; i < accuracy; i++ ) {
				// PEAKS
				GameObject	new_peak				=	Instantiate( object_peak, gameObject.transform );
				new_peak.transform.position			=	transform.position;
				new_peak.name						=	"Peak_" + i.ToString();

				new_peak.transform.eulerAngles		=	new Vector3( 0, 0, 0 );
				new_peak.transform.localScale		=	new Vector3( const_SIZE, 1, const_SIZE );
				new_peak.transform.position			=	new Vector3( position_x, 0.5f, position_z );
				array_peak[line,i]					=	new_peak;

				position_x							=	position_x + ( const_SIZE + const_SPACE );
			}
			position_x	=	-(((const_SIZE * accuracy) + (const_SPACE * (accuracy-1)))/2);
			position_z	=	position_z + ( const_SIZE + const_SPACE );
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	Work() {

		for ( int line = 0; line < accuracy; line++ ) {
			for ( int i = 0; i < accuracy; i++ ) {
				int trans	=	0;

				if ( line+1 < accuracy ) { trans = 1; }
				else { break; }

				var	hei_now		=	array_peak[line,i].transform.position;
				var hei_new		=	array_peak[line+trans,i].transform.position;
				var pos_now		=	array_peak[line,i].transform.localScale;
				var pos_new		=	array_peak[line+trans,i].transform.localScale;
				var color		=	array_peak[line+trans,i].GetComponent<Renderer>().material.color;

				array_peak[line,i].transform.localScale = new Vector3( pos_now.x, pos_new.y, pos_now.z );
				array_peak[line,i].transform.position = new Vector3( hei_now.x, hei_new.y, hei_now.z );
				array_peak[line,i].GetComponent<Renderer>().material.color = SetColor( color );

				// EMISSION
				var		material_peak	=	array_peak[line,i].GetComponent<Renderer>().material;

				if ( script_settings.vis_emission ) {
					float		probeP				=	hei_new.y / height_max;
					Color		emission_colorP		=	new Color( color.r * probeP, color.g * probeP, color.b * probeP );
					material_peak.SetColor( "_EmissionColor", emission_colorP );
				} else {
					material_peak.SetColor( "_EmissionColor", new Color( 0.1f, 0.1f, 0.1f ) );
				}

			}
		}

		spectrum_oryginal		=	DescreaseSpectrum( script_player.getSpectrum(), accuracy );

		for ( int i = 0; i < accuracy; i++ ) {
			if ( array_peak[accuracy-1,i] != null ) {
				// PEAKS
				var		local_peak		=	array_peak[accuracy-1,i].transform;
				var		render_peak		=	array_peak[accuracy-1,i].GetComponent<Renderer>();
				var		material_peak	=	render_peak.material;
				float	height			=	spectrum_oryginal[i] * height_upgrade;

				if ( spectrum_modified[i] < height ) { spectrum_modified[i] = height; }
				else { spectrum_modified[i] = spectrum_modified[i] - speed_peak; }
				if ( spectrum_modified[i] > height_max ) { spectrum_modified[i] = height_max; }
				if ( spectrum_modified[i] < 1 ) { spectrum_modified[i] = 1; }

				float	peak_pos				=	spectrum_modified[i];
				float	peak_hei				=	spectrum_modified[i]/2;
				render_peak.material.color		=	SetColor( render_peak.material.color );
				local_peak.localScale			=	new Vector3( const_SIZE, peak_pos, const_SIZE );
				local_peak.position				=	new Vector3( local_peak.position.x, peak_hei, local_peak.position.z );

				// EMISSION
				if ( script_settings.vis_emission ) {
					Color		color				=	material_peak.color;
					float		probeP				=	peak_hei / height_max;
					Color		emission_colorP		=	new Color( color.r * probeP, color.g * probeP, color.b * probeP );
					material_peak.SetColor( "_EmissionColor", emission_colorP );
				} else {
					material_peak.SetColor( "_EmissionColor", new Color( 0.1f, 0.1f, 0.1f ) );
				}
			}
		}
	}

	// ------------------------------------------------------------------------------------------
	public	void	Disable() {
		for ( int line = 0; line < accuracy; line++ ) {
			for ( int i = 0; i < accuracy; i++ ) {
				Destroy( array_peak[line,i] );
			}
		}
		array_peak			=	new GameObject[0,0];
		spectrum_oryginal	=	new float[0];
		spectrum_modified	=	new float[0];
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
	private	float[]	DescreaseSpectrum( float[] spectrum, int accu ) {
		float[]	new_float		=	new float[accu];
		float	average			=	0;
		int		spectrum_start	=	0;
		int		spectrum_end	=	0;

		//int		spectrum_add	=	128 / accu;
		//for ( int i = 0; i < accu; i++ ) {
		//	spectrum_start	=	(int) spectrum_end;
		//	spectrum_end	=	(int) spectrum_end + spectrum_add;
		//
		//	if ( i == 15 ) { spectrum_end = (int) spectrum_end - 1; }
		//	for ( int s = spectrum_start; s <= spectrum_end; s++ ) { average += (spectrum[s] /** Normalize(s)*/); }
		//	new_float[i] = average / (spectrum_end - spectrum_start);
		//}

		for ( int i = 0; i < accuracy; i++ ) {
			average		=	0;

			if ( i < 8 ) {
				spectrum_start	=	(int) spectrum_end;
				spectrum_end	=	(int) spectrum_end + 1;
			} else {
				spectrum_start	=	(int) spectrum_end;
				spectrum_end	=	(int) Mathf.Abs((Mathf.Pow(2.0f,(-2.0f + (i-2)/2.0f))*(-3 - 3 * Mathf.Pow((-1),(i-2)) - 2*Mathf.Sqrt(2) + 2 * Mathf.Pow((-1),(i-2)) * Mathf.Sqrt(2))));
				if ( spectrum_end % 2 != 0 ) { spectrum_end = (int) spectrum_end + 1; }
				if ( i == 15 ) { spectrum_end = (int) spectrum_end - 1; }
			}
			//Debug.Log(init_N + " -> " + ends_N);
			for ( int s = spectrum_start; s <= spectrum_end; s++ ) { average += (spectrum[s] * Normalize(s)); }
			new_float[i] = average / (spectrum_end - spectrum_start);
		}

		return new_float;
	}

	// ------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------
	private	Color	SetColor( Color color ) {

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
			if ( array_peak[accuracy-1,i] != null ) {
				var	render				=	array_peak[accuracy-1,i].GetComponent<Renderer>();
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
// ####################################################################################################