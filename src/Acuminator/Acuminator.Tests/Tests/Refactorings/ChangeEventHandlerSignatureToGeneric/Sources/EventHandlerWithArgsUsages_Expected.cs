﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects
{
	public class SOInvoiceEntry : PXGraph<SOInvoiceEntry, SOInvoice>
	{
		protected virtual void _(Events.RowSelected<SOInvoice> args)
		{
			args.Cache.RaiseExceptionHandling<SOInvoice.refNbr>(args.Row, null, new PXSetPropertyException("Invalid number"));
		}
	}

	public class SOInvoice : IBqlTable
	{
		#region RefNbr
		[PXDBString(8, IsKey = true, InputMask = "")]
		public string RefNbr { get; set; }
		public abstract class refNbr : IBqlField { }
		#endregion
	}
}