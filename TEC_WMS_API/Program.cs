using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Service;


var builder = WebApplication.CreateBuilder(args);

//Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region JWT Token Generate
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


#endregion

#region Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CustomPolicy", x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

});
#endregion

#region Database Config
builder.Services.AddSingleton<DatabaseConfig>();
#endregion

#region Repositories
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<ILoginRepository, LoginRepository>();
#endregion

#region Service
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<LoginService, LoginService>();
builder.Services.AddScoped<IDevice, DeviceService>();
builder.Services.AddScoped<IBinConfig, BinConfigService>();
builder.Services.AddScoped<IWareHouse, WareHouseService>();
builder.Services.AddScoped<IBinMaster, BinMasterService>();
#endregion


var app = builder.Build();

app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CustomPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
