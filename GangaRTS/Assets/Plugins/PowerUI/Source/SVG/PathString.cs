using System;
using System.Collections;
using System.Collections.Generic;
using Blaze;
using System.Text;


namespace Svg{
	
	/// <summary>
	/// Handles the SVG path string data.
	/// </summary>
	
	public static class PathString{
		
		/// <summary>Maps an SVG path command to the number of parameters it has.</summary>
		private static Dictionary<char,int> ParamCounts;
		
		/// <summary>Loads a vector path from an SVG path string.</summary>
		public static VectorPath Load(string pathString){
			
			VectorPath p=new VectorPath();
			return Load(pathString,p);
			
		}
		
		/// <summary>Loads a vector path from an SVG path string into the given path.</summary>
		public static VectorPath Load(string pathString,VectorPath data){
			
			if(string.IsNullOrEmpty(pathString)){
				return null;
			}
			
			if(ParamCounts==null){
				
				// Create the lookup:
				ParamCounts=new Dictionary<char,int>();
				
				// Add the entries:
				ParamCounts['a']=7;
				ParamCounts['c']=6;
				ParamCounts['h']=1;
				ParamCounts['l']=2;
				ParamCounts['m']=2;
				ParamCounts['q']=4;
				ParamCounts['s']=4;
				ParamCounts['t']=2;
				ParamCounts['v']=1;
				ParamCounts['z']=0;
			
			}
			
			// The current parameter count limit for the current command:
			int currentLimit=0;
			int currentParam=-1;
			char command='\0';
			float[] parameters=new float[7];
			
			// The current value being read:
			StringBuilder currentParameter=new StringBuilder();
			bool containsDot=false;
			
			// For each character:
			for(int i=0;i<pathString.Length;i++){
				
				// Get the current character:
				char current=pathString[i];
				
				// If its a number or . then we're building a parameters number.
				// Otherwise, it's possibly some junk such as a space (which can terminate building a param).
				// Aside from that its the 1 char command.
				
				int charcode=(int)current;
				
				if((charcode==46 && !containsDot) || (charcode>=48 && charcode<=57) || (charcode==(int)'-' && currentParameter.Length==0)){
					
					if(charcode==46){
						containsDot=true;
					}
					
					// It's a number or .!
					currentParameter.Append(current);
					
				}else{
					
					// Anything else terminates the current parameter, if we're building one:
					if(currentParameter.Length!=0){
						// Terminate the parameter
						parameters[currentParam++]=float.Parse(currentParameter.ToString());
						currentParameter.Length=0;
						containsDot=false;
					}
					
					// If it's not something like a space, then we're starting a new command.
					if(current!=' ' && current!='\n' && current!='\t' && current!='\r' && current!=',' && current!='.' && current!='-'){
						
						// and build its command.
						AddCommand(data,command,parameters,currentLimit);
						
						// It's the command:
						command=current;
						currentLimit=ParamCounts[char.ToLower(command)];
						currentParam=0;
						
					}else if(currentParam>=currentLimit){
						
						// This happens when a command is given 2+ sets of parameters.
						// For example, you can tell it to repeatedly create 
						// straight lines by using the line command only once.
						
						// Build the command:
						AddCommand(data,command,parameters,currentLimit);
						
						// Reset param count:
						currentParam=0;
						
					}
					
				}
				
			}
			
			// Build the final command.
			AddCommand(data,command,parameters,currentLimit);
			
			return data;
			
		}
		
		/// <summary>
		/// Used to generate the smooth curve control points.
		/// </summary>
		public static void Reflect(VectorPath path,out float cx,out float cy){
			// - Take the last control point from the previous curve
			// - Reflect it around the start point
			// - Result is 'my' first control point
			
			VectorPoint node=path.LatestPathNode;
			
			if(node==null){
				cx=0f;
				cy=0f;
				return;
			}
			
			float lastControlX=0f;
			float lastControlY=0f;
			
			float reflectAroundX=node.X;
			float reflectAroundY=node.Y;
			
			// Try as a curve:
			CurveLinePoint clp=node as CurveLinePoint;
			
			if(clp==null){
				
				// Try quad point instead:
				QuadLinePoint qlp=node as QuadLinePoint;
				
				if(qlp==null){
					cx=path.LatestPathNode.X;
					cy=path.LatestPathNode.Y;
					return;
				}
				
				lastControlX=qlp.Control1X;
				lastControlY=qlp.Control1Y;
				
			}else{
				
				lastControlX=clp.Control2X;
				lastControlY=clp.Control2Y;
				
			}
			
			// Reflect lastControl around reflectAround:
			// reflectAround-(COORD-reflectAround)
			
			cx=2f*reflectAroundX-lastControlX;
			cy=2f*reflectAroundY-lastControlY;
			
		}
		
		/// <summary>Adds the given parsed command into the given path.</summary>
		public static void AddCommand(VectorPath path,char command,float[] param,int currentLimit){
			
			if(command=='\0'){
				return;
			}
			
			// Get the lc command:
			char lower=char.ToLower(command);
			
			// A lowercase char is relative
			// (and its lowercase if the lower one matches the original):
			bool isRelative=(command==lower);
			
			// Current point:
			float curX=0f;
			float curY=0f;
			float c1x=0f;
			float c1y=0f;
			
			if(path.LatestPathNode!=null){
				// Get current point:
				curX=path.LatestPathNode.X;
				curY=path.LatestPathNode.Y;
			}
			
			switch(lower){
				
				case 'a':
					// Eliptical arc
					
					if(isRelative){
						param[5]+=curX;
						param[6]+=curY;
					}
					
					path.EllipseArc(param[0],param[1],param[2],param[5],param[6],(param[3]==1f),(param[4]==1f));
					
				break;
				
				case 'c':
					// Curve to
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
						param[2]+=curX;
						param[3]+=curY;
						param[4]+=curX;
						param[5]+=curY;
					}
					
					path.CurveTo(param[0],param[1],param[2],param[3],param[4],param[5]);
				break;
				
				case 'h':
					// Horizontal line to
					
					if(isRelative){
						param[0]+=curX;
					}
					
					path.LineTo(param[0],curY);
				break;
				
				case 'l':
					// Line to
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
					}
					
					path.LineTo(param[0],param[1]);
				break;
				
				case 'm':
					// Move to
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
					}
					
					path.MoveTo(param[0],param[1]);
				break;
				
				case 'q':
					// Quadratic bezier to
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
						param[2]+=curX;
						param[3]+=curY;
					}
					
					path.QuadraticCurveTo(param[0],param[1],param[2],param[3]);
				break;
				
				case 's':
					// Smooth curve to
					Reflect(path,out c1x,out c1y);
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
						param[2]+=curX;
						param[3]+=curY;
					}
					
					path.CurveTo(c1x,c1y,param[0],param[1],param[2],param[3]);
				break;
				
				case 't':
					// Smooth quadratic bezier to
					Reflect(path,out c1x,out c1y);
					
					if(isRelative){
						param[0]+=curX;
						param[1]+=curY;
					}
					
					path.QuadraticCurveTo(c1x,c1y,param[0],param[1]);
				break;
				
				case 'v':
					// Vertical line to
					
					if(isRelative){
						param[0]+=curY;
					}
					
					path.LineTo(curX,param[0]);
				break;
				
				case 'z':
					// Close path
					path.ClosePath();
				break;
				
			}
			
		}
		
	}
	
}