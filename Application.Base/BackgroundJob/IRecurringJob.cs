using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.BackgroundJob
{
    public interface IRecurringJob
    {
        public void AddOrUpdate(
            Expression<Action> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);
        public void AddOrUpdate<T>(
            Expression<Action<T>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate(
            Expression<Action> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            Expression<Action<T>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate(
            string recurringJobId,
            Expression<Action> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            string recurringJobId,
            Expression<Action<T>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue)
        {
            AddOrUpdate(recurringJobId, methodCall, cronExpression(), timeZone, queue);
        }

        public void AddOrUpdate(
            string recurringJobId,
            Expression<Action> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);
        public void AddOrUpdate<T>(
            string recurringJobId,
            Expression<Action<T>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate(
            Expression<Func<Task>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            Expression<Func<T, Task>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue)
        {
            AddOrUpdate(methodCall, cronExpression(), timeZone, queue);
        }

        public void AddOrUpdate(
            Expression<Func<Task>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            Expression<Func<T, Task>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate(
            string recurringJobId,
            Expression<Func<Task>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            string recurringJobId,
            Expression<Func<T, Task>> methodCall,
            Func<string> cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate(
            string recurringJobId,
            Expression<Func<Task>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void AddOrUpdate<T>(
            string recurringJobId,
            Expression<Func<T, Task>> methodCall,
            string cronExpression,
            TimeZoneInfo timeZone = null,
            string queue = EnqueuedState.DefaultQueue);

        public void RemoveIfExists(string recurringJobId);
        public void Trigger(string recurringJobId);
    }
}
