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
	
	internal class MetaBlockLength{
		
		internal int meta_block_length;
		internal bool input_end;
		internal bool is_uncompressed;
		internal bool is_metadata;
		
		
		internal MetaBlockLength(){
			meta_block_length=0;
			input_end=false;
			is_uncompressed=false;
			is_metadata=false;
		}
	}
	
	/* Contains a collection of huffman trees with the same alphabet size. */
	internal class HuffmanTreeGroup{
		
		public int alphabet_size;
		public int num_htrees;
		public uint[] htrees;
		public HuffmanCode[] codes;
		
		internal HuffmanTreeGroup(int alphabet_size,int num_htrees){
			
			this.alphabet_size = alphabet_size;
			this.num_htrees = num_htrees;
			this.codes = new HuffmanCode[num_htrees + num_htrees * Decoder.kMaxHuffmanTableSize[(alphabet_size + 31) >> 5]];  
			this.htrees = new uint[num_htrees];
			
		}
		
		internal void Decode(BitReader br) {
			
			int table_size;
			int next = 0;
			
			for (int i = 0; i < num_htrees; ++i) {
				htrees[i] = (uint)next;
				table_size = Decoder.ReadHuffmanCode(alphabet_size, codes, next, br);
				next += table_size;
			}
			
		}
		
	}
	
	internal class ContextMap{
		public int num_htrees;
		public byte[] context_map;
	}
	
	internal class OutputStream{
		internal bool FixedSize;
		internal byte[] buffer;
		internal int pos_;
		
		internal OutputStream(byte[] b,bool fixedSize){
			buffer=b;
			FixedSize=fixedSize;
		}
		
		internal void Write(byte[] buff,int count){
			
			System.Array.Copy(buff,0,buffer,pos_,count);
			pos_+=count;
			
		}
		
	}
	
	public static class Decoder{
		
		const int kDefaultCodeLength = 8;
		const int kCodeLengthRepeatCode = 16;
		const int kNumLiteralCodes = 256;
		const int kNumInsertAndCopyCodes = 704;
		const int kNumBlockLengthCodes = 26;
		const int kLiteralContextBits = 6;
		const int kDistanceContextBits = 2;
		
		const int HUFFMAN_TABLE_BITS = 8;
		const int HUFFMAN_TABLE_MASK = 0xff;
		/* Maximum possible Huffman table size for an alphabet size of 704, max code
		 * length 15 and root table bits 8. */
		const int HUFFMAN_MAX_TABLE_SIZE = 1080;
		
		const int CODE_LENGTH_CODES = 18;
		static readonly byte[] kCodeLengthCodeOrder = new byte[]{
		  1, 2, 3, 4, 0, 5, 17, 6, 16, 7, 8, 9, 10, 11, 12, 13, 14, 15,
		};
		
		const int NUM_DISTANCE_SHORT_CODES = 16;
		static readonly byte[] kDistanceShortCodeIndexOffset = new byte[]{
		  3, 2, 1, 0, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 2
		};
		
		static readonly sbyte[] kDistanceShortCodeValueOffset = new sbyte[]{
		  0, 0, 0, 0, -1, 1, -2, 2, -3, 3, -1, 1, -2, 2, -3, 3
		};
		
		internal static readonly ushort[] kMaxHuffmanTableSize = new ushort[]{
		  256, 402, 436, 468, 500, 534, 566, 598, 630, 662, 694, 726, 758, 790, 822,
		  854, 886, 920, 952, 984, 1016, 1048, 1080
		};
		
		public static int DecodeWindowBits(BitReader br) {
			int n;
			if (br.ReadBits(1) == 0) {
				return 16;
			}

			n = (int)br.ReadBits(3);
			if (n > 0) {
				return 17 + n;
			}
			
			n = (int)br.ReadBits(3);
			if (n > 0) {
				return 8 + n;
			}

			return 17;
		}
		
		/* Decodes a number in the range [0..255], by reading 1 - 11 bits. */
		public static byte DecodeVarLenUint8(BitReader br) {
			if (br.ReadBits(1)!=0) {
				
				int nbits = (int)br.ReadBits(3);
				
				if (nbits == 0) {
					return 1;
				} else {
					return (byte)( br.ReadBits(nbits) + (1 << nbits) );
				}
				
			}
			
			return 0;
		}
		
		internal static MetaBlockLength DecodeMetaBlockLength(BitReader br){

			MetaBlockLength mbl = new MetaBlockLength();  
			uint size_nibbles;
			uint size_bytes;
			
			mbl.input_end = (br.ReadBits(1)==1);
			
			if (mbl.input_end && br.ReadBits(1)!=0) {
				return mbl;
			}

			size_nibbles = br.ReadBits(2) + 4;

			if (size_nibbles == 7) {
				mbl.is_metadata = true;

				if (br.ReadBits(1) != 0){
					throw new Exception("Invalid reserved bit");
				}

				size_bytes = br.ReadBits(2);
				if (size_bytes == 0){
					return mbl;
				}

				for (int i = 0; i < size_bytes; i++) {
					uint next_byte = br.ReadBits(8);
					if (i + 1 == size_bytes && size_bytes > 1 && next_byte == 0){
						throw new Exception("Invalid size byte");
					}

					mbl.meta_block_length |= (int)next_byte << (i * 8);
				}
			} else {
				for (int i = 0; i < size_nibbles; i++) {
					uint next_nibble = br.ReadBits(4);
					if (i + 1 == size_nibbles && size_nibbles > 4 && next_nibble == 0){
						throw new Exception("Invalid size nibble");
					}
					
					mbl.meta_block_length |= (int)next_nibble << (i * 4);
				}
			}

			mbl.meta_block_length++;

			if (!mbl.input_end && !mbl.is_metadata) {
				mbl.is_uncompressed = br.ReadBits(1)==1;
			}
			
			return mbl;
		}
		
		/* Decodes the next Huffman code from bit-stream. */
		public static ushort ReadSymbol(HuffmanCode[] table,int index,BitReader br) {
			// int start_index = index;

			int nbits;
			br.FillBitWindow();
			index += (int)( (br.val_ >> br.bit_pos_) & HUFFMAN_TABLE_MASK );
			nbits = table[index].Bits - HUFFMAN_TABLE_BITS;
			
			if (nbits > 0) {
				br.bit_pos_ += HUFFMAN_TABLE_BITS;
				index += table[index].Value;
				index += (int)( (br.val_ >> br.bit_pos_) & ((1 << nbits) - 1) );
			}
			
			br.bit_pos_ += table[index].Bits;
			return table[index].Value;
			
		}
		
		public static void ReadHuffmanCodeLengths(byte[] code_length_code_lengths,int num_symbols,byte[] code_lengths,BitReader br) {
			
			int symbol = 0;
			int prev_code_len = kDefaultCodeLength;
			int repeat = 0;
			int repeat_code_len = 0;
			int space = 32768;

			HuffmanCode[] table = new HuffmanCode[32];

			for (int i = 0; i < 32; i++){
				table[i]=new HuffmanCode(0, 0);
			}

			Huffman.BuildHuffmanTable(table, 0, 5, code_length_code_lengths, CODE_LENGTH_CODES);

			while (symbol < num_symbols && space > 0) {
				int p = 0;
				byte code_len;

				br.ReadMoreInput();
				br.FillBitWindow();
				p += (int)( (br.val_ >> br.bit_pos_) & 31 );
				br.bit_pos_ += table[p].Bits;
				code_len = (byte)table[p].Value;
				
				if (code_len < kCodeLengthRepeatCode) {
					repeat = 0;
					
					code_lengths[symbol++] = (byte)code_len;
					
					if (code_len != 0) {
						prev_code_len = code_len;
						space -= 32768 >> code_len;
					}
					
				} else {
					int extra_bits = code_len - 14;
					int old_repeat;
					int repeat_delta;
					int new_len = 0;
					
					if (code_len == kCodeLengthRepeatCode) {
						new_len = prev_code_len;
					}
					
					if (repeat_code_len != new_len) {
						repeat = 0;
						repeat_code_len = new_len;
					}
					
					old_repeat = repeat;
					
					if (repeat > 0) {
						repeat -= 2;
						repeat <<= extra_bits;
					}
					
					repeat += (int)br.ReadBits(extra_bits) + 3;
					repeat_delta = repeat - old_repeat;
					if (symbol + repeat_delta > num_symbols) {
						throw new Exception("[ReadHuffmanCodeLengths] symbol + repeat_delta > num_symbols");
					}

					for (int x = 0; x < repeat_delta; x++){
						code_lengths[symbol + x] = (byte)repeat_code_len;
					}

					symbol += repeat_delta;

					if (repeat_code_len != 0) {
						space -= repeat_delta << (15 - repeat_code_len);
					}
				}
			}
			
			if (space != 0) {
				throw new Exception("[ReadHuffmanCodeLengths] space = " + space);
			}

			for (; symbol < num_symbols; symbol++){
				code_lengths[symbol] = 0;
			}
			
		}
		
		public static int ReadHuffmanCode(int alphabet_size,HuffmanCode[] tables,int table,BitReader br) {
			int table_size = 0;
			byte simple_code_or_skip;
			byte[] code_lengths = new byte[alphabet_size];

			br.ReadMoreInput();
			
			/* simple_code_or_skip is used as follows:
			1 for simple code;
			0 for no skipping, 2 skips 2 code lengths, 3 skips 3 code lengths */
			simple_code_or_skip = (byte)br.ReadBits(2);
			
			if (simple_code_or_skip == 1) {
				
				/* Read symbols, codes & code lengths directly. */
				int max_bits_counter = alphabet_size - 1;
				int max_bits = 0;
				int[] symbols = new int[4];
				int num_symbols = (int)br.ReadBits(2) + 1;
				
				while (max_bits_counter>0) {
					max_bits_counter >>= 1;
					max_bits++;
				}
				
				for (int i = 0; i < num_symbols; i++) {
					symbols[i] = (int)br.ReadBits(max_bits) % alphabet_size;
					code_lengths[symbols[i]] = 2;
				}
				
				code_lengths[symbols[0]] = 1;
				
				switch (num_symbols) {
					case 1:
					break;
					case 3:
						if ((symbols[0] == symbols[1]) ||
							(symbols[0] == symbols[2]) ||
							(symbols[1] == symbols[2])) {
							throw new Exception("[ReadHuffmanCode] invalid symbols");
						}
					break;
					case 2:
						if (symbols[0] == symbols[1]) {
							throw new Exception("[ReadHuffmanCode] invalid symbols");
						}

						code_lengths[symbols[1]] = 1;
					break;
					case 4:
						if ((symbols[0] == symbols[1]) ||
							(symbols[0] == symbols[2]) ||
							(symbols[0] == symbols[3]) ||
							(symbols[1] == symbols[2]) ||
							(symbols[1] == symbols[3]) ||
							(symbols[2] == symbols[3])) {
								throw new Exception("[ReadHuffmanCode] invalid symbols");
						}

						if (br.ReadBits(1)==1) {
							code_lengths[symbols[2]] = 3;
							code_lengths[symbols[3]] = 3;
						} else {
							code_lengths[symbols[0]] = 2;
						}
					break;
				}
				
			} else {  /* Decode Huffman-coded code lengths. */
				
				byte[] code_length_code_lengths = new byte[CODE_LENGTH_CODES];
				int space = 32;
				int num_codes = 0;
				
				for (int i = simple_code_or_skip; i < CODE_LENGTH_CODES && space > 0; i++) {
					
					int code_len_idx = kCodeLengthCodeOrder[i];
					int p = 0;
					ushort v;
					br.FillBitWindow();
					p += (int)(br.val_ >> br.bit_pos_) & 15;
					HuffmanCode huff=Huffman.CodeLengthCodeLengths[p];
					br.bit_pos_ += huff.Bits;
					v = huff.Value;
					code_length_code_lengths[code_len_idx] = (byte)v;
					
					if (v != 0) {
						space -= (32 >> v);
						++num_codes;
					}
					
				}
				
				if (!(num_codes == 1 || space == 0)){
					throw new Exception("[ReadHuffmanCode] invalid num_codes or space");
				}
				
				ReadHuffmanCodeLengths(code_length_code_lengths, alphabet_size, code_lengths, br);
			}
			
			table_size = Huffman.BuildHuffmanTable(tables, table, HUFFMAN_TABLE_BITS, code_lengths, alphabet_size);
			
			if (table_size == 0) {
				throw new Exception("[ReadHuffmanCode] Huffman.BuildHuffmanTable failed: ");
			}
			
			return table_size;
		}
		
		internal static int ReadBlockLength(HuffmanCode[] table,int index,BitReader br) {
			int code;
			int nbits;
			code = ReadSymbol(table, index, br);
			nbits = Prefix.kBlockLengthPrefixCode[code].NBits;
			return Prefix.kBlockLengthPrefixCode[code].Offset + (int)br.ReadBits(nbits);
		}
		
		internal static int TranslateShortCodes(int code,byte[] ringbuffer,int index) {
			int val;
			
			if (code < NUM_DISTANCE_SHORT_CODES) {
				index += kDistanceShortCodeIndexOffset[code];
				index &= 3;
				val = ringbuffer[index] + kDistanceShortCodeValueOffset[code];
			} else {
				val = code - NUM_DISTANCE_SHORT_CODES + 1;
			}
			
			return val;
		}
		
		internal static void MoveToFront(byte[] v,int index){
			byte value = v[index];
			
			for (int i=index; i>0; i--){
				v[i] = v[i - 1];
			}
			
			v[0] = value;
		}
		
		internal static void InverseMoveToFrontTransform(byte[] v,int v_len) {
			byte[] mtf = new byte[256];
			
			for (int i = 0; i < 256; ++i) {
				mtf[i] = (byte)i;
			}
			
			for (int i = 0; i < v_len; ++i) {
				byte index = v[i];
				v[i] = mtf[index];
				
				if (index!=0){
					MoveToFront(mtf, index);
				}
				
			}
			
		}
		
		internal static ContextMap DecodeContextMap(int context_map_size,BitReader br) {
			
			ContextMap ctx = new ContextMap();
			bool use_rle_for_zeros;
			int max_run_length_prefix = 0;
			
			br.ReadMoreInput();
			int num_htrees = ctx.num_htrees = DecodeVarLenUint8(br) + 1;
			
			byte[] context_map = ctx.context_map = new byte[context_map_size];
			
			if (num_htrees <= 1) {
				return ctx;
			}

			use_rle_for_zeros = br.ReadBits(1)==1;
			
			if (use_rle_for_zeros) {
				max_run_length_prefix = (int)br.ReadBits(4) + 1;
			}

			HuffmanCode[] table = new HuffmanCode[HUFFMAN_MAX_TABLE_SIZE];
			
			for (int i = 0; i < HUFFMAN_MAX_TABLE_SIZE; i++) {
				table[i] = new HuffmanCode(0, 0);
			}

			ReadHuffmanCode(num_htrees + max_run_length_prefix, table, 0, br);

			for (int i = 0; i < context_map_size;) {
				
				br.ReadMoreInput();
				int code = ReadSymbol(table, 0, br);
				
				if (code == 0) {
					context_map[i] = 0;
					i++;
				} else if (code <= max_run_length_prefix) {
					
					int reps = 1 + (1 << code) + (int)br.ReadBits(code);
					
					while ((--reps)>0) {
						
						if (i >= context_map_size) {
							throw new Exception("[DecodeContextMap] i >= context_map_size");
						}
						
						context_map[i] = 0;
						i++;
					}
					
				} else {
					context_map[i] = (byte)( code - max_run_length_prefix );
					i++;
				}
			}
			
			if (br.ReadBits(1)==1) {
				InverseMoveToFrontTransform(context_map, context_map_size);
			}

			return ctx;
		}

		internal static void DecodeBlockType(int max_block_type,HuffmanCode[] trees,int tree_type,byte[] block_types,byte[] ringbuffers,byte[] indexes,BitReader br) {
			
			int ringbuffer = tree_type * 2;
			int index = tree_type;
			int type_code = ReadSymbol(trees, tree_type * HUFFMAN_MAX_TABLE_SIZE, br);
			int block_type;
			
			if (type_code == 0) {
				block_type = ringbuffers[ringbuffer + (indexes[index] & 1)];
			} else if (type_code == 1) {
				block_type = ringbuffers[ringbuffer + ((indexes[index] - 1) & 1)] + 1;
			} else {
				block_type = type_code - 2;
			}
			
			if (block_type >= max_block_type) {
				block_type -= max_block_type;
			}
			
			block_types[tree_type] = (byte)block_type;
			ringbuffers[ringbuffer + (indexes[index] & 1)] = (byte)block_type;
			indexes[index]++;
			
		}
		
		internal static void CopyUncompressedBlockToOutput(OutputStream output,int len,int pos,byte[] ringbuffer,int ringbuffer_mask,BitReader br) {
			int rb_size = ringbuffer_mask + 1;
			int rb_pos = pos & ringbuffer_mask;
			int br_pos = br.pos_ & BitReader.IBUF_MASK;
			int nbytes;
		
			/* For short lengths copy byte-by-byte */
			if (len < 8 || br.bit_pos_ + (len << 3) < br.bit_end_pos_) {
				
				while (len-- > 0) {
					br.ReadMoreInput();
					ringbuffer[rb_pos++] = (byte)br.ReadBits(8);
					
					if (rb_pos == rb_size) {
						output.Write(ringbuffer, rb_size);
						rb_pos = 0;
					}
					
				}
				
				return;
			}
			
			if (br.bit_end_pos_ < 32) {
				throw new Exception("[CopyUncompressedBlockToOutput] br.bit_end_pos_ < 32");
			}

			/* Copy remaining 0-4 bytes from br.val_ to ringbuffer. */
			while (br.bit_pos_ < 32) {
				ringbuffer[rb_pos] = (byte)(br.val_ >> br.bit_pos_);
				br.bit_pos_ += 8;
				++rb_pos;
				--len;
			}
		
			/* Copy remaining bytes from br.buf_ to ringbuffer. */
			nbytes = (br.bit_end_pos_ - br.bit_pos_) >> 3;
			
			if (br_pos + nbytes > BitReader.IBUF_MASK) {
				int tail = BitReader.IBUF_MASK + 1 - br_pos;
				
				System.Array.Copy(br.buf_,br_pos,ringbuffer,rb_pos,tail);
				
				nbytes -= tail;
				rb_pos += tail;
				len -= tail;
				br_pos = 0;
			}
			
			System.Array.Copy(br.buf_,br_pos,ringbuffer,rb_pos,nbytes);
			
			rb_pos += nbytes;
			len -= nbytes;
			
			/* If we wrote past the logical end of the ringbuffer, copy the tail of the
			ringbuffer to its beginning and flush the ringbuffer to the output. */
			if (rb_pos >= rb_size) {
				output.Write(ringbuffer, rb_size);
				rb_pos -= rb_size;    
				
				System.Array.Copy(ringbuffer,rb_size,ringbuffer,0,rb_pos);
				
			}

			/* If we have more to copy than the remaining size of the ringbuffer, then we
			first fill the ringbuffer from the input and then flush the ringbuffer to
			the output */
			while (rb_pos + len >= rb_size) {
				nbytes = rb_size - rb_pos;
				if (br.input_.Read(ringbuffer, rb_pos, nbytes) < nbytes) {
					throw new Exception("[CopyUncompressedBlockToOutput] not enough bytes");
				}
				
				output.Write(ringbuffer, rb_size);
				len -= nbytes;
				rb_pos = 0;
			}

			/* Copy straight from the input onto the ringbuffer. The ringbuffer will be
			flushed to the output at a later time. */
			if (br.input_.Read(ringbuffer, rb_pos, len) < len) {
				throw new Exception("[CopyUncompressedBlockToOutput] not enough bytes");
			}

			/* Restore the state of the bit reader. */
			br.Reset();
		}
		
		/* Advances the bit reader position to the next byte boundary and verifies
		   that any skipped bits are set to zero. */
		public static bool JumpToByteBoundary(BitReader br) {
			int new_bit_pos = (br.bit_pos_ + 7) & ~7;
			uint pad_bits = br.ReadBits(new_bit_pos - br.bit_pos_);
			return pad_bits == 0;
		}
		
		public static int BrotliDecompressedSize(Stream buffer) {
			
			BitReader br = new BitReader(buffer);
			DecodeWindowBits(br);
			MetaBlockLength o = DecodeMetaBlockLength(br);
			
			return o.meta_block_length;
		}
		
		internal static void BrotliDecompress(Stream input,OutputStream output) {
			int i;
			int pos = 0;
			bool input_end = false;
			int window_bits = 0;
			int max_backward_distance;
			int max_distance = 0;
			int ringbuffer_size;
			int ringbuffer_mask;
			byte[] ringbuffer;
			int ringbuffer_end;
			/* This ring buffer holds a few past copy distances that will be used by */
			/* some special distance codes. */
			byte[] dist_rb = new byte[]{ 16, 15, 11, 4 };
			int dist_rb_idx = 0;
			/* The previous 2 bytes used for context. */
			byte prev_byte1 = 0;
			byte prev_byte2 = 0;
			
			HuffmanTreeGroup[] hgroup = new HuffmanTreeGroup[]{
				new HuffmanTreeGroup(0, 0), new HuffmanTreeGroup(0, 0), new HuffmanTreeGroup(0, 0)
			};
			
			/* We need the slack region for the following reasons:
					 - always doing two 8-byte copies for fast backward copying
					 - transforms
					 - flushing the input ringbuffer when decoding uncompressed blocks */
			const int kRingBufferWriteAheadSlack = 128 + BitReader.READ_SIZE;
			
			BitReader br = new BitReader(input);

			/* Decode window size. */
			window_bits = DecodeWindowBits(br);
			max_backward_distance = (1 << window_bits) - 16;
			
			ringbuffer_size = 1 << window_bits;
			ringbuffer_mask = ringbuffer_size - 1;
			ringbuffer = new byte[ringbuffer_size + kRingBufferWriteAheadSlack + Dictionary.MaxDictionaryWordLength];
			ringbuffer_end = ringbuffer_size;

			HuffmanCode[] block_type_trees = new HuffmanCode[3 * HUFFMAN_MAX_TABLE_SIZE];
			HuffmanCode[] block_len_trees = new HuffmanCode[3 * HUFFMAN_MAX_TABLE_SIZE];
			
			for (int x = 0; x < 3 * HUFFMAN_MAX_TABLE_SIZE; x++) {
				block_type_trees[x] = new HuffmanCode(0, 0);
				block_len_trees[x] = new HuffmanCode(0, 0);
			}

			while (!input_end) {
				int meta_block_remaining_len = 0;
				bool is_uncompressed;
				int[] block_length = new int[]{ 1 << 28, 1 << 28, 1 << 28 };
				byte[] block_type = new byte[]{ 0 };
				byte[] num_block_types = new byte[]{ 1, 1, 1 };
				byte[] block_type_rb = new byte[]{ 0, 1, 0, 1, 0, 1 };
				byte[] block_type_rb_index = new byte[]{ 0 };
				int distance_postfix_bits;
				int num_direct_distance_codes;
				int distance_postfix_mask;
				int num_distance_codes;
				byte[] context_map = null;
				byte[] context_modes = null;
				int num_literal_htrees;
				byte[] dist_context_map = null;
				int num_dist_htrees;
				int context_offset = 0;
				int context_map_slice = 0;
				int literal_htree_index = 0;
				int dist_context_offset = 0;
				int dist_context_map_slice = 0;
				int dist_htree_index = 0;
				int context_lookup_offset1 = 0;
				int context_lookup_offset2 = 0;
				byte context_mode;
				uint htree_command;
				
				for (i = 0; i < 3; ++i) {
					hgroup[i].codes = null;
					hgroup[i].htrees = null;
				}

				br.ReadMoreInput();
				
				MetaBlockLength _out = DecodeMetaBlockLength(br);
				meta_block_remaining_len = _out.meta_block_length;
				
				if (pos + meta_block_remaining_len > output.buffer.Length) {
					
					if(output.FixedSize){
						throw new Exception("Brotli decompressor: The provided buffer to decompress into was too small. Brotli is used by WOFF2 files.");
					}
					
					/* We need to grow the output buffer to fit the additional data. */
					byte[] tmp = new byte[ pos + meta_block_remaining_len ];
					
					System.Array.Copy(output.buffer,0,tmp,0,output.pos_);
					
					output.buffer = tmp;
				}
				
				input_end = _out.input_end;
				is_uncompressed = _out.is_uncompressed;
				
				if (_out.is_metadata) {
					JumpToByteBoundary(br);
					
					for (; meta_block_remaining_len > 0; --meta_block_remaining_len) {
						br.ReadMoreInput();
						/* Read one byte and ignore it. */
						br.ReadBits(8);
					}
					
					continue;
				}
				
				if (meta_block_remaining_len == 0) {
					continue;
				}
				
				if (is_uncompressed) {
					br.bit_pos_ = (br.bit_pos_ + 7) & ~7;
					CopyUncompressedBlockToOutput(output, meta_block_remaining_len, pos,
																				ringbuffer, ringbuffer_mask, br);
					pos += meta_block_remaining_len;
					continue;
				}
				
				for (i = 0; i < 3; ++i) {
					num_block_types[i] = (byte)( DecodeVarLenUint8(br) + 1 );
					if (num_block_types[i] >= 2) {
						ReadHuffmanCode(num_block_types[i] + 2, block_type_trees, i * HUFFMAN_MAX_TABLE_SIZE, br);
						ReadHuffmanCode(kNumBlockLengthCodes, block_len_trees, i * HUFFMAN_MAX_TABLE_SIZE, br);
						block_length[i] = ReadBlockLength(block_len_trees, i * HUFFMAN_MAX_TABLE_SIZE, br);
						block_type_rb_index[i] = 1;
					}
				}
				
				br.ReadMoreInput();
				
				distance_postfix_bits = (int)br.ReadBits(2);
				num_direct_distance_codes = NUM_DISTANCE_SHORT_CODES + ((int)br.ReadBits(4) << distance_postfix_bits);
				distance_postfix_mask = (1 << distance_postfix_bits) - 1;
				num_distance_codes = (num_direct_distance_codes + (48 << distance_postfix_bits));
				context_modes = new byte[num_block_types[0]];

				for (i = 0; i < num_block_types[0]; ++i) {
					 br.ReadMoreInput();
					 context_modes[i] = (byte)(br.ReadBits(2) << 1);
				}
				
				ContextMap _o1 = DecodeContextMap(num_block_types[0] << kLiteralContextBits, br);
				num_literal_htrees = _o1.num_htrees;
				context_map = _o1.context_map;
				
				ContextMap _o2 = DecodeContextMap(num_block_types[2] << kDistanceContextBits, br);
				num_dist_htrees = _o2.num_htrees;
				dist_context_map = _o2.context_map;
				
				hgroup[0] = new HuffmanTreeGroup(kNumLiteralCodes, num_literal_htrees);
				hgroup[1] = new HuffmanTreeGroup(kNumInsertAndCopyCodes, num_block_types[1]);
				hgroup[2] = new HuffmanTreeGroup(num_distance_codes, num_dist_htrees);

				for (i = 0; i < 3; ++i) {
					hgroup[i].Decode(br);
				}

				context_map_slice = 0;
				dist_context_map_slice = 0;
				context_mode = context_modes[block_type[0]];
				context_lookup_offset1 = Context.LookupOffsets[context_mode];
				context_lookup_offset2 = Context.LookupOffsets[context_mode + 1];
				htree_command = hgroup[1].htrees[0];

				while (meta_block_remaining_len > 0) {
					int cmd_code;
					int range_idx;
					int insert_code;
					int copy_code;
					int insert_length;
					int copy_length;
					int distance_code;
					int distance;
					byte context;
					int j;
					int copy_dst;

					br.ReadMoreInput();
					
					if (block_length[1] == 0) {
						DecodeBlockType(num_block_types[1],
														block_type_trees, 1, block_type, block_type_rb,
														block_type_rb_index, br);
						block_length[1] = ReadBlockLength(block_len_trees, HUFFMAN_MAX_TABLE_SIZE, br);
						htree_command = hgroup[1].htrees[block_type[1]];
					}
					
					--block_length[1];
					cmd_code = ReadSymbol(hgroup[1].codes, (int)htree_command, br);
					range_idx = cmd_code >> 6;
					
					if (range_idx >= 2) {
						range_idx -= 2;
						distance_code = -1;
					} else {
						distance_code = 0;
					}
					
					insert_code = Prefix.kInsertRangeLut[range_idx] + ((cmd_code >> 3) & 7);
					
					copy_code = Prefix.kCopyRangeLut[range_idx] + (cmd_code & 7);
					
					insert_length = Prefix.kInsertLengthPrefixCode[insert_code].Offset +
							(int)br.ReadBits(Prefix.kInsertLengthPrefixCode[insert_code].NBits);
							
					copy_length = Prefix.kCopyLengthPrefixCode[copy_code].Offset +
							(int)br.ReadBits(Prefix.kCopyLengthPrefixCode[copy_code].NBits);
							
					prev_byte1 = ringbuffer[pos-1 & ringbuffer_mask];
					prev_byte2 = ringbuffer[pos-2 & ringbuffer_mask];
					
					for (j = 0; j < insert_length; ++j) {
						br.ReadMoreInput();

						if (block_length[0] == 0) {
							DecodeBlockType(num_block_types[0],
															block_type_trees, 0, block_type, block_type_rb,
															block_type_rb_index, br);
							block_length[0] = ReadBlockLength(block_len_trees, 0, br);
							context_offset = block_type[0] << kLiteralContextBits;
							context_map_slice = context_offset;
							context_mode = context_modes[block_type[0]];
							context_lookup_offset1 = Context.LookupOffsets[context_mode];
							context_lookup_offset2 = Context.LookupOffsets[context_mode + 1];
						}
						
						context = Context.Lookup[context_lookup_offset1 + prev_byte1];
						
						if(context==0){
							context=Context.Lookup[context_lookup_offset2 + prev_byte2];
						}
						
						literal_htree_index = context_map[context_map_slice + context];
						--block_length[0];
						
						prev_byte2 = prev_byte1;
						prev_byte1 = (byte)ReadSymbol(hgroup[0].codes, (int)hgroup[0].htrees[literal_htree_index], br);
						ringbuffer[pos & ringbuffer_mask] = prev_byte1;
						
						if ((pos & ringbuffer_mask) == ringbuffer_mask) {
							output.Write(ringbuffer, ringbuffer_size);
						}
						
						++pos;
					}
					
					meta_block_remaining_len -= insert_length;
					
					if (meta_block_remaining_len <= 0){
						break;
					}
					
					if (distance_code < 0) {
						context=0;
						
						br.ReadMoreInput();
						
						if (block_length[2] == 0) {
							DecodeBlockType(num_block_types[2],
															block_type_trees, 2, block_type, block_type_rb,
															block_type_rb_index, br);
							block_length[2] = ReadBlockLength(block_len_trees, 2 * HUFFMAN_MAX_TABLE_SIZE, br);
							dist_context_offset = block_type[2] << kDistanceContextBits;
							dist_context_map_slice = dist_context_offset;
						}
						
						--block_length[2];
						
						context = (byte)(copy_length > 4 ? 3 : copy_length - 2);
						dist_htree_index = dist_context_map[dist_context_map_slice + context];
						distance_code = ReadSymbol(hgroup[2].codes, (int)hgroup[2].htrees[dist_htree_index], br);
						
						if (distance_code >= num_direct_distance_codes) {
							int nbits;
							int postfix;
							int offset;
							distance_code -= num_direct_distance_codes;
							postfix = distance_code & distance_postfix_mask;
							distance_code >>= distance_postfix_bits;
							nbits = (distance_code >> 1) + 1;
							offset = ((2 + (distance_code & 1)) << nbits) - 4;
							distance_code = num_direct_distance_codes +
									((offset + (int)br.ReadBits(nbits)) <<
									 distance_postfix_bits) + postfix;
						}
						
					}

					/* Convert the distance code to the actual distance by possibly looking */
					/* up past distnaces from the ringbuffer. */
					distance = TranslateShortCodes(distance_code, dist_rb, dist_rb_idx);
					
					if (distance < 0) {
						throw new Exception("[BrotliDecompress] invalid distance");
					}
					
					if (pos < max_backward_distance &&
							max_distance != max_backward_distance) {
						max_distance = pos;
					} else {
						max_distance = max_backward_distance;
					}

					copy_dst = pos & ringbuffer_mask;

					if (distance > max_distance) {
						
						if (copy_length >= Dictionary.MinDictionaryWordLength &&
								copy_length <= Dictionary.MaxDictionaryWordLength) {
							int offset = (int)Dictionary.OffsetsByLength[copy_length];
							int word_id = distance - max_distance - 1;
							int shift = Dictionary.SizeBitsByLength[copy_length];
							int mask = (1 << shift) - 1;
							int word_idx = word_id & mask;
							int transform_idx = word_id >> shift;
							offset += word_idx * copy_length;
							
							if (transform_idx < Transforms.kNumTransforms) {
								int len = Transforms.TransformDictionaryWord(ringbuffer, copy_dst, offset, copy_length, transform_idx);
								copy_dst += len;
								pos += len;
								meta_block_remaining_len -= len;
								if (copy_dst >= ringbuffer_end) {
									output.Write(ringbuffer, ringbuffer_size);
									
									for (int _x = 0; _x < (copy_dst - ringbuffer_end); _x++){
										ringbuffer[_x] = ringbuffer[ringbuffer_end + _x];
									}
									
								}
								
							} else {
								throw new Exception("Invalid backward reference. pos: " + pos + " distance: " + distance +
									" len: " + copy_length + " bytes left: " + meta_block_remaining_len);
							}
							
						} else {
							throw new Exception("Invalid backward reference. pos: " + pos + " distance: " + distance +
								" len: " + copy_length + " bytes left: " + meta_block_remaining_len);
						}
						
					} else {
						if (distance_code > 0) {
							dist_rb[dist_rb_idx & 3] = (byte)distance;
							++dist_rb_idx;
						}
						
						if (copy_length > meta_block_remaining_len) {
							throw new Exception("Invalid backward reference. pos: " + pos + " distance: " + distance +
								" len: " + copy_length + " bytes left: " + meta_block_remaining_len);
						}

						for (j = 0; j < copy_length; ++j) {
						
							ringbuffer[pos & ringbuffer_mask] = ringbuffer[(pos - distance) & ringbuffer_mask];
							
							if ((pos & ringbuffer_mask) == ringbuffer_mask) {
								output.Write(ringbuffer, ringbuffer_size);
							}
							
							++pos;
							--meta_block_remaining_len;
						}
					}

					/* When we get here, we must have inserted at least one literal and */
					/* made a copy of at least length two, therefore accessing the last 2 */
					/* bytes is valid. */
					prev_byte1 = ringbuffer[(pos - 1) & ringbuffer_mask];
					prev_byte2 = ringbuffer[(pos - 2) & ringbuffer_mask];
				}

				/* Protect pos from overflow, wrap it around at every GB of input data */
				pos &= 0x3fffffff;
			}
			
			output.Write(ringbuffer, pos & ringbuffer_mask);
		}
	
	}
	
}