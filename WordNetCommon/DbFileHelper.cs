using System;
using System.Collections.Generic;
using System.IO;

namespace WordNet.Common
{
	internal class DbFileHelper
	{
		#region Methods

		#region GetIndexForType
		/// <summary>
		/// Builds a list of dictionary index files for the specified Part of Speech (POS)
		/// </summary>
		/// <param name="type">The POS to return the index of</param>
		/// <returns>A list of dictionary file paths is successfull; otherwise an empty list</returns>
		internal static List<string> GetIndexForType( DbPartOfSpeechType type )
		{
			List<string> retVal = new List<string>();

			if ( type == DbPartOfSpeechType.All )
			{
				retVal.Add( GetDbIndexPath( DbPartOfSpeechType.Adj ) );
				retVal.Add( GetDbIndexPath( DbPartOfSpeechType.Adv ) );
				retVal.Add( GetDbIndexPath( DbPartOfSpeechType.Noun ) );
				retVal.Add( GetDbIndexPath( DbPartOfSpeechType.Verb ) );
			}
			else
			{
				retVal.Add( GetDbIndexPath( type ) );
			}

			return retVal;
		}
		#endregion GetIndexForType

		#region GetDBaseForType
		/// <summary>
		/// Builds a list of dictionary data files for the specified Part of Speech (POS)
		/// </summary>
		/// <param name="type">The POS to return the data of</param>
		/// <returns>A list of dictionary file paths is successfull; otherwise an empty list</returns>
		internal static List<string> GetDBaseForType( DbPartOfSpeechType type )
		{
			List<string> retVal = new List<string>();

			if ( type == DbPartOfSpeechType.All )
			{
				retVal.Add( GetDbDataPath( DbPartOfSpeechType.Adj ) );
				retVal.Add( GetDbDataPath( DbPartOfSpeechType.Adv ) );
				retVal.Add( GetDbDataPath( DbPartOfSpeechType.Noun ) );
				retVal.Add( GetDbDataPath( DbPartOfSpeechType.Verb ) );
			}
			else
			{
				retVal.Add( GetDbDataPath( type ) );
			}

			return retVal;
		}
		#endregion GetDBaseForType

		#region GetDbDataPath
		/// <summary>
		/// Builds the expected dictionary data file for the specified Part of Speech (POS)
		/// </summary>
		/// <param name="type">The POS to build the path for</param>
		/// <returns>The expected dictionary data file</returns>
		private static string GetDbDataPath( DbPartOfSpeechType type )
		{
			return GetDbFilePath( DbType.Data, type );
		}
		#endregion GetDbDataPath

		#region GetDbIndexPath
		/// <summary>
		/// Builds the expected dictionary index file for the specified Part of Speech (POS)
		/// </summary>
		/// <param name="type">The POS to build the path for</param>
		/// <returns>The expected dictionary index file</returns>
		private static string GetDbIndexPath( DbPartOfSpeechType type )
		{
			return GetDbFilePath( DbType.Index, type );
		}
		#endregion GetDbIndexPath

		#region GetDbFilePath
		/// <summary>
		/// Builds the expected database file path
		/// </summary>
		/// <param name="db">The type of database file</param>
		/// <param name="pos">The part of speech</param>
		/// <returns>The expected database file path</returns>
		private static string GetDbFilePath( DbType db, DbPartOfSpeechType pos )
		{
			string filePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Dict" );
			filePath = Path.Combine( filePath, string.Format( "{0}.{1}", db, pos ) );
			return filePath;
		}
		#endregion GetDbFilePath

		#endregion Methods
	}
}
