using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly name section (custom).</summary>
	public class NameSection : Section{
		
		public NameSection():base("name"){}
		
		
		/*
		public override void Load(Reader reader,int length){
			
			// Name subsections
			
		}
		*/
		
	}
	
}