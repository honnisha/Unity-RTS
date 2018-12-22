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
using System.IO;


namespace Brotli{
	
	public struct HuffmanCode{
		
		/// <summary>Number of bits used for this symbol</summary>
		public byte Bits;
		/// <summary>Symbol value or table offset</summary>
		public ushort Value;
		
		
		public HuffmanCode(byte bits,ushort value){
			Bits=bits;
			Value=value;
		}
		
	}
	
	public static class Huffman{
		
		public const int MAX_LENGTH = 15;
		
		/* Returns reverse(reverse(key, len) + 1, len), where reverse(key, len) is the
		   bit-wise reversal of the len least significant bits of key. */
		public static int GetNextKey(int key,int len) {
			
			int step = 1 << (len - 1);
			
			while ((key & step)!=0) {
				step >>= 1;
			}
			
			return (key & (step - 1)) + step;
		}
		
		/* Static Huffman code for the code length code lengths */
		internal static readonly HuffmanCode[] CodeLengthCodeLengths = new HuffmanCode[]{
			new HuffmanCode(2, 0), new HuffmanCode(2, 4), new HuffmanCode(2, 3), new HuffmanCode(3, 2), 
			new HuffmanCode(2, 0), new HuffmanCode(2, 4), new HuffmanCode(2, 3), new HuffmanCode(4, 1),
			new HuffmanCode(2, 0), new HuffmanCode(2, 4), new HuffmanCode(2, 3), new HuffmanCode(3, 2), 
			new HuffmanCode(2, 0), new HuffmanCode(2, 4), new HuffmanCode(2, 3), new HuffmanCode(4, 5)
		};
		
		/* Stores code in table[0], table[step], table[2*step], ..., table[end] */
		/* Assumes that end is an integer multiple of step */
		public static void ReplicateValue(HuffmanCode[] table,int i,int step,int end,HuffmanCode code) {
			do {
				end -= step;
				table[i + end] = new HuffmanCode(code.Bits, code.Value);
			} while (end > 0);
		}

		/* Returns the table width of the next 2nd level table. count is the histogram
		   of bit lengths for the remaining symbols, len is the code length of the next
		   processed symbol */
		public static int NextTableBitSize(ushort[] count,int len,int root_bits) {
			int left = 1 << (len - root_bits);
			
			while (len < MAX_LENGTH) {
				left -= count[len];

				if (left <= 0){
					break;
				}

				len++;
				left <<= 1;
			}
			
			return len - root_bits;
		}
		
		public static int BuildHuffmanTable(HuffmanCode[] root_table,int table,int root_bits,byte[] code_lengths,int code_lengths_size){
			
			int start_table = table;
			HuffmanCode code;            /* current table entry */
			int len;             /* current code length */
			int symbol;          /* symbol index in original or sorted table */
			int key;             /* reversed prefix code */
			int step;            /* step size to replicate values in current table */
			int low;             /* low bits for current root entry */
			int mask;            /* mask for low bits */
			int table_bits;      /* key length of current table */
			int table_size;      /* size of current table */
			int total_size;      /* sum of root table size and 2nd level table sizes */
			int[] sorted;          /* symbols sorted by code length */
			ushort[] count = new ushort[MAX_LENGTH + 1];  /* number of codes of each length */
			int[] offset = new int[MAX_LENGTH + 1];  /* offsets in sorted table for each length */
			
			sorted = new int[code_lengths_size];

			/* build histogram of code lengths */
			for (symbol = 0; symbol < code_lengths_size; symbol++) {
				count[code_lengths[symbol]]++;
			}
			
			/* generate offsets into sorted symbol table by code length */
			offset[1] = 0;
			for (len = 1; len < MAX_LENGTH; len++) {
				offset[len + 1] = offset[len] + count[len];
			}
			
			/* sort symbols by length, by symbol order within each length */
			for (symbol = 0; symbol < code_lengths_size; symbol++) {
				if (code_lengths[symbol] != 0) {
					sorted[offset[code_lengths[symbol]]++] = symbol;
				}
			}

			table_bits = root_bits;
			table_size = 1 << table_bits;
			total_size = table_size;

			/* special case code with only one value */
			if (offset[MAX_LENGTH] == 1) {
				for (key = 0; key < total_size; key++) {
					root_table[table + key] = new HuffmanCode(0, (ushort)sorted[0]);
				}

				return total_size;
			}

			/* fill in root table */
			key = 0;
			symbol = 0;
			
			for (len = 1, step = 2; len <= root_bits; ++len, step <<= 1) {
				for (; count[len] > 0; count[len]--) {
					code = new HuffmanCode((byte)len, (ushort)sorted[symbol++]);
					ReplicateValue(root_table, table + key, step, table_size, code);
					key = GetNextKey(key, len);
				}
			}

			/* fill in 2nd level tables and add pointers to root table */
			mask = total_size - 1;
			low = -1;
			
			for (len = root_bits + 1, step = 2; len <= MAX_LENGTH; ++len, step <<= 1) {
				for (; count[len] > 0; count[len]--) {
					
					if ((key & mask) != low) {
						table += table_size;
						table_bits = NextTableBitSize(count, len, root_bits);
						table_size = 1 << table_bits;
						total_size += table_size;
						low = key & mask;
						root_table[start_table + low] = new HuffmanCode((byte)(table_bits + root_bits), (ushort)((table - start_table) - low));
					}
					
					code = new HuffmanCode((byte)(len - root_bits), (ushort)sorted[symbol++]);
					ReplicateValue(root_table, table + (key >> root_bits), step, table_size, code);
					key = GetNextKey(key, len);
					
				}
			}
			
			return total_size;
		}
		
	}
	
}