using System;

namespace DepartmentStructure
{
    public class DepartmentDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid? ParentDepartmentID { get; set; }
    }
}