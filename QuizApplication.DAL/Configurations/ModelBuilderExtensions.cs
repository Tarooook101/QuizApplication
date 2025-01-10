using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuizApplication.DAL.Configurations
{
    public static class ModelBuilderExtensions
    {
        public static void SetQueryFilterOnAllEntities<TInterface>(
            this ModelBuilder builder,
            Expression<Func<TInterface, bool>> filterExpression)
        {
            var entities = builder.Model.GetEntityTypes()
                .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                .Select(e => e.ClrType);

            foreach (var entity in entities)
            {
                var parameter = Expression.Parameter(entity);
                var body = ReplacingExpressionVisitor.Replace(
                    filterExpression.Parameters.First(),
                    parameter,
                    filterExpression.Body);
                var lambda = Expression.Lambda(body, parameter);
                builder.Entity(entity).HasQueryFilter(lambda);
            }
        }
    }
}
