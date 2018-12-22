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

	public class CffGlyphParser{
		
		public float X;
		public float Y;
		public int NStems;
		public float Width;
		public bool HasWidth;
		public int SubrsBias; // set
		public int GsubrsBias;
		public CffStack Stack;
		public float NominalWidthX;
		public FontParser Parser;
		public Glyph Glyph;
		public FontFace Font;
		public CffSubPosition[] GSubrs;
		public CffSubPosition[] Subrs;
		public float DefaultWidthX;
		public float ScaleRatio;
		public bool FullLoad=true;
		
		
		public CffGlyphParser(FontParser parser,FontFace font){
			
			Font=font;
			Parser=parser;
			ScaleRatio=1f/font.UnitsPerEmF;
			Stack=new CffStack();
		}
		
		public void Reset(Glyph glyph){
			
			X=0f;
			Y=0f;
			NStems=0;
			HasWidth=false;
			Width=DefaultWidthX;
			Stack.Clear();
			
			Glyph=glyph;
		}
		
		public void LoadFully(Glyph glyph,LoadMetaPoint meta){
			
			// Reset this parser:
			Reset(glyph);
			
			// Parse now:
			Parse(meta.Start,meta.Length);
			
			// Apply width:
			glyph.AdvanceWidth=Width * ScaleRatio;
			
			// Close if we haven't already:
			glyph.ClosePath();
			
			if(Font.WindingUnknown){
				// Find the winding now:
				Font.FindWinding(glyph);
			}
			
		}
		
		public Glyph LoadGlyph(int start,int length){
			
			// Create our glyph:
			Glyph glyph=new Glyph(Font);
			
			if(FullLoad){
				
				// Reset this parser:
				Reset(glyph);
				
				// Parse now:
				Parse(start,length);
				
				// Apply width:
				glyph.AdvanceWidth=Width * ScaleRatio;
				
				// Close if we haven't already:
				glyph.ClosePath();
				
				if(Font.WindingUnknown){
					// Find the winding now:
					Font.FindWinding(glyph);
				}
				
			}else{
				
				// Increase unloaded count:
				Font.UnloadedGlyphs++;
				
				// Add position info:
				glyph.AddPathNode(new LoadMetaPoint(start,length));
				
			}
			
			return glyph;
		}
		
		private void ParseStems(){
			
			// The number of stem operators on the stack is always even.
			// If the value is uneven, that means a width is specified.
			
			if(Stack.IsOdd && !HasWidth){
				Width=Stack.Shift()+NominalWidthX;
			}
			
			NStems+=Stack.Length>>1;
			Stack.Clear();
			HasWidth=true;
		}
		
		
		private void Parse(int start,int codeLength){
			
			// Seek there now:
			Parser.Position=start;
			
			// Where should the parser quit?
			int max=start+codeLength;
			
			float c1x;
			float c1y;
			float c2x;
			float c2y;
			int subIndex;
			CffSubPosition subCode;
			
			// For each bytecode..
			while(Parser.Position<max){
				
				// Grab the byte:
				byte v=Parser.ReadByte();
			
				switch(v){
					case 1: // hstem
						ParseStems();
					break;
					case 3: // vstem
						ParseStems();
					break;
					case 4: // vmoveto
						
						if(Stack.Length>1 &&!HasWidth){
							Width=Stack.Shift()+NominalWidthX;
							HasWidth=true;
						}
						
						Y+=Stack.Shift();
						
						Glyph.ClosePath();
						
						// Move:
						Glyph.MoveTo(X*ScaleRatio,Y*ScaleRatio);
						
					break;
					case 5: // rlineto
						
						while (Stack.Length>0){
							
							X+=Stack.Shift();
							Y+=Stack.Shift();
							
							Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
							
						}
						
					break;
					case 6: // hlineto
					
					while (Stack.Length>0){
						
						X+=Stack.Shift();
						Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
						
						if(Stack.Empty){
							break;
						}
						
						Y+=Stack.Shift();
						Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
						
					}
					
					break;
					case 7: // vlineto
					
					while (Stack.Length > 0) {
						Y += Stack.Shift();
						Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
						
						if(Stack.Length==0){
							break;
						}
						
						X+=Stack.Shift();
						Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
					}
					
					break;
					case 8: // rrcurveto
						
						while (Stack.Length > 0) {
							c1x = X + Stack.Shift();
							c1y = Y + Stack.Shift();
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X = c2x + Stack.Shift();
							Y = c2y + Stack.Shift();
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
						}
						
					break;
					case 10: // callsubr
						
						subIndex=(int)Stack.Pop() + SubrsBias;
						subCode=Subrs[subIndex];
						
						if(subCode!=null){
							
							// Cache the position:
							subIndex=Parser.Position;
							
							// Parse:
							Parse(subCode.Position,subCode.Length);
							
							// Re-apply:
							Parser.Position=subIndex;
							
						}
						
					break;
					case 11: // return
					return;
					case 12: // escape
						v=Parser.ReadByte();
					break;
					case 14: // endchar
						
						if(Stack.Length > 0 && !HasWidth) {
							Width = Stack.Shift() + NominalWidthX;
							HasWidth = true;
						}
						
						// Close the glyph:
						Glyph.ClosePath();
						
					break;
					case 18: // hstemhm
						ParseStems();
					break;
					case 19: // hintmask
					case 20: // cntrmask
						ParseStems();
						Parser.Position += (NStems + 7) >> 3;
					break;
					case 21: // rmoveto
						
						if (Stack.Length > 2 && !HasWidth) {
							Width = Stack.Shift() + NominalWidthX;
							HasWidth = true;
						}
						
						X += Stack.Shift();
						Y += Stack.Shift();
						
						Glyph.ClosePath();
						
						// Move now:
						Glyph.MoveTo(X*ScaleRatio,Y*ScaleRatio);
						
					break;
					case 22: // hmoveto
						
						if(Stack.Length > 1 && !HasWidth){
							Width = Stack.Shift() + NominalWidthX;
							HasWidth = true;
						}
						
						X+=Stack.Shift();
						
						Glyph.ClosePath();
						
						// Move now:
						Glyph.MoveTo(X*ScaleRatio,Y*ScaleRatio);
						
					break;
					case 23: // vstemhm
						ParseStems();
					break;
					case 24: // rcurveline
						
						while (Stack.Length > 2) {
							
							c1x =X+ Stack.Shift();
							c1y =Y+ Stack.Shift();
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X=c2x + Stack.Shift();
							Y=c2y + Stack.Shift();
							
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
							
						}
						
						X+= Stack.Shift();
						Y+= Stack.Shift();
						Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
						
					break;
					case 25: // rlinecurve
					
						while (Stack.Length>6){
							
							X += Stack.Shift();
							Y += Stack.Shift();
							Glyph.LineTo(X*ScaleRatio,Y*ScaleRatio);
							
						}
					
						c1x = X + Stack.Shift();
						c1y = Y + Stack.Shift();
						c2x = c1x + Stack.Shift();
						c2y = c1y + Stack.Shift();
						X = c2x + Stack.Shift();
						Y = c2y + Stack.Shift();
						Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
						
					break;
					case 26: // vvcurveto
					
						if(Stack.IsOdd){
							X+=Stack.Shift();
						}
						
						while(Stack.Length>0){
							
							c1x = X;
							c1y = Y + Stack.Shift();
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X = c2x;
							Y = c2y + Stack.Shift();
							
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
							
						}
					
					break;
					case 27: // hhcurveto
						
						if(Stack.IsOdd){
							Y+=Stack.Shift();
						}
						
						while(Stack.Length>0){
							
							c1x = X + Stack.Shift();
							c1y = Y;
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X = c2x + Stack.Shift();
							Y = c2y;
							
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
							
						}
						
					break;
					case 28: // shortint
						
						Stack.Push(Parser.ReadInt16());
						
					break;
					case 29: // callgsubr
						
						subIndex = (int)Stack.Pop() + GsubrsBias;
						subCode = GSubrs[subIndex];
						
						if(subCode!=null){
							
							// Cache the position:
							subIndex=Parser.Position;
							
							// Parse:
							Parse(subCode.Position,subCode.Length);
							
							// Re-apply:
							Parser.Position=subIndex;
							
						}
						
					break;
					case 30: // vhcurveto
						
						while (Stack.Length > 0){
						
							c1x = X;
							c1y = Y + Stack.Shift();
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X = c2x + Stack.Shift();
							Y = c2y + (Stack.Length==1?Stack.Shift():0);
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio,X*ScaleRatio,Y*ScaleRatio);
							
							if(Stack.Empty){
								break;
							}
							
							c1x = X + Stack.Shift();
							c1y = Y;
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							Y = c2y + Stack.Shift();
							X = c2x + (Stack.Length==1?Stack.Shift():0);
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio, X*ScaleRatio, Y*ScaleRatio);
							
						}
						
					break;
					case 31: // hvcurveto
					
						while(Stack.Length>0){
						
							c1x = X + Stack.Shift();
							c1y = Y;
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							Y = c2y + Stack.Shift();
							X = c2x + (Stack.Length==1 ? Stack.Shift() : 0);
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio, X*ScaleRatio, Y*ScaleRatio);
							
							if(Stack.Empty){
								break;
							}
							
							c1x = X;
							c1y = Y + Stack.Shift();
							c2x = c1x + Stack.Shift();
							c2y = c1y + Stack.Shift();
							X = c2x + Stack.Shift();
							Y = c2y + (Stack.Length==1?Stack.Shift():0);
							
							Glyph.CurveTo(c1x*ScaleRatio, c1y*ScaleRatio, c2x*ScaleRatio, c2y*ScaleRatio, X*ScaleRatio, Y*ScaleRatio);
							
						}
					
					break;
					default:
						
						if(v<32){
							
							// Faulty operator.
							return;
							
						}else if(v < 247){
							
							Stack.Push(v - 139);
							
						}else if (v < 251) {
							
							Stack.Push((v - 247) * 256 + Parser.ReadByte() + 108);
							
						} else if (v < 255) {
							
							Stack.Push(-(v - 251) * 256 - Parser.ReadByte() - 108);
						
						} else {
							
							Stack.Push((float)Parser.ReadInt32() / 65536f);
							
						}
						
					break;
					
				}
			}
			
		}
		
	}
	
}