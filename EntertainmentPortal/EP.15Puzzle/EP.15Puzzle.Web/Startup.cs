﻿using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using AutoMapper;

using EP._15Puzzle.Logic.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EP._15Puzzle.Logic.Profiles;
using EP._15Puzzle.Logic;
using EP._15Puzzle.Logic.Commands;
using EP._15Puzzle.Logic.Validators;
using EP._15Puzzle.Web.Filters;
using EP._15Puzzle.Web.Hubs;
using FluentValidation.AspNetCore;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using NSwag.AspNetCore;
using NJsonSchema;
using NSwag;

namespace EP._15Puzzle.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton(typeof(NoticeHub).Assembly);
            services.AddAuthenticationCore();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.Authority = Configuration.GetSection("Urls:Is4").Value;
                    opt.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("bearer",
                    cfg => cfg.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser());
            });
            

            services.AddMediatR(typeof(NewDeckCommand).Assembly);
            services.AddMediatR(typeof(GetDeckQuery).Assembly);
            services.AddSwaggerDocument(cfg =>
            {
                cfg.SchemaType = NJsonSchema.SchemaType.OpenApi3;
                cfg.AddSecurity("oauth", new[] { "pyatnashki_api" }, new OpenApiSecurityScheme()
                {
                    Flow = OpenApiOAuth2Flow.Implicit,
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    AuthorizationUrl = $"{Configuration.GetSection("Urls:Is4").Value}/connect/authorize",
                    Scopes = new Dictionary<string, string>()
                    {
                        {"pyatnashki_api", "Access to 15Puzzle application." }
                    }

                });
            });
            services.AddAutoMapper(cfg=>cfg.AllowNullCollections=true,typeof(DeckProfile).Assembly);
            services.AddDeckServices();

            services.AddMvc(opt =>
                {
                    opt.Filters.Clear();
                    opt.Filters.Add(typeof(GlobalExceptionFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddFluentValidation(cfg =>
            {
                cfg.RegisterValidatorsFromAssemblyContaining<MoveTileValidator>();
                cfg.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            }); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMediator mediator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseCors(opt =>
                opt.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(Configuration.GetSection("Urls:Api").Value, Configuration.GetSection("Urls:Is4").Value, Configuration.GetSection("Urls:Front").Value)
                    .AllowCredentials());
            app.UseSignalR(routes => { routes.MapHub<NoticeHub>("/notice");});
            
            app.UseAuthentication();
            //app.UseIdentityServer();
            mediator.Send(new CreateDatabaseCommand()).Wait();

            app.UseOpenApi().UseSwaggerUi3(opt =>
            {
                opt.OAuth2Client = new OAuth2ClientSettings()
                {
                    AppName = "pyatnashki",
                    ClientId = "swagger"
                };
            });
            app.UseMvc();
        }
    }
}