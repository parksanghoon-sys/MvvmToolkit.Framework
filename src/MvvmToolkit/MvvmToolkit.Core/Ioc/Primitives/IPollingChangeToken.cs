using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Primitives
{
    internal interface IPollingChangeToken : IChangeToken
    {
        CancellationTokenSource? CancellationTokenSource { get; }
    }
}
