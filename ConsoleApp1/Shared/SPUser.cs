namespace SPLib.Shared
{
    public class SPUser
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string LoginName { get; set; }
        public string JobTitle { get; set; }

        public SPUser() 
        {
            Id = -1;
        }

        public SPUser(dynamic input)
        {
            this.Id = input.Id;
            this.Title = input.Title;
            this.Email = input.Email;
            this.JobTitle = input.JobTitle;
            this.LoginName = input.LoginName;
        }

        public SPUser(SPUser input)
        {
            this.Id = input.Id;
            this.Title = input.Title;
            this.Email = input.Email;
            this.JobTitle = input.JobTitle;
            this.LoginName = input.LoginName;
        }

    }
}
