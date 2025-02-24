using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SmartE_commerce.Classes;
using SmartE_commerce.Data;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartE_commerce.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class UsersController(JwtOptions jwtOptions , ApplicationDbContext dbContext) : ControllerBase
    {
        private readonly string _connectionString = "Server=db14374.public.databaseasp.net; Database=db14374; User Id=db14374; Password=4Cd_Zo%57!Kn; Encrypt=False; MultipleActiveResultSets=True;";


        [HttpPost]
        [Route("login")]
        public ActionResult<string> AuthenticatUser(AuthenticationRequest request)
        {
            var response = new Dictionary<string, object>(); 
            //response["message"] = "success";
            //response["Token"] = accessToken;
            //return Ok(response);
            var user = dbContext.Set<Buyer>().FirstOrDefault(x => x.Email == request.UserEmail &&
            x.password == request.Password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var TokenDecriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audiencs,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,user.Buyer_ID.ToString()),
                    new (ClaimTypes.Name,user.Buyer_Name),
                    new (ClaimTypes.Email,user.Email)

                })
            };
            var securityToken = tokenHandler.CreateToken(TokenDecriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

             // كائن رئيسي يحتوي على البيانات والصور
            response["message"] = "success";
            response["Token"] = accessToken;
            return Ok(response);
        }



        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> AddAuthenticatUser(BuyerPost buyer)
        {
            if (!Regex.IsMatch(buyer.Email, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|outlook\.com|hotmail\.com|yahoo\.com|icloud\.com|gov|edu|org|net|com)$"))
                return BadRequest("invalid Email");

            if (!Regex.IsMatch(buyer.password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,}$"))
                return BadRequest("invalid password");

            try
            {
                                                                 
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddBuyerv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", buyer.Buyer_Name);
                        command.Parameters.AddWithValue("@Email", buyer.Email);
                        command.Parameters.AddWithValue("@Password", buyer.password);
                        command.Parameters.AddWithValue("@Location", buyer.Location);
                        command.Parameters.AddWithValue("@phon", buyer.phone);

                        await command.ExecuteNonQueryAsync();
                    }
                }

               
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }



           var user = dbContext.Set<Buyer>().FirstOrDefault(x => x.Email == buyer.Email &&
           x.password == buyer.password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var TokenDecriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audiencs,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,user.Buyer_ID.ToString()),
                    new (ClaimTypes.Name,user.Buyer_Name),
                    new (ClaimTypes.Email,user.Email)

                })
            };
            var securityToken = tokenHandler.CreateToken(TokenDecriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            var response = new Dictionary<string, object>(); // كائن رئيسي يحتوي على البيانات والصور
            response["message"] = "success";
            response["Token"] = accessToken;
            return Ok(response);

        }

        //===========Dashpord=============

        [HttpPost]
        [Route("loginDash")]
        public ActionResult<string> AuthenticatUserDash(AuthenticationRequest request)
        {
            var response = new Dictionary<string, object>(); // كائن رئيسي يحتوي على البيانات والصور

            var user = dbContext.Set<Seller>().FirstOrDefault(x => x.Email == request.UserEmail &&
            x.password == request.Password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var TokenDecriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audiencs,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,user.Seller_ID.ToString()),
                    new (ClaimTypes.Name,user.seller_Name),
                    new (ClaimTypes.Email,user.Email)

                })
            };
            var securityToken = tokenHandler.CreateToken(TokenDecriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

             // كائن رئيسي يحتوي على البيانات والصور
            response["message"] = "success";
            response["Token"] = accessToken;
            return Ok(response);
        }



        [HttpPost]
        [Route("signinDash")]
        public async Task<IActionResult> AddAuthenticatUserDash(SellerPost seller)
        {
            if (!Regex.IsMatch(seller.Email, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|outlook\.com|hotmail\.com|yahoo\.com|icloud\.com|gov|edu|org|net|com)$"))
                return BadRequest("invalid Email");

            if (!Regex.IsMatch(seller.password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{8,}$"))
                return BadRequest("invalid password");

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("Sp_AddSellerv4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", seller.seller_Name);
                        command.Parameters.AddWithValue("@Email", seller.Email);
                        command.Parameters.AddWithValue("@Password", seller.password);
                        command.Parameters.AddWithValue("@Location", seller.Location);
                        command.Parameters.AddWithValue("@phon", seller.phone);
                        command.Parameters.AddWithValue("@package", seller.phone);


                        await command.ExecuteNonQueryAsync();
                    }
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }



            var user = dbContext.Set<Seller>().FirstOrDefault(x => x.Email == seller.Email &&
            x.password == seller.password);

            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var TokenDecriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audiencs,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SiningKey)),
                SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier,user.Seller_ID.ToString()),
                    new (ClaimTypes.Name,user.seller_Name),
                    new (ClaimTypes.Email,user.Email)

                })
            };
            var securityToken = tokenHandler.CreateToken(TokenDecriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            var response = new Dictionary<string, object>(); // كائن رئيسي يحتوي على البيانات والصور
            response["message"] = "success";
            response["Token"] = accessToken;
            return Ok(response);

        }
    }

}
