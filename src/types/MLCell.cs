namespace DotNetDoctor.csmatio.types
{
    using System;
    using System.Collections;

    /// <summary>
	/// This class represents an Matlab Char array (matrix)
	/// </summary>
	/// <author>David Zier (david.zier@gmail.com)</author>
	public class MLCell : MLArray
	{
		private ArrayList _cells;

		/// <summary>
		/// Create a basic empty <c>MLCell</c> object for a specific set of dimensions
		/// </summary>
		/// <param name="Name">Array name</param>
		/// <param name="Dims">Array dimensions</param>
		public MLCell( string Name, int[] Dims ) :
			this( Name, Dims, MLArray.mxCELL_CLASS, 0 ) {}

		/// <summary>
		/// Normally this constructor is used only by <c>MatFileReader</c> and <c>MatFileWriter</c>
		/// </summary>
		/// <param name="Name">Array name</param>
		/// <param name="Dims">Array dimensions</param>
		/// <param name="Type">Array type: here <c>mxCELL_CLASS</c></param>
		/// <param name="Attributes">Array flags</param>
		public MLCell( String Name, int[] Dims, int Type, int Attributes ) :
			base( Name, Dims, Type, Attributes )
		{
			this._cells = new ArrayList(this.M*this.N);

			for( int i = 0; i < this.M*this.N; i++ )
			{
				this._cells.Add( new MLEmptyArray() );
			}
		}

		/// <summary>
		/// Public accessor to the cell array for 2-dimensions.
		/// </summary>
		public MLArray this[ int m, int n ]
		{
			set{ this._cells[ this.GetIndex(m,n) ] = value; }
			get{ return (MLArray)this._cells[ this.GetIndex(m,n) ]; }
		}

		/// <summary>
		/// Public accessor to the cell array for 1 dimension.
		/// </summary>
		public MLArray this[ int Index ]
		{
			set{ this._cells[ Index ] = value; }
			get{ return (MLArray)this._cells[ Index ]; }
		}

		/// <summary>
		/// Public Get Accessor for the cells array list
		/// </summary>
		public ArrayList Cells
		{
			get{ return this._cells; }
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
				for( int n = 0; n < this.N; n++ )
				{
					sb.Append( this[m,n].ContentToString() );
					sb.Append("\t");
				}
				sb.Append("\n");
			}
			return sb.ToString();
		}


	}
}
