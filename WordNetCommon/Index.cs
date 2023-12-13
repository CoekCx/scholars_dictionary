using System.Collections.Generic;

namespace WordNet.Common
{
	internal struct Index
	{
		#region Member Variables

		private long mIdxOffset;		/* byte offset of entry in index file */
		private string mWord;			/* word string */
		private string mPartOfSpech;			/* part of speech */
		private int mSenseCount;		/* sense (collins) count */
		private int mOffsetCount;		/* number of offsets */
		private int mTaggedSensesCount;		/* number senses that are tagged */
		private List<long> mSynSetsOffsets;	/* offsets of synsets containing word */
		private int mPointersUsedCount;		/* number of pointers used */
		private List<int> mPointersUsed;		/* pointers used */

		#endregion Member Variables

		#region Properties

		#region Empty
		/// <summary>
		/// Get an empty index structure
		/// </summary>
		public static Index Empty { get { return new Index(); } }
		#endregion Empty

		#region IdxOffset
		/// <summary>
		/// byte offset of entry in index file
		/// </summary>
		public long IdxOffset { get { return mIdxOffset; } set { mIdxOffset = value; } }
		#endregion IdxOffset

		#region Word
		/// <summary>
		/// word string
		/// </summary>
		public string Word { get { return mWord; } set { mWord = value; } }
		#endregion Word

		#region PartOfSpech
		/// <summary>
		/// part of speech
		/// </summary>
		public string PartOfSpech { get { return mPartOfSpech; } set { mPartOfSpech = value; } }
		#endregion PartOfSpech

		#region SenseCount
		/// <summary>
		/// sense (collins) count
		/// </summary>
		public int SenseCount { get { return mSenseCount; } set { mSenseCount = value; } }
		#endregion SenseCount

		#region OffsetCount
		/// <summary>
		/// number of offsets
		/// </summary>
		public int OffsetCount { get { return mOffsetCount; } set { mOffsetCount = value; } }
		#endregion OffsetCount

		#region TaggedSensesCount
		/// <summary>
		/// number senses that are tagged
		/// </summary>
		public int TaggedSensesCount { get { return mTaggedSensesCount; } set { mTaggedSensesCount = value; } }
		#endregion TaggedSensesCount

		#region SynSetsOffsets
		/// <summary>
		/// offsets of synsets containing word
		/// </summary>
		public List<long> SynSetsOffsets { get { return mSynSetsOffsets; } set { mSynSetsOffsets = value; } }
		#endregion SynSetsOffsets

		#region PointersUsedCount
		/// <summary>
		/// number of pointers used
		/// </summary>
		public int PointersUsedCount { get { return mPointersUsedCount; } set { mPointersUsedCount = value; } }
		#endregion PointersUsedCount

		#region PointersUsed
		/// <summary>
		/// pointers used
		/// </summary>
		public List<int> PointersUsed { get { return mPointersUsed; } set { mPointersUsed = value; } }
		#endregion PointersUsed

		#endregion Properties
	}
}
