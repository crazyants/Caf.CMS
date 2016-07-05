//Contributor:  Nicholas Mayne


namespace CAF.WebSite.Application.Services.Authentication.External
{
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}