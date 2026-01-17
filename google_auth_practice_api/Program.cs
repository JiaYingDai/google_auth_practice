using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Authentication
builder.Services.AddAuthentication(options =>
{
    // 設定預設驗證方案為 Cookie (登入成功後用 Cookie 紀錄)
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // 設定當使用者觸發 "Challenge" (需要登入) 時，預設使用 Google
    // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/account/login";
}) // 必須加入 Cookie 才能存登入狀態
.AddGoogle(googleOptions => // Google Authentication
{
    // 從 secrets.json 讀取設定
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    // (選用) 自動對應使用者資訊，例如把 Google 的 Name 對應到 User.Identity.Name
    googleOptions.SaveTokens = true;
});

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

// 啟用驗證中間件
app.UseAuthentication();

app.MapControllers();

app.Run();
