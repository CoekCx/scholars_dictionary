using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordNet.Common
{
	public class DictionaryHelper
	{
		#region Member Variables

		private static List<string> completedFiles = new List<string>();

		#endregion Member Variables

		#region Methods

		#region GetDefinition
		/// <summary>
		/// Finds definitions for a specified word
		/// </summary>
		/// <param name="word">The word to define</param>
		/// <returns>A dictionary of wrods and definition pertenant to the specified word</returns>
		public static Dictionary<string, List<Definition>> GetDefinition( string word )
		{
			Dictionary<string, List<Definition>> retVal = new Dictionary<string, List<Definition>>();
			List<string> fileListIndex = DbFileHelper.GetIndexForType( DbPartOfSpeechType.All );
			List<string> fileListData = DbFileHelper.GetDBaseForType( DbPartOfSpeechType.All );

			for( int i = 0; i < fileListIndex.Count; i++ )
			{
				long offset = FileParser.FastSearch( word.ToLower(), fileListIndex[ i ] );
				if( offset > 0 )
				{
					Index idx = FileParser.ParseIndex( offset, fileListIndex[ i ] );
					foreach( long synSetOffset in idx.SynSetsOffsets )
					{
						try
						{
							Definition def = FileParser.ParseDefinition( synSetOffset, fileListData[ i ], word );
							string wordKey = string.Join( ", ", def.Words.ToArray() );
							if( !retVal.ContainsKey( wordKey ) )
								retVal.Add( wordKey, new List<Definition>() );

							retVal[ wordKey ].Add( def );
						}
						catch( Exception ex )
						{
							string message = ex.Message;
						}
					}
				}
			}

			return retVal;
		}
		#endregion GetDefinition

		#endregion Methods
	}
}
