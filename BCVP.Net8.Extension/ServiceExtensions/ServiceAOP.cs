using BCVP.Net8.Common;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BCVP.Net8.Extension.ServiceExtensions
{
    /// <summary>
    /// 攔截器 AOP 
    /// </summary>
    /// IInterceptor 重點為實現 Castle.Core
    public class ServiceAOP : IInterceptor
    {
        /// <summary>
        /// 實例化 IInterceptor 唯一方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            // 事前
            string json;
            try
            {
                json = JsonConvert.SerializeObject(invocation.Arguments);
            }
            catch (Exception ex)
            {
                json = "無法序列化，可能是lambda表達式等原因造成，請重構。" + ex.ToString();
            }

            DateTime startTime = DateTime.Now;
            AOPLogInfo apiLogAopInfo = new AOPLogInfo
            {
                RequestTime = startTime.ToString("yyyy-MM-dd hh:mm:ss fff"),
                OpUserName = "",
                RequestMethodName = invocation.Method.Name,
                RequestParamsName = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString())),
                ResponseJsonData = json
            };

            // 事中
            try
            {
                // 在被攔截的方法執行完成後，繼續執行當前方法，注意是被攔截的是非同步的
                invocation.Proceed();


                // 非同步獲取異常，先執行
                if (IsAsyncMethod(invocation.Method))
                {

                    //Wait task execution and modify return value
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                            (Task)invocation.ReturnValue,
                            async () => await SuccessAction(invocation, apiLogAopInfo, startTime), /*成功时执行*/
                            ex =>
                            {
                                LogEx(ex, apiLogAopInfo);
                            });
                    }
                    //Task<TResult>
                    else
                    {
                        invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                            invocation.Method.ReturnType.GenericTypeArguments[0],
                            invocation.ReturnValue,
                            async (o) => await SuccessAction(invocation, apiLogAopInfo, startTime, o), /*成功时执行*/
                            ex =>
                            {
                                LogEx(ex, apiLogAopInfo);
                            });
                    }

                }
                else
                {
                    // 同步1
                    string jsonResult;
                    try
                    {
                        jsonResult = JsonConvert.SerializeObject(invocation.ReturnValue);
                    }
                    catch (Exception ex)
                    {
                        jsonResult = "無法序列化，可能是lambda表達式等原因造成，請重構。" + ex.ToString();
                    }

                    DateTime endTime = DateTime.Now;
                    string ResponseTime = (endTime - startTime).Milliseconds.ToString();
                    apiLogAopInfo.ResponseTime = endTime.ToString("yyyy-MM-dd hh:mm:ss fff");
                    apiLogAopInfo.ResponseIntervalTime = ResponseTime + "ms";
                    apiLogAopInfo.ResponseJsonData = jsonResult;
                    Console.WriteLine(JsonConvert.SerializeObject(apiLogAopInfo));
                }
            }
            catch (Exception ex)
            {
                LogEx(ex, apiLogAopInfo);
                throw;
            }

        }

        private async Task SuccessAction(IInvocation invocation, AOPLogInfo apiLogAopInfo, DateTime startTime, object o = null)
        {
            DateTime endTime = DateTime.Now;
            string ResponseTime = (endTime - startTime).Milliseconds.ToString();
            apiLogAopInfo.ResponseTime = endTime.ToString("yyyy-MM-dd hh:mm:ss fff");
            apiLogAopInfo.ResponseIntervalTime = ResponseTime + "ms";
            apiLogAopInfo.ResponseJsonData = JsonConvert.SerializeObject(o);

            await Task.Run(() =>
            {
                Console.WriteLine("執行成功 --> " + JsonConvert.SerializeObject(apiLogAopInfo));
            });
        }

        private void LogEx(Exception ex, AOPLogInfo dataIntercept)
        {
            if (ex != null)
            {
                Console.WriteLine("error!!!:" + ex.Message + JsonConvert.SerializeObject(dataIntercept));
            }
        }

        public static bool IsAsyncMethod(MethodInfo method)
        {
            return
                method.ReturnType == typeof(Task) ||
                method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }
    }

    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<object, Task> postAction,
        Action<Exception> finalAction)
        {
            Exception exception = null;
            try
            {
                var result = await actualReturnValue;
                await postAction(result);
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        /// <summary>
        /// 看似沒有使用，但是有使用反射的方式使用
        /// </summary>
        /// <param name="taskReturnType"></param>
        /// <param name="actualReturnValue"></param>
        /// <param name="action"></param>
        /// <param name="finalAction"></param>
        /// <returns></returns>
        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue,
             Func<object, Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper)
                .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)
                .MakeGenericMethod(taskReturnType)
                .Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }
}
