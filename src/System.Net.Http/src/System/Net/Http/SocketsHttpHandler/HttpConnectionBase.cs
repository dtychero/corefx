﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal abstract class HttpConnectionBase
    {
        public abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        internal abstract void Trace(string message, string memberName = null);

        private long CreationTickCount { get; } = Environment.TickCount64;

        // Check if lifetime expired on connection.
        public bool LifetimeExpired(long nowTicks, TimeSpan lifetime)
        {
            bool expired =
                lifetime != Timeout.InfiniteTimeSpan &&
                (lifetime == TimeSpan.Zero || (nowTicks - CreationTickCount) > lifetime.TotalMilliseconds);

            if (expired && NetEventSource.IsEnabled) Trace($"Connection no longer usable. Alive {TimeSpan.FromMilliseconds((nowTicks - CreationTickCount))} > {lifetime}.");
            return expired;
        }
    }
}
