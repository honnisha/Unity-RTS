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
using Dom;
using System.Text;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// The Web worker API.
	/// </summary>
	public class Worker{
		
		public Worker(string fileURL){
			
			// Download that file now:
			DataPackage package=new DataPackage(fileURL);
			
			// Apply onload method:
			package.onload=delegate(UIEvent e){
				
				// Start the worker
				// TODO
				// Compiles with it's own global scope.
				
			};
			
			// Send now:
			package.send();
			
		}
		
	}
	
	public partial class WorkerGlobalScope{
		
		/// <summary>Escapes the given string, essentially making any HTML it contains literal.</summary>
		public string escapeHTML(string html){
			return Dom.Text.Escape(html);
		}
		
		/// <summary>Parses the given text into a number.</summary>
		public int parseInt(string text){
			if(string.IsNullOrEmpty(text)){
				return 0;
			}
			
			return int.Parse(text);
		}
		
	}
	
	#region WindowTimers
	
	public partial class WorkerGlobalScope{
		
		/// <summary>Clears intervals.</summary>
		public void clearInterval(UITimer timer){
			if(timer!=null){
				timer.Stop();
			}
		}
		
		/// <summary>Sets an interval.</summary>
		public UITimer setInterval(OnUITimer method,int ms){
			return new UITimer(false,ms,method);
		}
		
		/// <summary>Sets a timeout.</summary>
		public UITimer setTimeout(OnUITimer method,int ms){
			return new UITimer(true,ms,method);
		}
		
	}
	
	#endregion
	
}