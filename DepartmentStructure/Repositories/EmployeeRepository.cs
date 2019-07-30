using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepartmentStructure
{
    public class EmployeeRepository
    {
        private readonly IMapper mapper;
        private decimal nextId;

        public EmployeeRepository(IMapper mapper)
        {
            this.mapper = mapper;
            using (Context db = new Context())
            {
                if (db.Empoyee.Count() > 0)
                    nextId = db.Empoyee.Max(x => x.ID);
            }
            nextId++;
        }

        public Empoyee Add(Empoyee employee)
        {
            using (Context db = new Context())
            {
                while (db.Empoyee.Any(x => x.ID == nextId)) ;
                    nextId++;
                employee.ID = nextId++;
                db.Empoyee.Add(employee);
                db.SaveChanges();
                return employee;
            }
        }

        public bool Update(Empoyee newEmployee)
        {
            using (Context db = new Context())
            {
                Empoyee employee = db.Empoyee.Find(newEmployee.ID);
                if (employee == null)
                    return false;
                mapper.Map(newEmployee, employee);
                db.SaveChanges();
                return true;
            }
        }

        public void Remove(decimal id)
        {
            using (Context db = new Context())
            {
                var employee = db.Empoyee.Find(id);
                if (employee == null)
                    return;
                db.Empoyee.Remove(employee);
                db.SaveChanges();
            }
        }

        public List<Empoyee> FindAll(Func<Empoyee,bool> predicate)
        {
            using (Context db = new Context())
                return db.Empoyee.Where(predicate).ToList();
        }
    }
}
