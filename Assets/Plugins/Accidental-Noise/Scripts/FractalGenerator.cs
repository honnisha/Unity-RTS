using UnityEngine;
using System.Collections;
using AccidentalNoise;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class FractalGenerator : ExampleBase {

	public FractalType fractalType = FractalType.FBM;
	public BasisTypes basisType = BasisTypes.GRADIENT;
	public InterpTypes interpType = InterpTypes.QUINTIC;

	public int octaves = 6;
	public double frequency = 2.0;
	public double lacunarity = 2.0;
	public bool doGenerate = true;

	public ModuleBase GetFractal(){
		Fractal ground_shape_fractal = new Fractal(fractalType,
											basisType,
											interpType,
											octaves, 
											frequency, 
											null);
		ground_shape_fractal.SetLacunarity(lacunarity);
		return ground_shape_fractal as ModuleBase;
	}


	void Update(){
		if(doGenerate){
			GenerateInOneFrame(GetFractal()); 
			doGenerate = false;
		}
	}
}
