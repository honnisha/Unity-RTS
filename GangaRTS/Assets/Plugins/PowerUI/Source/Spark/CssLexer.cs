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
using Dom;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Css.Units;


namespace Css{
	
	/// <summary>A callback used when a property is read into the given style.</summary>
	public delegate int OnReadProperty(Style style,string pName,Css.Value value);
	
	/// <summary>
	/// Handles the parsing of CSS content.
	/// </summary>
	
	public class CssLexer:Dom.StringReader{
		
		/// <summary>A builder used when parsing text.</summary>
		public StringBuilder Builder=new StringBuilder();
		/// <summary>True if any {selector blocks} should be parsed as literal property maps.
		/// used by e.g. the @styleset rule.</summary>
		public bool PropertyMapMode;
		/// <summary>True if we're in @rule mode.</summary>
		public bool AtRuleMode;
		/// <summary>The current scope of this lexer.</summary>
		public Node Scope;
		/// <summary>Current stylesheet.</summary>
		public StyleSheet Sheet;
		/// <summary>Are we reading the selector?</summary>
		public bool SelectorMode;
		/// <summary>The latest parent rule.</summary>
		public Css.Rule LatestRule;
		
		
		/// <summary>The document this lexer is working with.</summary>
		public ReflowDocument Document{
			get{
				if(Scope==null){
					// Global
					return null;
				}
				
				return Scope.document as ReflowDocument;
			}
		}
		
		/// <summary>Css variables, if any.</summary>
		public Dictionary<string,Css.Value> Variables{
			get{
				
				if(Scope==null){
					return null;
				}
				
				return Document.CssVariables;
				
			}
		}
		
		
		public CssLexer(string text,Node scope):base(text){
			Scope=scope;
			
			// First, check for the UTF8 BOM (rather awkwardly encoded in UTF16 here):
			if(text!=null && text.Length>=1 && (int)text[0]==0xFEFF){
				
				// We're UTF-8. Skip the BOM:
				Position++;
				
			}
			
			// Skip any initial junk:
			SkipJunk();
			
		}
		
		/// <summary>Reads a rule from this lexer.</summary>
		public Rule ReadRules(out Rule[] set){
			
			// Lexer is in selector mode - this simply blocks # from being recognised as a colour.
			SelectorMode=true;
			
			// Read a CSS value. This includes the block itself:
			Css.Value value=ReadValue();
			
			// Convert it:
			Rule mainRule=ConvertToRule(LatestRule,value,Sheet,out set);
			
			LatestRule=mainRule;
			return mainRule;
		}
		
		/// <summary>Converts a value to a rule. Note that it can be a set of rules (RuleSet).</summary>
		public static Rule ConvertToRule(Rule parentRule,Css.Value value,StyleSheet sheet,out Rule[] ruleSet){
			
			ruleSet=null;
			
			if(value==null){
				// Nothing else.
				return null;
			}
			
			// Is it an @ rule? Note that they can actually be 'nested' inside an,array,of,selectors (happens with @media)
			AtRuleUnit atRule=value[0] as AtRuleUnit;
			
			if(atRule!=null){
				
				// Let the @ rule handle the value:
				return atRule.AtRule.LoadRule(parentRule,sheet,value);
				
			}
			
			// One or more selectors followed by a block unit.
			// Get the block:
			int max=value.Count-1;
			
			SelectorBlockUnit block=value[max] as SelectorBlockUnit;
			
			if(block==null){
				
				// Try as a set instead:
				ValueSet set=value[max] as ValueSet;
				
				if(set==null){
					// Invalid/ unrecognised selector block. Ignore it.
					return null;
				}
				
				// Get last one in the set:
				block=set[set.Count-1] as SelectorBlockUnit;
				
				// still null?
				if(block==null){
					// Invalid/ unrecognised selector block. Ignore it.
					return null;
				}
				
				// Check again for an @rule:
				Css.Value v0=value[0];
				
				if(v0!=null && v0[0] is AtRuleUnit){
					
					// Got an at rule!
					atRule=v0[0] as AtRuleUnit;
					
					return atRule.AtRule.LoadRule(parentRule,sheet,value);
					
				}
				
				// Clear last one:
				set[set.Count-1]=null;
				
			}else{
				// Clear the block from the value:
				value[max]=null;
			}
			
			// Get the style object:
			Style style=block.Style;
			
			// Read the selector(s):
			List<Selector> into=new List<Selector>();
			ReadSelectors(sheet,value,into);
			
			if(into.Count==0){
				return null;
			}
			
			if(into.Count!=1){
				ruleSet=new Rule[into.Count];
			}
			
			// Got a specifity override? -spark-specifity:x
			Css.Value specifityOverride=null;
			int specOverride=-1;
			
			if(style.Properties.TryGetValue(Css.Properties.SparkSpecifity.GlobalProperty,out specifityOverride)){
				
				// Yep! Pull the integer value:
				specOverride=specifityOverride.GetInteger(null,null);
				
			}
			
			for(int i=0;i<into.Count;i++){
				
				// Get it:
				Selector s=into[i];
				
				// Create the rule:
				StyleRule rule=new StyleRule(style);
				rule.ParentStyleSheet=sheet;
				rule.Selector=s;
				s.Rule=rule;
				
				// Must use a copy of the style if its a secondary selector.
				// This is because the 'specifity' of CSS properties is defined by how specific
				// a selector is. So, when there's multiple selectors, we have to clone the style.
				
				Style currentStyle=i==0?style:style.Clone();
				
				// Apply the selectors specifity to the style:
				int specifity=s.Specifity;
				
				if(specOverride!=-1){
					specifity=specOverride;
				}
				
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in currentStyle.Properties){
					
					// Apply:
					kvp.Value.SetSpecifity(specifity);
					
				}
				
				if(ruleSet==null){
					
					return rule;
				}
				
				ruleSet[i]=rule;
				
			}
			
