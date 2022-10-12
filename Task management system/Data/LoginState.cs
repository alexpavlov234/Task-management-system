using Task_management_system.Models;
using Task_management_system.Pages;

namespace Task_management_system.Data
{
    public class LoginState
    {
        public bool IsLoggedIn { get; set; }
        public User loggedUser { get; set; }


        public void SetLogin(bool login, User user)
        {
            IsLoggedIn = login;
            this.loggedUser = user;
            NotifyDataChanged();
        }
        //Pointless
        public void Logout()
        {
            SetLogin(false, null);          
        }
      
        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
    }
}
