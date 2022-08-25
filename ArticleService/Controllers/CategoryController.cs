﻿using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IArticleCategoryApp articleCategoryApp;

        public CategoryController(IArticleCategoryApp articleCategoryApp)
        {
            this.articleCategoryApp = articleCategoryApp;
        }

        /// <summary>
        /// 获取用户的分类
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<List<CategoryView>>> My()
        {
            var result = new Response<List<CategoryView>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleCategoryApp.GetUserCategory(Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message; 
            }
            return result;
        }

        /// <summary>
        /// 更新分类
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Update(UpdateCategoryReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleCategoryApp.UpdateCategory(request,Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 创建分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        public async Task<Response<string>> Create(AddCategoryReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleCategoryApp.AddCategory(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        public async Task<Response<string>> Delete(List<DelCategoryReq> request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await articleCategoryApp.DeletelCategory(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
