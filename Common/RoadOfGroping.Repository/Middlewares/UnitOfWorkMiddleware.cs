using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            }
        }
    }
}

//有缺陷，暂时只做数据保存