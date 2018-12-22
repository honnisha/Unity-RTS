//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;


namespace InfiniText{
	
	/// <summary>
	/// The various lookup sub-tables.
	/// See https://www.microsoft.com/typography/OTSPEC/GSUB.htm#SS for the info.</summary>
	
	public class LookupSubTable{
		
		
		public void ReadCoverage(FontParser parser,int start){
			
			ushort offset=parser.ReadUInt16();
			
			int pos=parser.Position;
			
			// Go there:
			parser.Position=offset+start;
			
			/*
			ushort fmt=parser.ReadUInt16();
			ushort count=parser.ReadUInt16();
			
			if(fmt==1){
				
				for (int i = 0; i < count; i++) {
					
					// Just the glyphID:
					ushort glyphID=parser.ReadUInt16();
					
				}
				
			}else{
				
				for (int i = 0; i < count; i++) {
					
					ushort start=parser.ReadUInt16();
					ushort end=parser.ReadUInt16();
					ushort index=parser.ReadUInt16();
					
				}
				
			}
			*/
			
			// Restore:
			parser.Position=pos;
			
		}
		
		public virtual void Load(FontParser parser,ushort fmt,int start){
			
		}
		
		protected void Fail(ushort format){
			
			throw new Exception("Broken font - "+GetType()+" table format wasn't right ("+format+").");
			
		}
		
	}
	
	
	public class SingleSubTable:LookupSubTable{
		
		public ushort DeltaGlyphId;
		
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
			if(fmt==1){
				
				ReadCoverage(parser,start);
				DeltaGlyphId=parser.ReadUInt16();
				
			}else if(fmt==2){
				
				ReadCoverage(parser,start);
				
				
			}else{
				Fail(fmt);
			}
			
		}
		
	}
	
	public class AlternateSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class MultiSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class LigatureSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class ContextualSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class ChainingContextualSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class ExtensionSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
	public class ReverseChainingContextualSubTable:LookupSubTable{
		
		public override void Load(FontParser parser,ushort fmt,int start){
			
		}
		
	}
	
}