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
	/// A set of values for a particular mapped property.
	/// </summary>
	
	public class PropertyValueSet:PropertyValue{
		
		public PropertyValue[] Set;
		
		
		public PropertyValueSet(){}
		
		public PropertyValueSet(params PropertyValue[] set){
			Set=set;
		}
		
		
		public override int GetID(){
			return 6;
		}
		
		public void Add(PropertyValue value){
			
			if(Set==null){
				Set=new PropertyValue[]{value};
			}else{
				
				// Create:
				PropertyValue[] newSet=new PropertyValue[Set.Length+1];
				
				// Transfer:
				Array.Copy(Set,0,newSet,0,Set.Length);
				
				// Apply last:
				newSet[Set.Length]=value;
				
				// Apply:
				Set=newSet;
				
			}
			
		}
		
		public PropertyValue this[int index]{
			get{
				return Set[index];
			}
			set{
				Set[index]=value;
			}
		}
		
		public int Length{
			get{
				return Set.Length;
			}
		}
		
		public int Count{
			get{
				return Set.Length;
			}
		}
		
		public bool Remove(PropertyValue value){
			
			// Get the index:
			int index=GetIndex(value);
			
			if(index!=-1){
				return Remove(index);
			}
			
			return false;
			
		}
		
		public int GetIndex(PropertyValue value){
			
			if(Set==null){
				return -1;
			}
			
			for(int i=0;i<Set.Length;i++){
				if(Set[i]==value){
					return i;
				}
			}
			
			return -1;
			
		}
		
		public bool Remove(int index){
			
			if(Set==null || Set.Length<=index || index<0){
				return false;
			}
			
			if(Set.Length==1){
				Set=null;
			}else{
				
				// Create set:
				PropertyValue[] newSet=new PropertyValue[Set.Length-1];
				
				// Copy before index:
				Array.Copy(Set,0,newSet,0,index);
				
				// Copy after index:
				int afterCount=Set.Length-index-1;
				
				if(afterCount>0){
					Array.Copy(Set,index+1,newSet,index,afterCount);
				}
				
				// Apply set:
				Set=newSet;
				
			}
			
			return true;
			
		}
		
		public override PropertyValue Create(){
			return new PropertyValueSet();
		}
		
		public override PropertyValue Copy(){
			
			PropertyValueSet value=new PropertyValueSet();
			
			if(Set!=null){
				
				// Create new set:
				value.Set=new PropertyValue[Set.Length];
				
				// For each value:
				for(int i=0;i<Set.Length;i++){
					
					// Get current:
					PropertyValue current=Set[i];
					
					// Copy it too if it was non-null:
					if(current!=null){
						value.Set[i]=current.Copy();
					}
					
				}
				
			}
			
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			// How many?
			int count=(int)reader.ReadCompressed();
			
			Set=new PropertyValue[count];
			
			for(int i=0;i<count;i++){
				
				// Read value:
				Set[i]=PropertyValues.ReadPropertyValue(reader);
				
			}
			
		}
		
		public override void Write(Writer writer){
			
			if(Set==null){
				writer.WriteCompressed(0);
				return;
			}
			
			int count=Set.Length;
			writer.WriteCompressed((uint)count);
			
			for(int i=0;i<count;i++){
				
				PropertyValue value=Set[i];
				
				// Write the type:
				writer.WriteCompressed((uint)value.GetID());
				
				// Write value:
				value.Write(writer);
				
			}
			
		}
		
	}
	
}