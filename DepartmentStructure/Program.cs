using AutoMapper;
using DepartmentStructure.Extention;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepartmentStructure
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var container = new StandardKernel();
            container.Bind<IMapper>().ToConstant(new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmployeeDTO, Empoyee>();
                cfg.CreateMap<Empoyee, Empoyee>()
                    .ForMember(dest => dest.Department, opt => opt.Ignore());
                cfg.CreateMap<Empoyee, EmployeeDTO>();

                cfg.CreateMap<DepartmentDTO, Department>();
                cfg.CreateMap<Department, DepartmentDTO>();
                cfg.CreateMap<Department, Department>()
                    .ForMember(dest => dest.NestedDepartments, opt => opt.Ignore())
                    .ForMember(dest => dest.ParentDepartment, opt => opt.Ignore())
                    .ForMember(dest => dest.Empoyee, opt => opt.Ignore());
            })));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.Get<DepartmentsForm>());
        }

    }
}
