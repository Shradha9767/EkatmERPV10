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
public class LoginAPIController : Controller
{
    IConfiguration _config;
    private readonly ICommonFunctions _iCommonFunc;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoginAPIController(ICommonFunctions IcommonFunctions, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _iCommonFunc = IcommonFunctions;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
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
    //[AllowAnonymous]
    [HttpPost]
    //[HttpGet]
    public IActionResult GetIsValidUser(string AppEnvMasterId, string UserLoginId, string UserPwd)
    {
        try
        {
            string status = "success";
            string tokenString = "";
            string message = "";
            string response = Unauthorized().ToString();
            ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
            string IsValidUser = "";
            var user = ObjLogin.IsValidLogin(AppEnvMasterId, UserLoginId, UserPwd);
            if (user.IsValidUser == true)
            {
                IsValidUser = "true";
                var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim("UserMasterId",user.UserMasterId),
                new Claim("UserName",user.UserName),
                new Claim("UserLoginId",user.UserLoginId),
                new Claim("AppEnvMasterId",user.AppEnvMasterId),
                new Claim("Plant",user.Plant),
                new Claim("EkatmDbCnnStr",user.EkatmDbCnnStr),
                new Claim("EkatmMsgCnnStr",user.EkatmMsgCnnStr),
                new Claim("RgtCnnStr",user.RgtCnnStr),
                new Claim("DeptMasterId",user.DeptMasterId),
                new Claim("PlantId",user.PlantId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                tokenString = GenerateJSONWebToken(claims).ToString();
                message = "Login Successfully";
            }
            else
            {
                status = "failed";
                message = "Invalid username/password";
            }
            var query = new { Role = "Admin", Token = tokenString, Message = message, Status = status };
            return Ok(query);
        }
        catch (Exception ex)
        {
            var query = new { Role = "", Token = "", Message = ex.Message, Status = "Error" };
            return Ok(query);
        }
    }
    public string GenerateJSONWebToken(Claim[] claims)
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
    [HttpGet]
    [Authorize]
    public ActionResult GetCurrentUser()
    {
        var currentUser = HttpContext.User;
        return Ok(new ClsTokenDetails
        {
            UserName = currentUser.Claims.FirstOrDefault(c => c.Type == "name").Value,
            UserLoginId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserLoginId").Value,
            AppEnvMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value,
            Plant = currentUser.Claims.FirstOrDefault(c => c.Type == "Plant").Value
        });
    }
    [Authorize]
    [HttpGet]
    public IActionResult GETAutorized()
    {
        try
        {
            var currentUser = HttpContext.User;
            string Plant = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "Plant").Value);
            //DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
            string name = currentUser.Claims.FirstOrDefault(c => c.Type == "name").Value;
            string UserLoginId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserLoginId").Value;
            string AppEnvMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value;
            return Ok(Plant);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public string GETPassword(string txtEmail, string userId)
    {
        try
        {
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
//public IActionResult GetIsValidUser(string AppEnvMasterId, string UserLoginId, string UserPwd)
//{
//    try
//    {
//        //   IActionResult response = Unauthorized();
//        string status = "success";
//        string tokenString = "";
//        string message = "";
//        ClsLogin ObjLogin = new ClsLogin(_iCommonFunc, _httpContextAccessor);
//        string IsValidUser = "";
//        var user = ObjLogin.IsValidLogin(AppEnvMasterId, UserLoginId, UserPwd);
//        if (user.IsValidUser == true)
//        {
//            IsValidUser = "true";
//            var claims = new[] {
//                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
//                new Claim("Plant",user.Plant),
//                new Claim("UserLoginId",user.UserLoginId),
//                new Claim("AppEnvMasterId",user.AppEnvMasterId),
//                new Claim("EkatmDbCnnStr",user.EkatmDbCnnStr),
//                new Claim("EkatmMsgCnnStr",user.EkatmMsgCnnStr),
//                new Claim("RgtCnnStr",user.RgtCnnStr),
//                new Claim("DeptMasterId",user.DeptMasterId),
//                new Claim("PlantId",user.PlantId),
//                new Claim("UserMasterId",user.UserMasterId),
//                new Claim("RptXml",""),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };
//            tokenString = GenerateJSONWebToken(claims).ToString();
//            message = "Login Successfully";
//        }
//        else
//        {
//            status = "failed";
//            message = "Invalid username/password";
//        }
//        var query = new { Role = "Admin", Token = tokenString, Message = message, Status = status };
//        return Ok(query);
//    }
//    catch (Exception ex)
//    {
//        //   throw new Exception(ex.Message);
//        var query = new { Role = "", Token = "", Message = ex.Message, Status = "Error" };
//        return Ok(query);
//    }
//}
