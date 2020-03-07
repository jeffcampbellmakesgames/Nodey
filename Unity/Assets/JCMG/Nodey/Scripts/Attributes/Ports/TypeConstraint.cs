namespace JCMG.Nodey
{
	/// <summary> Tells which types of input to allow </summary>
	public enum TypeConstraint
	{
		/// <summary> Allow all types of input </summary>
		None,

		/// <summary>
		///     Allow connections where input value type is assignable from output value type (eg. ScriptableObject -->
		///     Object)
		/// </summary>
		Inherited,

		/// <summary> Allow only similar types </summary>
		Strict,

		/// <summary>
		///     Allow connections where output value type is assignable from input value type (eg. Object -->
		///     ScriptableObject)
		/// </summary>
		InheritedInverse
	}
}
