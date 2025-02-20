using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Primitives
{
    //
    // 요약:
    //     Propagates notifications that a change has occurred.
    public interface IChangeToken
    {
        //
        // 요약:
        //     Gets a value that indicates if a change has occurred.
        bool HasChanged { get; }

        //
        // 요약:
        //     Indicates if this token will pro-actively raise callbacks. If false, the token
        //     consumer must poll Microsoft.Extensions.Primitives.IChangeToken.HasChanged to
        //     detect changes.
        bool ActiveChangeCallbacks { get; }

        //
        // 요약:
        //     Registers for a callback that will be invoked when the entry has changed. Microsoft.Extensions.Primitives.IChangeToken.HasChanged
        //     MUST be set before the callback is invoked.
        //
        // 매개 변수:
        //   callback:
        //     The System.Action`1 to invoke.
        //
        //   state:
        //     State to be passed into the callback.
        //
        // 반환 값:
        //     An System.IDisposable that is used to unregister the callback.
        IDisposable RegisterChangeCallback(Action<object?> callback, object? state);
    }
}
