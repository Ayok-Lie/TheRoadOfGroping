using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using RoadOfGroping.Common.Attributes;
using RoadOfGroping.Repository.UnitOfWorks;

namespace RoadOfGroping.Repository.Middlewares
{
    public class UnitOfWorkMiddleware : IMiddleware
    {
        public UnitOfWorkMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var unitOfWorkAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.GetMetadata<DisabledUnitOfWorkAttribute>();

            if (unitOfWorkAttribute?.Disabled == true)
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
            else
            {
                // 获取服务中多个DbContext
                var unitOfWorks = context.RequestServices.GetServices<IUnitOfWork>();
                foreach (var unitOfWork in unitOfWorks)
                {
                    // 开启事务
                    await unitOfWork.BeginTransactionAsync();
                }
                try
                {
                    await next.Invoke(context);
                    foreach (var unitOfWork in unitOfWorks)
                    {
                        // 提交事务
                        await unitOfWork.CommitTransactionAsync();
                    }
                }
                catch (Exception ex)
                {
                    foreach (var d in unitOfWorks)
                    {
                        await d.RollbackTransactionAsync();
                    }
                    throw;
                }
            }
        }
    }
}

//有缺陷，暂时只做数据保存
//报错，数据回滚的时候无法显示错误信息