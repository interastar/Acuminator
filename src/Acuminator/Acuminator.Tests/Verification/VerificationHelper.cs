﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using PX.Data;

namespace Acuminator.Tests.Verification
{
	public static class VerificationHelper
	{
		private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
		private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
		private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
		private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
		private static readonly MetadataReference PXDataReference = MetadataReference.CreateFromFile(typeof(PXGraph).Assembly.Location);
		private static readonly MetadataReference PXCommonReference = MetadataReference.CreateFromFile(typeof(PX.Common.PXContext).Assembly.Location);

		internal static string DefaultFilePathPrefix = "Test";
		internal static string CSharpDefaultFileExt = "cs";
		internal static string VisualBasicDefaultExt = "vb";
		internal static string TestProjectName = "TestProject";


		/// <summary>
		/// Given an array of strings as sources and a language, turn them into a project and return the documents and spans of it.
		/// </summary>
		/// <param name="sources">Classes in the form of strings</param>
		/// <param name="language">The language the source code is in</param>
		/// <returns>A Tuple containing the Documents produced from the sources and their TextSpans if relevant</returns>
		public static Document[] GetDocuments(string[] sources, string language)
		{
			if (language != LanguageNames.CSharp && language != LanguageNames.VisualBasic)
			{
				throw new ArgumentException("Unsupported Language");
			}

			var project = CreateProject(sources, language);
			var documents = project.Documents.ToArray();

			if (sources.Length != documents.Length)
			{
				throw new SystemException("Amount of sources did not match amount of Documents created");
			}

			return documents;
		}

		/// <summary>
		/// Create a Document from a string through creating a project that contains it.
		/// </summary>
		/// <param name="source">Classes in the form of a string</param>
		/// <param name="language">The language the source code is in</param>
		/// <returns>A Document created from the source string</returns>
		public static Document CreateDocument(string source, string language = LanguageNames.CSharp)
		{
			return CreateProject(new[] { source }, language).Documents.First();
		}

		public static Document CreateCSharpDocument(string source, params string[] additionalSources)
		{
			var sources = new List<string>(additionalSources) { source };
			return CreateProject(sources.ToArray()).Documents.Last();
		}

		/// <summary>
		/// Create a project using the inputted strings as sources.
		/// </summary>
		/// <param name="sources">Classes in the form of strings</param>
		/// <param name="language">The language the source code is in</param>
		/// <returns>A Project created out of the Documents created from the source strings</returns>
		private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
		{
			string fileNamePrefix = DefaultFilePathPrefix;
			string fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

			var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

			var workspace = new AdhocWorkspace();
			workspace.Options = workspace.Options.WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, true)
												 .WithChangedOption(FormattingOptions.SmartIndent, LanguageNames.CSharp, FormattingOptions.IndentStyle.Smart)
												 .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4)
												 .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4);

			var solution = workspace.CurrentSolution
									.AddProject(projectId, TestProjectName, TestProjectName, language)
									.AddMetadataReference(projectId, CorlibReference)
									.AddMetadataReference(projectId, SystemCoreReference)
									.AddMetadataReference(projectId, CSharpSymbolsReference)
									.AddMetadataReference(projectId, CodeAnalysisReference)
									.AddMetadataReference(projectId, PXDataReference)
									.AddMetadataReference(projectId, PXCommonReference);

			var project = solution.GetProject(projectId);
			var parseOptions = project.ParseOptions.WithFeatures(
				project.ParseOptions.Features.Union(new[] { new KeyValuePair<string, string>("IOperation", "true") }));
			solution = solution.WithProjectParseOptions(projectId, parseOptions);

			int count = 0;

			foreach (var source in sources)
			{
				var newFileName = fileNamePrefix + count + "." + fileExt;
				var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
				solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
				count++;
			}

			return solution.GetProject(projectId);
		}


		/// <summary>
		/// Given a document, turn it into a string based on the syntax root
		/// </summary>
		/// <param name="document">The Document to be converted to a string</param>
		/// <returns>A string containing the syntax of the Document after formatting</returns>
		public static string GetStringFromDocument(Document document)
		{
			var simplifiedDoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
			var root = simplifiedDoc.GetSyntaxRootAsync().Result;
			root = Formatter.Format(root, Formatter.Annotation, simplifiedDoc.Project.Solution.Workspace);
			return root.GetText().ToString();
		}

		/// <summary>
		/// Apply the inputted CodeAction to the inputted document.
		/// </summary>
		/// <param name="document">The Document to apply the fix on</param>
		/// <param name="codeAction">A CodeAction that will be applied to the Document.</param>
		/// <returns>A Document with the changes from the CodeAction</returns>
		public static Document ApplyCodeAction(Document document, CodeAction codeAction)
		{
			var operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
			var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
			return solution.GetDocument(document.Id);
		}


		/// <summary>
		/// Compare two collections of Diagnostics,and return a list of any new diagnostics that appear only in the second collection.
		/// Note: Considers Diagnostics to be the same if they have the same Ids.  In the case of multiple diagnostics with the same Id in a row,
		/// this method may not necessarily return the new one.
		/// </summary>
		/// <param name="diagnostics">The Diagnostics that existed in the code before the CodeFix was applied</param>
		/// <param name="newDiagnostics">The Diagnostics that exist in the code after the CodeFix was applied</param>
		/// <returns>A list of Diagnostics that only surfaced in the code after the CodeFix was applied</returns>
		public static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics, IEnumerable<Diagnostic> newDiagnostics)
		{
			var oldArray = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
			var newArray = newDiagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();

			int oldIndex = 0;
			int newIndex = 0;

			while (newIndex < newArray.Length)
			{
				if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
				{
					++oldIndex;
					++newIndex;
				}
				else
				{
					yield return newArray[newIndex++];
				}
			}
		}

		/// <summary>
		/// Get the existing compiler diagnostics on the inputted document.
		/// </summary>
		/// <param name="document">The Document to run the compiler diagnostic analyzers on</param>
		/// <returns>The compiler diagnostics that were found in the code</returns>
		public static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
		{
			return document.GetSemanticModelAsync().Result.GetDiagnostics();
		}
	}
}