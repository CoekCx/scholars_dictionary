namespace WordNet.Common
{
	internal class Constants
	{
		public static int DONT_KNOW = 0;
		public static int DIRECT_ANT = 1;	/* direct antonyms (cluster head) */
		public static int INDIRECT_ANT = 2;	/* indrect antonyms (similar) */
		public static int PERTAINY = 3;	/* no antonyms or similars (pertainyms) */

		public static int POS_NOUN = 1;
		public static int POS_VERB = 2;
		public static int POS_ADJ = 3;
		public static int POS_ADV = 4;

		public static long KEY_LEN = ( 1024 );
		public static long LINE_LEN = ( 1024 * 25 );

		public static char[ ] Tokenizer = new char[ ] { ' ', '\n', '\r' };

		#region Pointer Type Constants

		#region PointerTypes
		public static string[ ] PointerTypes = new string[ ]
			{
				"",				/* 0 not used */
				"!",			/* 1 ANTPTR */
				"@",			/* 2 HYPERPTR */
				"~",			/* 3 HYPOPTR */
				"*",			/* 4 ENTAILPTR */
				"&",			/* 5 SIMPTR */
				"#m",			/* 6 ISMEMBERPTR */
				"#s",			/* 7 ISSTUFFPTR */
				"#p",			/* 8 ISPARTPTR */
				"%m",			/* 9 HASMEMBERPTR */
				"%s",			/* 10 HASSTUFFPTR */
				"%p",			/* 11 HASPARTPTR */
				"%",			/* 12 MERONYM */
				"#",			/* 13 HOLONYM */
				">",			/* 14 CAUSETO */
				"<",			/* 15 PPLPTR */
				"^",			/* 16 SEEALSO */
				"\\",			/* 17 PERTPTR */
				"=",			/* 18 ATTRIBUTE */
				"$",			/* 19 VERBGROUP */
				"+",		        /* 20 NOMINALIZATIONS */
				";",			/* 21 CLASSIFICATION */
				"-",			/* 22 CLASS */
			/* additional searches, but not pointers.  */
				"",				/* SYNS */
				"",				/* FREQ */
				"+",			/* FRAMES */
				"",				/* COORDS */
				"",				/* RELATIVES */
				"",				/* HMERONYM */
				"",				/* HHOLONYM */
				"",				/* WNGREP */
				"",				/* OVERVIEW */
				";c",			/* CLASSIF_CATEGORY */
				";u",			/* CLASSIF_USAGE */
				";r",			/* CLASSIF_REGIONAL */
				"-c",			/* CLASS_CATEGORY */
				"-u",			/* CLASS_USAGE */
				"-r",			/* CLASS_REGIONAL */
				"@i",			/* INSTANCE */
				"~i"			/* INSTANCES */
			};
		#endregion PointerTypes

		#region PointerTypeContants
		internal class PointerTypeContants
		{
			public static int ANTPTR = 1;	/* ! */
			public static int HYPERPTR = 2;	/* @ */
			public static int HYPOPTR = 3;	/* ~ */
			public static int ENTAILPTR = 4;	/* * */
			public static int SIMPTR = 5;	/* & */

			public static int ISMEMBERPTR = 6;	/* #m */
			public static int ISSTUFFPTR = 7;	/* #s */
			public static int ISPARTPTR = 8;	/* #p */

			public static int HASMEMBERPTR = 9;	/* %m */
			public static int HASSTUFFPTR = 10;	/* %s */
			public static int HASPARTPTR = 11;	/* %p */

			public static int MERONYM = 12;	/* % (not valid in lexicographer file) */
			public static int HOLONYM = 13;	/* # (not valid in lexicographer file) */
			public static int CAUSETO = 14;	/* > */
			public static int PPLPTR = 15;	/* < */
			public static int SEEALSOPTR = 16;	/* ^ */
			public static int PERTPTR = 17;	/* \ */
			public static int ATTRIBUTE = 18;	/* = */
			public static int VERBGROUP = 19;	/* $ */
			public static int DERIVATION = 20;	/* + */
			public static int CLASSIFICATION = 21;	/* ; */
			public static int CLASS = 22;	/* - */
		}
		#endregion PointerTypeContants

		#endregion Pointer Type Contants
	}
}
