﻿using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.HackathonDemo.ViewOrder
{
	public class ARInvoiceEntry : PXGraph<ARInvoiceEntry, ARInvoice>
	{
		public PXSelect<ARInvoice> ARInvoices;

		public PXSelect<SOTran> SOTrans;
	}
}