			return null;
			
		}
		
		/// <summary>Reads one selector without a block.</summary>
		public Selector ReadSelector(){
			
			SelectorMode=true;
			
			// Read a CSS value. This doesn't include the block:
			Css.Value value=ReadValue();
			
			return ReadSelectors(Sheet,value,null);
			
		}
		
		/// <summary>Reads one or more selectors without a block.</summary>
		public void ReadSelectors(List<Selector> all){
			
			SelectorMode=true;
			
			// Read a CSS value. This doesn't include the block:
			Css.Value value=ReadValue();
			
			ReadSelectors(Sheet,value,all);
			
		}
		
		/// <summary>Reads a single selector matcher. E.g. :hover or .test</summary>
		public static SelectorMatcher ReadSelectorMatcher(Css.Value value){
			
			if(value==null){
				return null;
			}
			
			if(value.GetType()==typeof(SquareBracketUnit)){
				
				// [attr]. Make the match object:
				return (value as SquareBracketUnit).GetMatch();
				
			}
			
			// Selector fragment.
			string text=value[0].Text;
			RootMatcher result=null;
			
			if(text=="#"){
				// ID selector next.
				string id=value[1].Text;
				
				// Create ID root:
				result=new RootIDMatcher();
				result.Text=id;
				return result;
				
			}else if(text=="."){
				// Class selector next.
				string className=value[1].Text;
				
				// Create class root:
				result=new RootClassMatcher();
				result.Text=className;
				return result;
				
			}else if(text=="*"){
				
				// Everything:
				return new RootUniversalMatcher();
				
			}else if(text==":"){
				// Pseudo class or function next. May also be another colon.
				// after and before map to actual style properties.
				
				Css.Value current=value[1];
				
				if(current!=null && !current.IsFunction){
					
					string keywordText=current.Text;
					
					// Second colon?
					if(keywordText==":"){
						// Skip the double colon.
						current=value[2];
						keywordText=current.Text;
						
					}
					
					// Keyword?
					current=CssKeywords.Get(keywordText.ToLower());
					
				}
				
				if(current==null){
					
					// Selector failure - return a blank one.
					return null;
					
				}
				
				// Convert the value into a matcher:
				return current.GetSelectorMatcher();
				
			}
			
			// It's just a tag:
			string tag=text.ToLower();
			
			// Create tag root:
			result=new RootTagMatcher();
			result.Text=tag;
			return result;
			
		}
		
