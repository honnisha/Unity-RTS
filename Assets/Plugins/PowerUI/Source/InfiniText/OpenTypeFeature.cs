//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;


namespace InfiniText{
	
	/// <summary>
	/// Used to enable an OpenType feature.
	/// </summary>
	
	public class OpenTypeFeature{
		
		/// <summary>The name of the feature.</summary>
		public string Name;
		/// <summary>An extra parameter if the feature uses one.</summary>
		public int Parameter;
		
		
		public OpenTypeFeature(string name){
			Name=name;
		}
		
		public OpenTypeFeature(string name,int param){
			Name=name;
			Parameter=param;
		}
		
	}
	
}