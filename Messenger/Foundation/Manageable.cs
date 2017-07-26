﻿using System;

namespace Messenger.Foundation
{
    /// <summary>
    /// 生命周期可控的对象接口
    /// </summary>
    public interface IManageable : IDisposable
    {
        bool CanStart { get; }
        bool IsStarted { get; }
        bool IsDisposed { get; }
        void Start();
    }

    /// <summary>
    /// 生命周期可控的对象
    /// </summary>
    public abstract class Manageable : IManageable
    {
        protected bool _started = false;
        protected bool _disposed = false;
        protected object _loc = new object();

        public virtual bool CanStart => IsStarted == false && IsDisposed == false;
        public virtual bool IsStarted => _started;
        public virtual bool IsDisposed => _disposed;

        public virtual void Start() { }
        public void Dispose() { lock (_loc) { Dispose(true); } }
        protected abstract void Dispose(bool flag);
    }
}
