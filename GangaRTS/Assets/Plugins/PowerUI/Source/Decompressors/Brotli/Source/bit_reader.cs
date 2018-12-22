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
	
	/// <summary>
	/// Bit reading helpers.
	/// </summary>
	
	public class BitReader{
		
		public const int READ_SIZE = 4096;
		public const int IBUF_SIZE =  (2 * READ_SIZE + 32);
		public const int IBUF_MASK =  (2 * READ_SIZE - 1);
		
		public static readonly uint[] kBitMask = new uint[]{
			0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767,
			65535, 131071, 262143, 524287, 1048575, 2097151, 4194303, 8388607, 16777215
		};
		
		public byte[] buf_;
		public Stream input_;
		public int buf_ptr_;
		public uint val_;
		public int pos_;
		public int bit_pos_;
		public int bit_end_pos_;
		public bool eos_;
		
		/* Input byte buffer, consist of a ringbuffer and a "slack" region where */
		/* bytes from the start of the ringbuffer are copied. */
		public BitReader(Stream input) {
			buf_ = new byte[IBUF_SIZE];
			input_ = input;    /* input callback */

			Reset();
		}

		public bool Reset() {
			buf_ptr_ = 0;      /* next input will write here */
			val_ = 0;          /* pre-fetched bits */
			pos_ = 0;          /* byte position in stream */
			bit_pos_ = 0;      /* current bit-reading position in val_ */
			bit_end_pos_ = 0;  /* bit-reading end position from LSB of val_ */
			eos_ = false;          /* input stream is finished */

			ReadMoreInput();
			
			for (int i = 0; i < 4; i++) {
				val_ |= (uint)buf_[pos_] << (8 * i);
				pos_++;
			}
			
			return bit_end_pos_ > 0;
		}
		
		/* Fills up the input ringbuffer by calling the input callback.
		   Does nothing if there are at least 32 bytes present after current position.
		   Returns 0 if either:
			- the input callback returned an error, or
			- there is no more input and the position is past the end of the stream.
		   After encountering the end of the input stream, 32 additional zero bytes are
		   copied to the ringbuffer, therefore it is safe to call this function after
		   every 32 bytes of input is read.
		*/
		internal void ReadMoreInput() {
			
			if (bit_end_pos_ > 256) {
				return;
			} else if (eos_) {
				
				if (bit_pos_ > bit_end_pos_){
					throw new Exception("Unexpected end of input " + bit_pos_ + " " + bit_end_pos_);
				}
				
			} else {
				int dst = buf_ptr_;
				int bytes_read = input_.Read(buf_, dst, READ_SIZE);
				if (bytes_read < 0) {
					throw new Exception("Unexpected end of input");
				}
				
				if (bytes_read < READ_SIZE) {
					eos_ = true;
					
					/* Store 32 bytes of zero after the stream end. */
					for (int p = 0; p < 32; p++){
						buf_[dst + bytes_read + p] = 0;
					}
					
				}

				if (dst == 0) {
					/* Copy the head of the ringbuffer to the slack region. */
					for (int p = 0; p < 32; p++){
						buf_[(READ_SIZE << 1) + p] = buf_[p];
					}
					
					buf_ptr_ = READ_SIZE;
				} else {
					buf_ptr_ = 0;
				}
				
				bit_end_pos_ += bytes_read << 3;
			}
		}
		
		/* Guarantees that there are at least 24 bits in the buffer. */
		internal void FillBitWindow() {   
			
			while (bit_pos_ >= 8) {
				val_ >>= 8;
				val_ |= (uint)buf_[pos_ & IBUF_MASK] << 24;
				++pos_;
				bit_pos_ = bit_pos_ - 8 >> 0;
				bit_end_pos_ = bit_end_pos_ - 8 >> 0;
			}
			
		}

		/* Reads the specified number of bits from Read Buffer. */
		public uint ReadBits(int n_bits){
			if (32 - bit_pos_ < n_bits) {
				FillBitWindow();
			}
			
			uint val = ((val_ >> bit_pos_) & kBitMask[n_bits]);
			bit_pos_ += n_bits;
			return val;
		}
		
	}
	
}