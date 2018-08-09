using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AepApp.ViewModel;
using Xamarin.Forms;

namespace AepApp.ViewModels
{
    public class TestPersonViewModel : INotifyPropertyChanged
    {
        private DatabaseContext _context;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<Person> allPersons;
        private ObservableCollection<Plan> allPlans;
        public ObservableCollection<Person> AllPersons
        {
            get { return allPersons; }
            set
            {
                allPersons = value;
            }
        }
        public ObservableCollection<Plan> AllPlans
        {
            get { return allPlans; }
            set
            {
                allPlans = value;
            }
        }

        public Person currentPerson;
        public Plan currentPlan;
        public Person CurrentPerson
        {
            get { return currentPerson; }
            set
            {
                currentPerson = value;
                OnPropertyChanged();
            }
        }
        public Plan CurrentPlan
        {
            get { return currentPlan; }
            set
            {
                currentPlan = value;
                OnPropertyChanged();
            }
        }
        public ICommand EditPersonCommand { get; private set; }





        public TestPersonViewModel()
        {
            _context = new DatabaseContext();
            var personList = _context.Persons.ToList<Person>();
            AllPersons = new ObservableCollection<Person>(personList);
            EditPersonCommand = new Command(EditPerson);
        }


        public void SavePersons(List<Person> persons)
        {
            if (persons != null)
            {
                foreach (var p in persons)
                {
                    _context.Persons.Add(p);
                    AllPersons.Add(p);
                }
                _context.SaveChanges();
            }
        }

        async void EditPerson()
        {
            _context.Persons.Update(CurrentPerson);
            _context.SaveChanges();

            Person p = AllPersons.Where(a => a.PersonId == CurrentPerson.PersonId).FirstOrDefault();
            p.Name = CurrentPerson.Name;
            p.Age = CurrentPerson.Age;

            await Application.Current.MainPage.Navigation.PopAsync();
        }




        public class Person : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public int PersonId { get; set; }
            private string name;
            private string age;
            private string age2;

            public Person()
            {

            }

            public string Name
            {
                get { return name; }
                set { name = value; OnPropertyChanged(); }
            }

            public string Age
            {
                get { return age; }
                set { age = value; OnPropertyChanged(); }
            }

            public string Age2
            {
                get { return age2; }
                set { age2 = value; OnPropertyChanged(); }
            }

        }

        public class Plan : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public int PlanId { get; set; }
            private string planName;
            public string PlanName
            {
                get { return planName; }
                set { planName = value; OnPropertyChanged(); }
            }
        }
    }
}
