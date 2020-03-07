using System;

namespace JCMG.Nodey
{
	/// <summary>
	///     Mark a serializable field as an output port. You can access this through
	///     <see cref = "GetOutputPort(string)"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class OutputAttribute : Attribute
	{
		[Obsolete("Use dynamicPortList instead")]
		public bool instancePortList
		{
			get { return dynamicPortList; }
			set { dynamicPortList = value; }
		}

		public ShowBackingValue backingValue;
		public ConnectionType connectionType;

		public bool dynamicPortList;
		public TypeConstraint typeConstraint;

		/// <summary>
		///     Mark a serializable field as an output port. You can access this through
		///     <see cref = "GetOutputPort(string)"/>
		/// </summary>
		/// <param name = "backingValue"> Should we display the backing value for this port as an editor field? </param>
		/// <param name = "connectionType"> Should we allow multiple connections? </param>
		/// <param name = "typeConstraint"> Constrains which input connections can be made from this port </param>
		/// <param name = "dynamicPortList">
		///     If true, will display a reorderable list of outputs instead of a single port. Will
		///     automatically add and display values for lists and arrays
		/// </param>
		public OutputAttribute(ShowBackingValue backingValue = ShowBackingValue.Never,
		                       ConnectionType connectionType = ConnectionType.Multiple,
		                       TypeConstraint typeConstraint = TypeConstraint.None, bool dynamicPortList = false)
		{
			this.backingValue = backingValue;
			this.connectionType = connectionType;
			this.dynamicPortList = dynamicPortList;
			this.typeConstraint = typeConstraint;
		}

		/// <summary>
		///     Mark a serializable field as an output port. You can access this through
		///     <see cref = "GetOutputPort(string)"/>
		/// </summary>
		/// <param name = "backingValue"> Should we display the backing value for this port as an editor field? </param>
		/// <param name = "connectionType"> Should we allow multiple connections? </param>
		/// <param name = "dynamicPortList">
		///     If true, will display a reorderable list of outputs instead of a single port. Will
		///     automatically add and display values for lists and arrays
		/// </param>
		[Obsolete("Use constructor with TypeConstraint")]
		public OutputAttribute(ShowBackingValue backingValue, ConnectionType connectionType, bool dynamicPortList) : this(
			backingValue,
			connectionType,
			TypeConstraint.None,
			dynamicPortList)
		{
		}
	}
}
