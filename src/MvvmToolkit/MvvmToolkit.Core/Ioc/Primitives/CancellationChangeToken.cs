using MvvmToolkit.Core.Ioc.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Primitives
{
    public class CancellationChangeToken : IChangeToken
    {
        public bool ActiveChangeCallbacks { get; private set; } = true;


        public bool HasChanged => Token.IsCancellationRequested;

        private CancellationToken Token { get; }
        public CancellationChangeToken(CancellationToken cancellationToken)
        {
            Token = cancellationToken;
        }

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            return ChangeCallbackRegistrar.UnsafeRegisterChangeCallback(callback, state, Token, delegate (CancellationChangeToken s)
            {
                s.ActiveChangeCallbacks = false;
            }, this);
        }
    }
}
