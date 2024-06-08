using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Repositories
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        protected BaseSpecification()
        {

        }
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        public virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
        public void ApplyPagingByPage(int page, int itemsPerPage)
        {
            if (itemsPerPage > 0)
            {
                page = page < 1 ? 0 : page - 1;
                Skip = itemsPerPage * page;
                Take = itemsPerPage;
                IsPagingEnabled = true;
            }
        }

        public virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        public virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        public ISpecification<T> And(ISpecification<T> specification)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> And(Expression<Func<T, bool>> right)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> right)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> Not()
        {
            throw new NotImplementedException();
        }

        public bool IsSatisfiedBy(T entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Prepare(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public T SatisfyingItemFrom(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> Init(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public ISpecification<T> InitEmpty()
        {
            throw new NotImplementedException();
        }
    }
}
