using Microsoft.Extensions.Hosting;
using MvvmToolkit.App.Helper;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MvvmToolkit.App
{
    public abstract class GenericHostApplication : Application
    {
        private static IHost? Host { get; set; }
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

    }
}
