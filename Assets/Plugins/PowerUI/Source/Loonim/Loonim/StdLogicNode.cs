using System;


namespace Loonim{
	
    /// <summary>
    /// A node with 4 input nodes. E.g. a < b etc.
    /// </summary>
    public class StdLogicNode:TextureNode{
		
		public TextureNode IfTrue{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode IfFalse{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public StdLogicNode():base(4){}
		
	}
	
}