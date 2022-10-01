﻿using HunterPie.UI.Architecture.Dispatcher;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace HunterPie.Features.Notification;
internal class AppNotificationManager
{
    private static long _count = 0;
    private static readonly ConcurrentDictionary<long, FrameworkElement> _timeouts = new();
    public static ObservableCollection<FrameworkElement> Notifications { get; } = new();

    public static void Push(FrameworkElement notification, TimeSpan timeout)
    {
        _count++;

        if (_timeouts.TryAdd(_count, notification))
            Notifications.Add(notification);

        new DispatcherJob(() =>
        {
            if (_timeouts.TryRemove(KeyValuePair.Create(_count, notification)))
                Notifications.Remove(notification);
        }, timeout);

    }
}
