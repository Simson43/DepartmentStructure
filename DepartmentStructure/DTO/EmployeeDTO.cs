using DepartmentStructure.Extension;
using DepartmentStructure.Extention;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DepartmentStructure
{
    public class EmployeeDTO : INotifyPropertyChanged
    {
        private DateTime dateOfBirth = new DateTime(1753, 1, 1);
        private string position;
        private string firstName;
        private string surName;
        private string patronymic;
        private string docSeries;
        private int age;
        private string docNumber;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Browsable(false)]
        public decimal ID { get; set; }

        [Browsable(false)]
        public Guid DepartmentID { get; set; }

        [DisplayName("Должность*")]
        public string Position
        {
            get => position;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    position = value.FirstToUpper();
                    NotifyPropertyChanged();
                }
            }
        }

        [DisplayName("Имя*")]
        public string FirstName
        {
            get => firstName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    firstName = value.FirstToUpper();
                    NotifyPropertyChanged();
                }
            }
        }

        [DisplayName("Фамилия*")]
        public string SurName
        {
            get => surName;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    surName = value.FirstToUpper();
                    NotifyPropertyChanged();
                }
            }
        }

        [DisplayName("Отчество")]
        public string Patronymic
        {
            get => patronymic;
            set
            {
                patronymic = value?.FirstToUpper() ?? value;
                NotifyPropertyChanged();
            }
        }

        [DisplayName("Возраст")]
        public int Age
        {
            get => age;
            private set
            {
                age = value;
                NotifyPropertyChanged();
            }
        }

        [DisplayName("Дата рождения*")]
        public string DateOfBirth
        {
            get => dateOfBirth.ToString("dd.MM.yyyy");
            set
            {
                if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    var res = parsedDate.GetAge();
                    if (res < 0 || res > 100)
                        return;
                    dateOfBirth = parsedDate;
                    Age = res;
                    NotifyPropertyChanged();
                }
            }
        }

        [DisplayName("Серия паспорта*")]
        public string DocSeries
        {
            get => docSeries;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    docSeries = value.ToUpper();
                    NotifyPropertyChanged();
                }
            }
        }

        [DisplayName("Номер папорта*")]
        public string DocNumber
        {
            get => docNumber;
            set
            {
                docNumber = value;
                NotifyPropertyChanged();
            }
        }

        [Browsable(false)]
        public bool IsNew { get; set; }

        [Browsable(false)]
        public bool IsValid(out string message)
        {
            message = null;
            if (string.IsNullOrEmpty(Position))
                message = "Должность не может быть пустой";
            else if (!Position.Take(1).All(x => char.IsLetter(x)))
                message = "Должность должна начинаться с прописной буквы";
            else if (string.IsNullOrEmpty(FirstName))
                message = "Имя не может быть пустым";
            else if (!FirstName.All(x => char.IsLetter(x)))
                message = "Имя может содержать только буквы";
            else if (string.IsNullOrEmpty(SurName))
                message = "Фамилия не может быть пустой";
            else if (!SurName.All(x => char.IsLetter(x)))
                message = "Фамилия может содержать только буквы";
            else if (!string.IsNullOrEmpty(Patronymic) && !Patronymic.All(x => char.IsLetter(x)))
                message = "Отчество может содержать только буквы";
            else if (string.IsNullOrEmpty(DocSeries))
                message = "Серия документа не может быть пустой";
            else if (DocSeries.Length != 4 || !DocSeries.All(x => char.IsLetterOrDigit(x)))
                message = "Серия документа должна содержать 4 символа (буквы или цифры)";
            else if (string.IsNullOrEmpty(DocNumber))
                message = "Номер документа не может быть пустым";
            else if (DocNumber.Length != 6 || !DocNumber.All(x => char.IsDigit(x)))
                message = "Номер документа должен содержать 6 цифр";
            else if (Age < 0 || DateTime.Today.Year - dateOfBirth.Year < 0)
                message = "Возраст не может быть отрицательным";
            else if (Age > 100 || DateTime.Today.Year - dateOfBirth.Year > 100)
                message = "Возраст не может превышать 100";
            else
                return true;
            return false;
        }
    }
}