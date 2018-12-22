//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>A plugin such as Flash or Java.</summary>
	public class Plugin{
		
		public string description;
		
		public string filename{
			get{
				return System.IO.Path.GetFileName(dllPath);
			}
		}
		
		public string name;
		public string version;
		public string vendor;
		
		public string dllPath;
		
		/// <summary>The mime types.</summary>
		public PluginMimeType[] mimeTypes;
		
	}
	
	public struct PluginMimeType{
		
		public string description;
		public string type;
		
		public PluginMimeType(string t,string desc){
			type=t;
			description=desc;
		}
		
	}
	
}