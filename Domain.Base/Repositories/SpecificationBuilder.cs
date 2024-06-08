using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Repositories
{
    public abstract class SpecificationBuilder<T> : ISpecification<T>
    {
        public abstract Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        protected SpecificationBuilder()
        {
        }

        public static SpecificationBuilder<T> Create()
        {
            return new NullSpecification<T>();
        }

        public bool IsSatisfiedBy(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (Criteria == null)
            {
                throw new Exception("Criteria cannot be null");
            }

            var criteria = Criteria.Compile();
            return criteria(entity);
        }

        public T SatisfyingItemFrom(IQueryable<T> query)
        {
            return Prepare(query).SingleOrDefault();
        }

        public IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query)
        {
            return Prepare(query);
        }

        public IQueryable<T> Prepare(IQueryable<T> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (Criteria == null)
            {
                throw new Exception("Criteria cannot be null");
            }

            var q = query.Where(Criteria);
            return q;
        }

        public ISpecification<T> Init(Expression<Func<T, bool>> expression)
        {
            return new ExpressionSpecification<T>(expression);
        }

        public ISpecification<T> InitEmpty()
        {
            return new NullSpecification<T>();
        }

        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        public ISpecification<T> And(Expression<Func<T, bool>> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return new AndSpecification<T>(this, new ExpressionSpecification<T>(other));
        }

        public ISpecification<T> Or(ISpecification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return new OrSpecification<T>(this, new ExpressionSpecification<T>(other));
        }

        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        protected class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression from, to;

            public SwapVisitor(Expression from, Expression to)
            {
                this.from = from;
                this.to = to;
            }

            public override Expression Visit(Expression node)
            {
                return node == from ? to : base.Visit(node);
            }
        }



        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void ApplyPaging(int skip, int take)
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

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }
    }

    public class NullSpecification<T> : SpecificationBuilder<T>
    {
        public override Expression<Func<T, bool>> Criteria { get; }

        public NullSpecification()
        {
        }
    }

    public class ExpressionSpecification<T> : SpecificationBuilder<T>
    {
        private readonly Expression<Func<T, bool>> _Criteria;

        public override Expression<Func<T, bool>> Criteria { get => _Criteria; }

        public ExpressionSpecification(Expression<Func<T, bool>> Criteria)
        {
            _Criteria = Criteria;
        }
    }

    public class AndSpecification<T> : SpecificationBuilder<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public override Expression<Func<T, bool>> Criteria
        {
            get
            {
                return _left.Criteria != null ? And(_left.Criteria, _right.Criteria) : _right.Criteria;
            }
        }

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private static Expression<Func<T, bool>> And(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var visitor = new SwapVisitor(left.Parameters[0], right.Parameters[0]);
            var binaryExpression = Expression.AndAlso(visitor.Visit(left.Body), right.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(binaryExpression, right.Parameters);
            return lambda;
        }
    }

    public class OrSpecification<T> : SpecificationBuilder<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public override Expression<Func<T, bool>> Criteria
        {
            get
            {
                return _left.Criteria != null ? Or(_left.Criteria, _right.Criteria) : _right.Criteria;
            }
        }

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private static Expression<Func<T, bool>> Or(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var visitor = new SwapVisitor(left.Parameters[0], right.Parameters[0]);
            var binaryExpression = Expression.OrElse(visitor.Visit(left.Body), right.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(binaryExpression, right.Parameters);
            return lambda;
        }
    }

    public class NotSpecification<T> : SpecificationBuilder<T>
    {
        private readonly ISpecification<T> _left;

        public override Expression<Func<T, bool>> Criteria
        {
            get
            {
                return Not(_left.Criteria);
            }
        }

        public NotSpecification(ISpecification<T> left)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
        }

        private static Expression<Func<T, bool>> Not(Expression<Func<T, bool>> left)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            var notExpression = Expression.Not(left.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(notExpression, left.Parameters.Single());
            return lambda;
        }
    }
}
