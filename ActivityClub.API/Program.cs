using ActivityClub.API.Middlewares;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Implementations;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Implementations;
using ActivityClub.Services.Interfaces;
using ActivityClub.Services.Mapping;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;





namespace ActivityClub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();


            //Add Authentication(JWT based) 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });



            //AutoMapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(ActivityClubProfile));




            builder.Services.AddEndpointsApiExplorer();
            
            
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ActivityClub API", Version = "v1" });

                // 1) Define the Bearer auth scheme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                // 2) Require Bearer token by default (for endpoints that need it)
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                       new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                           }
                       },
                       Array.Empty<string>()
                    }
                });
            });


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGuideService, GuideService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ILookupService, LookupService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IEventMemberService, EventMemberService>();
            builder.Services.AddScoped<IEventGuideService, EventGuideService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddAuthorization();



            builder.Services.AddDbContext<ActivityClubDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ActivityClubDb")));


            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            // Global exception handler should be FIRST (wraps everything)
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                //app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // authentication (auth)
            app.UseAuthorization();  // authorization (auth)


            app.MapControllers();

            app.Run();
        }
    }
}
