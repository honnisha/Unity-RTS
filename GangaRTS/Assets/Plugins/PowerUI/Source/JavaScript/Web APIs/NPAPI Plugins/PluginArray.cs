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
	
	/// <summary>Plugin array as used by navigator.plugins.</summary>
	public class PluginArray : ICollection<Plugin>{
		
		public int Count{
			get{
				return 0;
			}
		}
		
		public void Clear(){}
		public void Add(Plugin p){}
		public bool Remove(Plugin p){ return false; }
		public bool Contains(Plugin p){ return false; }
		public void CopyTo(Plugin[] s,int i){}
		
        public IEnumerator<Plugin> GetEnumerator()
        {
            yield return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
			yield return null;
        }
		
		public bool IsReadOnly{
			get{
				return true;
			}
		}
		
		public Plugin this[int index]{
			get{
				return null;
			}
		}
		
		public Plugin this[string index]{
			get{
				return null;
			}
		}
		
	}
	
}