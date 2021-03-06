﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kamilla.WPF
{
    /// <summary>
    /// Contains extension methods to the classes of the Windows Presentation Framework (WPF).
    /// </summary>
    public static class WpfExtensions
    {
        /// <summary>
        /// Thread-safely executes a function on a
        /// <see cref="System.Windows.Threading.DispatcherObject"/>.
        /// 
        /// The calling thread does not wait until the thread that is associated with
        /// <see cref="System.Windows.Threading.DispatcherObject"/> finished execution.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the <see cref="System.Windows.Threading.DispatcherObject"/> implementation.
        /// </typeparam>
        /// <param name="dispatcherObject">
        /// The <see cref="System.Windows.Threading.DispatcherObject"/> on which the function is executed.
        /// </param>
        /// <param name="action">
        /// The function to execute.
        /// </param>
        public static void ThreadSafe<T>(this T dispatcherObject, Action<T> action) where T : DispatcherObject
        {
            if (dispatcherObject == null)
                throw new ArgumentNullException("dispatcherObject");

            if (action == null)
                throw new ArgumentNullException("action");

            if (!dispatcherObject.Dispatcher.CheckAccess())
                dispatcherObject.Dispatcher.Invoke(action, dispatcherObject);
            else
                action(dispatcherObject);
        }

        /// <summary>
        /// Thread-safely executes a function on a
        /// <see cref="System.Windows.Threading.DispatcherObject"/>.
        /// 
        /// The calling thread does not wait until the thread that is associated with
        /// <see cref="System.Windows.Threading.DispatcherObject"/> finished execution.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the <see cref="System.Windows.Threading.DispatcherObject"/> implementation.
        /// </typeparam>
        /// <param name="dispatcherObject">
        /// The <see cref="System.Windows.Threading.DispatcherObject"/> on which the function is executed.
        /// </param>
        /// <param name="action">
        /// The function to execute.
        /// </param>
        public static void ThreadSafeBegin<T>(this T dispatcherObject, Action<T> action) where T : DispatcherObject
        {
            if (dispatcherObject == null)
                throw new ArgumentNullException("dispatcherObject");

            if (action == null)
                throw new ArgumentNullException("action");

            if (!dispatcherObject.Dispatcher.CheckAccess())
                dispatcherObject.Dispatcher.BeginInvoke(action, dispatcherObject);
            else
                action(dispatcherObject);
        }

        /// <summary>
        /// Thread-safely executes a function on a
        /// <see cref="System.Windows.Threading.DispatcherObject"/>
        /// and returns the result of the operation.
        /// </summary>
        /// <typeparam name="TDispatcherObject">
        /// Type of the <see cref="System.Windows.Threading.DispatcherObject"/> implementation.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// Type of the result of the function.
        /// </typeparam>
        /// <param name="dispatcherObject">
        /// The <see cref="System.Windows.Threading.DispatcherObject"/> on which
        /// the function is executed.
        /// </param>
        /// <param name="func">
        /// The function to execute.
        /// </param>
        /// <returns>
        /// Result of the function.
        /// </returns>
        public static TResult ThreadSafe<TDispatcherObject, TResult>
            (this TDispatcherObject dispatcherObject, Func<TDispatcherObject, TResult> func)
            where TDispatcherObject : DispatcherObject
        {
            if (dispatcherObject == null)
                throw new ArgumentNullException("control");

            if (func == null)
                throw new ArgumentNullException("func");

            if (!dispatcherObject.Dispatcher.CheckAccess())
                return (TResult)dispatcherObject.Dispatcher.Invoke(func, dispatcherObject);

            return func(dispatcherObject);
        }

        class SafeSetOwnerLambda
        {
            Window m_window;
            Window m_owner;

            public SafeSetOwnerLambda(Window window, Window owner)
            {
                m_window = window;
                m_owner = owner;
            }

            public void Window_Activated(object sender, EventArgs e)
            {
                m_window.Owner = m_owner;
                m_window.Activated -= Window_Activated;
            }
        }

        public static void SafeSetOwner(this Window window, Window owner)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            if (window == owner || owner.Owner == window)
                throw new ArgumentException();

            try
            {
                window.Owner = owner;
            }
            catch
            {
                // Manual closure implementation
                window.Activated += new SafeSetOwnerLambda(window, owner).Window_Activated;
            }
        }

        public static string GetDisplayString(this KeyGesture gesture)
        {
            var mods = gesture.Modifiers;

            if (mods != ModifierKeys.None)
            {
                var builder = new StringBuilder(32);

                if ((mods & ModifierKeys.Windows) != 0)
                    builder.Append("Win+");
                if ((mods & ModifierKeys.Alt) != 0)
                    builder.Append("Alt+");
                if ((mods & ModifierKeys.Control) != 0)
                    builder.Append("Ctrl+");
                if ((mods & ModifierKeys.Shift) != 0)
                    builder.Append("Shift+");

                builder.Append(gesture.Key.ToString());

                return builder.ToString();
            }

            return gesture.Key.ToString();
        }
    }
}
