using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using PromptEngineering.Services;

namespace PromptEngineering
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
            
            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .WithExposedHeaders("*")
                .AllowAnyHeader();
            }));
            // Register the configuration class
            services.Configure<MySettings>(Configuration.GetSection("MySettings"));

            // Add AutoMapper services            
            services.AddAutoMapper(cfg => cfg.AddMaps(GetType().Assembly, typeof(MappingProfile).Assembly));

            // Register the services
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatServices, ChatServices>();
            services.AddScoped<IMasterRepository, MasterRepository>();
            services.AddScoped<IMasterServices, MasterServices>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersServices, UsersServices>();
            services.AddScoped<IFilesServices, FilesServices>();
            services.AddScoped<ICopilotCLIServices, CopilotCLIServices>();
            services.AddScoped<IRepoRepository, RepoRepository>();
            services.AddScoped<IRepoServices, RepoServices>();
            services.AddScoped<IDocsRepository, DocsRepository>();
            services.AddScoped<IDocsServices, DocsServices>();
            services.AddScoped<IOllamaServices, OllamaServices>();            
            services.AddScoped<ILayoutRepository, LayoutRepository>();
            services.AddScoped<ILayoutServices, LayoutServices>();
            services.AddScoped<IGuardrailsRepository, GuardrailsRepository>();
            services.AddScoped<IGuardrailsServices, GuardrailsServices>();


            //services.AddScoped<IHuggingFaceServices, HuggingFaceServices>();

            // Register the HttpClientFactory
            services.AddHttpClient();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PromptEngineering", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "PromptEngineering v1"));

            //app.UseSwaggerUI(c =>
            //{
            //    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
            //    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "PromptEngineering v1");

            //});

            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "PromptEngineering v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Cors");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
