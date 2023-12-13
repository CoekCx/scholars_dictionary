using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordNet.Common
{
	public class FileParser
	{
		#region Methods

		#region ParseIndex
		/// <summary>
		/// Parses an index structure from the specified file at the specified offset
		/// </summary>
		/// <param name="offset">The ofset in the file at which the index exists</param>
		/// <param name="dbFileName">The full path to the database file to open</param>
		/// <returns>A populated index structure in successful; otherwise an empty index structure</returns>
		internal static Index ParseIndex( long offset, string dbFileName )
		{
			Index retVal = Index.Empty;
			retVal.IdxOffset = 0;
			retVal.OffsetCount = 0;
			retVal.PartOfSpech = string.Empty;
			retVal.PointersUsed = new List<int>();
			retVal.PointersUsedCount = 0;
			retVal.SenseCount = 0;
			retVal.SynSetsOffsets = new List<long>();
			retVal.TaggedSensesCount = 0;
			retVal.Word = string.Empty;

			string data = string.Empty;

			if( string.IsNullOrEmpty( data ) )
				data = ReadRecord( offset, dbFileName );

			if( !string.IsNullOrEmpty( data ) )
			{
				int i = 0;
				string[] tokens = data.Split( Constants.Tokenizer, StringSplitOptions.RemoveEmptyEntries );

				retVal.IdxOffset = offset;

				retVal.Word = tokens[ i ];
				i++;

				retVal.PartOfSpech = tokens[ i ];
				i++;

				retVal.SenseCount = Convert.ToInt32( tokens[ i ] );
				i++;

				retVal.PointersUsedCount = Convert.ToInt32( tokens[ i ] );
				i++;

				for( int j = 0; j < retVal.PointersUsedCount; j++ )
				{
					int pointerIndex = GetPointerTypeIndex( tokens[ i + j ] );
					retVal.PointersUsed.Add( pointerIndex );
				}
				i = ( i + retVal.PointersUsedCount );

				retVal.OffsetCount = Convert.ToInt32( tokens[ i ] );
				i++;

				retVal.TaggedSensesCount = Convert.ToInt32( tokens[ i ] );
				i++;

				for( int j = 0; j < retVal.OffsetCount; j++ )
				{
					long synSetOffset = Convert.ToInt64( tokens[ i + j ] );
					retVal.SynSetsOffsets.Add( synSetOffset );
				}
			}

			return retVal;
		}
		#endregion ParseIndex

		#region ParseDefinition
		/// <summary>
		/// Parses a word definition at the specified offset in the specified file
		/// </summary>
		/// <param name="offset">The offset in the file at which to begin parsing</param>
		/// <param name="dbFileName">The full path of the file to open</param>
		/// <param name="word">The word that will be defined by the parsed definition</param>
		/// <returns>A populated Definition object is successful; otherwise null</returns>
		internal static Definition ParseDefinition( long offset, string dbFileName, string word )
		{
			Definition retVal = null;
			try
			{
				Definition tempDef = new Definition();
				string data = ReadRecord( offset, dbFileName );
				if( !string.IsNullOrEmpty( data ) )
				{
					int i = 0;
					bool foundPert = false;
					string[] tokens = data.Split( Constants.Tokenizer, StringSplitOptions.RemoveEmptyEntries );

					tempDef.Position = Convert.ToInt64( tokens[ i ] );
					i++;

					if( tempDef.Position != offset )
						throw new ArithmeticException( "The stream position is not aligned with the specified offset!" );

					tempDef.FileNumber = Convert.ToInt32( tokens[ i ] );
					i++;

					tempDef.PartOfSpeech = tokens[ i ];
					i++;

					if( GetSynSetTypeCode( tempDef.PartOfSpeech ) == DbPartOfSpeechType.Satellite )
						tempDef.DefinitionType = Constants.INDIRECT_ANT;

					tempDef.WordCount = Convert.ToInt32( tokens[ i ] );
					i++;

					for( int j = 0; j < tempDef.WordCount * 2; j += 2 ) //Step by two for lexid
					{
						string tempWord = tokens[ i + j ];
						if( !string.IsNullOrEmpty( tempWord ) )
							tempDef.Words.Add( DecodeWord( tempWord ) );

						if( tempWord.ToLower() == word.ToLower() )
							tempDef.WhichWord = ( i + j );
					}
					i = ( i + ( tempDef.WordCount * 2 ) );

					tempDef.PtrCount = Convert.ToInt32( tokens[ i ] );
					i++;

					for( int j = i; j < ( i + ( tempDef.PtrCount * 4 ) ); j += 4 )
					{
						int pointerIndex = GetPointerTypeIndex( tokens[ j ] );
						long pointerOffset = Convert.ToInt64( tokens[ j + 1 ] );
						int pointerPartOfSpeech = GetPartOfSpeech( Convert.ToChar( tokens[ j + 2 ] ) );
						string lexToFrom = tokens[ j + 3 ];
						int lexFrom = Convert.ToInt32( lexToFrom.Substring( 0, 2 ) );
						int lexTo = Convert.ToInt32( lexToFrom.Substring( 1, 2 ) );

						tempDef.PtrTypes.Add( pointerIndex );
						tempDef.PtrOffsets.Add( pointerOffset );
						tempDef.PtrPartOfSpeech.Add( pointerPartOfSpeech );
						tempDef.PtrFromFields.Add( lexFrom );
						tempDef.PtrToFields.Add( lexTo );

						if( AssertDatabaseType( dbFileName, DbPartOfSpeechType.Adj ) && tempDef.DefinitionType == Constants.DONT_KNOW )
						{
							if( pointerIndex == Constants.PointerTypeContants.ANTPTR )
							{
								tempDef.DefinitionType = Constants.DIRECT_ANT;
							}
							else if( pointerIndex == Constants.PointerTypeContants.PERTPTR )
							{
								foundPert = true;
							}
						}
					}
					i += ( tempDef.PtrCount * 4 );

					if( AssertDatabaseType( dbFileName, DbPartOfSpeechType.Adj ) &&
						tempDef.DefinitionType == Constants.DONT_KNOW && foundPert )
					{
						tempDef.DefinitionType = Constants.PERTAINY;
					}

					if( AssertDatabaseType( dbFileName, DbPartOfSpeechType.Verb ) )
					{
						int verbFrames = Convert.ToInt32( tokens[ i ] );
						tempDef.VerbFrameCount = verbFrames;
						i++;

						for( int j = i; j < i + ( tempDef.VerbFrameCount * 3 ); j += 3 )
						{
							int frameId = Convert.ToInt32( tokens[ j + 1 ] );
							int frameTo = Convert.ToInt32( tokens[ j + 2 ] );

							tempDef.FrameIds.Add( frameId );
							tempDef.FrameToFields.Add( frameTo );
						}
						i += ( tempDef.VerbFrameCount * 3 );
					}
					i++;

					string definition = string.Join( " ", tokens, i, tokens.Length - i );
					tempDef.DefinitionText = definition;

					tempDef.SenseNumbers = new List<int>( new int[ tempDef.WordCount ] );
					for( int j = 0; j < tempDef.WordCount; j++ )
					{
						tempDef.SenseNumbers[ j ] = GetSearchSense( tempDef, j );
					}
				}
				retVal = tempDef;
			}
			catch
			{
				retVal = null;
			}
			return retVal;
		}
		#endregion ParseDefinition

		#region GetSearchSense
		/// <summary>
		/// Calculates the search sense in a definition by the specified word
		/// </summary>
		/// <param name="def">The definition to use</param>
		/// <param name="whichWord">The word to calculate on</param>
		/// <returns>Any non-negative number if successfull; otherwise -1</returns>
		private static int GetSearchSense( Definition def, int whichWord )
		{
			int retVal = -1;
			DbPartOfSpeechType indexType = GetSynSetTypeCode( def.PartOfSpeech );
			string dbFileName = DbFileHelper.GetIndexForType( indexType )[ 0 ];
			if( File.Exists( dbFileName ) )
			{
				long offset = FastSearch( def.Words[ whichWord ], dbFileName );
				Index idx = ParseIndex( offset, dbFileName );

				for( int i = 0; i < idx.OffsetCount; i++ )
				{
					retVal = 0;
					if( idx.SynSetsOffsets[ i ] == def.Position )
					{
						retVal = i + 1;
						break;
					}
				}
			}

			return retVal;
		}
		#endregion GetSearchSense

		#region ReadIndex
		/// <summary>
		/// Reads the record at the specified offset from the specified file
		/// </summary>
		/// <param name="offset">The offset (record id) to read</param>
		/// <param name="dbFileName">The full path of the file to read from</param>
		/// <returns>The record as a string is successfull; otherwise an empty string</returns>
		internal static string ReadRecord( long offset, string dbFileName )
		{
			string retVal = string.Empty;

			using( StreamReader reader = new StreamReader( dbFileName, true ) )
			{
				reader.BaseStream.Seek( offset, SeekOrigin.Begin );
				retVal = reader.ReadLine();
				reader.Close();
			}

			return retVal;
		}
		#endregion ReadIndex

		#region FastSearch
		/// <summary>
		/// Searches the specified file for the specified keyword
		/// </summary>
		/// <param name="keyword">The keyword to find</param>
		/// <param name="dbFileName">The full path to the file to search in</param>
		/// <returns>The offset in the file at which the word was found; otherwise 0</returns>
		internal static long FastSearch( string keyword, string dbFileName )
		{
			long retVal = 0L;
			string key = string.Empty;
			Encoding enc = Encoding.Default;

			using( StreamReader reader = new StreamReader( dbFileName, true ) )
			{
				enc = reader.CurrentEncoding;
				reader.Close();
			}

			using( FileStream fs = File.OpenRead( dbFileName ) )
			{
				long diff = 666;
				string line = string.Empty;

				fs.Seek( 0, SeekOrigin.End );
				long top = 0;
				long bottom = fs.Position;
				long mid = ( bottom - top ) / 2;

				do
				{
					fs.Seek( mid - 1, SeekOrigin.Begin );
					if( mid != 1 )
					{
						while( fs.ReadByte() != '\n' && fs.Position < fs.Length )
						{
							retVal = fs.Position;
						}
					}

					byte[] btData = new byte[ Constants.KEY_LEN ];
					int count = fs.Read( btData, 0, btData.Length );
					fs.Seek( fs.Position - count, SeekOrigin.Begin );

					string readData = enc.GetString( btData );
					key = readData.Split( Constants.Tokenizer )[ 0 ];

					if( string.Compare( key, keyword ) != 0 )
					{
						if( string.Compare( key, keyword ) < 0 )
						{
							top = mid;
							diff = ( bottom - top ) / 2;
							mid = top + diff;
						}

						if( string.Compare( key, keyword ) > 0 )
						{
							bottom = mid;
							diff = ( bottom - top ) / 2;
							mid = top + diff;
						}
					}
				}
				while( string.Compare( key, keyword ) != 0 && diff != 0 );
			}

			if( string.Compare( key, keyword ) != 0 )
				retVal = 0L;
			else
				retVal++;

			return retVal;
		}
		#endregion FastSearch

		#region AssertDatabaseType
		/// <summary>
		/// Determines if the specified database file represents the specified part of speech.
		/// </summary>
		/// <param name="dbFileName">The full path to the database file</param>
		/// <param name="type">The part of speech to check for</param>
		/// <returns>True is the file represents the part of speech; otherwise false</returns>
		private static bool AssertDatabaseType( string dbFileName, DbPartOfSpeechType type )
		{
			string strType = Path.GetExtension( dbFileName );
			strType = strType.Substring( 1, strType.Length - 1 );
			return ( strType.ToLower() == type.ToString().ToLower() );
		}
		#endregion AssertDatabaseType

		#region GetPointerTypeIndex
		/// <summary>
		/// Converts a text annotation of a marker into its numeric equivelant
		/// </summary>
		/// <param name="pointerMark">The marker text to convert</param>
		/// <returns>Any non-negative numer if successfull; otherwise -1</returns>
		internal static int GetPointerTypeIndex( string pointerMark )
		{
			int retVal = -1;
			for( int i = 0; i < Constants.PointerTypes.Length; i++ )
			{
				string pointer = Constants.PointerTypes[ i ];
				if( pointer.ToLower().Trim() == pointerMark.ToLower().Trim() )
				{
					retVal = i;
					break;
				}
			}
			return retVal;
		}
		#endregion GetPointerTypeIndex

		#region GetSynSetTypeCode
		/// <summary>
		/// Converts the text annotation of a speech part into the corosponding POS (Part of Speech)
		/// </summary>
		/// <param name="data">The text to convert</param>
		/// <returns>The corosponding part of spech if successfull; otherwise DbPartOfSpeechType.All</returns>
		public static DbPartOfSpeechType GetSynSetTypeCode( string data )
		{
			char pos = data[ 0 ];
			switch( pos )
			{
				case 'n':
					return DbPartOfSpeechType.Noun;
				case 'a':
					return DbPartOfSpeechType.Adj;
				case 'v':
					return DbPartOfSpeechType.Verb;
				case 's':
					return DbPartOfSpeechType.Satellite;
				case 'r':
					return DbPartOfSpeechType.Adv;
			}
			return DbPartOfSpeechType.All;
		}
		#endregion GetSynSetTypeCode

		#region GetPartOfSpeech
		/// <summary>
		/// Converts a text annotation of a POS (Part of Speech) to the corosponding numeric value
		/// </summary>
		/// <param name="data">The text to convert</param>
		/// <returns>Any non-negative number is successfull; otherwise -1</returns>
		internal static int GetPartOfSpeech( char data )
		{
			switch( data )
			{
				case 'n':
					return ( Constants.POS_NOUN );
				case 'a':
				case 's':
					return ( Constants.POS_ADJ );
				case 'v':
					return ( Constants.POS_VERB );
				case 'r':
					return ( Constants.POS_ADV );
			}
			return -1;
		}
		#endregion GetPartOfSpeech

		#region EncodeWord
		/// <summary>
		/// Converts a compound word/phrase into the recognized format
		/// </summary>
		/// <param name="data">The text to convert</param>
		/// <returns>A normalized string</returns>
		private static string EncodeWord( string data )
		{
			string retVal = string.Empty;
			retVal = data.Replace( ' ', '_' );
			return retVal;
		}
		#endregion EncodeWord

		#region DecodeWord
		/// <summary>
		/// Converts a normalized string into a presentable format
		/// </summary>
		/// <param name="data">The normalized string</param>
		/// <returns>A presentable format string</returns>
		private static string DecodeWord( string data )
		{
			string retVal = string.Empty;
			retVal = data.Replace( '_', ' ' );
			return retVal;
		}
		#endregion DecodeWord

		#endregion Methods
	}
}

