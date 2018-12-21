﻿using Acuminator.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace Acuminator.Utilities.DiagnosticSuppression
{
	public class SuppressionFile
	{
		private const string SuppressMessageElement = "suppressMessage";
		private const string IdAttribute = "id";
		private const string TargetElement = "target";
		private const string SyntaxNodeElement = "syntaxNode";
		private const string SuppressionFileExtension = ".acuminator";

		internal string AssemblyName { get; }

		internal string Path { get; }

		internal bool GenerateSuppressionBase { get; }

		private HashSet<SuppressMessage> Messages { get; }

		private SuppressionFile(string assemblyName, string path, bool generateSuppressionBase, HashSet<SuppressMessage> messages)
		{
			AssemblyName = assemblyName;
			Path = path;
			GenerateSuppressionBase = generateSuppressionBase;
			Messages = messages;
		}

		internal bool ContainsMessage(SuppressMessage message)
		{
			return Messages.Contains(message);
		}

		internal static bool IsSuppressionFile(string path)
		{
			return SuppressionFileExtension.Equals(System.IO.Path.GetExtension(path), StringComparison.Ordinal);
		}

		internal static SuppressionFile Load(ISuppressionFileSystemService fileSystemService,
			(string path, bool generateSuppressionBase) loadInfo)
		{
			loadInfo.path.ThrowOnNull(nameof(loadInfo.path));

			string assemblyName = fileSystemService.GetFileName(loadInfo.path);

			if (string.IsNullOrEmpty(assemblyName))
			{
				throw new FormatException("Acuminator suppression file name cannot be empty");
			}

			var messages = new HashSet<SuppressMessage>();

			if (!loadInfo.generateSuppressionBase)
			{
				messages = LoadMessages(fileSystemService, loadInfo.path);
			}

			return new SuppressionFile(assemblyName, loadInfo.path, loadInfo.generateSuppressionBase, messages);
		}

		internal void AddMessage(SuppressMessage message)
		{
			Messages.Add(message);
		}

		internal XDocument MessagesToDocument()
		{
			var document = XDocument.Load(Path);
			var root = document.Root;

			root.RemoveNodes();

			foreach (var message in Messages)
			{
				root.Add(ElementFromMessage(message));
			}

			return document;
		}

		private static XElement ElementFromMessage(SuppressMessage message)
		{
			return new XElement(SuppressMessageElement,
				new XAttribute(IdAttribute, message.Id),
				new XElement(TargetElement, message.Target),
				new XElement(SyntaxNodeElement, message.SyntaxNode));
		}

		private static SuppressMessage MessageFromElement(XElement element)
		{
			var id = element.Attribute(IdAttribute).Value;
			var targetElement = element.Element(TargetElement);
			var target = targetElement.Value;
			var syntaxNode = element.Element(SyntaxNodeElement).Value;

			return new SuppressMessage(id, target, syntaxNode);
		}

		private static HashSet<SuppressMessage> LoadMessages(ISuppressionFileSystemService fileSystemService, string path)
		{
			var messages = new HashSet<SuppressMessage>();
			var document = fileSystemService.Load(path);
			var messageElements = document.Root.Elements(SuppressMessageElement);

			foreach (var element in messageElements)
			{
				messages.Add(MessageFromElement(element));
			}

			return messages;
		}
	}
}
