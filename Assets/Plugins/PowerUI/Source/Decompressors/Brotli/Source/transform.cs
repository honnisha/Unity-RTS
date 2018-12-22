//--------------------------------------
//         Brotli Decompressor
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2016 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------


using System;


namespace Brotli{
	
	/// <summary>Common transforms.</summary>
	
	public struct Transform{
		
		public string Prefix;
		public byte Transformation;
		public string Suffix;
		
		
		public Transform(string prefix,byte trans,string suffix){
			Prefix=prefix;
			Transformation=trans;
			Suffix=suffix;
		}
		
	}

	public static class Transforms{
		
		const byte kIdentity       = 0;
		const byte kOmitLast1      = 1;
		const byte kOmitLast2      = 2;
		const byte kOmitLast3      = 3;
		const byte kOmitLast4      = 4;
		const byte kOmitLast5      = 5;
		const byte kOmitLast6      = 6;
		const byte kOmitLast7      = 7;
		const byte kOmitLast8      = 8;
		const byte kOmitLast9      = 9;
		const byte kUppercaseFirst = 10;
		const byte kUppercaseAll   = 11;
		const byte kOmitFirst1     = 12;
		const byte kOmitFirst2     = 13;
		const byte kOmitFirst3     = 14;
		const byte kOmitFirst4     = 15;
		const byte kOmitFirst5     = 16;
		const byte kOmitFirst6     = 17;
		const byte kOmitFirst7     = 18;
		const byte kOmitFirst8     = 19;
		const byte kOmitFirst9     = 20;
		
		internal static int kNumTransforms{
			get{
				return kTransforms.Length;
			}
		}
		
		private readonly static Transform[] kTransforms = new Transform[]{
			new Transform(         "", kIdentity,       ""           ),
			new Transform(         "", kIdentity,       " "          ),
			new Transform(        " ", kIdentity,       " "          ),
			new Transform(         "", kOmitFirst1,     ""           ),
			new Transform(         "", kUppercaseFirst, " "          ),
			new Transform(         "", kIdentity,       " the "      ),
			new Transform(        " ", kIdentity,       ""           ),
			new Transform(       "s ", kIdentity,       " "          ),
			new Transform(         "", kIdentity,       " of "       ),
			new Transform(         "", kUppercaseFirst, ""           ),
			new Transform(         "", kIdentity,       " and "      ),
			new Transform(         "", kOmitFirst2,     ""           ),
			new Transform(         "", kOmitLast1,      ""           ),
			new Transform(       ", ", kIdentity,       " "          ),
			new Transform(         "", kIdentity,       ", "         ),
			new Transform(        " ", kUppercaseFirst, " "          ),
			new Transform(         "", kIdentity,       " in "       ),
			new Transform(         "", kIdentity,       " to "       ),
			new Transform(       "e ", kIdentity,       " "          ),
			new Transform(         "", kIdentity,       "\""         ),
			new Transform(         "", kIdentity,       "."          ),
			new Transform(         "", kIdentity,       "\">"        ),
			new Transform(         "", kIdentity,       "\n"         ),
			new Transform(         "", kOmitLast3,      ""           ),
			new Transform(         "", kIdentity,       "]"          ),
			new Transform(         "", kIdentity,       " for "      ),
			new Transform(         "", kOmitFirst3,     ""           ),
			new Transform(         "", kOmitLast2,      ""           ),
			new Transform(         "", kIdentity,       " a "        ),
			new Transform(         "", kIdentity,       " that "     ),
			new Transform(        " ", kUppercaseFirst, ""           ),
			new Transform(         "", kIdentity,       ". "         ),
			new Transform(        ".", kIdentity,       ""           ),
			new Transform(        " ", kIdentity,       ", "         ),
			new Transform(         "", kOmitFirst4,     ""           ),
			new Transform(         "", kIdentity,       " with "     ),
			new Transform(         "", kIdentity,       "'"          ),
			new Transform(         "", kIdentity,       " from "     ),
			new Transform(         "", kIdentity,       " by "       ),
			new Transform(         "", kOmitFirst5,     ""           ),
			new Transform(         "", kOmitFirst6,     ""           ),
			new Transform(    " the ", kIdentity,       ""           ),
			new Transform(         "", kOmitLast4,      ""           ),
			new Transform(         "", kIdentity,       ". The "     ),
			new Transform(         "", kUppercaseAll,   ""           ),
			new Transform(         "", kIdentity,       " on "       ),
			new Transform(         "", kIdentity,       " as "       ),
			new Transform(         "", kIdentity,       " is "       ),
			new Transform(         "", kOmitLast7,      ""           ),
			new Transform(         "", kOmitLast1,      "ing "       ),
			new Transform(         "", kIdentity,       "\n\t"       ),
			new Transform(         "", kIdentity,       ":"          ),
			new Transform(        " ", kIdentity,       ". "         ),
			new Transform(         "", kIdentity,       "ed "        ),
			new Transform(         "", kOmitFirst9,     ""           ),
			new Transform(         "", kOmitFirst7,     ""           ),
			new Transform(         "", kOmitLast6,      ""           ),
			new Transform(         "", kIdentity,       "("          ),
			new Transform(         "", kUppercaseFirst, ", "         ),
			new Transform(         "", kOmitLast8,      ""           ),
			new Transform(         "", kIdentity,       " at "       ),
			new Transform(         "", kIdentity,       "ly "        ),
			new Transform(    " the ", kIdentity,       " of "       ),
			new Transform(         "", kOmitLast5,      ""           ),
			new Transform(         "", kOmitLast9,      ""           ),
			new Transform(        " ", kUppercaseFirst, ", "         ),
			new Transform(         "", kUppercaseFirst, "\""         ),
			new Transform(        ".", kIdentity,       "("          ),
			new Transform(         "", kUppercaseAll,   " "          ),
			new Transform(         "", kUppercaseFirst, "\">"        ),
			new Transform(         "", kIdentity,       "=\""        ),
			new Transform(        " ", kIdentity,       "."          ),
			new Transform(    ".com/", kIdentity,       ""           ),
			new Transform(    " the ", kIdentity,       " of the "   ),
			new Transform(         "", kUppercaseFirst, "'"          ),
			new Transform(         "", kIdentity,       ". This "    ),
			new Transform(         "", kIdentity,       ","          ),
			new Transform(        ".", kIdentity,       " "          ),
			new Transform(         "", kUppercaseFirst, "("          ),
			new Transform(         "", kUppercaseFirst, "."          ),
			new Transform(         "", kIdentity,       " not "      ),
			new Transform(        " ", kIdentity,       "=\""        ),
			new Transform(         "", kIdentity,       "er "        ),
			new Transform(        " ", kUppercaseAll,   " "          ),
			new Transform(         "", kIdentity,       "al "        ),
			new Transform(        " ", kUppercaseAll,   ""           ),
			new Transform(         "", kIdentity,       "='"         ),
			new Transform(         "", kUppercaseAll,   "\""         ),
			new Transform(         "", kUppercaseFirst, ". "         ),
			new Transform(        " ", kIdentity,       "("          ),
			new Transform(         "", kIdentity,       "ful "       ),
			new Transform(        " ", kUppercaseFirst, ". "         ),
			new Transform(         "", kIdentity,       "ive "       ),
			new Transform(         "", kIdentity,       "less "      ),
			new Transform(         "", kUppercaseAll,   "'"          ),
			new Transform(         "", kIdentity,       "est "       ),
			new Transform(        " ", kUppercaseFirst, "."          ),
			new Transform(         "", kUppercaseAll,   "\">"        ),
			new Transform(        " ", kIdentity,       "='"         ),
			new Transform(         "", kUppercaseFirst, ","          ),
			new Transform(         "", kIdentity,       "ize "       ),
			new Transform(         "", kUppercaseAll,   "."          ),
			new Transform( "\xc2\xa0", kIdentity,       ""           ),
			new Transform(        " ", kIdentity,       ","          ),
			new Transform(         "", kUppercaseFirst, "=\""        ),
			new Transform(         "", kUppercaseAll,   "=\""        ),
			new Transform(         "", kIdentity,       "ous "       ),
			new Transform(         "", kUppercaseAll,   ", "         ),
			new Transform(         "", kUppercaseFirst, "='"         ),
			new Transform(        " ", kUppercaseFirst, ","          ),
			new Transform(        " ", kUppercaseAll,   "=\""        ),
			new Transform(        " ", kUppercaseAll,   ", "         ),
			new Transform(         "", kUppercaseAll,   ","          ),
			new Transform(         "", kUppercaseAll,   "("          ),
			new Transform(         "", kUppercaseAll,   ". "         ),
			new Transform(        " ", kUppercaseAll,   "."          ),
			new Transform(         "", kUppercaseAll,   "='"         ),
			new Transform(        " ", kUppercaseAll,   ". "         ),
			new Transform(        " ", kUppercaseFirst, "=\""        ),
			new Transform(        " ", kUppercaseAll,   "='"         ),
			new Transform(        " ", kUppercaseFirst, "='"         )
		};

