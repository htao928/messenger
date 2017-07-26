﻿using Messenger.Foundation;
using Messenger.Modules;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Messenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public event EventHandler<KeyEventArgs> TextBoxKeyDown;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Trace.Listeners.Add(new Logger($"{nameof(Messenger)}-{DateTime.Now:yyyyMMdd}.log"));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.KeyDownEvent, new KeyEventHandler((s, arg) => TextBoxKeyDown?.Invoke(s, arg)));

            DispatcherUnhandledException += (s, arg) =>
            {
                arg.Handled = true;
                Fallback.Show(null, "Unhandled Exception", arg.Exception);
                Shutdown(1);
            };

            SessionEnding += (s, arg) =>
            {
                Interact.Close();
            };
        }
    }
}
