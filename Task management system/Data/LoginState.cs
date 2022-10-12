using Task_management_system.Models;
using Task_management_system.Pages;

namespace Task_management_system.Data
{
    public class LoginState
    {
        public bool IsLoggedIn { get; set; }
        public User loggedUser { get; set; }
        public event Action OnChange;

        public void SetLogin(bool login, User user)
        {
            IsLoggedIn = login;
            this.loggedUser = user;
            NotifyStateChanged();
        }
        //Pointless
        public void Logout()
        {
            SetLogin(false, null);          
        }
        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}