		public static int ToUpperCase(byte[] p,int i) {
			if (p[i] < 0xc0) {
				if (p[i] >= 97 && p[i] <= 122) {
					p[i] ^= 32;
				}
				return 1;
			}

			/* An overly simplified uppercasing model for utf-8. */
			if (p[i] < 0xe0) {
				p[i + 1] ^= 32;
				return 2;
			}

			/* An arbitrary transform for three byte characters. */
			p[i + 2] ^= 5;
			return 3;
		}
		
		public static int TransformDictionaryWord(byte[] dst,int idx,int word,int len, int trans) {
			
			Transform transform=kTransforms[trans];
			
			string prefix = transform.Prefix;
			string suffix = transform.Suffix;
			int t = transform.Transformation;
			int skip = t < kOmitFirst1 ? 0 : t - (kOmitFirst1 - 1);
			int i = 0;
			int start_idx = idx;
			int uppercase;

			if (skip > len) {
				skip = len;
			}

			int prefix_pos = 0;
			
			while (prefix_pos < prefix.Length) {
				dst[idx++] = (byte)prefix[prefix_pos++];
			}

			word += skip;
			len -= skip;

			if (t <= kOmitLast9) {
				len -= t;
			}

			for (i = 0; i < len; i++) {
				dst[idx++] = Dictionary.Data[word + i];
			}

			uppercase = idx - len;

			if (t == kUppercaseFirst) {
				ToUpperCase(dst, uppercase);
			} else if (t == kUppercaseAll) {
				while (len > 0) {
					int step = ToUpperCase(dst, uppercase);
					uppercase += step;
					len -= step;
				}
			}

			int suffix_pos = 0;
			
			while (suffix_pos < suffix.Length) {
				dst[idx++] = (byte)suffix[suffix_pos++];
			}
			
			return idx - start_idx;
		}	
	
	}
	
}