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
using System.IO;

#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

namespace PowerUI{

	/// <summary>
	/// Used by window.navigator.
	/// </summary>
	
	public partial class Navigator{
		
		internal static Dictionary<string,Plugin> PluginMap;
		internal static PluginArray LoadedPlugins;
		
		
		/// <summary>Is java enabled?</summary>
		public bool javaEnabled{
			get{
				return false;
			}
		}
		
		/// <summary>Installed plugins.</summary>
		public PluginArray plugins{
			get{
				
				if(LoadedPlugins==null){
					LoadPluginList();
				}
				
				return LoadedPlugins;
			}
		}
		
		#if UNITY_STANDALONE_WIN
		
		private void CollectPluginsRegistry(PluginArray arr,RegistryKey key){
			
			foreach (string v in key.GetSubKeyNames()){
				
				// Open it up:
				RegistryKey pluginData = key.OpenSubKey(v);
				
				Plugin p=new Plugin();
				
				// General meta:
				p.dllPath=pluginData.GetValue("Path","") as string;
				
				p.description=pluginData.GetValue("Description","") as string;
				p.name=pluginData.GetValue("ProductName","") as string;
				p.vendor=pluginData.GetValue("Vendor","") as string;
				p.version=pluginData.GetValue("Version","") as string;
				
				// May have an optional 'MimeTypes' folder:
				RegistryKey mimeTypes = pluginData.OpenSubKey("MimeTypes");
				
				List<PluginMimeType> types=new List<PluginMimeType>();
				
				if(mimeTypes!=null){
					
					foreach (string mT in mimeTypes.GetSubKeyNames()){
						
						// Open it up:
						RegistryKey mtData = mimeTypes.OpenSubKey(mT);
						
						// Add:
						types.Add(new PluginMimeType(mT,mtData.GetValue("Description","") as string));
						
						// Map:
						PluginMap[mT]=p;
						
					}
					
				}
				
				p.mimeTypes=types.ToArray();
				
			}
			
		}
		
		// private void CollectPluginsDir(PluginArray arr,string dir){}
		
		#endif
		
		/// <summary>Loads the Gecko plugin list.</summary>
		private void LoadPluginList(){
			
			PluginArray arr=new PluginArray();
			PluginMap=new Dictionary<string,Plugin>();
			LoadedPlugins=arr;
			
			#if UNITY_STANDALONE_WIN
			
			/*
			// Get moz path:
			string mozPath=System.Environment.GetEnvironmentVariable("MOZ_PLUGIN_PATH");
			
			if(mozPath!=null){
				
				CollectPluginsDir(arr,mozPath);
				
			}
			
			if(Directory.Exists("%APPDATA%\\Mozilla\\plugins")){
				
				CollectPluginsDir(arr,"%APPDATA%\\Mozilla\\plugins");
				
			}
			*/
			
			// From the registry:
			try{
				CollectPluginsRegistry(arr,Registry.CurrentUser.OpenSubKey("Software\\MozillaPlugins"));
			}catch{
				// Privs
			}
			
			try{
				CollectPluginsRegistry(arr,Registry.LocalMachine.OpenSubKey("Software\\MozillaPlugins"));
			}catch{
				// Privs
			}
			
			#endif
			
		}
		
	}
	
}