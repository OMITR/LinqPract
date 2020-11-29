using dbPract.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace dbPract
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();
        static void Main(string[] args)
        {
            Task5();
        }

        static void Task1()
        {
            var employees = (from e in _context.Employees
                             where e.Salary > 48000
                             orderby e.Salary
                             select new
                             {
                                 FirstName = e.FirstName,
                                 LastName = e.LastName,
                                 MiddleName = e.MiddleName,
                                 JobTitle = e.JobTitle,
                                 Salary = e.Salary
                             })
                             .ToList();


            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task2()
        {
            var employees = (from e in _context.Employees
                            where e.LastName == "Brown"
                            select e).ToList();

            var addresses = new Addresses();
            addresses.AddressText = "8 Mile Rd.";
            addresses.TownId = 27;

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                e.Address = addresses;
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary} {e.Address}");
            }

            _context.SaveChanges();
            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task3()
        {
            var startDate = new DateTime(2002, 1, 1);
            var endDate = new DateTime(2005, 12, 31);

            var result = (from e in _context.Employees
                         join ep in _context.EmployeesProjects
                         on e.EmployeeId equals ep.EmployeeId
                         join p in _context.Projects
                         on ep.ProjectId equals p.ProjectId
                         where p.StartDate >= startDate && p.EndDate <= endDate
                         select new
                         {
                             EmployeeId = ep.EmployeeId,
                             ProjectId = ep.ProjectId,
                             FirstName = e.FirstName,
                             LastName = e.LastName,
                             MiddleName = e.MiddleName,
                             Manager = e.Manager,
                             ProjectName = p.Name,
                             ProjectStartDate = p.StartDate,
                             ProjectEndDate = p.EndDate
                         })
                         .Take(5)
                         .ToList();

            var sb = new StringBuilder();

            foreach (var r in result)
            {
                sb.AppendLine(
                    r.ProjectEndDate == null
                    ? $"{r.FirstName} {r.LastName} {r.Manager.FirstName} {r.Manager.LastName} {r.ProjectName} {r.ProjectStartDate} НЕ ЗАВЕРШЁН"
                    : $"{r.FirstName} {r.LastName} {r.Manager.FirstName} {r.Manager.LastName} {r.ProjectName} {r.ProjectStartDate} {r.ProjectEndDate}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task4()
        {
            var empId = Convert.ToInt32(Console.ReadLine());

            var employees = (from e in _context.Employees
                            where e.EmployeeId == empId
                            select new
                            {
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                MiddleName = e.MiddleName,
                                JobTitle = e.JobTitle
                            })
                            .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} - {e.JobTitle}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
            GetProjectsById(empId);
        }

        static void GetProjectsById(int empId)
        {
            var projId = (from ep in _context.EmployeesProjects
                where ep.EmployeeId == empId
                select new
                {
                    ProjectId = ep.ProjectId
                })
                .ToList();

            var sb = new StringBuilder();

            for (var i = 0; i < projId.Count; i++)
            {
                var projects = (from p in _context.Projects
                               where p.ProjectId == projId
                               .ElementAt(i)
                               .ProjectId
                               select new
                               {
                                   Name = p.Name
                               })
                               .ToList();

                foreach (var p in projects)
                {
                    sb.AppendLine($"{p.Name}");
                }
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task5()
        {
            var departments = (from d in _context.Departments
                               where d.Employees.Count() < 5
                               select new
                               {
                                   Name = d.Name
                               })
                               .ToList();

            var sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name}");
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task6()
        {
            var depName = Console.ReadLine();
            Decimal per = Convert.ToDecimal(Console.ReadLine());

            var department = from d in _context.Departments
                             where d.Name == depName
                             select new
                             {
                                Name = d.Name,
                                Employees = d.Employees
                             };

            var employees = (from e in _context.Employees
                             where e.Department.Name == depName
                             select e)
                            .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                Decimal updSalary = e.Salary;
                e.Salary = updSalary + updSalary * per / 100;

                sb.AppendLine($"{e.FirstName} {e.LastName} {e.Salary}");
            }

            _context.SaveChanges();
            Console.WriteLine(sb.ToString().TrimEnd());
        }

        static void Task8()
        {
            string townName = Console.ReadLine();

            Towns town = _context.Towns
                .FirstOrDefault(t => t.Name == townName);

            var addresses = (from a in _context.Addresses
                             where a.Town.Name == townName
                             select a)
                            .ToList();

            foreach (var a in addresses)
            {
                a.TownId = null;
            }

            _context.Remove(town);
            _context.SaveChanges();
        }
    }
}