		/// <summary>Reads one or more selectors from the given value.</summary>
		public static Selector ReadSelectors(StyleSheet sheet,Css.Value value,List<Selector> all){
			
			ValueSet valSet=value as ValueSet;
			
			if(valSet!=null && valSet.Spacer==","){
				
				// a,b,c{}
				Selector selector=null;
				
				// For each selector..
				int max=value.Count;
				
				for(int i=0;i<max;i++){
					
					Css.Value current=value[i];
					
					if(current==null){
						// Happens right at the end where the block was.
						continue;
					}
					
					Selector inner=LoadSingleSelector(sheet,current,all);
					
					if(inner==null){
						// Fail the whole thing:
						return null;
					}
					
					if(i==0){
						selector=inner;
					}
					
					if(all!=null){
						// Add it to the all set:
						all.Add(inner);
					}
					
					
				}
				
				return selector;
				
			}
			
			return LoadSingleSelector(sheet,value,all);
			
		}
		
		/// <summary>Creates an * node and adds it to the given set.</summary>
		private static RootUniversalMatcher CreateUniversal(List<RootMatcher> roots){
			
			// Create class root:
			RootUniversalMatcher root=new RootUniversalMatcher();
			
			// Add to root set:
			roots.Add(root);
			
			return root;
		}
		
