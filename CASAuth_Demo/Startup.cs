using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using Infrastructure.Business_Logic;
using BusinessManagement.Repo;

namespace CASAuth_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider services)
        {
            Configuration = configuration;
            serviceProvider = services;
            applicationDbContext = new ApplicationDbContext(configuration);
        }

        public IConfiguration Configuration { get; }
        private IServiceProvider serviceProvider { get; set; }
        private ApplicationDbContext applicationDbContext;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
            {
            var connectionString = Configuration.GetConnectionString("Default");
            
            services.AddMvc( options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IRepository<AppUser>, Repository<AppUser>>();

            services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString,b => b.MigrationsAssembly("Infrastructure")));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                   
                    opt.LoginPath = new PathString("/api/Account/Login");
                    opt.Cookie = new CookieBuilder
                    {
                        Name = "CasIU.Edu-cookie"
                    };
                    opt.Events = new CookieAuthenticationEvents
                    {
                        OnSigningIn = context =>
                        {
                            //when signin
                            
                           var _name =  context.Principal.Identity.Name;
                            var identity = new ClaimsIdentity();
                            identity.AddClaim(new Claim(ClaimTypes.Name, _name + ".edu"));
                            context.Principal.AddIdentity(identity);
                            //Ensure that user doesn't exist in Db;
                            var _userName = context.Principal.Claims.Where(c => c.Type == "userName").Select(c => c.Value).FirstOrDefault();
                            var userRepo = (IRepository<AppUser>)context.HttpContext.RequestServices.GetService(typeof(IRepository<AppUser>))??throw new NullReferenceException(nameof(IRepository<AppUser>));
                            var userExisted =  userRepo.GetAll().FirstOrDefault(u => u.UserName == _userName);
                            if(userExisted == null)
                            {
                                var user = new AppUser
                                {
                                    // Id = applicationDbContext.appUsers.Max(c => c.Id) + 1,
                                    Department = int.Parse(context.Principal.Claims.Where(c => c.Type == "userDept").Select(c => c.Value).FirstOrDefault()),
                                    Name = context.Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Select(c => c.Value).FirstOrDefault(),
                                    NameIdentifier = context.Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Select(c => c.Value).FirstOrDefault(),
                                    UserType = int.Parse(context.Principal.Claims.Where(c => c.Type == "userType").Select(c => c.Value).FirstOrDefault()),
                                    UserStatus = int.Parse(context.Principal.Claims.Where(c => c.Type == "userStatus").Select(c => c.Value).FirstOrDefault()),
                                    UserName = _userName
                                };
                                userRepo.Insert(user);
                                userRepo.Save();
                            }
                           
                            return Task.FromResult(0);

                        },
                        OnSigningOut = context =>
                        {
                            var casUrl = new Uri(Configuration["CasBaseUrl"]);
                            var links = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
                            var serviceUrl = links.GetUriByPage(context.HttpContext, "/Index");
                            var redirectUri = UriHelper.BuildAbsolute(
                          casUrl.Scheme,
                          new HostString(casUrl.Host, casUrl.Port),
                          casUrl.LocalPath, "/logout",
                          QueryString.Create("service", serviceUrl));
                            var logoutRedirectContext = new RedirectContext<CookieAuthenticationOptions>(
                           context.HttpContext,
                           context.Scheme,
                           context.Options,
                           context.Properties,
                           redirectUri
                        );
                            context.Response.StatusCode = 204; //Prevent RedirectToReturnUrl
                            context.Options.Events.RedirectToLogout(logoutRedirectContext);
                            return Task.CompletedTask;
                        }
                    };
                }).AddCAS(opt =>
                {
                    opt.CasServerUrlBase = Configuration["CasBaseUrl"];
                    opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opt.SaveTokens = true;
                  
                 
                    
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
