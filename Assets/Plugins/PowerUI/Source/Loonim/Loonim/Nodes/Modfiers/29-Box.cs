using System;


namespace Loonim{
	
    /// <summary>
    /// Puts a texture into a "box". Primarily used by graphs. Essentially adjusts its start/end points in 2D or 3D space.
	/// Graphs flatline outside this box.
    /// </summary>
    public class Box:TextureNode{
		
		public TextureNode StartX{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode EndX{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode StartY{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public TextureNode EndY{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		public TextureNode StartZ{
			get{
				return Sources[5];
			}
			set{
				Sources[5]=value;
			}
		}
		
		public TextureNode EndZ{
			get{
				return Sources[6];
			}
			set{
				Sources[6]=value;
			}
		}
		
		public Box():base(7){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			double startX=StartX.GetValue(x,y);
			double startY=StartY.GetValue(x,y);
			
			double endX=EndX.GetValue(x,y);
			double endY=EndY.GetValue(x,y);
			
			if(x<startX){
				// This makes it clamp:
				x=startX;
			}else if(x>endX){
				// This makes it clamp:
				x=endX;
			}else{
				
				// Box x:
				x=(x-startX)/(endX-startX);
				
			}
			
			if(y<startY){
				// This makes it "flatline":
				y=startY;
			}else if(y>endY){
				// This makes it "flatline":
				y=endY;
			}else{
				
				// Box y:
				y=(y-startY)/(endY-startY);
				
			}
			
			// Read there:
			return SourceModule.GetColour(x,y);
			
		}
		
		public override double GetWrapped(double x,double y,int wrap){
			
			double startX=StartX.GetWrapped(x,y,wrap);
			double startY=StartY.GetWrapped(x,y,wrap);
			
			double endX=EndX.GetWrapped(x,y,wrap);
			double endY=EndY.GetWrapped(x,y,wrap);
			
			if(x<startX){
				// This makes it clamp:
				x=startX;
			}else if(x>endX){
				// This makes it clamp:
				x=endX;
			}else{
				
				// Box x:
				x=(x-startX)/(endX-startX);
				
			}
			
			if(y<startY){
				// This makes it "flatline":
				y=startY;
			}else if(y>endY){
				// This makes it "flatline":
				y=endY;
			}else{
				
				// Box y:
				y=(y-startY)/(endY-startY);
				
			}
			
			// Read there:
			return SourceModule.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x,double y,double z){
			
			double startX=StartX.GetValue(x,y,z);
			double startY=StartY.GetValue(x,y,z);
			double startZ=StartY.GetValue(x,y,z);
			
			double endX=EndX.GetValue(x,y,z);
			double endY=EndY.GetValue(x,y,z);
			double endZ=EndY.GetValue(x,y,z);
			
			if(x<startX){
				// This makes it clamp:
				x=startX;
			}else if(x>endX){
				// This makes it clamp:
				x=endX;
			}else{
				
				// Box x:
				x=(x-startX)/(endX-startX);
				
			}
			
			if(y<startY){
				// This makes it "flatline":
				y=startY;
			}else if(y>endY){
				// This makes it "flatline":
				y=endY;
			}else{
				
				// Box y:
				y=(y-startY)/(endY-startY);
				
			}
			
			if(z<startZ){
				// This makes it "flatline":
				z=startZ;
			}else if(z>endZ){
				// This makes it "flatline":
				z=endZ;
			}else{
				
				// Box z:
				z=(z-startZ)/(endZ-startZ);
				
			}
			
			// Read there:
			return SourceModule.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x,double y){
			
			double startX=StartX.GetValue(x,y);
			double startY=StartY.GetValue(x,y);
			
			double endX=EndX.GetValue(x,y);
			double endY=EndY.GetValue(x,y);
			
			if(x<startX){
				// This makes it clamp:
				x=startX;
			}else if(x>endX){
				// This makes it clamp:
				x=endX;
			}else{
				
				// Box x:
				x=(x-startX)/(endX-startX);
				
			}
			
			if(y<startY){
				// This makes it "flatline":
				y=startY;
			}else if(y>endY){
				// This makes it "flatline":
				y=endY;
			}else{
				
				// Box y:
				y=(y-startY)/(endY-startY);
				
			}
			
			// Read there:
			return SourceModule.GetValue(x,y);
			
		}
		
		public override double GetValue(double x){
			
			double startX=StartX.GetValue(x);
			double startY=StartY.GetValue(x);
			
			double endX=EndX.GetValue(x);
			double endY=EndY.GetValue(x);
			
			if(x<startX){
				// This makes it "flatline":
				x=startX;
			}else if(x>endX){
				// This makes it "flatline":
				x=endX;
			}else{
				
				// Box x:
				x=(x-startX)/(endX-startX);
				
			}
			
			// Read there:
			double y=SourceModule.GetValue(x);
			
			if(y<startY){
				// This makes it "flatline":
				y=startY;
			}else if(y>endY){
				// This makes it "flatline":
				y=endY;
			}else{
				
				// Box y:
				y=(y-startY)/(endY-startY);
				
			}
			
			// Return y:
			return y;
			
		}
		
		public override int TypeID{
			get{
				return 29;
			}
		}
		
	}
	
}