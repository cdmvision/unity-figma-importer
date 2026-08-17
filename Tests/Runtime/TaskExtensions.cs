using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Cdm.Figma.Tests
{
    public static class TaskExtensions
    {
        public static async Task<bool> Throws<T>(this Task task) where T : Exception
        {
            try
            {
                await task;
            }
            catch (T)
            {
                return true;
            }
 
            return false;
        }
        
        public static IEnumerator AsEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
 
            yield return null;
        }
        
        public static IEnumerator AsEnumerator<T>(this Task<T> task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
 
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
 
            yield return null;
        }
        
        public static void WaitOrThrow(this Task task)
        {
            try
            {
                task.GetAwaiter().GetResult();
            }
            catch (AggregateException ex)
            {
                ex.Handle(inner => throw inner);
            }
        }
        
        public static T WaitOrThrow<T>(this Task<T> task)
        {
            try
            {
                return task.GetAwaiter().GetResult();
            }
            catch (AggregateException ex)
            {
                ex.Handle(inner => throw inner);
            }

            return default;
        }
    }
}