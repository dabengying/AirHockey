using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace AirHockey{

	/// <summary>
	/// Represents an RGBA color.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Color4 : ISerializable, ICloneable
	{
		#region Private Fields
		private float _red,_green,_blue,_alpha;
		#endregion

		#region Constructores
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class.
		/// </summary>
		/// <remarks>
		/// Default values are 1.0f for Alpha and 0.0f for the color channels.
		/// </remarks>
		public Color4()
		{
			_red	= 0.0f;
			_green	= 0.0f;
			_blue	= 0.0f;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <remarks>The alpha channel value is set to 1.0f.</remarks>
		public Color4(float red, float green, float blue)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <param name="alpha">Alpha channel value.</param>
		public Color4(float red, float green, float blue, float alpha)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= alpha;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class using values from another instance.
		/// </summary>
		/// <param name="color">A <see cref="Color4"/> instance.</param>
		public Color4(Color4 color)
		{
			_red	= color.R;
			_green	= color.G;
			_blue	= color.B;
			_alpha	= color.A;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class from a blend of two colors.
		/// </summary>
		/// <param name="source">The blend source color.</param>
		/// <param name="dest">The blend destination color.</param>
		/// <param name="opacity">The opacity value.</param>
		public Color4(Color4 source, Color4 dest, float opacity)
		{
			_red	= MathFunctions.LinearInterpolation(source.R, dest.R, opacity);
			_green	= MathFunctions.LinearInterpolation(source.G, dest.G, opacity);
			_blue	= MathFunctions.LinearInterpolation(source.B, dest.B, opacity);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Color4"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected Color4(SerializationInfo info, StreamingContext context)
		{
			_red	= info.GetSingle("Red");
			_green	= info.GetSingle("Green");
			_blue	= info.GetSingle("Blue");
			_alpha	= info.GetSingle("Alpha");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the red channel value.
		/// </summary>
		public float R
		{
			get { return _red; }
			set { _red = value;}
		}
		/// <summary>
		/// Gets or sets the green channel value.
		/// </summary>
		public float G
		{
			get { return _green; }
			set { _green = value;}
		}
		/// <summary>
		/// Gets or sets the blue channel value.
		/// </summary>
		public float B
		{
			get { return _blue; }
			set { _blue = value;}
		}
		/// <summary>
		/// Gets or sets the alpha channel value.
		/// </summary>
		public float A
		{
			get { return _alpha; }
			set { _alpha = value;}
		}
		/// <summary>
		/// Gets the color's intensity.
		/// </summary>
		/// <remarks>
		/// Intensity = (R + G + B) / 3
		/// </remarks>
		public float Intensity
		{
			get { return (_red + _green + _blue) / 3.0f; }
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Color4"/> object.
		/// </summary>
		/// <returns>The <see cref="Color4"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Color4(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Color4"/> object.
		/// </summary>
		/// <returns>The <see cref="Color4"/> object this method creates.</returns>
		public Color4 Clone()
		{
			return new Color4(this);
		}
		#endregion
	
		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Red", _red);
			info.AddValue("Green", _green);
			info.AddValue("Blue", _blue);
			info.AddValue("Alpha", _alpha);
		}
		#endregion


		
		#region Public Methods
		/// <summary>
		/// Clamp the RGBA value to [0, 1] range.
		/// </summary>
		/// <remarks>
		/// Values above 1.0f are clamped to 1.0f.
		/// Values below 0.0f are clamped to 0.0f.
		/// </remarks>
		public void Clamp()
		{
			if (_red < 0.0f) 
				_red = 0.0f;
			else if (_red > 1.0f) 
				_red = 1.0f;

			if (_green < 0.0f) 
				_green = 0.0f;
			else if (_green > 1.0f) 
				_green = 1.0f;

			if (_blue < 0.0f) 
				_blue = 0.0f;
			else if (_blue > 1.0f) 
				_blue = 1.0f;
		
			if (_alpha < 0.0f) 
				_alpha = 0.0f;
			else if (_alpha > 1.0f) 
				_alpha = 1.0f;
		}
		/// <summary>
		/// Calculates the color's HSV values.
		/// </summary>
		/// <param name="h">The Hue value.</param>
		/// <param name="s">The Saturation value.</param>
		/// <param name="v">The Value value.</param>
        /*
		public void ToHSV(out float h, out float s, out float v)
		{
			float min = MathFunctions.MinValue((Vector3)this);
			float max = MathFunctions.MaxValue((Vector3)this);
			v = max;

			float delta = max - min;
			if( max != 0.0f )
			{
				s = delta / max;
			}
			else 
			{
				// r = g = b = 0.0f --> s = 0, v is undefined
				s = 0.0f;
				h = 0.0f;
				return;
			}

			if(_red == max)
			{
				h = ( _green - _blue ) / delta;		// between yellow & magenta
			}
			else if(_green == max)
			{
				h = 2 + ( _blue - _red ) / delta;	// between cyan & yellow
			}
			else
			{
				h = 4 + ( _red - _green ) / delta;	// between magenta & cyan
			}

			h *= 60.0f; // degrees
			if( h < 0.0f )
				h += 360.0f;

		}
         */
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _red.GetHashCode() ^ _green.GetHashCode() ^ _blue.GetHashCode() ^ _alpha.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Color4"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			Color4 color = obj as Color4;
			if (color != null)
				return (_red == color.R) && (_green == color.G) && (_blue == color.B) && (_alpha == color.A);
			else 
				return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("ColorF({0}, {1}, {2}, {3})", _red, _green, _blue, _alpha);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified <see cref="Color4"/> instances are equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="Color4"/> instance.</param>
		/// <param name="v">The right-hand <see cref="Color4"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="Color4"/> instances are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Color4 u, Color4 v)
		{
			return Object.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified <see cref="Color4"/> instances are not equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="Color4"/> instance.</param>
		/// <param name="v">The right-hand <see cref="Color4"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="Color4"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Color4 u, Color4 v)
		{
			return !Object.Equals(u,v);
		}
		#endregion


		/// <summary>
		/// Converts the color structure to an array of single-precision floating point values.
		/// </summary>
		/// <param name="color">A <see cref="Color4"/> instance.</param>
		/// <returns>An array of single-precision floating point values.</returns>
		public static explicit operator float[](Color4 color)
		{
			float[] array = new float[4];
			array[0] = color.R;
			array[1] = color.G;
			array[2] = color.B;
			array[3] = color.A;
			return array;
		}


	}
}
