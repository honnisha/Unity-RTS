using System;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summray>A simple delegate for use by Promise.
	/// They can optionally return a Promise which will be passed on to the next handler in the chain.</summary>
	public delegate object PromiseDelegate(object info);
	/// <summray>Used when constructing a promise.</summary>
	public delegate object PromiseSetupDelegate(object reject, object resolve);
	/// <summary>A delegate void which returns nothing.</summary>
	public delegate void PromiseDelegateVoid(object info);
	
	/// <summary>A promise object for chaining events.</summary>
	public class Promise{
		
		/// <summary>Promise state values.</summary>
		public const int PROMISE_PENDING=0;
		public const int PROMISE_FULFILLED=1;
		public const int PROMISE_REJECTED=2;
		
		/// <summary>The state of this promise. Pending, fulfilled, rejected.</summary>
		private int state;
		/// <summary>If this promise has already changed state then another 
		/// promise can be the target of any late then calls.</summary>
		private Promise target;
		internal object latestMessage;
		private List<PromiseFrame> eventsToRun;
		
		
		/// <summary>An instant resolving promise.</summary>
		public static Promise Resolve(object o){
		   var promise = new Promise();
		   promise.resolve(o);
		   return promise;
		}

		
		public Promise(){}
		
		public Promise(PromiseSetupDelegate setup){
			// Immediately call the setup method:
			setup((PromiseDelegate)((object result) => {
				resolve(result);
				return null;
			}), (PromiseDelegate)((object result) => {
				reject(result);
				return null;
			}));
		}
		
		/// <summary>The status of this promise.</summary>
		public string statusText{
			get{
				switch(state){
					default:
					case PROMISE_PENDING:
						return "pending";
					case PROMISE_FULFILLED:
						return "fulfilled";
					case PROMISE_REJECTED:
						return "rejected";
				}
			}
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(Promise both){
			return then(both,both);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegate onFulfil){
			return then(onFulfil,(Promise)null);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(Promise onFulfil,Promise onReject){
			return then(toDelegate(true,onFulfil),toDelegate(false,onReject));
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegate onFulfil,PromiseDelegate onReject){
			
			if(target!=null){
				return target.then(onFulfil,onReject);
			}
			
			// Create the frame now:
			PromiseFrame frame=new PromiseFrame();
			frame.success=onFulfil;
			frame.fail=onReject;
			
			// Already complete?
			if(state!=PROMISE_PENDING){
				
				// Instantly run:
				Promise prom=frame.run( (state==PROMISE_FULFILLED), latestMessage );
				
				if(prom!=null){
					
					// We've got a new target!
					target=prom;
					return prom;
					
				}
				
				return this;
			}
			
			
			// Add a frame now:
			if(eventsToRun==null){
				eventsToRun=new List<PromiseFrame>();
			}
			
			eventsToRun.Add(frame);
			
			return this;
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(Promise onFulfil,PromiseDelegate onReject){
			return then(toDelegate(true,onFulfil),onReject);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegate onFulfil,Promise onReject){
			return then(onFulfil,toDelegate(false,onReject));
		}
		
		/// <summary>Adds a rejection handler.</summary>
		public Promise @catch(PromiseDelegate onReject){
			return then((PromiseDelegate)null,onReject);
		}
		
		/// <summary>Adds a rejection handler.</summary>
		public Promise @catch(Promise onReject){
			return then((PromiseDelegate)null,onReject);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegateVoid onFulfil){
			return then(toDelegate(onFulfil),(Promise)null);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegateVoid onFulfil,PromiseDelegateVoid onReject){
			return then(toDelegate(onFulfil),toDelegate(onReject));
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(Promise onFulfil,PromiseDelegateVoid onReject){
			return then(toDelegate(true,onFulfil),toDelegate(onReject));
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegateVoid onFulfil,Promise onReject){
			return then(toDelegate(onFulfil),toDelegate(false,onReject));
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegateVoid onFulfil,PromiseDelegate onReject){
			return then(toDelegate(onFulfil),onReject);
		}
		
		/// <summary>Provides methods to run when this promise completes.</summary>
		public Promise then(PromiseDelegate onFulfil,PromiseDelegateVoid onReject){
			return then(onFulfil,toDelegate(onReject));
		}
		
		/// <summary>Adds a rejection handler.</summary>
		public Promise @catch(PromiseDelegateVoid onReject){
			return then((PromiseDelegate)null,toDelegate(onReject));
		}
		
		/// <summary>Rejects the promise with the given reason.</summary>
		public void reject(object reason){
			
			if(state!=PROMISE_PENDING){
				return;
			}
			
			latestMessage=reason;
			
			// Failed!
			state=PROMISE_REJECTED;
			
			// Run rejections with the result:
			run(false,reason);
		}
		
		/// <summary>Resolves the promise. If the given object is also a promise, 
		/// then it 'waits' for that one to resolve too.</summary>
		public void resolve(object result){
			
			if(state!=PROMISE_PENDING){
				return;
			}
			
			latestMessage=result;
			
			if(result is Promise){
				
				// Wait for it to resolve:
				(result as Promise).then(delegate(object info){
					// All ok!
					resolve(info);
					return null;
				},delegate(object info){
					// We got rejected:
					reject(info);
					return null;
				});
				
			}else{
				// Ok!
				state=PROMISE_FULFILLED;
				
				// Run OnFulfilled with the result:
				run(true,result);
			}
			
		}
		
		/// <summary>Runs each promise delegate. Note that if any method fails, this will catch it.</summary>
		private void run(bool success,object result){
			
			List<PromiseFrame> pd=eventsToRun;
			eventsToRun=null;
			
			if(pd==null){
				return;
			}
			
			try{
				for(int i=0;i<pd.Count;i++){
					
					// Get the frame:
					PromiseFrame frame=pd[i];
					
					// Run it now:
					Promise prom=frame.run(success, result);
					
					if(prom!=null){
						
						// We're now targeting it instead:
						if(prom.target!=null){
							prom=prom.target;
						}
						
						target=prom;
						
						// Yep! All further methods are added to *that* promise and we quit here.
						for(int x=i+1;x<pd.Count;x++){
							
							// Add directly:
							if(prom.eventsToRun==null){
								prom.eventsToRun=new List<PromiseFrame>();
							}
							
							// Add the frame:
							prom.eventsToRun.Add(pd[x]);
							
						}
						
						// If prom is actually already done, instantly run it too:
						if(prom.state!=PROMISE_PENDING){
							
							// Run it now!
							prom.run(prom.state==PROMISE_FULFILLED, prom.latestMessage);
							
						}
						
						return;
						
					}
					
				}
			}catch(Exception e){
				Dom.Log.Add("Promise method failure: "+e.ToString());
			}
		}
		
		/// <summary>Creates a proxy delegate for one which returns void.</summrry>
		private PromiseDelegate toDelegate(PromiseDelegateVoid deleg){
			
			if(deleg==null){
				return null;
			}
			
			return new PromiseDelegate(delegate(object x){
				deleg(x);
				return null;
			});
			
		}
		
		/// <summary>Adds an object to the given set. If the set doesn't exist, it creates one.</summary>
		private PromiseDelegate toDelegate(bool success,Promise toAdd){
			
			if(success){
				return new PromiseDelegate(delegate(object info){
					if(toAdd != null){
						toAdd.resolve(info);
					}
					return info;
				});
			}
			
			return new PromiseDelegate(delegate(object info){
				if(toAdd != null){
					toAdd.reject(info);
				}
				return info;
			});
			
		}
		
		/// <summary>Adds an object to the given set. If the set doesn't exist, it creates one.</summary>
		private List<PromiseDelegate> add(List<PromiseDelegate> set,bool success,PromiseDelegate toAdd){
			if(toAdd==null){
				return set;
			}
			
			// Already complete?
			if(state!=PROMISE_PENDING){
				
				// Instantly run:
				if((success && state==PROMISE_FULFILLED)){
					toAdd(latestMessage);
				}else if((!success && state==PROMISE_REJECTED)){
					toAdd(latestMessage);
				}
				
				return set;
			}
			
			if(set==null){
				set=new List<PromiseDelegate>();
			}
			
			set.Add(toAdd);
			return set;
		}
		
		/// <summary>A promise which runs when all of the given ones completed. If any fail, it fails.</summary>
		public static Promise all(System.Array promises){
			
			// A promise which runs when they all did:
			Promise allResult=new Promise();
			
			if(promises==null || promises.Length==0){
				// Immediately complete.
				// This just makes any future .Then() calls insta-run.
				allResult.resolve(new object[0]);
				return allResult;
			}
			
			PromiseGroupData pgd=new PromiseGroupData();
			pgd.set=promises;
			
			
			for(int i=0;i<promises.Length;i++){
				
				// The current promise:
				Promise promise=(promises.GetValue(i) as Promise);
				
				if(promise!=null){
					
					promise.then(
						delegate(object info){
							// Success!
							pgd.count++;
							
							// make sure results is the right size:
							if(pgd.results==null || pgd.results.Length!=promises.Length){
								
								// Resize:
								object[] newResults=new object[promises.Length];
								
								if(pgd.results!=null){
									// Transfer:
									Array.Copy(pgd.results,0,newResults,0,pgd.results.Length);
								}
								
								// Set as results:
								pgd.results=newResults;
								
							}
							
							// Store the result:
							pgd.results[i]=info;
							
							if(pgd.count==pgd.set.Length){
								
								// Done! We use the complete results set here:
								allResult.resolve(pgd.results);
								
							}
							
							return null;
						},
						delegate(object info){
							// Failed! The whole group fails.
							allResult.reject(info);
							return null;
						}
					);
				
				}
				
			}
			
			return allResult;
			
		}
		
		/// <summary>A promise which runs when *any* of the given ones complete. If any fail, it fails.</summary>
		public static Promise race(System.Array promises){
			
			// A promise which runs when the first did:
			Promise result=new Promise();
			
			if(promises==null || promises.Length==0){
				// Immediately complete.
				// This just makes any future .Then() calls insta-run.
				result.resolve(null);
				return result;
			}
			
			for(int i=0;i<promises.Length;i++){
				
				// The current promise:
				Promise promise=(promises.GetValue(i) as Promise);
				
				if(promise!=null){
					
					promise.then(
						delegate(object info){
							// Success! The whole group is done.
							result.resolve(info);
							return null;
						},
						delegate(object info){
							// Failed! The whole group fails.
							result.reject(info);
							return null;
						}
					);
				
				}
				
			}
			
			return result;
			
		}
		
	}
	
	/// <summary>A single frame in a promise.</summary>
	internal class PromiseFrame{
		
		/// <summary>A delegate to run on success.</summary>
		internal PromiseDelegate success;
		/// <summary>A delegate to run on failure.</summary>
		internal PromiseDelegate fail;
		
		
		/// <summary>Runs this frame now.</summary>
		internal Promise run(bool wasSuccessful,object message){
			
			object response=null;
			
			if(wasSuccessful){
				if(success!=null){
					response=success(message);
				}
				
			}else if(fail!=null){
				response=fail(message);
			}
			
			// Did it respond with a promise?
			return (response as Promise);
		}
		
	}
	
	internal class PromiseGroupData{
		
		/// <summary>The number of successful results so far.</summary>
		public int count=0;
		/// <summary>The original group of promises.</summary>
		public System.Array set;
		/// <summary>All the results from all of the successful promises.</summary>
		public object[] results;
		
	}
	
}