		private static Selector LoadSingleSelector(StyleSheet sheet,Css.Value value,List<Selector> all){
			
			// Create the selector:
			Selector selector=new Selector();
			selector.Value=value;
			
			if(sheet!=null){
				// Default NS:
				selector.Namespace=sheet.Namespace;
			}
			
			List<LocalMatcher> locals=null;
			List<RootMatcher> roots=new List<RootMatcher>();
			RootMatcher currentRoot=null;
			
			// Current state:
			bool nextTarget=false;
			bool wasWhitespace=false;
			int max=value.Count;
			SelectorMatcher addMatcher=null;
			RootMatcher addRoot=null;
			
			// For each selector fragment..
			for(int i=0;i<max;i++){
				
				Css.Value current=value[i];
				
				if(current==null){
					// Happens right at the end where the block was.
					continue;
				}
				
				if(current.GetType()==typeof(SquareBracketUnit)){
					
					// [attr]
					SquareBracketUnit attrib=current as SquareBracketUnit;
					
					// Make the match object:
					AttributeMatch match=attrib.GetMatch();
					
					addMatcher=match;
					
				}else if(current.Type==ValueType.Text){
					
					// Selector fragment.
					string text=current.Text;
					
					if(text==" "){
						
						wasWhitespace=true;
						continue;
						
					}
					
					if(text=="#"){
						// ID selector next.
						i++;
						string id=value[i].Text;
						
						// Create ID root:
						addRoot=new RootIDMatcher();
						addRoot.Text=id;
						
					}else if(text=="."){
						// Class selector next.
						i++;
						string className=value[i].Text;
						
						// Create class root:
						addRoot=new RootClassMatcher();
						addRoot.Text=className;
						
						// Note that if you chain multiple class selectors (.a.b)
						// This is called twice and PreviousMatcher/NextMatcher remain null.
						// It then simply doesn't change element (which is what we want!).
						
					}else if(text=="*"){
						
						// Apply last locals:
						if(currentRoot!=null && locals!=null){
							currentRoot.SetLocals(locals);
							locals.Clear();
						}
			
						// Everything:
						currentRoot=CreateUniversal(roots);
						
					}else if(text==":"){
						// Pseudo class or function next. May also be another colon.
						// after and before map to actual style properties.
						
						i++;
						
						current=value[i];
						
						if(current!=null && !current.IsFunction){
							
							string keywordText=current.Text;
							
							// Second colon?
							if(keywordText==":"){
								// Skip the double colon.
								i++;
								current=value[i];
								keywordText=current.Text;
								
							}
							
							// Keyword?
							current=CssKeywords.Get(keywordText.ToLower());
							
						}
						
						if(current==null){
							
							// Selector failure - return a blank one.
							if(all!=null){
								all.Clear();
							}
							
							return null;
							
						}
						
						// Convert the value into a matcher:
						SelectorMatcher sm=current.GetSelectorMatcher();
						
						if(sm==null){
							
							// Selector failure - return a blank one.
							if(all!=null){
								all.Clear();
							}
							
							return null;
							
						}
						
						// Add it!
						addMatcher=sm;
						
					}else if(text==">"){
						
						// Following "thing" being a direct child.
						addMatcher=new DirectParentMatch();
						
					}else if(text==">>"){
						
						// Following "thing" being a descendant (same as space).
						addMatcher=new ParentMatch();
						
					}else if(text=="|"){
						// Namespace declaration was before.
						
						// Selector's namespace name is the latest root:
						string nsName=currentRoot.Text;
						
						// Pop the root:
						roots.RemoveAt(roots.Count -1);
						
						if(roots.Count>0){
							currentRoot=roots[roots.Count-1];
						}else{
							currentRoot=null;
						}
						
						// Note: It's null for * which we can just ignore.
						if(nsName!=null && sheet!=null && sheet.Namespaces!=null){
							// Get the namespace:
							CssNamespace ns;
							sheet.Namespaces.TryGetValue(nsName.ToLower(),out ns);
							selector.Namespace=ns;
						}
						
					}else if(text=="!"){
						
						// CSS selectors level 4 selector target.
						nextTarget=true;
						continue;
						
					}else if(text=="+"){
						// Whenever this follows the following "thing" directly.
						
						addMatcher=new DirectPreviousSiblingMatch();
						
					}else if(text=="~"){
						// Whenever this follows the following "thing".
						
						addMatcher=new PreviousSiblingMatch();
						
					}else{
						
						// It's just a tag:
						string tag=text.ToLower();
						
						// Create tag root:
						addRoot=new RootTagMatcher();
						addRoot.Text=tag;
						
					}
					
				}
				
				if(addRoot!=null){
					
					if(currentRoot!=null && locals!=null){
						currentRoot.SetLocals(locals);
						locals.Clear();
					}
					
					if(currentRoot!=null && wasWhitespace && currentRoot.NextMatcher==null){
						// Space before it!
						
						// Following "thing" being a descendant.
						currentRoot.NextMatcher=new ParentMatch();
						currentRoot.NextMatcher.Selector=selector;
						
						wasWhitespace=false;
						
					}
					
					// Add to root set:
					roots.Add(addRoot);
					currentRoot=addRoot;
					
					// Set selector:
					addRoot.Selector=selector;
					
					// Clear:
					addRoot=null;
					
				}
				
				if(addMatcher!=null){
					
					// Always clear whitespace:
					wasWhitespace=false;
					
					// Create implicit *:
					if(roots.Count==0){
						
						if(currentRoot!=null && locals!=null){
							currentRoot.SetLocals(locals);
							locals.Clear();
						}
						
						currentRoot=CreateUniversal(roots);
					}
					
					// Set selector:
					addMatcher.Selector=selector;
					
					if(addMatcher is StructureMatcher){
						
						// Structural matcher:
						currentRoot.NextMatcher=addMatcher as StructureMatcher;
						
					}else if(addMatcher is PseudoSelectorMatch){
						
						// Pseudo matcher:
						selector.PseudoElement=addMatcher as PseudoSelectorMatch;
						
					}else if(addMatcher is LocalMatcher){
						
						if(locals==null){
							locals=new List<LocalMatcher>();
						}
						
						// Local matcher:
						locals.Add(addMatcher as LocalMatcher);
						
					}
					
					addMatcher=null;
					
				}
				
				if(nextTarget && currentRoot!=null){
					
					nextTarget=false;
					
					// Update target:
					currentRoot.IsTarget=true;
					selector.Target=currentRoot;
					
				}
				
			}
			
			// Apply last locals:
			if(currentRoot!=null && locals!=null){
				currentRoot.SetLocals(locals);
				locals.Clear();
			}
			
			// Always ensure at least 1:
			if(roots.Count==0){
				// *:
				CreateUniversal(roots);
			}
			
			// Update selector info now:
			selector.Roots=roots.ToArray();
			selector.RootCount=roots.Count;
			selector.LastRoot=roots[roots.Count-1];
			selector.FirstRoot=roots[0];
			
			// Set target if needed:
			if(selector.Target==null){
				
				// Last root is the target:
				selector.Target=selector.LastRoot;
				selector.Target.IsTarget=true;
				
			}
			
			// Go through them backwards applying PreviousMatcher too:
			// (Not including 0).
			for(int i=roots.Count-1;i>0;i--){
				
				// Hook up:
				roots[i].PreviousMatcher=roots[i-1].NextMatcher;
				
			}
			
			if(sheet!=null){
				// Compute specifity now:
				selector.GetSpecifity(sheet.Priority);
			}
			
			// Add to all set, if there is one:
			if(all!=null && all.Count==0){
				all.Add(selector);
			}
			
			return selector;
		}
		
