using System;

namespace RQuote
{
    public class CustomerDetails : ViewModelBase
    {
        private string firstName;
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string lastName;
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        private string phone;
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }
        private string orgName;
        public string OrganizationName
        {
            get
            {
                return orgName;
            }
            set
            {
                orgName = value;
                OnPropertyChanged("OrganizationName");
            }
        }
        private string projName;
        public string ProjectName
        {
            get
            {
                return projName;
            }
            set
            {
                projName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        public CustomerDetails()
        {
            FirstName = LastName = Email = Phone = OrganizationName = ProjectName = "";
        }

        [Newtonsoft.Json.JsonIgnore]
        public bool IsInvalid
        {
            get
            {
                bool isInValid = String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(Phone) || String.IsNullOrEmpty(OrganizationName) || String.IsNullOrEmpty(ProjectName);
                return isInValid;
            }
        }
    }
}