namespace WordNet.Common
{
	#region DbPartOfSpeechType
	/// <summary>
	/// Denotes a recognized part of speech
	/// </summary>
	public enum DbPartOfSpeechType
	{
		All = 0,
		Noun = 1,
		Verb = 2,
		Adj = 3,
		Adv = 4,
		Satellite = 5,
		AdjSat = 5
	}
	#endregion DbPartOfSpeechType

	#region DbType
	/// <summary>
	/// Denotes the a type of database
	/// </summary>
	internal enum DbType
	{
		Index = 1,
		Data = 2
	}
	#endregion DbType
}
