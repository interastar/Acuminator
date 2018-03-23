﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using PX.Analyzers.Vsix;
using PX.Analyzers.Vsix.Utilities;


namespace PX.Analyzers.Vsix
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	internal sealed class DescriptionFromResourcesAttribute : DescriptionAttribute
	{		
		public string ResourceKey { get; }

		public override string Description => VSIXResource.ResourceManager.GetStringResourceSafe(ResourceKey);

		public DescriptionFromResourcesAttribute(string resourceKey)
		{
			resourceKey.ThrowOnNullOrWhiteSpace(nameof(resourceKey));
			ResourceKey = resourceKey;
		}	
	}
}