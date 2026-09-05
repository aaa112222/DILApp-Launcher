using System;
using System.Windows;
using System.Windows.Threading;

namespace DILApp;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.DispatcherUnhandledException += App_DispatcherUnhandledException;
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		base.OnStartup(e);
	}

	private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		try
		{
			string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DILApp_crash.log");
			System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DispatcherUnhandled: {e.Exception}\n\n");
		}
		catch { }
		e.Handled = true;
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DILApp_crash.log");
			System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Unhandled: {e.ExceptionObject}\nIsTerminating: {e.IsTerminating}\n\n");
		}
		catch { }
	}
}