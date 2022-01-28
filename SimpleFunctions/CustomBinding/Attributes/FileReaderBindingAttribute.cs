﻿using Microsoft.Azure.WebJobs.Description;
using System;

namespace CustomBinding
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class FileReaderBindingAttribute : Attribute
    {
        [AutoResolve]
        public string Location { get; set; }
    }
}
