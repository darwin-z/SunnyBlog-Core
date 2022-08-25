﻿using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Request;
using UserService.Response;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoApp photoApp;
        public PhotoController(IPhotoApp photoApp)
        {
            this.photoApp = photoApp;
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<UploadResult>> Upload(UploadPhotoReq request)
        {
            var result = new Response<UploadResult>();
            try
            {
                //获取token中的用户ID
                var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value);
                result.Result = await photoApp.UploadPhoto(request,Convert.ToInt32(userId));
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