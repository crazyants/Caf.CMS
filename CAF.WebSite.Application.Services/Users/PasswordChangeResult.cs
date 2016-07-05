using System.Collections.Generic;

namespace CAF.WebSite.Application.Services.Users
{
    public class PasswordChangeResult
    {
        public IList<string> Errors { get; set; }

        public PasswordChangeResult() 
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddError(string error) 
        {
            this.Errors.Add(error);
        }
    }
}
