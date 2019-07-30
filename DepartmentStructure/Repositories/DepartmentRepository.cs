using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepartmentStructure
{
    public class DepartmentRepository
    {
        private IMapper mapper;

        public DepartmentRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Department Find(Guid id)
        {
            using (Context db = new Context())
                return db.Department.Find(id);
        }

        public List<Department> GetAll()
        {
            using (Context db = new Context())
                return db.Department.ToList();
        }

        public Department Add(Department department)
        {
            using (Context db = new Context())
            {
                Guid id = Guid.NewGuid();
                while (db.Department.Any(x => x.ID == id)) 
                    id = Guid.NewGuid();
                department.ID = id;
                db.Department.Add(department);
                db.SaveChanges();
                return department;
            }
        }

        public bool Update(Department updatedDepartment)
        {
            using (Context db = new Context())
            {
                var department = db.Department.Find(updatedDepartment.ID);
                if (department == null)
                    return false;
                mapper.Map(updatedDepartment, department);
                db.SaveChanges();
                return true;
            }
        }

        public void Remove(Guid id)
        {
            using (Context db = new Context())
            {
                var department = db.Department.Find(id);
                if (department == null)
                    return;
                RemoveAllNested(department, db);
                db.SaveChanges();
            }
        }

        private void RemoveAllNested(Department department, Context db)
        {
            foreach (var dep in db.Department.Where(x => x.ParentDepartmentID == department.ID))
                RemoveAllNested(dep, db);
            var employees = db.Empoyee.Where(x => x.DepartmentID == department.ID);
            db.Empoyee.RemoveRange(employees);
            db.Department.Remove(department);
        }
    }
}
