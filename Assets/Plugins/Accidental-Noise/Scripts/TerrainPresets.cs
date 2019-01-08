using System.Collections;
using AccidentalNoise;

public enum PresetType {
	mountains, caves, cavesAndMountains, fractal
}

public class TerrainPresets {

	public static ModuleBase GetPreset(PresetType preset){
		switch(preset){
			case PresetType.mountains: return TerrainPresets.Mountains();
			case PresetType.caves: return TerrainPresets.Caves();
			case PresetType.cavesAndMountains: return TerrainPresets.CavesAndMountains();
			case PresetType.fractal: return TerrainPresets.FractalExample();
		}
		return TerrainPresets.CavesAndMountains();
	}

	public static ModuleBase FractalExample(){
		Fractal ground_shape_fractal = new Fractal(FractalType.FBM,
											BasisTypes.GRADIENT,
											InterpTypes.QUINTIC,
											6, 2, null);
		return ground_shape_fractal as ModuleBase;
	}


	public static ModuleBase Mountains(){
		Gradient ground_gradient = new Gradient(0, 0, 0, 1);
		Fractal ground_shape_fractal = new Fractal(FractalType.FBM,
													BasisTypes.GRADIENT,
													InterpTypes.QUINTIC,
													6, 2, null);

		ScaleOffset ground_scale = new ScaleOffset(0.5, 0, ground_shape_fractal);
		ScaleDomain ground_scale_y = new ScaleDomain(ground_scale, null, 0);
		TranslatedDomain ground_perturb = new TranslatedDomain(ground_gradient, null, ground_scale_y);
		Fractal ground_overhang_fractal = new Fractal(FractalType.FBM,
														BasisTypes.GRADIENT,
														InterpTypes.QUINTIC,
														6, 2, 23434);
		ScaleOffset ground_overhang_scale = new ScaleOffset(0.2, 0, ground_overhang_fractal);
		TranslatedDomain ground_overhang_perturb = new TranslatedDomain(ground_perturb, ground_overhang_scale, null);

		Select selection = new Select(ground_overhang_perturb, 0, 1, 0.5, null);
		return selection as ModuleBase;
	}


	public static ModuleBase Caves(){
		Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 2, 4533);
		Select cave_select = new Select(cave_shape, 1, 0, 0.6, 0);

		Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, null);
		ScaleOffset cave_perturb_scale = new ScaleOffset(0.25, 0, cave_perturb_fractal);
		TranslatedDomain cave_perturb = new TranslatedDomain(cave_select, cave_perturb_scale, null);

		Select selection = new Select(cave_perturb, 0, 1, 0.5, null);

		return selection;
	}


	public static ModuleBase CavesAndMountains(){
		AccidentalNoise.Gradient ground_gradient = new AccidentalNoise.Gradient(0, 0, 0, 1);

		// lowlands
		Fractal lowland_shape_fractal = new Fractal(FractalType.BILLOW, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 2, 0.25, null);
		AutoCorrect lowland_autocorrect = new AutoCorrect(lowland_shape_fractal, 0, 1);
		ScaleOffset lowland_scale = new ScaleOffset(0.125, -0.45, lowland_autocorrect);
		ScaleDomain lowland_y_scale = new ScaleDomain(lowland_scale, null, 0);
		TranslatedDomain lowland_terrain = new TranslatedDomain(ground_gradient, null, lowland_y_scale);

		// highlands
		Fractal highland_shape_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 4, 2, null);
		AutoCorrect highland_autocorrect = new AutoCorrect(highland_shape_fractal, -1, 1);
		ScaleOffset highland_scale = new ScaleOffset(0.25, 0, highland_autocorrect);
		ScaleDomain highland_y_scale = new ScaleDomain(highland_scale, null, 0);
		TranslatedDomain highland_terrain = new TranslatedDomain(ground_gradient, null, highland_y_scale);

		// mountains
		Fractal mountain_shape_fractal = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 8, 1, null);
		AutoCorrect mountain_autocorrect = new AutoCorrect(mountain_shape_fractal, -1, 1);
		ScaleOffset mountain_scale = new ScaleOffset(0.3, 0.15, mountain_autocorrect);
		ScaleDomain mountain_y_scale = new ScaleDomain(mountain_scale, null, 0.15);
		TranslatedDomain mountain_terrain = new TranslatedDomain(ground_gradient, null, mountain_y_scale);

		// terrain
		Fractal terrain_type_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 3, 0.125, null);
		AutoCorrect terrain_autocorrect = new AutoCorrect(terrain_type_fractal, 0, 1);
		ScaleDomain terrain_type_y_scale = new ScaleDomain(terrain_autocorrect, null, 0);
		Cache terrain_type_cache = new Cache(terrain_type_y_scale);
		Select highland_mountain_select = new Select(terrain_type_cache, highland_terrain, mountain_terrain, 0.55, 0.2);
		Select highland_lowland_select = new Select(terrain_type_cache, lowland_terrain, highland_mountain_select, 0.25, 0.15);
		Cache highland_lowland_select_cache = new Cache(highland_lowland_select);
		Select ground_select = new Select(highland_lowland_select_cache, 0, 1, 0.5, null);

		// caves
		Fractal cave_shape = new Fractal(FractalType.RIDGEDMULTI, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 1, 4, null);
		Bias cave_attenuate_bias = new Bias(highland_lowland_select_cache, 0.65);
		Combiner cave_shape_attenuate = new Combiner(CombinerTypes.MULT, cave_shape, cave_attenuate_bias);
		Fractal cave_perturb_fractal = new Fractal(FractalType.FBM, BasisTypes.GRADIENT, InterpTypes.QUINTIC, 6, 3, null);
		ScaleOffset cave_perturb_scale = new ScaleOffset(0.5, 0, cave_perturb_fractal);
		TranslatedDomain cave_perturb = new TranslatedDomain(cave_shape_attenuate, cave_perturb_scale, null);
		Select cave_select = new Select(cave_perturb, 1, 0, 0.75, 0);

		return new Combiner(CombinerTypes.MULT, cave_select, ground_select) as ModuleBase;
	}


}