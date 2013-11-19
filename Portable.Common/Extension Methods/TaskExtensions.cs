﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task<T> WaitNoLongerThan<T>(this Task<T> task, int delayInMilliseconds)
        {
            return WaitNoLongerThan(task, TimeSpan.FromMilliseconds(delayInMilliseconds));
        }

        public static Task<T> WaitNoLongerThan<T>(this Task<T> task, TimeSpan delay)
        {
            return WaitNoLongerThan(task, delay, CancellationToken.None);
        }

        public static Task<T> WaitNoLongerThan<T>(this Task<T> task, TimeSpan delay, CancellationToken cancelToken)
        {
            return Task.WhenAny(task, Task.Delay(delay, cancelToken)).ContinueWith(o => task.Result, cancelToken);
        }

        public static Task WaitNoLongerThan(this Task task, int delayInMilliseconds)
        {
            return WaitNoLongerThan(task, TimeSpan.FromMilliseconds(delayInMilliseconds));
        }

        public static Task WaitNoLongerThan(this Task task, TimeSpan delay)
        {
            return WaitNoLongerThan(task, delay, CancellationToken.None);
        }

        public static Task WaitNoLongerThan(this Task task, TimeSpan delay, CancellationToken cancelToken)
        {
            return Task.WhenAny(task, Task.Delay(delay, cancelToken));
        }

        public static T WaitOnResult<T>(this Task<T> task)
        {
            if (task.IsCompleted)
                return task.Result;
            else
            {
                task.Wait();
                return task.Result;
            }
        }

        public static TaskAwaiter GetAwaiter(this TimeSpan span)
        {
            return Task.Delay(span).GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks).GetAwaiter();
        }

        public static async void Fire(this Task task, Action<Exception> onException = null)
        {
            try
            {
                await Task.Run(async () => await task);
            }
            catch(Exception e)
            {
                if (onException != null)
                    onException(e);
            }
        }
    }
}
