﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects
{
	public class ARInvoiceEntryExt : PXGraphExtension<ARInvoiceEntry>
	{
		public ARInvoice GetInvoice(string refNbr)
		{
			var invoice = PXSelect<ARInvoice, Where<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>
				.Select(PXGraph.CreateInstance<ARInvoiceEntry>(), refNbr);
		}
	}

	public class ARInvoiceEntry : PXGraph<ARInvoiceEntry, ARInvoice>
	{
	}

	public class ARInvoice : IBqlTable
	{
		#region RefNbr
		[PXDBString(8, IsKey = true, InputMask = "")]
		public string RefNbr { get; set; }
		public abstract class refNbr : IBqlField { }
		#endregion
	}
}