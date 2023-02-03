using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Configuracion de Conexion a SQL

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

//Añadimos cache
builder.Services.AddResponseCaching();

//Llave JWT
string key  = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//Agrgamos Repositorios

builder.Services.AddScoped<ICategoriaRepositorio,CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//Agregamos el Automapper

builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Configuracion Autenticacion
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey= true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience= false
        };
    });

// Add services to the container.

builder.Services.AddControllers(opcion =>
{
    //Cache profile. Un cache global
    opcion.CacheProfiles.Add("PorDefecto20Seg", new CacheProfile() { Duration = 30 });
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticacion JWT usando el esquema Bearer. \r\n\r\n" +
        "Ingresa la palabra 'Bearer' seguida de un [Espacio] y despues su token en el campo de la parte inferior  \r\n\r\n" +
        "Ejemplo: \"Bearer dasdbcsysde434234h3sdnsd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
             new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                              {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
             },
             new List<string>()
            
        }
    });
});


//Soporte para CORS
//Se pueden habilitar: 1-Un dominio, 2-multiples dominios,
//3-cualquier dominio (Tener en cuenta seguridad)
//Usamos de ejemplo el dominio: http://Localhost:3223, se debe cambiar por el correcto
//Se usa (*) para todos los dominios

builder.Services.AddCors( p => p.AddPolicy("PolicyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Soporte para CORS
app.UseCors("PolicyCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
