using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DepartmentStructure
{
    public class ViewModel
    {
        public ViewModel(IMapper mapper, EmployeeRepository employeeRepo, DepartmentRepository departmentRepo)
        {
            this.mapper = mapper;
            this.employeeRepo = employeeRepo;
            this.departmentRepo = departmentRepo;
            departments = departmentRepo.GetAll()
                .ToDictionary(key => key.Name, value => mapper.Map<DepartmentDTO>(value));
        }

        private readonly IMapper mapper;
        private readonly EmployeeRepository employeeRepo;
        private readonly DepartmentRepository departmentRepo;
        private Dictionary<string, DepartmentDTO> departments = new Dictionary<string, DepartmentDTO>();
        public BindingList<EmployeeDTO> ShowedEmployees { get; private set; } = new BindingList<EmployeeDTO>();

        public void FillTreeFromDB(TreeView treeView)
        {
            new DepTreeFiller(treeView).Fill(departments.Values.ToList());
        }
        
        public void ShowEmployeesFromTheDepartment(string departmentName)
        {
            ShowedEmployees.Clear();
            var employees = employeeRepo.FindAll(x => x.DepartmentID == departments[departmentName].ID)
                .Select(x => mapper.Map<EmployeeDTO>(x));
            foreach (var employee in employees)
                ShowedEmployees.Add(employee);
        }


        public void ReplaceEmployee(EmployeeDTO employeeDTO, string newDepartmentName)
        {
            employeeDTO.DepartmentID = departments[newDepartmentName].ID;
            employeeRepo.Update(mapper.Map<Empoyee>(employeeDTO));
        }

        public void UpdateEmployee(int index)
        {
            var employeeDTO = ShowedEmployees[index];
            if (employeeDTO.IsNew)
                employeeRepo.Add(mapper.Map<Empoyee>(employeeDTO));
            else if (!employeeRepo.Update(mapper.Map<Empoyee>(employeeDTO)))
                MessageBox.Show("Не удалось обновить сотрудника");
        }

        public void RemoveEmployee(int index)
        {
            employeeRepo.Remove(ShowedEmployees[index].ID);
            ShowedEmployees.RemoveAt(index);
        }

        public void CreateEmployee(string departmentName)
        {
            var employeeDTO = new EmployeeDTO()
            {
                DepartmentID = departments[departmentName].ID,
                IsNew = true
            };
            ShowedEmployees.Add(employeeDTO);
        }


        public DepartmentDTO AddDepartment(string parentName)
        {
            var departmentDTO = new DepartmentDTO()
            {
                ParentDepartmentID = parentName == null ? (Guid?)null : departments[parentName].ID,
                Name = GetNewDepartmentName()
            };
            departments[departmentDTO.Name] = departmentDTO;
            var department = departmentRepo.Add(mapper.Map<Department>(departmentDTO));
            departmentDTO.ID = department.ID;
            return departmentDTO;
        }
        
        public void UpdateDepartmentName(string oldName, string newName)
        {
            var department = departments[oldName];
            department.Name = newName;
            departmentRepo.Update(mapper.Map<Department>(department));
        }

        public void RemoveDepartment(string departmentName)
        {
            departmentRepo.Remove(departments[departmentName].ID);
            departments.Remove(departmentName);
            ShowedEmployees.Clear();
        }

        public void ReplaceDepartment(string departmentName, string newParentDepartmentName)
        {
            var department = departments[departmentName];
            var newParentDepartment = departments[newParentDepartmentName];
            department.ParentDepartmentID = newParentDepartment.ID;
            departmentRepo.Update(mapper.Map<Department>(department));
        }

        private string GetNewDepartmentName()
        {
            string name = "Новый отдел";
            int index = 1;
            while (departments.ContainsKey($"{name}-{index}"))
                index++;
            name = $"{name}-{index}";
            return name;
        }
    }
}
