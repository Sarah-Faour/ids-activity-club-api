using ActivityClub.API.Middlewares;
using ActivityClub.Data.Models;
using ActivityClub.Repositories.Implementations;
using ActivityClub.Repositories.Interfaces;
using ActivityClub.Services.Implementations;
using ActivityClub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ActivityClub.Services.Mapping;
using AutoMapper;


namespace ActivityClub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            
            //AutoMapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(ActivityClubProfile));




            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IGuideService, GuideService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ILookupService, LookupService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IEventMemberService, EventMemberService>();
            builder.Services.AddScoped<IEventGuideService, EventGuideService>();


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

                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
