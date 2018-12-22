//--------------------------------------
//           Property values 
// standard set of referenceable values
//   Used mainly by Blade and Loonim.
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using BinaryIO;


namespace Values{
	
	/// <summary>
	/// A single value for a particular property.
	/// Properties are on complete objects, or e.g. materials.
	/// </summary>
	
	[Values.Preserve]
	public class PropertyValue{
		
		/// <summary>True if the value got changed.</summary>
		public bool Changed=true;
		public int UnresolvedID=-1;
		
		
		public virtual int GetID(){
			return -1;
		}
		
		public bool ResolveRequired{
			get{
				return (UnresolvedID!=-1);
			}
		}
		
		public virtual PropertyValue Copy(){
			return Create();
		}
		
		public virtual PropertyValue Create(){
			return null;
		}
		
		public virtual void Read(Reader reader){}
		
		public virtual void Write(Writer writer){}
		
	}
	
}