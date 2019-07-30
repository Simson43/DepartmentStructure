using System;

namespace DepartmentStructure.Extention
{
    public static class DateTimeExtension
    {
        public static int GetAge(this DateTime birthDay)
        {
            var today = DateTime.Today;
            var t = today.Year - birthDay.Year;
            return t - (birthDay > today.AddYears(-t) ? 1 : 0);
        }
    }
}
