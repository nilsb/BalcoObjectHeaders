using System.Collections.Generic;
using System.Threading.Tasks;

namespace SPLib.Shared
{
    public class SPMultiUser
    {
        public List<string> SelectedUserLogins { get; set; }
        public List<SPUser> Users { get; set; }

        public SPMultiUser() 
        {
            SelectedUserLogins = new List<string>();
            Users = new List<SPUser>();
        }

        public SPMultiUser(SPMultiUser input) 
        {
            if(input != null)
            {
                SelectedUserLogins = input.SelectedUserLogins;
                Users = input.Users;
            }
        }

        public SPMultiUser(List<SPUser> users)
        {
            SelectedUserLogins = new List<string>();
            Users = new List<SPUser>();

            foreach(var user in users)
            {
                Users.Add(user);
                SelectedUserLogins.Add(user.LoginName);
            }
        }

        public async Task EnsureUsersFromLoginsAsync(SPWeb web)
        {
            if(this.SelectedUserLogins.Count > 0)
            {
                if (this.Users == null)
                    this.Users = new List<SPUser>();

                this.Users.Clear();

                foreach(var accountName in this.SelectedUserLogins)
                {
                    var user = await web.EnsureUserAsync(accountName);

                    this.Users.Add(user);
                }
            }
        }

        public void EnsureUsersFromLogins(SPWeb web)
        {
            if (this.SelectedUserLogins.Count > 0)
            {
                if (this.Users == null)
                    this.Users = new List<SPUser>();

                this.Users.Clear();

                foreach (var accountName in this.SelectedUserLogins)
                {
                    var user = web.EnsureUser(accountName);

                    this.Users.Add(user);
                }
            }
        }
    }
}
