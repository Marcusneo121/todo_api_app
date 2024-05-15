using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using todo_api_app.Entities;
using todo_api_app.Data;
using todo_api_app.Dtos;

namespace todo_api_app.Utils;

public class JWTManagerUtils
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly IConfiguration _configuration;
    private readonly DBContext _dbContext;

    public JWTManagerUtils(IConfiguration configuration, DBContext dBContext)
    {
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _configuration = configuration;
        _dbContext = dBContext;
    }

    public RefreshTokenDto? GenerateRefreshToken(AccessTokenDto accessTokenDto)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Refresh_Secret"]!);
            // Console.WriteLine($"Configuration: {Convert.ToBase64String(key)}");

            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, accessTokenDto.Name),
                new Claim(ClaimTypes.Sid, accessTokenDto.Id.ToString()),
                new Claim(ClaimTypes.Email, accessTokenDto.Email),
            };

            var identity = new ClaimsIdentity(claims);
            DateTime expirationDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:Refresh_Secret_Day"]!));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Subject = identity,
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return new RefreshTokenDto(RefreshToken: _jwtSecurityTokenHandler.WriteToken(token), ExpiredDate: expirationDate);
        }
        catch (Exception)
        {
            return null;
        }
        // var randomNumber = new byte[32];
        // using (var rng = RandomNumberGenerator.Create())
        // {
        //     rng.GetBytes(randomNumber);
        //     return Convert.ToBase64String(randomNumber);
        // }
    }

    public string? GenerateAccessToken(AccessTokenDto accessTokenDto)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);
            // Console.WriteLine($"Configuration: {Convert.ToBase64String(key)}");

            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, accessTokenDto.Name),
                new Claim(ClaimTypes.Sid, accessTokenDto.Id.ToString()),
                new Claim(ClaimTypes.Email, accessTokenDto.Email),
            };

            var identity = new ClaimsIdentity(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Subject = identity,
                Expires = DateTime.UtcNow.AddSeconds(int.Parse(_configuration["JWT:Secret_Second"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return _jwtSecurityTokenHandler.WriteToken(token);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public ClaimsPrincipal ValidateExtractRefreshTokenIdentity(string refreshTokenUserInput)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Refresh_Secret"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = tokenHandler.ValidateToken(refreshTokenUserInput,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            JwtSecurityToken? jwtSecurityToken = validatedToken as JwtSecurityToken;

            // Console.WriteLine($"JwtToken Result: {jwtSecurityToken}");

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Refresh Token.");
            }
            return claims;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new ApplicationException("Refresh Token has expired.");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Invalid Refresh Token.");
        }
    }

    public ClaimsPrincipal ValidateExtractAccessTokenIdentity(string refreshTokenUserInput)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = tokenHandler.ValidateToken(refreshTokenUserInput,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            JwtSecurityToken? jwtSecurityToken = validatedToken as JwtSecurityToken;

            // Console.WriteLine($"JwtToken Result: {jwtSecurityToken}");

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token.");
            }
            return claims;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new ApplicationException("Token has expired.");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Invalid Token.");
        }
    }
}