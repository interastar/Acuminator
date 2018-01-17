﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PX.Analyzers.Coloriser
{
	public static class Constants
	{
		public const string Priority = Microsoft.VisualStudio.Text.Classification.Priority.High;

		public const string DacFormat = nameof(DacFormat);
		public const string DacFieldFormat = nameof(DacFieldFormat);
		public const string BQLParameterFormat = nameof(BQLParameterFormat);
		public const string BQLOperatorFormat = nameof(BQLOperatorFormat);
		public const string BQLConstantPrefixFormat = nameof(BQLConstantPrefixFormat);
		public const string BQLConstantEndingFormat = nameof(BQLConstantEndingFormat);

        public const string BraceLevel_1_Format = nameof(BraceLevel_1_Format);
        public const string BraceLevel_2_Format = nameof(BraceLevel_2_Format);
        public const string BraceLevel_3_Format = nameof(BraceLevel_3_Format);

        public const string BraceLevel_4_Format = nameof(BraceLevel_4_Format);
        public const string BraceLevel_5_Format = nameof(BraceLevel_5_Format);
        public const string BraceLevel_6_Format = nameof(BraceLevel_6_Format);

        public const string BraceLevel_7_Format = nameof(BraceLevel_7_Format);
        public const string BraceLevel_8_Format = nameof(BraceLevel_8_Format);
        public const string BraceLevel_9_Format = nameof(BraceLevel_9_Format);

        public const int MaxBraceLevel = 9;
    }

	public static class Labels
	{
		public const string DacFormatLabel  = "NoBrains - DAC Format";
		public const string DacFieldFormatLabel = "NoBrains - DAC Field Format";
		public const string BQLParameterFormatLabel = "NoBrains - BQL parameters";
		public const string BQLOperatorFormatLabel = "NoBrains - BQL operators";
		public const string BQLConstantPrefixFormatLabel = "NoBrains - BQL constant - prefix";
		public const string BQLConstantEndingFormatLabel = "NoBrains - BQL constant - ending";

        public const string BraceLevel_1_FormatLabel = "NoBrains - BQL angle braces level 1";
        public const string BraceLevel_2_FormatLabel = "NoBrains - BQL angle braces level 2";
        public const string BraceLevel_3_FormatLabel = "NoBrains - BQL angle braces level 3";

        public const string BraceLevel_4_FormatLabel = "NoBrains - BQL angle braces level 4";
        public const string BraceLevel_5_FormatLabel = "NoBrains - BQL angle braces level 5";
        public const string BraceLevel_6_FormatLabel = "NoBrains - BQL angle braces level 6";

        public const string BraceLevel_7_FormatLabel = "NoBrains - BQL angle braces level 7";
        public const string BraceLevel_8_FormatLabel = "NoBrains - BQL angle braces level 8";
        public const string BraceLevel_9_FormatLabel = "NoBrains - BQL angle braces level 9";
    }
}