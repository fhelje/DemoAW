using System;
using System.Collections.Generic;

namespace DapperSamples
{
    public static class ConsoleWriter
    {
        public static void Print(this IEnumerable<ContactType> data, string header)
        {
            Decorate(header, () =>
            {
                foreach (var contactType in data)
                {
                    Console.WriteLine($"Id: {contactType.Id} - Name: {contactType.Name}");
                }
                
            });
        }

        public static void Print(this IEnumerable<Employee> data, string header)
        {
            Decorate(header, () =>
            {
                foreach (var emp in data)
                {
                    Console.WriteLine($"{emp.Id, -5} {emp.Name, -20} {emp.EmployeeNr, -5} {emp.Team, -15} {emp.Email, -50}");
                }
                
            });
        }
        public static void Print(this IEnumerable<Employee2> data, string header)
        {
            Decorate(header, () =>
            {
                foreach (var emp in data)
                {
                    Console.WriteLine($"{emp.Id, -5} {emp.Name, -20} {emp.EmployeeNr, -5} {emp.Team.Id, -3} {emp.Team.Name, -15} {emp.Email, -50}");
                }
                
            });
        }

        public static void Print(this IEnumerable<Employee3> data, string header)
        {
            Decorate(header, () =>
            {
                foreach (var emp in data)
                {
                    Console.WriteLine($"{emp.Id, -5} {emp.Name, -20} {emp.EmployeeNr, -5} {emp.Team.Id, -3} {emp.Team.Name, -15} {emp.Email, -50}");
                    foreach (var contact in emp.Contacts)
                    {
                        Console.WriteLine($"\t{contact.Id, -2} {contact.Type, -3} {contact.Value, -20}");
                    }
                }
                
            });
        }

        public static void Print(this Counts counts, string name)
        {
            Decorate(name, () =>
            {
                Console.WriteLine($"Employee count: {counts.EmployeeCount}");
                Console.WriteLine($"Contacts count: {counts.ContactInfoCount}");
            });
        }

        private static void Decorate(string header, Action rows)
        {
            Console.WriteLine("---------------------");
            Console.WriteLine(header);
            Console.WriteLine("=====================");
            rows();
            Console.WriteLine("---------------------");
            Console.WriteLine();
        }
    }
}