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
using PowerUI;


namespace Spa{
	
	/// <summary>A callback which can be used to get notified each frame.</summary>
	public delegate void OnSPAProgress(int frameID,SPAInstance instance);
	
	/// <summary>
	/// A single instance of an SPA animation.
	/// It's instanced like this so that the same animation
	/// can be played back multiple times at once; the instance
	/// keeps track of which frame the animation is currently at.
	/// </summary>
	
	public class SPAInstance{
		
		/// <summary>True if the animation is over.</summary>
		public bool Done;
		/// <summary>Which frame ID we're up to in the current sprite.</summary>
		public int AtFrame;
		/// <summary>The animation this instance belongs to.</summary>
		public SPA Animation;
		/// <summary>True if this animation should loop.</summary>
		public bool Loop=true;
		/// <summary>The frame ID in the overall animation.</summary>
		public int OverallFrame;
		/// <summary>How long to wait before advancing the frame, in seconds.</summary>
		public float FrameDelay;
		/// <summary>How long we've waited so far since the frame was last advanced.</summary>
		public float CurrentDelay;
		/// <summary>The overall elapsed time.</summary>
		public float Elapsed;
		/// <summary>The current sprite we're using. Contains the graphic itself.</summary>
		public SPASprite CurrentSprite;
		/// <summary>The material this instance is using.</summary>
		public Material AnimatedMaterial;
		/// <summary>A receiver for various events triggered by this SPA.</summary>
		public Dom.EventTarget EventReceiver;
		/// <summary>Currently playing instances are stored in a linked list.
		/// This is the instance in the list after this one.
		/// <see cref="PowerUI.SPA.FirstInstance"/>
		/// </summary>
		public SPAInstance InstanceAfter;
		/// <summary>Currently playing instances are stored in a linked list.
		/// This is theinstance in the list before this one.
		/// <see cref="PowerUI.SPA.FirstInstance"/>
		/// </summary>
		public SPAInstance InstanceBefore;
		
		/// <summary>Creates a new playable instance of the given SPA animation. You'll need to call Setup.</summary>
		public SPAInstance(SPA animation){
			Animation=animation;
			float fr=(float)animation.FrameRate;
			
			if(fr==0f){
				FrameDelay=float.MaxValue;
			}else{
				FrameDelay=1f/fr;
			}
			
		}
		
		/// <summary>Creates a new playable instance of the given SPA animation using the given shader set.</summary>
		public SPAInstance(SPA animation,ShaderSet shaders){
			Animation=animation;
			float fr=(float)animation.FrameRate;
			
			if(fr==0f){
				FrameDelay=float.MaxValue;
			}else{
				FrameDelay=1f/fr;
			}
			
			Setup(shaders);
		}
		
		/// <summary>Sets the given context. Enables the node to receive events.</summary>
		public void SetContext(Css.RenderableData context){
			EventReceiver=context.Node;
		}
		
		/// <summary>Dispatches an event to the event receiver, if there is one.</summary>
		public void dispatchEvent(string name){
			if(EventReceiver==null){
				return;
			}
			
			// Create the event:
			SpriteEvent se=new SpriteEvent(name);
			
			// Trusted but doesn't bubble:
			se.SetTrusted(false);
			
			// Apply this instance:
			se.instance=this;
			
			// Dispatch it now:
			EventReceiver.dispatchEvent(se);
			
		}
		
		/// <summary>Sets up this instance using the default shader set. Not lit by default.</summary>
		public void Setup(){
			Setup(ShaderSet.Standard.Isolated);
		}
		
		/// <summary>Sets up this instance using the default shader set.</summary>
		public void Setup(bool lit){
			Setup(lit ? ShaderSet.StandardLit.Isolated : ShaderSet.Standard.Isolated);
		}
		
		/// <summary>Sets up this instance with a given shader set.</summary>
		public void Setup(ShaderSet shaders){
			Setup(shaders.Isolated);
		}
		
		/// <summary>Sets this up with a particular shader.</summary>
		public void Setup(Shader shader){
			AnimatedMaterial=new Material(shader);
			SetSprite(0);
			dispatchEvent("spritestart");
		}
		
		/// <summary>Advances the animation. Different animations may be at different
		/// frame rates, so this regulates the frame rate too.</summary>
		public void Update(float deltaTime){
			CurrentDelay+=deltaTime;
			
			if(CurrentDelay<FrameDelay || CurrentSprite==null){
				return;
			}
			
			AtFrame++;
			OverallFrame++;
			Elapsed+=CurrentDelay;
			CurrentDelay=0f;
			
			if(AtFrame==CurrentSprite.FrameCount){
				AtFrame=0;
				// Time for the next sprite.
				int nextSpriteID=CurrentSprite.ID+1;
				
				if(nextSpriteID==Animation.Sprites.Length){
					
					if(Loop){
						nextSpriteID=0;
						OverallFrame++;
						dispatchEvent("spriteiteration");
					}else{
						Stop();
						return;
					}
					
				}
				
				SetSprite(nextSpriteID);
			}
			
			// Update the material offset.
			int imageX=AtFrame/CurrentSprite.VerticalFrameCount;
			int imageY=AtFrame-(imageX*CurrentSprite.VerticalFrameCount);
			
			if(AnimatedMaterial==null){
				// Nobody is displaying it!
				return;
			}
			
			AnimatedMaterial.SetTextureOffset(
												"_MainTex",
												new Vector2(
															imageX*CurrentSprite.TextureScale.x,
															imageY*CurrentSprite.TextureScale.y
															)
											);
		}
		
		/// <summary>Sets the sprite with the given ID as the active one.</summary>
		/// <param name="index">The ID of the sprite.</param>
		private void SetSprite(int index){
			CurrentSprite=Animation.Sprites[index];
			
			if(CurrentSprite.Delay!=0f){
				// Get the delay from the sprite:
				FrameDelay=CurrentSprite.Delay;
			}
			
			if(AnimatedMaterial==null){
				// Nobody is displaying it!
				return;
			}
			
			AnimatedMaterial.SetTexture("_MainTex",CurrentSprite.Sprite);
			// Update the material tiling:
			AnimatedMaterial.SetTextureScale("_MainTex",CurrentSprite.TextureScale);
		}
		
		/// <summary>Stops this instance. If you continue to display it after this is called,
		/// the animation will appear frozen on the last frame.</summary>
		public void Stop(){
			if(Done){
				return;
			}
			Done=true;
			
			// Remove this from the queue so that it's not updated anymore:
			if(InstanceBefore==null){
				SPA.FirstInstance=InstanceAfter;
			}else{
				InstanceBefore.InstanceAfter=InstanceAfter;
			}
			
			if(InstanceAfter==null){
				SPA.LastInstance=InstanceBefore;
			}else{
				InstanceAfter.InstanceBefore=InstanceBefore;
			}
			
			dispatchEvent("spriteend");
			
		}
		
	}
	
}