		/// <summary>Skips any junk such as whitespaces or comments.</summary>
		public void SkipJunk(){
			
			char current=Peek();
			
			while(current==' ' || current=='\r' || current=='\n' || current=='\t'){
				Read();
				current=Peek();
			}
			
			if(current=='/'){
				
				if(Peek(1)=='*'){
					// CSS block comment.
					Read();
					Read();
					current=Peek();
					
					while(current!=StringReader.NULL){
						
						if(current=='*'&&Peek(1)=='/'){
							//all done!
							Read();
							Read();
							break;
						}
						
						Read();
						current=Peek();
					}
					
					// Skip junk again:
					SkipJunk();
				/*
				}else if(Peek(1)=='/'){
					// Line comment.
					// Note: These are not in the spec because they are context sensitive.
					// E.g. background:url(http://...); would trigger it otherwise.
					Read();
					Read();
					
					current=Read();
					
					while(current!='\n' && current!='\r' && current!='\0'){
						current=Read();
					}
					
					// Skip junk again:
					SkipJunk();
				*/
				}
				
			}else if(current=='<' && Peek(1)=='-' && Peek(2)=='-'){
				
				Read();
				Read();
				Read();
				current=Peek();
				
				while(current!=StringReader.NULL){
					
					if(current=='-'&&Peek(1)=='-' && Peek(2)=='>'){
						//all done!
						Read();
						Read();
						Read();
						break;
					}
					
					Read();
					current=Peek();
				}
				
				// Skip junk again:
				SkipJunk();
				
			}
			
		}
		
		/// <summary>Gets the value of a CSS variable from the variable set.</summary>
		public Css.Value GetVariable(string name){
			
			if(Variables==null){
				return null;
			}
			
			Css.Value result;
			Variables.TryGetValue(name,out result);
			return result;
			
		}
		
		/// <summary>Adds a CSS variable to the variable set.</summary>
		public void AddVariable(string name,Css.Value value){
			
			if(Variables==null){
				Dom.Log.Add("Dropped a CSS variable ('"+name+"') as it's not in any suitable context.");
				return;
			}
			
			Variables[name]=value;
			
		}
		
		/// <summary>Attempt to recover the stream to a new readable state.</summary>
		public void ErrorRecovery(){
			
			// If we spot {, then read until }.
			char next=Read();
			
			while(next!=';' && next!='\0' && next!='{'){
				
				// Keep going:
				next=Read();
				
			}
			
			if(next=='{'){
				
				// Go recursive - skip a block:
				ErrorSkipBlock();
				
			}
			
			// Skip any junk:
			SkipJunk();
			
		}
		
		/// <summary>Skips a curley bracket block.</summary>
		private void ErrorSkipBlock(){
			
			char next=Read();
			
			while(next!='\0' && next!='}'){
				
				// Keep going:
				next=Read();
				
				if(next=='{'){
					
					// Recursive:
					ErrorSkipBlock();
					
				}
				
			}
			
		}
		
		/// <summary>Reads a function with the given lowercase name. There must be a bracket at the current read head.</summary>
		public Css.Value ReadFunction(string name){
			
			// Read off the open bracket:
			Read();
			
			// Get the global instance for the given function name (lowercase):
			CssFunction globalInstance=name=="" ? null : CssFunctions.Get(name);
			
			// Read the args:
			Css.Value parameters=null;
			
			if(globalInstance==null){
				
				// Unsupported function.
				parameters=ReadValue();
				
			}else{
				
				// Set literal value:
				if(globalInstance.LiteralValue){
					
					parameters=ReadLiteralValue();
					
				}else{
					
					parameters=ReadValue();
					
				}
			}
			
			// Skip any junk (which may be e.g. after a quote):
			SkipJunk();
			
			// Read off the close bracket:
			Read();
			
			if(name==""){
				// This occurs with nested brackets - it's just a set.
				return parameters;
			}
			
			if(globalInstance==null){
				// Don't know what this function is - act like it's not even there.
				return null;
			}
			
			// Copy the global instance:
			CssFunction result=globalInstance.Copy() as CssFunction;
			
			// Make sure params are a set:
			ValueSet set=parameters as ValueSet;
			
			if(set==null){
				// It's a single value, or null
				
				if(parameters!=null){
					// Push the single value:
					result.Values=new Css.Value[]{parameters};
				}
				
			}else{
				
				// Apply the parameters:
				result.Values=set.Values;
				
			}
			
			// Tell it that it's ready:
			result.OnValueReady(this);
			
			// Ok:
			return result;
			
		}
		
