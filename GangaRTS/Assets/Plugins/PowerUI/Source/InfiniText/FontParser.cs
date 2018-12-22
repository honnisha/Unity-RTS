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
	
	internal class TableInfo{
		public int Offset;
		
		public TableInfo(int offset){
			Offset=offset;
		}
		
	}
	
	public class FontParser{
		
		/// <summary>The tables in this parser.</summary>
		internal Dictionary<string,TableInfo> Tables=new Dictionary<string,TableInfo>();
		
		public byte[] Data;
		public int Position;
		public int IndexToLocFormat;
		public int GlyphCount;
		
		
		public FontParser(byte[] data){
			
			Data=data;
			
		}
		
		/// <summary>The location of the named table in the stream.</summary>
		public int TableOffset(string table){
			TableInfo info;
			
			if(Tables.TryGetValue(table,out info)){
				return info.Offset;
			}
			
			// Not found.
			return 0;
			
		}
		
		/// <summary>Handles a table which is located at 'offset' in the stream and has the given tag.</summary>
		public bool HandleTable(string tag,int offset,FontFace font){
			
			switch (tag){
				
				case "head":
					
					// Load the header:
					if(!HeaderTables.Load(this,offset,font,out IndexToLocFormat)){
						return false;
					}
					
				break;
				case "maxp":
					
					// Maxp table:
					MaxpTables.Load(this,offset,font,out GlyphCount);
					
				break;
				default:
					// Add to set to load when the rest do:
					Tables[tag]=new TableInfo(offset);
				break;
				
			}
			
			// All Ok
			return true;
			
		}
		
		public byte ReadByte(){
			
			return Data[Position++];
			
		}
		
		public int ReadOffset(int offSize){
		
			uint v=0;
			
			for(int i=0;i<offSize;i++){
				// Shift 8 places:
				v=(v<<8);
				
				// Append the next byte:
				v|=ReadByte();
			}
			
			return (int)v;
		
		}
		
		public short ReadInt16(){
			
			return (short)((Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public ushort ReadUInt16(){
			
			return (ushort)((Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public ushort ReadUInt16(ref int index){
			
			ushort result=(ushort)((Data[index] << 8) | Data[index+1]);
			
			index+=2;
			
			return result;
		}
		
		public short ReadInt16(ref int index){
			
			short result=(short)((Data[index] << 8) | Data[index+1]);
			
			index+=2;
			
			return result;
		}
		
		public int ReadInt32(){
			
			return (int)((Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public uint ReadUInt32(){
			
			return (uint)((Data[Position++] << 24) | (Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public int ReadInt24(){
			
			return (int)((Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public uint ReadUInt24(){
			
			return (uint)((Data[Position++] << 16) | (Data[Position++] << 8) | Data[Position++]);
			
		}
		
		public float ReadRevision(){
			
			int dec=ReadInt16();
			int frac=ReadInt16();
			
			return ((float)dec + (float)frac/10f);
			
		}
		
		public float ReadVersion(){
			
			int major = ReadUInt16();
			int minor = ReadUInt16();
			
			return major + minor / 0x1000f / 10f;
			
		}
		
		public float usWinAscent;
		public float usWinDescent;
		public float HheaAscender;
		public float HheaDescender;
		public float HheaLineGap;
		
		public void ApplyWindowsMetrics(FontFace font){
			
			if(usWinAscent==0f && usWinDescent==0f){
				
				// Apple metrics are our only hope!
				font.LineGap=HheaLineGap;
				font.Ascender=HheaAscender;
				font.Descender=HheaDescender;
				
			}else{
				
				// Get the internal leading (the leading included in ascent at the top):
				float internalLeading=usWinAscent + usWinDescent - 1f;
				
				// External leading (the gap between lines, at the bottom):
				float extLeading=(float)Math.Max(0f,HheaLineGap - ((usWinAscent + usWinDescent) - (HheaAscender - HheaDescender)));
				
				// Remove internal leading from ascender:
				font.Ascender=usWinAscent-internalLeading;
				font.Descender=usWinDescent;
				font.LineGap=internalLeading + extLeading;
				
			}
			
		}
		
		public ulong ReadTime(){
			
			Position+=8;
			return 0;
			
		}
		
		public string ReadString(int length){
			
			char[] result=new char[length];
			
			for(int i=0;i<length;i++){
				
				result[i]=(char)Data[Position++];
				
			}
			
			return new string(result);
			
		}
		
		public ulong ReadBase128(){
			ulong result = 0;
			
			for (int i = 0; i < 5; i++) {
				byte data = ReadByte();
				
				result = (ulong)(result << 7) | (ulong)((ulong)data & 0x7F);
				
				if ((result & 0xFE0000000)!=0) {
					throw new Exception("WOFF2: Base 128 number overflow");
				}
				
				if ((data & 0x80) == 0) {
					return result;
				}
				
			}
			
			return result;
			
		}
		
		public string ReadTag(){
			
			return ReadString(4);
			
		}
		
		public float ReadF2Dot14(){
			
			return (float) ReadInt16() / 16384f;
			
		}
		
		public int ReadFixed(out int frac){
			
			int dec=ReadInt16();
			frac=ReadInt16();
			
			return dec;
			
		}
		
	}

}