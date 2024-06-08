using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.BackgroundJob
{
    public interface IBackgroundJob
    {

        string Enqueue(Expression<Action> methodCall);
        
        string Enqueue(Expression<Func<Task>> methodCall);

        string Enqueue<T>(Expression<Action<T>> methodCall);

        string Enqueue<T>(Expression<Func<T, Task>> methodCall);
        
        string Schedule(
            Expression<Action> methodCall,
            TimeSpan delay);

        string Schedule(
            Expression<Func<Task>> methodCall,
            TimeSpan delay);

        string Schedule(
            Expression<Action> methodCall,
            DateTimeOffset enqueueAt);

        string Schedule(
            Expression<Func<Task>> methodCall,
            DateTimeOffset enqueueAt);

        string Schedule<T>(
            Expression<Action<T>> methodCall,
            TimeSpan delay);

        string Schedule<T>(
            Expression<Func<T, Task>> methodCall,
            TimeSpan delay);

        string Schedule<T>(
            Expression<Action<T>> methodCall,
            DateTimeOffset enqueueAt);

        string Schedule<T>(
            Expression<Func<T, Task>> methodCall,
            DateTimeOffset enqueueAt);

        bool Delete(string jobId);
        
        bool Delete(string jobId, string fromState);

        bool Requeue(string jobId);
        
        bool Requeue(string jobId, string fromState);

        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith(
            string parentId,
            Expression<Action> methodCall);

        string ContinueJobWith(
            string parentId,
            Expression<Action> methodCall);

        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith<T>(
            string parentId,
            Expression<Action<T>> methodCall);

        string ContinueJobWith<T>(
            string parentId,
            Expression<Action<T>> methodCall);
        
        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith(
            string parentId,
            Expression<Action> methodCall,
            JobContinuationOptions options);

        string ContinueJobWith(
            string parentId,
            Expression<Action> methodCall,
            JobContinuationOptions options);

        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith(
            string parentId,
            Expression<Func<Task>> methodCall,
            JobContinuationOptions options = JobContinuationOptions.OnlyOnSucceededState);

        string ContinueJobWith(
            string parentId,
            Expression<Func<Task>> methodCall,
            JobContinuationOptions options = JobContinuationOptions.OnlyOnSucceededState);

        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith<T>(
            string parentId,
            Expression<Action<T>> methodCall,
            JobContinuationOptions options);
        string ContinueJobWith<T>(
            string parentId,
            Expression<Action<T>> methodCall,
            JobContinuationOptions options);
        [Obsolete("Deprecated for clarity, please use ContinueJobWith method with the same arguments. Will be removed in 2.0.0.")]
        string ContinueWith<T>(
            string parentId,
            Expression<Func<T, Task>> methodCall,
            JobContinuationOptions options = JobContinuationOptions.OnlyOnSucceededState);

        string ContinueJobWith<T>(
            string parentId,
            Expression<Func<T, Task>> methodCall,
            JobContinuationOptions options = JobContinuationOptions.OnlyOnSucceededState);
    }
}