		/// <summary>Skips a whole block.</summary>
		public void SkipBlock(){
			char current=Read();
			int openBrackets=0;
			
			while(current!='\0'){
				
				if(current==';'){
					if(openBrackets==0){
						return;
					}
				}else if(current=='{'){
					openBrackets++;
				}else if(current=='}'){
					openBrackets--;
					
					if(openBrackets==0){
						return;
					}
				}
				
				current=Read();
			}
		}
		
		/// <summary>The lexer must be in literal mode for this. Reads a value from the stream. 
		/// May read a whole set or null if there's a ) or ; at the read head.</summary>
		public Css.Value ReadValue(){
			
			Value result=null;
			List<Value> values=null;
			ValueSet bottomSet=null;
			List<Value> bottomValues=null;
			bool important=false;
			
			// Skip junk:
			SkipJunk();
			
			
			char current=Peek();
			
			while( current!=')' && current!='}' && current!='\0' && current!=';' ){
				
				if(current==','){
					// read it off:
					Read();
					
					// Setup bottom set, if we need to:
					if(bottomSet==null){
						bottomSet=new ValueSet();
						bottomSet.Spacer=",";
						bottomValues=new List<Value>();
					}
				
					//"Push" values into bottom set.
					if(result!=null){
						
						if(values!=null){
							// Result is a set.
							ApplyList(result,values);
							values=null;
						}
						
						bottomValues.Add(result);
						
						result=null;
					}
					
				}else if(current=='!'){
					
					// Special lookout for "important".
					// We don't want it to create a set.
					Read();
					
					// Skip whitespaces.
					SkipJunk();
					
					// Read:
					Css.Value importantTest=ReadSingleValue();
					
					// Skip whitespaces.
					SkipJunk();
					
					// Got important?
					if(importantTest is Css.Keywords.ImportantKeyword){
						
						// Important!
						important=true;
						
					}else{
						
						// Act like we added both ! and importantTest.
						// Note that there might have been no spaces before it; they could be one value (but it's unlikely).
						
						if(result==null){
							result=new TextUnit("!");
						}else if(values==null){
							values=new List<Value>();
							values.Add(result);
							result=new ValueSet();
							values.Add(new TextUnit("!"));
						}else{
							values.Add(new TextUnit("!"));
						}
						
						if(importantTest!=null){
							
							if(values==null){
								values=new List<Value>();
								values.Add(result);
								result=new ValueSet();
								values.Add(importantTest);
							}else{
								values.Add(importantTest);
							}
							
						}
						
					}
					
					// Update current:
					current=Peek();
					
					continue;
					
				}
				
				if(result==null){
					result=ReadSingleValue();
					
					if(result==null){
						// Invalid rule
						
						// Skip whitespaces.
						SkipJunk();
						
						// Update current:
						current=Peek();
						
						if(current=='}' || current==';'){
							AtRuleMode=false;
							Read();
						}
						
						return null;
					}
					
				}else if(values==null){
					values=new List<Value>();
					values.Add(result);
					result=new ValueSet();
					values.Add(ReadSingleValue());
				}else{
					values.Add(ReadSingleValue());
				}
				
				if(SelectorMode && !AtRuleMode){
					
					// Check if we have a whitespace:
					if(Peek()==' '){
						
						if(values==null){
							values=new List<Value>();
							values.Add(result);
							result=new ValueSet();
							values.Add(new TextUnit(" "));
						}else{
							values.Add(new TextUnit(" "));
						}
						
					}
					
				}
				
				// Skip whitespaces.
				SkipJunk();
				
				// Update current:
				current=Peek();
				
			}
			
			if(current=='}' || current==';'){
				AtRuleMode=false;
				Read();
			}
			
			if(values!=null){
				// Update result with the values set:
				ApplyList(result,values);
			}
			
			if(bottomSet!=null){
				// Push result into it:
				bottomValues.Add(result);
				ApplyList(bottomSet,bottomValues);
				result=bottomSet;
			}
			
			if(important && result!=null){
				result.SetImportant(true);
			}
			
			return result;
			
		}
		
