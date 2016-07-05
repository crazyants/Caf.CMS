//Contributor:  Nicholas Mayne


namespace CAF.WebSite.Application.Services.Authentication.External
{
    public partial interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}