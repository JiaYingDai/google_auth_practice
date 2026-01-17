using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace google_auth_practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 使用者點擊「Google 登入」按鈕時呼叫此 API
        /// </summary>
        /// <returns></returns>
        [HttpGet("login")]
        public IActionResult Login(string redirectUrl)
        {
            // 安全檢查，檢查redirctUrl是否為前端網域名稱
            if (!IsUrlValid(redirectUrl))
            {
                return BadRequest("無效的Redirect URL，請確認您的來源網址。");
            }

            // 設定Redirect的URL
            var properties = new AuthenticationProperties { 
                RedirectUri = Url.Action("GoogleResponse"),
                Items =
                {
                    { "returnUrl", redirectUrl }
                }
            };

            // Challenge: 指定給Google驗證 (傳properties給Google，驗證成功跳轉到Redirect URL)
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Google 驗證成功後跳轉回來的 API
        /// </summary>
        /// <returns></returns>
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            // 取得cookie驗證結果
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return BadRequest("登入失敗");

            // 取出properties的returnUrl
            string returnUrl = result.Properties.Items["returnUrl"];

            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest("無效的Redirect URL，請確認您的來源網址。");
            }

            // 將使用者導向前端指定頁面returnUrl
            return Redirect(returnUrl);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("已登出");
        }

        /// <summary>
        /// 網址白名單檢查
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsUrlValid(string url) {
            // appsettings.json白名單
            string[] whiteUrlAry = _configuration.GetSection("AllowedUrl").Get<string[]>()
                       ?? Array.Empty<string>();

            // 檢查url有無存在白名單
            if (!whiteUrlAry.Contains(url))
            {
                return false;
            }

            return true;
        }
    }
}
