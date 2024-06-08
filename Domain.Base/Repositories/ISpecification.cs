using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Repositories
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        Expression<Func<T, object>> OrderBy { get; }
        Expression<Func<T, object>> OrderByDescending { get; }
        Expression<Func<T, object>> GroupBy { get; }



        bool IsSatisfiedBy(T entity);

        IQueryable<T> Prepare(IQueryable<T> query);

        T SatisfyingItemFrom(IQueryable<T> query);

        IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query);

        ISpecification<T> Init(Expression<Func<T, bool>> expression);

        ISpecification<T> InitEmpty();
        ISpecification<T> And(ISpecification<T> specification);

        ISpecification<T> And(Expression<Func<T, bool>> right);

        ISpecification<T> Or(ISpecification<T> specification);

        ISpecification<T> Or(Expression<Func<T, bool>> right);

        ISpecification<T> Not();


        public void AddInclude(Expression<Func<T, object>> includeExpression);

        public void AddInclude(string includeString);

        public void ApplyPaging(int skip, int take);
        public void ApplyPagingByPage(int page, int itemsPerPage);

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression);

        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);
        public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression);

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
