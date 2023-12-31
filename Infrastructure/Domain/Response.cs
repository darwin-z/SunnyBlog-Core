﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Response
    {
        /// <summary>
        /// 操作消息 当Status不为200时，显示详细的错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 操作状态码，200正常
        /// </summary>
        public int Status { get; set; }
        public Response()
        {
            Status = 200;
            Message = "操作成功";
        }
    }

    /// <summary>
    /// WebApi通用返回泛型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// 回传结果
        /// </summary>
        public T Result { get; set; }
    }
}