		private void ApplyList(Value set,List<Value> values){
			
			int count=values.Count;
			
			set.Count=count;
			for(int i=0;i<count;i++){
				set[i]=values[i];
			}
			
		}
		
		/// <summary>Reads a single value from the stream in literal mode.</summary>
		public Css.Value ReadLiteralValue(){
			
			SkipJunk();
			char current=Peek();
			
			if(current=='"' || current=='\''){
				
				// Quoted
				CssUnitHandlers set;
				CssUnits.AllStart.TryGetValue(current,out set);
				
				// Match on the pretext:
				Css.Value result=set.Handle(this,false);
				
				// Read the value:
				result=result.ReadStartValue(this);
				
				// Call ready:
				result.OnValueReady(this);
				
				// Ok!
				return result;
				
			}
			
			// E.g. textual keyword or function comes down here.
			// If we spot a (, call ReadValue() to get the parameter set.
			int textStart=Position;
			
			while(current!='\0' && current!=')'){
				
				Read();
				current=Peek();
				
			}
			
			// Text or keyword.
			return new TextUnit(Input.Substring(textStart,Position-textStart));
			
		}
		
		/// <summary>Reads a single value from the stream.</summary>
		public Css.Value ReadSingleValue(){
			
			Css.Value result=null;
			
			// Skip whitespaces.
			SkipJunk();
			
			char current=Peek();
			
			if(SelectorMode && current=='.'){
				Read();
				// Class selector.
				return new TextUnit(".");
			}
			
			int charCode=(int)current;
			
			CssUnitHandlers set;
			if(CssUnits.AllStart.TryGetValue(current,out set)){
				
				// Match on the pretext:
				result=set.Handle(this,false);
				
				if(result!=null){
					
					// Read the value:
					result=result.ReadStartValue(this);
					
					if(result==null){
						return null;
					}
					
					// Call ready:
					result.OnValueReady(this);
					
					// Ok!
					return result;
				}
				
			}
			
			// Number:
			int numberStart=Position;
			
			while( charCode==(int)'-' || charCode=='+' || (charCode>=(int)'0' && charCode<=(int)'9') || charCode==(int)'.'){
				
				if(numberStart==Position){
					
					// Special case if it's just +, . or -
					// Prefixed keywords fall through here too.
					
					if(charCode==(int)'.'){
						
						// Must be 0-9 next:
						int next=(int)Peek(1);
						
						if(next<(int)'0' || next>(int)'9'){
							// Non-numeric.
							break;
						}
						
					}else if(charCode==(int)'+' || charCode==(int)'-'){
						
						// Peek the next char. It must be either . or 0-9:
						int next=(int)Peek(1);
						
						if(next!=(int)'.' && (next<(int)'0' || next>(int)'9')){
							// Non-numeric.
							break;
						}
						
					}
					
				}
				
				// Read off:
				Read();
				
				// Go to the next one:
				current=Peek();
				
				// Get the code:
				charCode=(int)current;
				
			}
			
			if(numberStart!=Position){
				// Read a number of some kind. Now look for the units, if there are any.
				
				string num=Input.Substring(numberStart,Position-numberStart);
				
				if(CssUnits.AllEnd.TryGetValue(current,out set) && !SelectorMode){
					
					// Units handler is likely!
					result=set.Handle(this,true);
					
					if(result!=null){
						// Return the result:
						result=result.Copy();
						
						// Apply the number:
						float flt;
						float.TryParse(num,out flt);
						
						// Set:
						result.SetRawDecimal(flt);
						
						// Call ready:
						result.OnValueReady(this);
						
						return result;
					}
					
				}else{
					
					// Apply the number:
					float nFlt;
					float.TryParse(num,out nFlt);
					
					// Create:
					result=new DecimalUnit();
					
					// Set:
					result.SetRawDecimal(nFlt);
					
					// Call ready:
					result.OnValueReady(this);
					
					return result;
					
				}
				
			}
			
			if(charCode=='\\'){
				
				string characters="";
				
				while(true){
					
					// Special case here; we have one or more unicode escaped characters.
					Read();
					current=Peek();
					charCode=(int)current;
					int res=0;
					
					for(int i=0;i<6;i++){
						
						if(	charCode>=(int)'0' && charCode<=(int)'9' ){
							
							Read();
							// Apply to charcode:
							res=(res<<4) | (charCode-(int)'0');
							
							// Move on:
							current=Peek();
							charCode=(int)current;
							
						}else if( charCode>=(int)'a' && charCode<=(int)'f' ){
							
							Read();
							
							// Apply to charcode:
							res=(res<<4) | (charCode+10-(int)'a');
							
							// Move on:
							current=Peek();
							charCode=(int)current;
							
						}else{
							// No longer valid unicode.
							break;
						}
						
					}
					
					characters+=char.ConvertFromUtf32(res);
					
					if(charCode!='\\'){
						// Go again otherwise!
						break;
					}
					
				}
				
				return new TextUnit(characters);
			}
			
			// E.g. textual keyword or function comes down here.
			// If we spot a (, call ReadValue() to get the parameter set.
			int textStart=Position;
			
			while(charCode!=0 && (charCode>128 || charCode==45 || charCode==40 || (95<=charCode && charCode<=122) || 
				(65<=charCode && charCode<=90) || (48<=charCode && charCode<=57) )
				){
				
				if(current=='('){
					// Got a function name (if there is one).
					string name=Input.Substring(textStart,Position-textStart);
					result=ReadFunction(name);
					return result;
				}
				
				Read();
				current=Peek();
				charCode=(int)current;
				
			}
			
			if(textStart==Position && charCode!=0){
				// The only thing we've read is a delimiter.
				
				if( current=='\r' || current=='\n' || current==')' || current==' ' ||
					current=='}' || current=='{' || current==';' || current==','
				){
					
					// Handled elsewhere. Ignore these.
					// They all terminate upper level value readers (or are junk!).
					
				}else{
					
					// Add the single character into the buffer.
					Read();
					
					if(current=='$' || current=='*' || current=='^' || current=='~' || current=='|'){
						// Followed by equals?
						if(Peek()=='='){
							// Yep - include that in this same unit.
							Read();
							
							return new TextUnit(current+"=");
						}
					}else if(current=='>'){
						// Followed by another?
						if(Peek()=='>'){
							// Yep - include that in this same unit.
							Read();
							return new TextUnit(">>");
						}
					}
					
					return new TextUnit(current.ToString());
				}
			}
			
			// Text or keyword.
			string text=Input.Substring(textStart,Position-textStart);
			
			// Must not match keywords/ colours when in selector mode
			// (because keywords are always lowercase and colours act like *):
			if(SelectorMode){
				
				// Just text:
				return new TextUnit(text);
				
			}
			
			// Keyword tests.
			string keyword=text.ToLower();
			
			// Colour?
			bool wasColour;
			UnityEngine.Color32 col=Css.ColourMap.GetColourByName(keyword,out wasColour);
			
			if(wasColour){
				
				// It's a colour:
				ColourUnit cResult=new ColourUnit(col);
				
				// Call ready:
				cResult.OnValueReady(this);
				
				return cResult;
				
			}
			
			// Global keyword?
			result=CssKeywords.Get(keyword);
			
			if(result!=null){
				
				// Keyword! Can't share the global instance because of specifity.
				return result.Copy();
				
			}
			
			// Just treat as some text otherwise:
			return new TextUnit(text);
			
		}
		
		/// <summary>Reads a single value as a string only.</summary>
		public string ReadString(){
			
			SkipJunk();
			char current=Peek();
			int pos=Position;
			
			while(
				current!=')' && current!='}' && current!='\r' && current!='\n' && 
				current!='{' && current!=' ' && current!=';' && 
				current!=',' && current!='\0' && current!='\'' && current!='"'
			){
				
				Read();
				current=Peek();
				
			}
			
			// Text or keyword.
			return Input.Substring(pos,Position-pos);
			
		}
		
	}
	
}