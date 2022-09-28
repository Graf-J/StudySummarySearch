using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudySummarySearch.API.Data;
using StudySummarySearch.API.Services.AuthServices;
using StudySummarySearch.API.Services.KeywordServices;
using StudySummarySearch.API.Services.NameServices;
using StudySummarySearch.API.Services.SemesterServices;
using StudySummarySearch.API.Services.SubjectServices;
using StudySummarySearch.API.Services.SummaryServices;
using StudySummarySearch.API.Services.UserServices;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:TokenSecret").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<DataContext>();

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ISummaryService, SummaryService>();
builder.Services.AddTransient<ISemesterService, SemesterService>();
builder.Services.AddTransient<ISubjectService, SubjectService>();
builder.Services.AddTransient<IKeywordService, KeywordService>();
builder.Services.AddTransient<INameService, NameService>();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder => {
    builder
    .WithOrigins("https://study-summary-search.netlify.app", "http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
