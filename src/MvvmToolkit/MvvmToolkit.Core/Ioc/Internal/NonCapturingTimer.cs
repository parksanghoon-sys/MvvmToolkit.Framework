﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MvvmToolkit.Core.Ioc.Internal
{
    internal static class NonCapturingTimer
    {
        public static Timer Create(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            // Don't capture the current ExecutionContext and its AsyncLocals onto the timer
            bool restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                return new Timer(callback, state, dueTime, period);
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                {
                    ExecutionContext.RestoreFlow();
                }
            }
        }
    }
}
