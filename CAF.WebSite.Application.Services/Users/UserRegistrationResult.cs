using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Users
{
    public class UserRegistrationResult 
    {
        public IList<string> Errors { get; set; }

        public UserRegistrationResult() 
        {
            this.Errors = new List<string>();
        }

        public bool Success 
        {
            get { return this.Errors.Count == 0; }
        }

        public void AddError(string error) 
        {
            this.Errors.Add(error);
        }
    }
}
