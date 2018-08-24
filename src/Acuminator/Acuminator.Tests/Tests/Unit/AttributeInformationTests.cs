﻿using Acuminator.Analyzers;
using Acuminator.Tests.Helpers;
using Acuminator.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using PX.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper;
using Xunit;

namespace Acuminator.Tests
{
	
	public class AttributeInformationTests : DiagnosticVerifier
	{
		/* 
		 *  Tests attribute derived 
		 * */
		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AttributeInformationSimpleDacTest.cs")]
		public void TestAttributeSimpleInformation(string source) =>
			TestAttributeInformation(source, new List<bool> { false, true, false, false, true, false } );

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateAttributeInformationTest.cs")]
		public void TestAggregateAttribute(string source) =>
			TestAttributeInformation(source, new List<bool> { true });

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateRecursiveAttributeInformationTest.cs")]
		public void TestAggregateRegursiveAttribute(string source) =>
			TestAttributeInformation(source, new List<bool> { true, false });
		
		private void TestAttributeInformation(string source, List<bool> expected)
		{
			Document document = CreateDocument(source);
			SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
			var syntaxRoot = document.GetSyntaxRootAsync().Result;

			List<bool> actual = new List<bool>();
			var pxContext = new PXContext(semanticModel.Compilation);

			var properties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>();

			foreach (var property in properties)
			{
				var typeSymbol = semanticModel.GetDeclaredSymbol(property);
				var attributes = typeSymbol.GetAttributes();
				foreach (var attribute in attributes)
				{
					var attributeInformation = new AttributeInformation(pxContext);
					var defaultAttribute = pxContext.AttributeTypes.PXDefaultAttribute;
					actual.Add(attributeInformation.AttributeDerivedFromClass(attribute.AttributeClass, defaultAttribute));
				}
			}
			Assert.Equal(expected, actual);
		}

		/*
		 * Tests IsBoundAttribute 
		 */

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AttributeInformationSimpleDacTest.cs")]
		public void TestAreBoundAttributes(string source) =>
			_testIsBoundAttribute(source, new List<bool> { false, false, false ,true, false, false });

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateAttributeInformationTest.cs")]
		public void TestAreBoundAggregateAttributes(string source) =>
			_testIsBoundAttribute(source, new List<bool> { true });

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateRecursiveAttributeInformationTest.cs")]
		public void TestAreBoundAggregateRecursiveAttribute(string source) =>
			_testIsBoundAttribute(source, new List<bool> { false, true });

		private void _testIsBoundAttribute(string source, List<bool> expected)
		{
			Document document = CreateDocument(source);
			SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
			var syntaxRoot = document.GetSyntaxRootAsync().Result;

			List<bool> actual = new List<bool>();
			var pxContext = new PXContext(semanticModel.Compilation);

			var properties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>();

			foreach (var property in properties)
			{
				var typeSymbol = semanticModel.GetDeclaredSymbol(property);
				var attributes = typeSymbol.GetAttributes();
				foreach (var attribute in attributes)
				{
					var attributeInformation = new AttributeInformation(pxContext);
					actual.Add(attributeInformation.IsBoundAttribute(attribute.AttributeClass));
				}
			}
			Assert.Equal(expected, actual);
		}

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateAttributeInformationTest.cs")]
		private void _testListOfChildrensClass(string source)
		{
			Document document = CreateDocument(source);
			SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
			var syntaxRoot = document.GetSyntaxRootAsync().Result;

			var pxContext = new PXContext(semanticModel.Compilation);

			var properties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>();
			IEnumerable<ITypeSymbol> classesArray = null;
			Dictionary<IEnumerable<ITypeSymbol>, int> result = new Dictionary<IEnumerable<ITypeSymbol>, int>();

			foreach (var property in properties)
			{
				var typeSymbol = semanticModel.GetDeclaredSymbol(property);
				var attributes = typeSymbol.GetAttributes();
				foreach (var attribute in attributes)
				{
					var attributeInformation = new AttributeInformation(pxContext);
					result.Add(classesArray = attributeInformation.AttributesListDerivedFromClass(attribute.AttributeClass),classesArray.Count());
					
				}
			}
			Assert.Equal(result, result);
		}

		[Theory]
		[EmbeddedFileData(@"Dac\PX1030\Unit\AggregateRecursiveAttributeInformationTest.cs")]
		private void _testListOfChildrensClassRecursive(string source)
		{
			Document document = CreateDocument(source);
			SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
			var syntaxRoot = document.GetSyntaxRootAsync().Result;

			var pxContext = new PXContext(semanticModel.Compilation);

			var properties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>();
			IEnumerable<ITypeSymbol> classesArray = null;
			Dictionary<IEnumerable<ITypeSymbol>, int> result = new Dictionary<IEnumerable<ITypeSymbol>, int>();

			foreach (var property in properties)
			{
				var typeSymbol = semanticModel.GetDeclaredSymbol(property);
				var attributes = typeSymbol.GetAttributes();
				foreach (var attribute in attributes)
				{
					var attributeInformation = new AttributeInformation(pxContext);
					result.Add(classesArray = attributeInformation.AttributesListDerivedFromClass(attribute.AttributeClass), classesArray.Count());

				}
			}
			Assert.Equal(result, result);
		}


	}
}
