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
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{
	
	/// <summary>
	/// Used to enable an OpenType feature.
	/// https://en.wikipedia.org/wiki/List_of_typographic_features
	/// </summary>
	
	public class OpenTypeFeatureSet{
		
		/// <summary>The underlying set.</summary>
		public Dictionary<string,OpenTypeFeature> Set=new Dictionary<string,OpenTypeFeature>();
		
		/// <summary>Gets or creates the feature by its lowercase 4 character name.</summary>
		public OpenTypeFeature this[string feature]{
			get{
				
				OpenTypeFeature result;
				
				if(!Set.TryGetValue(feature,out result)){
					
					result=new OpenTypeFeature(feature);
					Set[feature]=result;
					
				}
				
				return result;
				
			}
		}
		
	}
	
}