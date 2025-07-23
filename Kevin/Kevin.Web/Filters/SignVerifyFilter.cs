﻿using Kevin.Common.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Web.Filters
{

    /// <summary>
    /// 签名过滤器
    /// </summary>
    public class SignVerifyFilter : Attribute, IActionFilter
    {


        /// <summary>
        /// 是否跳过签名验证，可用于控制器下单个接口指定跳过签名验证
        /// </summary>
        public bool IsSkip { get; set; }


        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {

            var filter = (SignVerifyFilter)context.Filters.Where(t => t.ToString() == (typeof(SignVerifyFilter).Assembly.GetName().Name + ".Filters.TokenVerify")).ToList().LastOrDefault();


            if (!filter.IsSkip)
            {
                var token = context.HttpContext.Request.Headers["Token"].ToString().ToLower();

                var rip = context.HttpContext.Connection.RemoteIpAddress.ToString();

                if (!rip.Contains("127.0.0.1"))
                {
                    var timeStr = context.HttpContext.Request.Headers["Time"].ToString();
                    var time = Common.DateTimeHelper.JsToTime(long.Parse(timeStr));

                    if (time.AddMinutes(10) > DateTime.Now)
                    {
                        string privatekey = "gPmgRr9Dp3wzubTaGIgmMSpfNiKqkIAA0C8gkaBSN0ca3GWxk3W6682KuXRpxnDq";

                        string strdata = privatekey + timeStr;


                        if (context.HttpContext.Request.Method == "POST")
                        {
                            string body = KevinHttpContext.GetRequestBody(context.HttpContext.RequestServices.GetService<IHttpContextAccessor>());

                            if (!string.IsNullOrEmpty(body))
                            {
                                body = Common.StringHelper.NoEmoji(body);
                                strdata = strdata + body;
                            }
                            else if (context.HttpContext.Request.HasFormContentType)
                            {
                                var fromlist = context.HttpContext.Request.Form.OrderBy(t => t.Key).ToList();

                                foreach (var fm in fromlist)
                                {
                                    string fmv = Common.StringHelper.NoEmoji(fm.Value.ToString());
                                    strdata = strdata + fm.Key + fmv;
                                }
                            }
                        }
                        else if (context.HttpContext.Request.Method == "GET")
                        {
                            var queryList = context.HttpContext.Request.Query.OrderBy(t => t.Key).ToList();

                            foreach (var query in queryList)
                            {
                                string qv = Common.StringHelper.NoEmoji(query.Value);
                                strdata = strdata + query.Key + qv;
                            }
                        }


                        string tk = Common.CryptoHelper.GetMd5(strdata).ToLower();

                        if (token != tk)
                        {
                            context.HttpContext.Response.StatusCode = 401;

                            context.Result = new JsonResult(new { errMsg = "非法 Token ！" });
                        }
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = 401;
                        context.Result = new JsonResult(new { errMsg = "Token 有效期以过！" });
                    }
                }
            }

        }


        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
