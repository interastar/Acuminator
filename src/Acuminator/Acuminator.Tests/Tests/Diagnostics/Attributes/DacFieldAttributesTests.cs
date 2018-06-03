﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acuminator.Analyzers;
using Acuminator.Tests.Helpers;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace Acuminator.Tests
{
	public class DacFieldAttributesTests : CodeFixVerifier
	{
		[Theory]
		[EmbeddedFileData(@"Attributes\Diagnostics\FieldAttributesTypeMismatch\DacExampleWithMultipleFieldAttributes.cs")]
		public virtual void Test_Dac_With_Multiple_Field_Attributes(string source) =>
			VerifyCSharpDiagnostic(source,
				CreatePX1023MultipleFieldAttributesDiagnosticResult(line: 24, column: 4),
				CreatePX1023MultipleFieldAttributesDiagnosticResult(line: 25, column: 4));

		[Theory]
		[EmbeddedFileData(@"Attributes\Diagnostics\FieldAttributesTypeMismatch\DacExampleFieldAttributesTypeMismatch.cs")]
		public virtual void Test_Dac_With_Property_Type_Not_Matching_Field_Attribute_Type(string source) =>
			VerifyCSharpDiagnostic(source,
				CreatePX1021FieldAttributeNotMatchingDacPropertyDiagnosticResult(line: 24, column: 4),
				CreatePX1021FieldAttributeNotMatchingDacPropertyDiagnosticResult(line: 26, column: 10),

				CreatePX1021FieldAttributeNotMatchingDacPropertyDiagnosticResult(line: 34, column: 4),
				CreatePX1021FieldAttributeNotMatchingDacPropertyDiagnosticResult(line: 35, column: 18));

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new DacPropertyAttributesAnalyzer();

		private DiagnosticResult CreatePX1021FieldAttributeNotMatchingDacPropertyDiagnosticResult(int line, int column)
		{
			return new DiagnosticResult
			{
				Id = Descriptors.PX1021_PXDBFieldAttributeNotMatchingDacProperty.Id,
				Message = Descriptors.PX1021_PXDBFieldAttributeNotMatchingDacProperty.Title.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", line, column)
					}
			};
		}

		private DiagnosticResult CreatePX1023MultipleFieldAttributesDiagnosticResult(int line, int column)
		{
			return new DiagnosticResult
			{
				Id = Descriptors.PX1023_DacPropertyMultipleFieldAttributes.Id,
				Message = Descriptors.PX1023_DacPropertyMultipleFieldAttributes.Title.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", line, column)
					}
			};
		}
	}
}
