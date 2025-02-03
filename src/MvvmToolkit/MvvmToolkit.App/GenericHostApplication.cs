﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MvvmToolkit.App.Helper;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace MvvmToolkit.App
{
    public abstract class GenericHostApplication : Application
    {
        private static IHost? Host { get; set; }
        private ILogger<GenericHostApplication>? _logger;
        private bool _canGenerateDump;
        private CoreDumpHelper.MiniDumpType _dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal;
        /// <summary>
        /// Dump가 발생했을때 덤프를 저장할 위치를 얻어옵니다.
        /// </summary>
        /// <returns>Dump의 저장 위치</returns>
        protected virtual string GetDumpPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            string? dirPath = Path.GetDirectoryName(assembly?.Location);
            string exeName = AppDomain.CurrentDomain.FriendlyName;
            string dateTime = DateTime.Now.ToString("[yyyy-MM-dd][HH-mm-ss-fff]", CultureInfo.InvariantCulture);

            return $"{dirPath}/[{exeName}]{dateTime}.dmp";
        }
        /// <summary>
        /// Crash 발생 시 Dump를 생성할 수 있는 옵션을 설정합니다.
        /// </summary>
        /// <param name="canGenerateDump">덤프를 생성할지 여부.</param>
        /// <param name="dumpType">생성할 덤프의 타입</param>
        protected void SetDumpOption(bool canGenerateDump, CoreDumpHelper.MiniDumpType dumpType = CoreDumpHelper.MiniDumpType.MiniDumpNormal)
        {
            _canGenerateDump = canGenerateDump;
            _dumpType = dumpType;
        }
        protected GenericHostApplication()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Dispatcher.UnhandledExceptionFilter += Dispatcher_UnhandledExceptionFilter;

            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
        /// <summary>
        /// Dispatchers the unhandled exception filter using the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Dispatcher_UnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
            try
            {
                e.RequestCatch = true;

                _logger?.LogError(e.Exception, $"[App Error Catch] {e.Exception}");
                _logger?.LogError(e.Exception, $"[App_DispatcherUnhandledExceptionFilter] {e.Exception.Message}");
            }
            catch
            {
                // ignored
            }
        }
        /// <summary>
        /// Dispatchers the unhandled exception using the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;

                _logger?.LogError(e.Exception, $"[App Error Catch] {e.Exception}");
                _logger?.LogError(e.Exception, $"[App_DispatcherUnhandledException] {e.Exception.Message}");
                if (_canGenerateDump)
                {
                    string dumpPath = GetDumpPath();
                    CoreDumpHelper.CreateMemoryDump(_dumpType, dumpPath);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
