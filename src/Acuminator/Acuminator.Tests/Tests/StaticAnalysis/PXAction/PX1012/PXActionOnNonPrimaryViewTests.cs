﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;
using Acuminator.Analyzers;
using Acuminator.Analyzers.FixProviders;
using Acuminator.Tests.Helpers;

namespace Acuminator.Tests
{
	public class PXActionOnNonPrimaryViewTests : CodeFixVerifier
	{
		[Theory]
		[EmbeddedFileData(@"PXAction\PX1012\Diagnostics\GraphWithNonPrimaryDacView.cs")] 
		public virtual void Test_Diagnostic_For_Graph_And_Graph_Extension(string source) =>
			VerifyCSharpDiagnostic(source,
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 23, column: 10, actionName: "Release1", mainDacName: "SOOrder"),
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 25, column: 10, actionName: "Release2", mainDacName: "SOOrder"),
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 34, column: 10, actionName: "Action1", mainDacName: "SOOrder"),
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 38, column: 10, actionName: "Action3", mainDacName: "SOOrder"),
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 47, column: 10, actionName: "Release1", mainDacName: "SOOrder"),
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 49, column: 10, actionName: "Release2", mainDacName: "SOOrder"));

		[Theory]
		[EmbeddedFileData(@"PXAction\PX1012\Diagnostics\DerivedGraphWithBaseGraphPrimaryDac.cs")]
		public virtual void Test_Diagnostic_For_Derived_Graph(string source) =>
			VerifyCSharpDiagnostic(source,
				CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(line: 26, column: 10, actionName: "Release1", mainDacName: "SOOrder"));

		[Theory]
		[EmbeddedFileData(@"PXAction\PX1012\Diagnostics\GraphWithNonPrimaryDacView.cs",
						  @"PXAction\PX1012\CodeFixes\GraphWithNonPrimaryDacViewExpected.cs")]
		public void Test_Code_Fix_For_Graph_And_Graph_Extension(string actual, string expected)
		{
			VerifyCSharpFix(actual, expected);
		}

		[Theory]
		[EmbeddedFileData(@"PXAction\PX1012\Diagnostics\DerivedGraphWithBaseGraphPrimaryDac.cs",
						  @"PXAction\PX1012\CodeFixes\DerivedGraphWithBaseGraphPrimaryDacExpected.cs")]
		public void Test_Code_Fix_For_Derived_Graph(string actual, string expected)
		{
			VerifyCSharpFix(actual, expected);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new PXActionOnNonPrimaryViewAnalyzer();

		protected override CodeFixProvider GetCSharpCodeFixProvider() => new PXActionOnNonPrimaryViewFix();

		private DiagnosticResult CreatePX1012ActionOnNonPrimaryViewDiagnosticResult(int line, int column, string actionName, 
																					string mainDacName)
		{
			string format = Descriptors.PX1012_PXActionOnNonPrimaryView.Title.ToString();
			string expectedMessage = string.Format(format, actionName, mainDacName);

			return new DiagnosticResult
			{
				Id = Descriptors.PX1012_PXActionOnNonPrimaryView.Id,
				Message = expectedMessage,
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", line, column)
					}
			};
		}
	}
}
