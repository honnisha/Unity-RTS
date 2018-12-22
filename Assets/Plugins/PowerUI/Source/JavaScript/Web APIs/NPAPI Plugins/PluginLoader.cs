#if UNITY_STANDALONE_WIN

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace PowerUI{
	
	/// <summary>
	/// Loads an NPAPI plugin from a DLL.
	/// Note that PowerUI does not store a list of plugins it has installed - you'll need to manage that yourself.
	/// </summary>
	
	public class NpApiPlugin : DllLoader{
		
		public const int NP_ABI_MASK=0;
		
		public const int NP_VERSION_MAJOR=0;
		public const int NP_VERSION_MINOR=19;
		
		/* RC_DATA types for version info - required */
		public const int NP_INFO_ProductVersion=1;
		public const int NP_INFO_MIMEType=2;
		public const int NP_INFO_FileOpenName=3;
		public const int NP_INFO_FileExtents=4;
		
		/* RC_DATA types for version info - used if found */
		public const int NP_INFO_FileDescription=5;
		public const int NP_INFO_ProductName=6;
		
		/* RC_DATA types for version info - optional */
		public const int NP_INFO_CompanyName=7;
		public const int NP_INFO_FileVersion=8;
		public const int NP_INFO_InternalName=9;
		public const int NP_INFO_LegalCopyright=10;
		public const int NP_INFO_OriginalFilename=11;
		
		/* Error codes */
		public const short NPERR_NO_ERROR=0;
		public const short NPERR_GENERIC_ERROR =1;
		public const short NPERR_INVALID_INSTANCE_ERROR=2;
		public const short NPERR_INVALID_FUNCTABLE_ERROR=3;
		public const short NPERR_MODULE_LOAD_FAILED_ERROR=4;
		public const short NPERR_OUT_OF_MEMORY_ERROR =5;
		public const short NPERR_INVALID_PLUGIN_ERROR =6;
		public const short NPERR_INVALID_PLUGIN_DIR_ERROR=7;
		public const short NPERR_INCOMPATIBLE_VERSION_ERROR=8;
		public const short NPERR_INVALID_PARAM=9;
		public const short NPERR_INVALID_URL =10;
		public const short NPERR_FILE_NOT_FOUND=11;
		public const short NPERR_NO_DATA= 12;
		public const short NPERR_STREAM_NOT_SEEKABLE=13;
		
		/// <summary>NP_Initialize function pointer mapped to a delegate.</summary>
		private NP_Initialize_Delegate NP_Initialize;
		
		
		public NpApiPlugin(string dllPath):base(dllPath){
			
			// Get the NPAPI functions:
			NP_Initialize=(NP_Initialize_Delegate)GetFunction("NP_Initialize",typeof(NP_Initialize_Delegate));
			
		}
		
		public void Initialize(){
			NP_Initialize();
		}
		
		#region NPAPI Delegates
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NP_Initialize_Delegate();
		
		#endregion
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPP{
		
		/// <summary>Plug-in private data</summary>
		public IntPtr pdata;
		/// <summary>UA private data</summary>
		public IntPtr ndata;
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPStream{
		
		/// <summary>Plug-in private data</summary>
		public IntPtr pdata;
		/// <summary>UA private data</summary>
		public IntPtr ndata;
		public string url;
		public uint end;
		public uint lastmodified;
		public IntPtr notifyData;
		public string headers; // version 0.17+
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPByteRange{
		/// <summary>Negative offset means from the end.</summary>
		public int offset;
		public uint length;
		public IntPtr next;
	}
	
	public enum NPPVariable{
		NPPVpluginNameString = 1,
		NPPVpluginDescriptionString,
		NPPVpluginWindowBool,
		NPPVpluginTransparentBool,
		NPPVjavaClass,                /* Not implemented in Mozilla 1.0 */
		NPPVpluginWindowSize,
		NPPVpluginTimerInterval,
		NPPVpluginScriptableInstance = (10 | NpApiPlugin.NP_ABI_MASK),
		NPPVpluginScriptableIID = 11,
		/* Introduced in Mozilla 0.9.9 */
		NPPVjavascriptPushCallerBool = 12,
		/* Introduced in Mozilla 1.0 */
		NPPVpluginKeepLibraryInMemory = 13,
		NPPVpluginNeedsXEmbed         = 14,
		/* Get the NPObject for scripting the plugin. Introduced in Firefox
		* 1.0 (NPAPI minor version 14).
		*/
		NPPVpluginScriptableNPObject  = 15,
		/* Get the plugin value (as \0-terminated UTF-8 string data) for
		* form submission if the plugin is part of a form. Use
		* NPN_MemAlloc() to allocate memory for the string data. Introduced
		* in Mozilla 1.8b2 (NPAPI minor version 15).
		*/
		NPPVformValue = 16
		#if XP_MACOSX
		/* Used for negotiating drawing models */
		, NPPVpluginDrawingModel = 1000
		#endif
	}
	
	/// <summary>
	/// The NPAPI NPN_* functions tbat are provided by the navigator and called by plugins.
	/// </summary>
	public class NavigatorNpApi{
		
		/// <summary>The target window.</summary>
		public Window Window;
		
		
		/*
		public void NPN_Version(ref int plugin_major, ref int plugin_minor,ref int netscape_major, ref int netscape_minor){
			
			plugin_major=NpApiPlugin.NP_VERSION_MAJOR;
			plugin_minor=NpApiPlugin.NP_VERSION_MINOR;
			
			netscape_major=UI.Major;
			netscape_minor=UI.Minor;
			
		}
		
		public short NPN_GetURLNotify(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			IntPtr notifyData
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_GetURL(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_PostURLNotify(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			uint len,
			[MarshalAs(UnmanagedType.LPStr)]ref string buf,
			byte file,
			IntPtr notifyData
		){
			
			url=Window.document.location.href;
			target="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_PostURL(
			NPP instance,
			[MarshalAs(UnmanagedType.LPStr)]ref string url,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			uint32 len,
			[MarshalAs(UnmanagedType.LPStr)]ref string buf,
			IntPtr file
		){
			
			url=Window.document.location.href;
			target="";
			buf="";
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_RequestRead(NPStream stream, NPByteRange rangeList){
			
			return NPERR_NO_ERROR;
			
		}
		
		public short NPN_NewStream(
			NPP instance,
			NPMIMEType type,
			[MarshalAs(UnmanagedType.LPStr)]ref string target,
			ref NPStream stream
		){
			
		}
		
		public int NPN_Write(NPP instance, NPStream stream, int len,void* buffer){
			
		}
		
		public short NPN_DestroyStream(NPP instance, NPStream* stream,NPReason reason){
			
		}
		
		public void NPN_Status(NPP instance, [MarshalAs(UnmanagedType.LPStr)]ref string message){
			
			message="";
			
		}
		
		[MarshalAs(UnmanagedType.LPStr)]
		public string NPN_UserAgent(NPP instance){
			return UI.UserAgent;
		}
		
		public void NPN_ReloadPlugins(byte reloadPages);
		public JRIEnv* NPN_GetJavaEnv(void);
		public jref NPN_GetJavaPeer(NPP instance);
		public short NPN_GetValue(NPP instance, NPNVariable variable, void *value);
		public short NPN_SetValue(NPP instance, NPPVariable variable,void *value);
		public void NPN_InvalidateRect(NPP instance, NPRect *invalidRect);
		public void NPN_InvalidateRegion(NPP instance, NPRegion invalidRegion);
		public void NPN_ForceRedraw(NPP instance);
		*/
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPNetscapeFuncs{
		public ushort size;
		public ushort version; // Newer versions may have additional fields added to the end
		public IntPtr geturl; // Make a GET request for a URL either to the window or another stream
		public IntPtr posturl; // Make a POST request for a URL either to the window or another stream
		public IntPtr requestread;
		public IntPtr newstream;
		public IntPtr write;
		public IntPtr destroystream;
		public IntPtr status;
		public IntPtr uagent;
		public IntPtr memalloc; // Allocates memory from the browser's memory space
		public IntPtr memfree; // Frees memory from the browser's memory space
		public IntPtr memflush;
		public IntPtr reloadplugins;
		public IntPtr getJavaEnv;
		public IntPtr getJavaPeer;
		public IntPtr geturlnotify; // Async call to get a URL
		public IntPtr posturlnotify; // Async call to post a URL
		public IntPtr getvalue; // Get information from the browser
		public IntPtr setvalue; // Set information about the plugin that the browser controls
		public IntPtr invalidaterect;
		public IntPtr invalidateregion;
		public IntPtr forceredraw;
		public IntPtr getstringidentifier; // Get a NPIdentifier for a given string
		public IntPtr getstringidentifiers;
		public IntPtr getintidentifier;
		public IntPtr identifierisstring;
		public IntPtr utf8fromidentifier; // Get a string from a NPIdentifier
		public IntPtr intfromidentifier;
		public IntPtr createobject; // Create an instance of a NPObject
		public IntPtr retainobject; // Increment the reference count of a NPObject
		public IntPtr releaseobject; // Decrement the reference count of a NPObject
		public IntPtr invoke; // Invoke a method on a NPObject
		public IntPtr invokeDefault; // Invoke the default method on a NPObject
		public IntPtr evaluate; // Evaluate javascript in the scope of a NPObject
		public IntPtr getproperty; // Get a property on a NPObject
		public IntPtr setproperty; // Set a property on a NPObject
		public IntPtr removeproperty; // Remove a property from a NPObject
		public IntPtr hasproperty; // Returns true if the given NPObject has the given property
		public IntPtr hasmethod; // Returns true if the given NPObject has the given Method
		public IntPtr releasevariantvalue; // Release a MNVariant (free memory)
		public IntPtr setexception;
		public IntPtr pushpopupsenabledstate;
		public IntPtr poppopupsenabledstate;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPPluginFuncs {
		public ushort size;
		public ushort version;
		public IntPtr newp;
		public IntPtr destroy;
		public IntPtr setwindow;
		public IntPtr newstream;
		public IntPtr destroystream;
		public IntPtr asfile;
		public IntPtr writeready;
		public IntPtr write;
		public IntPtr print;
		public IntPtr evt;
		public IntPtr urlnotify;
		public IntPtr javaClass;
		public IntPtr getvalue;
		public IntPtr setvalue;
	}
	
	public enum NPNVariable{
		NPNVxDisplay = 1,
		NPNVxtAppContext,
		NPNVnetscapeWindow,
		NPNVjavascriptEnabledBool,
		NPNVasdEnabledBool,
		NPNVisOfflineBool,
		/* 10 and over are available on Mozilla builds starting with 0.9.4 */
		NPNVserviceManager = (10 | NpApiPlugin.NP_ABI_MASK),
		NPNVDOMElement     = (11 | NpApiPlugin.NP_ABI_MASK),   /* available in Mozilla 1.2 */
		NPNVDOMWindow      = (12 | NpApiPlugin.NP_ABI_MASK),
		NPNVToolkit        = (13 | NpApiPlugin.NP_ABI_MASK),
		NPNVSupportsXEmbedBool = 14,
		/* Get the NPObject wrapper for the browser window. */
		NPNVWindowNPObject = 15,
		/* Get the NPObject wrapper for the plugins DOM element. */
		NPNVPluginElementNPObject = 16,
		NPNVSupportsWindowless = 17
		#if XP_MACOSX
			/* Used for negotiating drawing models */
			, NPNVpluginDrawingModel = 1000
		, NPNVsupportsCoreGraphicsBool = 2001
		#endif
	}
	
	public enum NPWindowType{
		Window = 1,
		Drawable
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPSavedData{
		public int len;
		public IntPtr buf;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPRect{
		public ushort top;
		public ushort left;
		public ushort bottom;
		public ushort right;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPSize{
		public int width;
		public int height;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPWindow{
		
		/// <summary>The native window handle. hWnd on Windows.</summary>
		public IntPtr nativeWindow;
		/// <summary>Position of top left corner relative to the UA.</summary>
		public uint x;
		/// <summary>Position of top left corner relative to the UA.</summary>
		public uint y;
		/// <summary>Maximum window size</summary>
		public uint width;
		/// <summary>Maximum window size</summary>
		public uint height;
		/// <summary>Clipping rectangle in port coordinates</summary>
		public NPRect clipRect;
		
		#if XP_UNIX && !XP_MACOSX
		/// <summary>Platform-dependent additonal data (old Mac only)</summary>
		public IntPtr ws_info;
		#endif
		
		/// <summary>Is this a window or a drawable?</summary>
		public NPWindowType type;
		
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPFullPrint{
		public byte pluginPrinted;/* Set TRUE if plugin handled fullscreen printing */
		public byte printOne;		 /* TRUE if plugin should print one copy to default printer */
		public IntPtr platformPrint; /* Platform-specific printing info */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPEmbedPrint{
		public NPWindow window;
		public IntPtr platformPrint; /* Platform-specific printing info */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPPrint_Union{
		public NPFullPrint fullPrint;   /* if mode is NP_FULL */
		public NPEmbedPrint embedPrint; /* if mode is NP_EMBED */
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPPrint{
		public ushort mode;               /* NP_FULL or NP_EMBED */
		public NPPrint_Union print;
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NPEvent{
		
		public ushort Event; // can also be a uint
		public uint WParam;
		public uint LParam;
		
	}
	
	/// <summary>
	/// Loads DLLs dynamically. Used by NPAPI to load e.g. Flash.
	/// </summary>
	public class DllLoader{
		
		/// <summary>A pointer to the DLL itself.</summary>
		private IntPtr Dll;
		
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllToLoad);
		
		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		internal static extern bool FreeLibrary(IntPtr hModule);
		
		/// <summary>Gets a function from the DLL as a C# delegate.</summary>
		internal object GetFunction(string name,Type delegateType){
			
			// Get the pointer:
			IntPtr functionPtr=GetProcAddress(Dll, name);
			
			// Map to a delegate:
			return Marshal.GetDelegateForFunctionPointer(functionPtr, delegateType);
			
		}
		
		/// <summary>Loads the named DLL.</summary>
		public DllLoader(string dllName){
			
			Dll = LoadLibrary(dllName);
			
		}
		
		/// <summary>Frees the DLL.</summary>
		~DllLoader()
		{
			FreeLibrary(Dll);
		}
		
	}
	
}

#endif