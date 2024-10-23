using Comm.WebUtil;

using NLog;
using NLog.Web;

using System.Text;

namespace CommonApi
{
    public class Program
    {

        public static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//���UBIG5

            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString); //��dapper�૬���]�w�A�Y�L�]�w�|�C�ӫ��A�����աA�y���ɶ�����

            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var builder = WebApplication.CreateBuilder(args);
            logger.Debug("init main");
            var MyAllowSpecificOrigins = "CorsPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {//setup cors policy
                                      builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(o => true);
                                  });
            });
            builder.Services.AddSingleton<JwtHelper>();
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add JWT authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<Jwtsettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SignKey);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            // needed for  ${aspnet-request-posted-body} with an API Controller. Must be before app.UseEndpoints
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
