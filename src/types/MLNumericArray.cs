namespace DotNetDoctor.csmatio.types
{
    using System;
    using System.Runtime.InteropServices;

    using DotNetDoctor.csmatio.common;

    /// <summary>
	/// Abstract class for numeric arrays.
	/// </summary>
	/// <author>David Zier (david.zier@gmail.com)</author>
	public abstract class MLNumericArray<T> : MLArray, IGenericArrayCreator<T>, IByteStorageSupport
	{
		private ByteBuffer _real;
		private ByteBuffer _imaginary;
		private byte[] _bytes;

		#region Contructors

		/// <summary>
		/// Constructs an abstract MLNumericArray class object
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Dims">The dimensions of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="Attributes">Any attributes associated with this array.</param>
		protected MLNumericArray(string Name, int[] Dims, int Type, int Attributes)
			: base(Name, Dims, Type, Attributes)
		{
			this._real = new ByteBuffer(this.Size * this.GetBytesAllocated);
			if (this.IsComplex)
				this._imaginary = new ByteBuffer(this.Size * this.GetBytesAllocated);
			this._bytes = new byte[this.GetBytesAllocated];
		}

		/// <summary>
		/// <a href="http://math.nist.gov/javanumerics/jama/">Jama</a> [math.nist.gov] style:
		/// construct a 2D real matrix from a one-dimensional packed array.
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="Vals">One-dimensional array of doubles, packed by columns.</param>
		/// <param name="M">The number of rows.</param>
		protected MLNumericArray(string Name, int Type, T[] Vals, int M)
			: this(Name, new int[] { M, Vals.Length / M }, Type, 0)
		{
			// Fill in the array
			for (int i = 0; i < Vals.Length; i++)
				this.Set(Vals[i], i);
		}

		/// <summary>
		/// <a href="http://math.nist.gov/javanumerics/jama/">Jama</a> [math.nist.gov] style:
		/// construct a 2D imaginary matrix from a one-dimensional packed array.
		/// </summary>
		/// <param name="Name">The name of the numeric array.</param>
		/// <param name="Type">The Matlab Array Class type for this array.</param>
		/// <param name="RealVals">One-dimensional array of doubles for the <i>real</i> part, 
		/// packed by columns</param>
		/// <param name="ImagVals">One-dimensional array of doubles for the <i>imaginary</i> part, 
		/// packed by columns</param>
		/// <param name="M">The number of columns</param>
		protected MLNumericArray(string Name, int Type, T[] RealVals, T[] ImagVals, int M)
			: this(Name, new int[] { M, RealVals.Length / M }, Type, MLArray.mtFLAG_COMPLEX)
		{
			if (ImagVals.Length != RealVals.Length)
				throw new ArgumentException("Attempting to create an imaginary numeric array where the " +
					"imaginary array is _not_ the same size as the real array.");

			// Fill in the imaginary array
			for (int i = 0; i < ImagVals.Length; i++)
			{
				this.SetReal(RealVals[i], i);
				this.SetImaginary(ImagVals[i], i);
			}
		}

		#endregion

		/// <summary>Gets the flags for this array.</summary>
		public override int Flags
		{
			get
			{
				return (int)((uint)(base._type & MLArray.mtFLAG_TYPE)
					| (uint)(base._attributes & 0xFFFFFF00));
			}
		}

		/// <summary>
		/// Gets a single real array element of A(m,n).
		/// </summary>
		/// <param name="M">Row index</param>
		/// <param name="N">Column index</param>
		/// <returns>Array Element</returns>
		public virtual T GetReal(int M, int N)
		{
			return this.GetReal(this.GetIndex(M, N));
		}

		/// <summary>
		/// Gets the <c>ByteBuffer</c> for the Real Numbers.
		/// </summary>
		/// <returns>The real buffer</returns>
		public ByteBuffer GetReal()
		{
			return this._real;
		}

		/// <summary>
		/// Get a single real array element
		/// </summary>
		/// <param name="Index">Column-packed vector index.</param>
		/// <returns>Array Element.</returns>
		public virtual T GetReal(int Index)
		{
			return this._Get(this._real, Index);
		}

		/// <summary>
		/// Sets a single real array element.
		/// </summary>
		/// <param name="Val">The element value.</param>
		/// <param name="M">The row index.</param>
		/// <param name="N">The column index.</param>
		public virtual void SetReal(T Val, int M, int N)
		{
			this.SetReal(Val, this.GetIndex(M, N));
		}

		/// <summary>
		/// Sets a single real array element.
		/// </summary>
		/// <param name="Val">The element value.</param>
		/// <param name="Index">Column-packed vector index.</param>
		public virtual void SetReal(T Val, int Index)
		{
			this._Set(this._real, Val, Index);
		}

		///// <summary>
		///// Sets real part of the matrix.
		///// </summary>
		///// <exception cref="ArgumentException">When the <c>Vector</c> is not the
		///// same length as the Numeric Array.</exception>
		///// <param name="Vector">A column-packed vector of elements.</param>
		//public void SetReal( T[] Vector )
		//{
		//    if( Vector.Length != Size )
		//        throw new ArgumentException("Matrix dimensions do not match. " + Size + " not " + Vector.Length );
		//    //Array.Copy( Vector, 0, _real, 0, Vector.Length );
		//    _real.CopyFrom(Vector);
		//}

		/// <summary>
		/// Sets a single imaginary array element.
		/// </summary>
		/// <param name="Val">Element value.</param>
		/// <param name="M">Row Index.</param>
		/// <param name="N">Column Index.</param>
		public virtual void SetImaginary(T Val, int M, int N)
		{
			if (this.IsComplex)
				this.SetImaginary(Val, this.GetIndex(M, N));
		}

		/// <summary>
		/// Sets a single imaginary array element.
		/// </summary>
		/// <param name="Val">Element Value</param>
		/// <param name="Index">Column-packed vector index.</param>
		public virtual void SetImaginary(T Val, int Index)
		{
			if (this.IsComplex)
				this._Set(this._imaginary, Val, Index);
		}

		/// <summary>
		/// Gets a single imaginary array element of A(m,n)
		/// </summary>
		/// <param name="M">Row index</param>
		/// <param name="N">Column index</param>
		/// <returns>Array element</returns>
		public virtual T GetImaginary(int M, int N)
		{
			return this.GetImaginary(this.GetIndex(M, N));
		}

		/// <summary>
		/// Gets a single imaginary array element.
		/// </summary>
		/// <param name="Index">Column-packed vector index</param>
		/// <returns>Array Element</returns>
		public virtual T GetImaginary(int Index)
		{
			return this._Get(this._imaginary, Index);
		}

		/// <summary>
		/// Gets the <c>ByteBuffer</c> for the Real Numbers.
		/// </summary>
		/// <returns>The real buffer</returns>
		public ByteBuffer GetImaginary()
		{
			if (this.IsComplex)
				return this._imaginary;
			else
				return null;
		}

		/// <summary>
		/// Does the same as <c>SetReal</c>.
		/// </summary>
		/// <param name="Val">Element Value</param>
		/// <param name="M">Row index</param>
		/// <param name="N">Column index</param>
		public void Set(T Val, int M, int N)
		{
			if (this.IsComplex)
				throw new MethodAccessException("Cannot use this method for Complex matrices");
			this.SetReal(Val, M, N);
		}

		/// <summary>
		/// Does the same as <c>SetReal</c>.
		/// </summary>
		/// <param name="Val">Element Value</param>
		/// <param name="Index">Column-packed vector index</param>
		public void Set(T Val, int Index)
		{
			if (this.IsComplex)
				throw new MethodAccessException("Cannot use this method for Complex matrices");
			this.SetReal(Val, Index);
		}
		/// <summary>
		/// Does the same as <c>GetReal</c>.
		/// </summary>
		/// <param name="M">Row index</param>
		/// <param name="N">Column index</param>
		/// <returns>An array element value.</returns>
		public T Get(int M, int N)
		{
			if (this.IsComplex)
				throw new MethodAccessException("Cannot use this method for Complex matrices");
			return this.GetReal(M, N);
		}

		/// <summary>
		/// Does the same as <c>GetReal</c>.
		/// </summary>
		/// <param name="Index">Column-packed vector index</param>
		/// <returns>An array element value.</returns>
		public T Get(int Index)
		{
			if (this.IsComplex)
				throw new MethodAccessException("Cannot use this method for Complex matrices");
			return this.GetReal(Index);
		}

		///// <summary>
		///// Does the same as <c>SetReal</c>
		///// </summary>
		///// <param name="Vector">A column-packed vector of elements.</param>
		//public void Set( T[] Vector )
		//{
		//    if( IsComplex )
		//        throw new MethodAccessException("Cannot use this method for Complex matrices");
		//    SetReal( Vector );
		//}

		private int _GetByteOffset(int Index)
		{
			return Index * this.GetBytesAllocated;
		}

		/// <summary>
		/// Gets a single objects data from a <c>ByteBuffer</c>.
		/// </summary>
		/// <param name="Buffer">The <c>ByteBuffer</c> object.</param>
		/// <param name="Index">A column-packed index.</param>
		/// <returns>The object data.</returns>
		protected virtual T _Get(ByteBuffer Buffer, int Index)
		{
			Buffer.Position(this._GetByteOffset(Index));
			Buffer.Get(ref this._bytes, 0, this._bytes.Length);
			return (T)this.BuildFromBytes(this._bytes);
		}

		/// <summary>
		/// Sets a single object data into a <c>ByteBuffer</c>
		/// </summary>
		/// <param name="Buffer">The <c>ByteBuffer</c> to where the object data will be stored.</param>
		/// <param name="Val">The object data.</param>
		/// <param name="Index">A column-packed index</param>
		protected void _Set(ByteBuffer Buffer, T Val, int Index)
		{
			Buffer.Position(this._GetByteOffset(Index));
			Buffer.Put(this.GetByteArray(Val));
		}

		/// <summary>
		/// Gets a two-dimensional array.
		/// </summary>
		/// <returns>2D array.</returns>
		public T[][] GetArray()
		{
			T[][] result = new T[this.M][];
			for (int m = 0; m < this.M; m++)
			{
				result[m] = new T[this.N];
				for (int n = 0; n < this.N; n++)
				{
					result[m][n] = this.GetReal(m, n);
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the imaginary part of the number array.
		/// </summary>
		public ByteBuffer ImaginaryByteBuffer
		{
			get { return this._imaginary; }
			set
			{
				if (!this.IsComplex)
					throw new MethodAccessException("Array is not complex");
				this._imaginary.Rewind();
				this._imaginary.Put(value);
			}
		}

		/// <summary>
		/// Gets the <c>ByteBuffer</c> for the real numbers in the 
		/// array.
		/// </summary>
		public ByteBuffer RealByteBuffer
		{
			get { return this._real; }
			set
			{
				this._real.Rewind();
				this._real.Put(value);
			}
		}

		/// <summary>
		/// Get a string representation for the content of the array.
		/// See <see cref="csmatio.types.MLArray.ContentToString()"/>
		/// </summary>
		/// <returns>A string representation.</returns>
		public override string ContentToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(this.Name + " = \n");

			if (this.Size > 1000)
			{
				// sb.Append("Cannot display variables with more than 1000 elements.");
				sb.Append(this.ToString());
				return sb.ToString();
			}
			for (int m = 0; m < this.M; m++)
			{
				sb.Append("\t");
				for (int n = 0; n < this.N; n++)
				{
					sb.Append(this.GetReal(m, n));
					if (this.IsComplex)
						sb.Append("+" + this.GetImaginary(m, n));
					sb.Append("\t");
				}
				sb.Append("\n");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Overridden equals operator, see <see cref="System.Object.Equals(System.Object)"/>
		/// </summary>
		/// <param name="o">A <c>System.Object</c> to be compared with.</param>
		/// <returns>True if the object match.</returns>
		public override bool Equals(object o)
		{
			if (o.GetType() == typeof(MLNumericArray<T>))
			{
				bool result = DirectByteBufferEquals(this._real, ((MLNumericArray<T>)o).GetReal()) &&
					Array.Equals(this.Dimensions, ((MLNumericArray<T>)o).Dimensions);

				if (this.IsComplex && result)
					result &= DirectByteBufferEquals(this._imaginary, ((MLNumericArray<T>)o).GetImaginary());
				return result;
			}
			return base.Equals(o);
		}

		/// <summary>
		/// Serves as a hash function for an MLNumericArray.
		/// </summary>
		/// <returns>A hashcode for this object</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		/// <summary>
		/// Equals implementation for a direct <c>ByteBuffer</c>
		/// </summary>
		/// <param name="buffa">The source buffer to be compared.</param>
		/// <param name="buffb">The destination buffer to be compared.</param>
		/// <returns><c>true</c> if buffers are equal in terms of content.</returns>
		private static bool DirectByteBufferEquals(ByteBuffer buffa, ByteBuffer buffb)
		{
			if (buffa == buffb)
				return true;

			if (buffa == null || buffb == null)
				return false;

			buffa.Rewind();
			buffb.Rewind();

			int length = buffa.Remaining;

			if (buffb.Remaining != length)
				return false;

			for (int i = 0; i < length; i++)
				if (buffa.Get() != buffb.Get())
					return false;

			return true;
		}


		#region GenericArrayCreator Members

		/// <summary>
		/// Creates a generic array.
		/// </summary>
		/// <param name="m">The number of columns in the array</param>
		/// <param name="n">The number of rows in the array</param>
		/// <returns>A generic array.</returns>
		public virtual T[] CreateArray(int m, int n)
		{
			return new T[m * n];
		}

		#endregion

		#region ByteStorageSupport Members

		/// <summary>
		/// Gets the number of bytes allocated for a type
		/// </summary>
		public virtual int GetBytesAllocated
		{
			get
			{
				int retval;
				Type tt = typeof(T);
				if (tt.IsValueType)
				{
					retval = Marshal.SizeOf(tt);
				}
				else
				{
					// tt is a reference type, so the size in memory is the pointer size.
					// We could return "retval = IntPtr.Size", but probably thats not what the user wants?
					// So tell him something went wrong.
					retval = -1;
				}

				return retval;
			}
		}

		/// <summary>
		/// Builds a numeric object from a byte array.
		/// </summary>
		/// <param name="bytes">A byte array containing the data.</param>
		/// <returns>A numeric object</returns>
		public virtual object BuildFromBytes(byte[] bytes)
		{
			if (bytes.Length != this.GetBytesAllocated)
			{
				throw new ArgumentException(
					"To build from a byte array, I need an array of size: " + this.GetBytesAllocated);
			}

			return this.BuildFromBytes2(bytes);
		}

		/// <summary>
		/// Gets a byte array from a numeric object.
		/// </summary>
		/// <param name="val">The numeric object to convert into a byte array.</param>
		public abstract byte[] GetByteArray(object val);

		/// <summary>
		/// Gets the type of numeric object that this byte storage represents
		/// </summary>
		public virtual Type GetStorageType
		{
			get { return typeof(T); }
		}

		/// <summary>
		/// Builds a numeric object from a byte array.
		/// </summary>
		/// <param name="bytes">A byte array containing the data.</param>
		/// <returns>A numeric object</returns>
		protected abstract object BuildFromBytes2(byte[] bytes);

		#endregion
	}
}
