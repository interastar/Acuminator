﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Analyzers.Test.Sources
{
	public class TypoInViewDelegateName_Bad : PXGraph<TypoInViewDelegateName_Bad>
	{
		public PXSelect<TypoInViewDelegateName_Bad_DAC> Documents;
		public PXSelect<TypoInViewDelegateName_Bad_DAC> CurrentDocument;

		public IEnumerable documents()
		{
			yield break;
		}

		public IEnumerable currentDocument()
		{
			return Documents.Current;
		}
	}

	public class TypoInViewDelegateName_Bad_DAC : IBqlTable
	{
	}
}
