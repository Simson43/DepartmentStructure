namespace DepartmentStructure.Extension
{
    public static class StringExtension
    {
        public static string FirstToUpper(this string value)
        {
            return value.Remove(0, 1).Insert(0, value[0].ToString().ToUpper());
        }
    }
}
