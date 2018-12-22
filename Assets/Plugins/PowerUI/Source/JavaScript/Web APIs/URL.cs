//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// The URL web API.
	/// </summary>
	
	public class URL:Location{
		
		public URL(string value):base(value){
			
		}
		
		public URL(string value,string baseURI):base(value,baseURI==null? null : new URL(baseURI)){
			
		}
		
		public URL(string value,URL baseURI):base(value,baseURI){
			
		}
		
		/*
		public static string createObjectURL(File blob){
			
		}
		
		public static string createObjectURL(Blob blob){
			
		}
		
		public static void revokeObjectURL(string objUrl){
			
		}
		*/
		
	}
	
}