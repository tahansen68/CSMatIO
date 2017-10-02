namespace DotNetDoctor.csmatio.types
{
    using System;

    /// <summary>
	/// This class represents an Matlab Char array (matrix)
	/// </summary>
	/// <author>David Zier (david.zier@gmail.com)</author>
	public class MLChar : MLArray, IGenericArrayCreator<char>
	{
		char[] _chars;

		/// <summary>
		/// Creates an <c>MLChar</c> object from a character string.
		/// </summary>
		/// <param name="Name">Array name</param>
		/// <param name="Val">A character string array</param>
		public MLChar( string Name, string Val ) :
			this( Name, new int[] { Val.Length == 0 ? 0 : 1, Val.Length } , MLArray.mxCHAR_CLASS, 0 )
		{
			this.Set( Val );
		}

		/// <summary>
		/// Normally this constructor is used only by <c>MatFileReader</c> and <c>MatFileWriter</c>
		/// </summary>
		/// <param name="Name">Array name</param>
		/// <param name="Dims">Array dimensions</param>
		/// <param name="Type">Array type: here <c>mxCHAR_CLASS</c></param>
		/// <param name="Attributes">Array flags</param>
		public MLChar( string Name, int[] Dims, int Type, int Attributes ) :
			base( Name, Dims, Type, Attributes )
		{
			this._chars = this.CreateArray( this.M, this.N );
		}

		#region GenericArrayCreator Members

		/// <summary>
		/// Creates a generic array.
		/// </summary>
		/// <param name="m">The number of columns in the array</param>
		/// <param name="n">The number of rows in the array</param>
		/// <returns>A generic array.</returns>
		public char[] CreateArray(int m, int n)
		{
			return new char[m*n];
		}

		#endregion

		/// <summary>
		/// Sets an individual <c>char</c> in the character array.
		/// </summary>
		/// <param name="Ch">The <c>char</c> to be set</param>
		/// <param name="Index">The index into the array</param>
		/// <exception cref="IndexOutOfRangeException">Thrown if the index either negative or
		/// greater than the length of the character array</exception>
		public void SetChar( char Ch, int Index )
		{
			if( Index < 0 || Index >= this._chars.Length )
				throw new IndexOutOfRangeException("The index value of '" + Index + "' is out of range for the character array" );

			this._chars[Index] = Ch;
		}

		/// <summary>
		/// Sets the character array to a specific character string.
		/// </summary>
		/// <param name="Val">A character string</param>
		public void Set( string Val )
		{
			char[] cha = Val.ToCharArray();
			for( int i = 0; i < this.N && i < Val.Length; i++ )
			{
				this.SetChar(cha[i],i);
			}
		}

		/// <summary>
		/// Get the entire character array.
		/// </summary>
		/// <returns></returns>
		public char[] ExportChar()
		{
			return this._chars;
		}

		/// <summary>
		/// Get a character within the 2D character matrix.
		/// </summary>
		/// <param name="m">The row index</param>
		/// <param name="n">The column index</param>
		/// <returns>The fetched character</returns>
		public char GetChar( int m, int n )
		{
			return this._chars[ this.GetIndex(m,n) ];
		}

		/// <summary>
		/// Overridden equals operator, see <see cref="System.Object.Equals(System.Object)"/>
		/// </summary>
		/// <param name="o">A <c>System.Object</c> to be compared with.</param>
		/// <returns>True if the object match.</returns>
		public override bool Equals( object o )
		{
			if( o.GetType() == typeof( MLChar ) )
			{
				return Array.Equals( this._chars, ((MLChar)o)._chars );
			}
			return base.Equals( o );
		}

		/// <summary>
		/// Serves as a hash function for an MLNumericArray.
		/// </summary>
		/// <returns>A hashcode for this object</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Gets the m-th character matrix's row as <c>string</c>.
		/// </summary>
		/// <param name="m">Row number</param>
		/// <returns><c>String</c></returns>
		public string GetString( int m )
		{
			System.Text.StringBuilder charBuff = new System.Text.StringBuilder();

			for( int n = 0; n < this.N; n++ )
			{
				charBuff.Append( this.GetChar( m, n ) );
			}

			return charBuff.ToString();
		}

		/// <summary>
		/// Get a string representation for the content of the array.
		/// </summary>
		/// <returns>A string representation.</returns>
		public override string ContentToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append( this.Name + " = \n" );

			for( int m = 0; m < this.M; m++ )
			{
				sb.Append("\t");
				System.Text.StringBuilder charBuff = new System.Text.StringBuilder();
				charBuff.Append("'");
				for( int n = 0; n < this.N; n++ )
					charBuff.Append( this.GetChar(m,n) );
				charBuff.Append("'");
				sb.Append( charBuff );
				sb.Append( "\n" );
			}

			return sb.ToString();
		}
	}
}
