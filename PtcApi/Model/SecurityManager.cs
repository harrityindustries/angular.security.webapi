using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PtcApi.Model;

namespace PtcApi.Security
{
    public class SecurityManager
    {

        private readonly JwtSettings _settings = null;

        public SecurityManager(JwtSettings settings)
        {
            _settings = settings;
        }
        public AppUserAuth ValidateUser(AppUser user)
        {
            AppUserAuth ret = new AppUserAuth();
            AppUser authUser = null;

            using (var db = new PtcDbContext())
            {
                authUser = db.AppUser.Where(u => u.UserName.ToLower() == user.UserName.ToLower() && u.Password == user.Password).FirstOrDefault();
            }

            if (authUser != null)
            {
                ret = BuildUserAuthObject(authUser);
            }

            return ret;
        }

        protected List<AppUserClaim> GetUserClaims(AppUser authUser)
        {
            List<AppUserClaim> list = new List<AppUserClaim>();

            try
            {
                using (var db = new PtcDbContext())
                {
                    list = db.AppUserClaim.Where(u => u.UserId == authUser.UserId).ToList();
                }
            }
            catch (System.Exception ex)
            {

                throw new Exception("Exception trying to retrieve user claims.", ex);
            }

            return list;
        }

        protected AppUserAuth BuildUserAuthObject(AppUser authUser)
        {
            AppUserAuth ret = new AppUserAuth();
            List<AppUserClaim> claims = new List<AppUserClaim>();

            //Set user props
            ret.UserName = authUser.UserName;
            ret.IsAuthenticated = true;
            ret.BearerToken = new Guid().ToString();

            //Get all claims for user
            ret.Claims = GetUserClaims(authUser);

            ret.BearerToken = BuildJwtToken(ret);

            return ret;
        }

        protected string BuildJwtToken(AppUserAuth authUser)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            //Create bare minimum JWT Claims
            List<Claim> jwtClaims = new List<Claim>();
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, authUser.UserName)); //subject
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); //jwt id

            //Add any custom claims
            jwtClaims.Add(new Claim("isAuthenticated", authUser.IsAuthenticated.ToString().ToLower()));
            
            //Add custom claims from the Claim array
            foreach (var claims in authUser.Claims)
            {
                jwtClaims.Add(new Claim(claims.ClaimType, claims.ClaimValue));
            }

            //Create jwt token object
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: jwtClaims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_settings.MinutesToExpiration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}