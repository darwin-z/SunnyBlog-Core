﻿using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.App.Interface;
using UserService.Response;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowApp followApp;

        public FollowController(IFollowApp followApp)
        {
            this.followApp = followApp;
        }

        /// <summary>
        /// 关注某用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sbId"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*follow*"} })]
        public async Task<Response<string>> Sb([FromQuery]int id)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.FollowSb(Convert.ToInt32(userId), id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 取消关注某用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*follow*" } })]
        public async Task<Response<string>> Cancel([FromQuery]int id)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.CancelFollowSb(Convert.ToInt32(userId), id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
        
        /// <summary>
        /// 查看用户的关注列表
        /// </summary>
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<List<FollowView>>> List(int id, string? condidtion = null)
        {
            var result = new Response<List<FollowView>>();
            try
            {
                List<SearchCondition> con = new List<SearchCondition>();
                try { con = JsonConvert.DeserializeObject<List<SearchCondition>>(condidtion); }
                catch (Exception) { }
                result.Result = await followApp.FollowList(con, id);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取关注状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<bool>> Status(int id)
        {
            var result = new Response<bool>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await followApp.FollowStatus(Convert.ToInt32(userId), id);
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
