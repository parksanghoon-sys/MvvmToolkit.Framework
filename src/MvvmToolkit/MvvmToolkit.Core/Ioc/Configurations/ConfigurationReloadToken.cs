using MvvmToolkit.Core.Ioc.Primitives;

namespace MvvmToolkit.Core.Ioc.Configurations
{
    public class ConfigurationReloadToken : IChangeToken
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        //
        // 요약:
        //     Indicates if this token will proactively raise callbacks. Callbacks are still
        //     guaranteed to be invoked, eventually.
        //
        // 반환 값:
        //     True if the token will proactively raise callbacks.
        public bool ActiveChangeCallbacks { get; private set; } = true;


        //
        // 요약:
        //     Gets a value that indicates if a change has occurred.
        //
        // 반환 값:
        //     True if a change has occurred.
        public bool HasChanged => _cts.IsCancellationRequested;

        //
        // 요약:
        //     Registers for a callback that will be invoked when the entry has changed. Microsoft.Extensions.Primitives.IChangeToken.HasChanged
        //     MUST be set before the callback is invoked.
        //
        // 매개 변수:
        //   callback:
        //     The callback to invoke.
        //
        //   state:
        //     State to be passed into the callback.
        //
        // 반환 값:
        //     The System.Threading.CancellationToken registration.
        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            return ChangeCallbackRegistrar.UnsafeRegisterChangeCallback(callback, state, _cts.Token, delegate (ConfigurationReloadToken s)
            {
                s.ActiveChangeCallbacks = false;
            }, this);
        }

        //
        // 요약:
        //     Used to trigger the change token when a reload occurs.
        public void OnReload()
        {
            _cts.Cancel();
        }

    }
}
