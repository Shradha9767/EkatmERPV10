using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using EkatmERPV10.CommonObjCC;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using static EkatmERPV10.Controllers.HomeController;
//public class TestAPIController : Controller
//{
//    public IActionResult Index()
//    {
//        return View();
//    }
//}
public class TestAPIController : Controller
{
    IConfiguration _config;
    private readonly RequestDelegate _next;
    HttpContext _context;
    private readonly ICommonFunctions _iCommonFunc;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TestAPIController(ICommonFunctions IcommonFunctions, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _iCommonFunc = IcommonFunctions;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetEnvis()
    {
        string EnviList = "";
        try
        {
            ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
            EnviList = ObjLogin.GetEnvironments();
            if (EnviList != "")
            {
                return EnviList;
            }
            else
            {
                EnviList = "DBNOTConfig";
                return EnviList;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("GetEnvis()-" + ex.Message);
        }
    }
    //public string GetPlants(string AppEnvMasterId, string PlantId)
    public string GetPlants()
    {
        string PlantList = "";
        ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
        try
        {
            PlantList = ObjLogin.GetPlants();
            return PlantList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    [AllowAnonymous]
    [HttpPost]
    public string GetIsValidUser(string AppEnvMasterId, string UserLoginId, string UserPwd)
    {
        try
        {
            string response = Unauthorized().ToString();
            //var user = AuthenticateUser(login);
            ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
            string IsValidUser = "";
            var user = ObjLogin.IsValidLogin(AppEnvMasterId, UserLoginId, UserPwd);
            if (user.IsValidUser == true)
            {
                IsValidUser = "true";
                var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim("Plant",user.Plant),
                new Claim("UserLoginId",user.UserLoginId),
                new Claim("AppEnvMasterId",user.AppEnvMasterId),
                 new Claim("pass",UserPwd),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                   };
                var tokenString = GenerateJSONWebToken(claims);
                response = tokenString.ToString();
            }
            else
            {
                IsValidUser = "Invalid Password And Plant";
            }
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    [Authorize]
    [HttpGet]
    public string getUser(string str)
    {
        ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
        return ""; //ObjLogin.GetUser(str);
    }
    private static string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    [Authorize]
    [HttpGet]
    public IEnumerable<WeatherForecast> getdata()
    {
        var currentUser = HttpContext.User;
        var claim1 = new Claim("test", "655");
        ClaimsIdentity bg = new ClaimsIdentity();
        bg.AddClaim(claim1);
        HttpContext.User.AddIdentity(bg);
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        });
    }
    [Authorize]
    [HttpGet]
    public string gettext(string str)
    {
        var list = HttpContext.User.Claims.ToList();
        //  var gvn=   currentUser.Claims.FirstOrDefault(c => c.Type == "test").Value;
        var currentUser = HttpContext.User;
        var AppEnvMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value;
        var pass = currentUser.Claims.FirstOrDefault(c => c.Type == "pass").Value;
        list.Add(new Claim("Test", "Devidas"));
        GenerateJSONWebToken(list.ToArray());
        //  var existingClaim = identity.FindFirst("test");
        return GenerateJSONWebToken(list.ToArray());
    }
    private string GenerateJSONWebToken(Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string GETPassword(string txtEmail, string userId)
    {
        try
        {
            var currentUser = HttpContext.User;
            DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
            ForgetPassword forgetPassword = new ForgetPassword(_iCommonFunc);
            string returnMsg = forgetPassword.GETPassword(txtEmail, userId);
            return returnMsg;
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}
public class WeatherForecast
{
    public string DateFormatted { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
    public int TemperatureF
    {
        get
        {
            return 32 + (int)(TemperatureC / 0.5556);
        }
    }
}
