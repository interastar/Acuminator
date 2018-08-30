﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Acuminator.Utilities;
using Acuminator.Utilities.Common;


namespace Acuminator.Analyzers
{
	internal static class DiagnosticUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRegisteredForCodeFix(this Diagnostic diagnostic, bool considerRegisteredByDefault = true)
		{
			diagnostic.ThrowOnNull(nameof(diagnostic));

			return diagnostic.Properties.TryGetValue(DiagnosticProperty.RegisterCodeFix, out string registered) 
				? registered == bool.TrueString
				: considerRegisteredByDefault;
		}
	}
}
