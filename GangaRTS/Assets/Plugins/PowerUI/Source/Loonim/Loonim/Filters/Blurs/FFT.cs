//--------------------------------------
//       Loonim Image Generator
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

// FFT Ported from Java by Kulestar.
// Original Java source copyright 2005 Huxtable.com. 
// All rights reserved.

using System;


namespace Loonim{

	/// <summary>
	/// Performs the fast fourier transform.
	/// </summary>
	
	public class FFT{
		
		// Weighting factors
		protected float[] w1;
		protected float[] w2;
		protected float[] w3;
		
		
		public FFT( int logN ) {
			// Prepare the weighting factors
			w1 = new float[logN];
			w2 = new float[logN];
			w3 = new float[logN];
			int N = 1;
			for ( int k = 0; k < logN; k++ ) {
				N <<= 1;
				double angle = -2.0 * System.Math.PI / N;
				w1[k] = (float)System.Math.Sin(0.5 * angle);
				w2[k] = -2.0f * w1[k] * w1[k];
				w3[k] = (float)System.Math.Sin(angle);
			}
		}

		private void Scramble( int n, float[] real, float[] imag ) {
			int j = 0;

			for ( int i = 0; i < n; i++ ) {
				if ( i > j ) {
					float t;
					t = real[j];
					real[j] = real[i];
					real[i] = t;
					t = imag[j];
					imag[j] = imag[i];
					imag[i] = t;
				}
				int m = n >> 1;
				while (j >= m && m >= 2) {
					j -= m;
					m >>= 1;
				}
				j += m;
			}
		}

		private void Butterflies( int n, int logN, int direction, float[] real, float[] imag ) {
			int N = 1;

			for ( int k = 0; k < logN; k++ ) {
				float w_re, w_im, wp_re, wp_im, temp_re, temp_im, wt;
				int half_N = N;
				N <<= 1;
				wt = direction * w1[k];
				wp_re = w2[k];
				wp_im = direction * w3[k];
				w_re = 1.0f;
				w_im = 0.0f;
				for ( int offset = 0; offset < half_N; offset++ ) {
					for( int i = offset; i < n; i += N ) {
						int j = i + half_N;
						float re = real[j];
						float im = imag[j];
						temp_re = (w_re * re) - (w_im * im);
						temp_im = (w_im * re) + (w_re * im);
						real[j] = real[i] - temp_re;
						real[i] += temp_re;
						imag[j] = imag[i] - temp_im;
						imag[i] += temp_im;
					}
					wt = w_re;
					w_re = wt * wp_re - w_im * wp_im + w_re;
					w_im = w_im * wp_re + wt * wp_im + w_im;
				}
			}
			if ( direction == -1 ) {
				float nr = 1.0f / n;
				for ( int i = 0; i < n; i++ ) {
					real[i] *= nr;
					imag[i] *= nr;
				}
			}
		}

		public void Transform1D( float[] real, float[] imag, int logN, int n, bool forward ) {
			Scramble( n, real, imag );
			Butterflies( n, logN, forward ? 1 : -1, real, imag );
		}

		public void Transform2D( float[] real, float[] imag, int cols, int rows, bool forward ) {
			int Log2cols = Log2(cols);
			int Log2rows = Log2(rows);
			int n = System.Math.Max(rows, cols);
			float[] rtemp = new float[n];
			float[] itemp = new float[n];

			// FFT the rows
			for ( int y = 0; y < rows; y++ ) {
				int offset = y*cols;
				Array.Copy(real, offset, rtemp, 0, cols );
				Array.Copy( imag, offset, itemp, 0, cols );
				Transform1D(rtemp, itemp, Log2cols, cols, forward);
				Array.Copy( rtemp, 0, real, offset, cols );
				Array.Copy( itemp, 0, imag, offset, cols );
			}

			// FFT the columns
			for ( int x = 0; x < cols; x++ ) {
				int index = x;
				for ( int y = 0; y < rows; y++ ) {
					rtemp[y] = real[index];
					itemp[y] = imag[index];
					index += cols;
				}
				Transform1D(rtemp, itemp, Log2rows, rows, forward);
				index = x;
				for ( int y = 0; y < rows; y++ ) {
					real[index] = rtemp[y];
					imag[index] = itemp[y];
					index += cols;
				}
			}
		}

		private int Log2( int n ) {
			int m = 1;
			int log2n = 0;

			while (m < n) {
				m *= 2;
				log2n++;
			}
			return m == n ? log2n : -1;
		}

	}

}