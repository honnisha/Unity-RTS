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
using InfiniText;
using Blaze;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// A CSS counter.
	/// </summary>
	public struct CssCounter{
		
		/// <summary>The name of this counter.</summary>
		public string Name;
		/// <summary>The current count value.</summary>
		public int Count;
		
		
		public CssCounter(string name){
			Name=name;
			Count=0;
		}
		
		public CssCounter(string name,int count){
			Name=name;
			Count=count;
		}
		
	}

}