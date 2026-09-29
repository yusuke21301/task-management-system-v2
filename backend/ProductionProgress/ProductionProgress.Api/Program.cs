using Microsoft.EntityFrameworkCore;
using ProductionProgress.Api.ExceptionHandlers;
using ProductionProgress.Application.Interfaces;
using ProductionProgress.Application.Services;
using ProductionProgress.Infrastructure.Authentication;
using ProductionProgress.Infrastructure.Data;
using ProductionProgress.Infrastructure.Repositories;
using ProductionProgress.Infrastructure.Security;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using ProductionProgress.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Controllerを使用できるようにする。
//
// JsonStringEnumConverterを追加することで、
// enumを0, 1, 2ではなく
// "NotStarted"、"InProgress"、"Completed"
// のような文字列としてJSONに出力する。
// ------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// ------------------------------------------------------------
// SignalRを使用できるようにする。
// ------------------------------------------------------------
builder.Services.AddSignalR();

// 本番環境ではRenderの環境変数 FrontendUrl から
// VercelのURLを取得する。
// ローカルではlocalhost:3000を使用する。
var frontendUrl =
    builder.Configuration["FrontendUrl"]
    ?? "http://localhost:3000";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("frontendUrl")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ------------------------------------------------------------
// パスワードのハッシュ化・検証
// ------------------------------------------------------------
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// ------------------------------------------------------------
// 初期管理者ユーザー作成
// ------------------------------------------------------------
builder.Services.AddScoped<InitialAdminSeeder>();

// ユーザー情報をDBから取得するRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ログイン処理を行うApplication Service
builder.Services.AddScoped<AuthService>();

// JWTアクセストークン生成
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<UserService>();

// ---------------------------------------------------------
// JWT認証
// ---------------------------------------------------------

// User SecretsなどからJWT設定を取得します。
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Keyが設定されていません。");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuerが設定されていません。");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audienceが設定されていません。");

// JWT Bearer認証を登録します。
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // -------------------------------------------------
                // JWTを発行したシステムが正しいか確認する
                // -------------------------------------------------
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                // -------------------------------------------------
                // JWTの利用対象がこのAPIであるか確認する
                // -------------------------------------------------
                ValidateAudience = true,
                ValidAudience = jwtAudience,

                // -------------------------------------------------
                // JWTの有効期限を確認する
                // -------------------------------------------------
                ValidateLifetime = true,

                // -------------------------------------------------
                // JWTの署名が正しいか確認する
                // -------------------------------------------------
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                // 有効期限を厳密に判定するため、
                // デフォルトの猶予時間を0にします。
                ClockSkew = TimeSpan.Zero
            };

            // SignalR接続時のJWT取得方法を設定する。
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var request = context.HttpContext.Request;

                    // SignalR JavaScriptクライアントは、
                    // WebSocket接続時にJWTをaccess_tokenとして送信する。
                    var accessToken =
                        request.Query["access_token"];

                    // SignalR Hubへの通信だけを対象にする。
                    if (!string.IsNullOrEmpty(accessToken) &&
                        request.Path.StartsWithSegments("/hubs/progress"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
    });

// [Authorize] を使用するために認可機能を登録します。
builder.Services.AddAuthorization();

// ------------------------------------------------------------
// SwaggerでAPI情報を取得できるようにする。
// ------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();

// ------------------------------------------------------------
// Swaggerドキュメントを生成するサービスをDIへ登録する。
//
// 今回のエラーはこの登録がないため、
// ISwaggerProviderを取得できなかったことが原因。
// ------------------------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    // Swagger UIにJWT Bearer認証を登録します。
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,

            // HTTPのBearer認証を使用する
            Scheme = "bearer",

            // 使用するトークン形式
            BearerFormat = "JWT",

            Description =
                "ログインAPIで取得したJWTを入力してください。"
        });

    // SwaggerでAPIを実行するときに、
    // AuthorizationヘッダーへJWTを付けられるようにします。
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});

// ------------------------------------------------------------
// アプリケーション全体の未処理例外を
// GlobalExceptionHandlerで共通処理する。
// ------------------------------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// ------------------------------------------------------------
// APIのエラーレスポンスをProblemDetails形式で
// 扱えるようにする。
// ------------------------------------------------------------
builder.Services.AddProblemDetails();

// ------------------------------------------------------------
// PostgreSQLの接続文字列を取得する。
// ------------------------------------------------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection が設定されていません。");

// ------------------------------------------------------------
// PostgreSQL用のDbContextをDIへ登録する。
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ------------------------------------------------------------
// RepositoryをDIへ登録する。
// ------------------------------------------------------------
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// ------------------------------------------------------------
// 工程マスタのRepositoryをDIコンテナへ登録する。
// ------------------------------------------------------------
builder.Services.AddScoped<IProcessRepository, ProcessRepository>();

// ------------------------------------------------------------
// ServiceをDIへ登録する。
// ------------------------------------------------------------
builder.Services.AddScoped<ITaskService, TaskService>();

// ------------------------------------------------------------
// 工程マスタの業務処理をDIコンテナへ登録する。
// ------------------------------------------------------------
builder.Services.AddScoped<IProcessService, ProcessService>();

var app = builder.Build();

// ---------------------------------------------------------
// 初期管理者ユーザーの登録
// ---------------------------------------------------------

// User Secretsや環境変数などから
// 初期管理者の情報を取得します。
var initialAdminUsername =
    app.Configuration["InitialAdmin:Username"];

var initialAdminPassword =
    app.Configuration["InitialAdmin:Password"];

// ユーザー名とパスワードの両方が設定されている場合のみ
// 初期管理者ユーザーの作成処理を実行します。
if (!string.IsNullOrWhiteSpace(initialAdminUsername)
    && !string.IsNullOrWhiteSpace(initialAdminPassword))
{
    // Scopedサービスを取得するため、
    // アプリ起動時専用のScopeを作成します。
    using var scope = app.Services.CreateScope();

    var seeder =
        scope.ServiceProvider.GetRequiredService<InitialAdminSeeder>();

    await seeder.SeedAdminAsync(
        initialAdminUsername,
        initialAdminPassword);
}

// ------------------------------------------------------------
// Controllerなどで処理されなかった例外を
// 登録したGlobalExceptionHandlerへ渡す。
// ------------------------------------------------------------
app.UseExceptionHandler();

// ------------------------------------------------------------
// 開発環境の場合のみSwaggerを有効にする。
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    // swagger.jsonを生成する。
    app.UseSwagger();

    // Swagger UIを表示する。
    app.UseSwaggerUI();
}

// CORSを適用する
// MapControllersやMapHubより前に配置する
app.UseCors("Frontend");

// JWTからユーザーを認証する
app.UseAuthentication();

// 認証されたユーザーにAPIを実行する権限があるか確認する
app.UseAuthorization();

app.MapControllers();

// SignalR Hubのエンドポイントを登録する。
// Next.jsは後でこのURLへSignalR接続する。
app.MapHub<ProgressHub>("/hubs/progress");

app.Run();

// ------------------------------------------------------------
// HTTPアクセスをHTTPSへリダイレクトする。
// 開発中はSwaggerからHTTP APIを直接確認するため一旦無効化する。
// ------------------------------------------------------------
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Integration TestからProgramクラスを参照できるようにする。
// WebApplicationFactory<Program>でAPIを起動するために必要。
public partial class Program
{
}