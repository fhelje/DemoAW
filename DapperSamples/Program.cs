using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Dapper;

namespace DapperSamples
{
    public class Program
    {
        private static string CS = "Server=localhost;Database=Demo;User Id=sa;Password=@Password123;";
        
        static void Main(string[] args)
        {
            var data = SimpleSelect().GetAwaiter().GetResult();
            data.Print("Simple");
            
            data = SimpleSelectWithParameter(1).GetAwaiter().GetResult();
            data.Print("Simple with parameter");

            var emps = OneToOneMapping().GetAwaiter().GetResult();
            emps.Print("One to one");
            
            emps = OneToOneFilterListMapping(new []{1, 2, 3}).GetAwaiter().GetResult();
            emps.Print("One to one with filter");
            
            // Inner object with one to one simple style
            var emp2 = InnerObjectVersion1(new[] {1, 2, 3, 4, 5, 6, 7}).GetAwaiter().GetResult();
            emp2.Print("Inner version 1");

            // Inner object with one to one dapper multi mapper
            emp2 = MultiMap(new[] {1, 2, 3, 4, 5, 6, 7}).GetAwaiter().GetResult();
            emp2.Print("Inner multi map");


            // Object with multiple child objects
            var emp3 = MultipleResultsets().GetAwaiter().GetResult();
            emp3.Print("Multiple resultsets");

            // Stored procedure
            var counts = StoredProc().GetAwaiter().GetResult();
            counts.Print("StoredProc");

        }


        private static async Task<IEnumerable<ContactType>> SimpleSelect()
        {
            using (var conn = new SqlConnection(CS))
            {
                return await conn.QueryAsync<ContactType>(
                    "SELECT * FROM ContactType"
                );
            }
        }

        private static async Task<IEnumerable<ContactType>> SimpleSelectWithParameter(int id)
        {
            using (var conn = new SqlConnection(CS))
            {
                return await conn.QueryAsync<ContactType>(
                    "SELECT * FROM ContactType WHERE Id = @id",
                    new { id }
                );
            }
        }

        private static async Task<IEnumerable<Employee>> OneToOneMapping()
        {
            using (var conn = new SqlConnection(CS))
            {
                return await conn.QueryAsync<Employee>(
                    @"SELECT 
                        e.Id,
                        e.Name,
                        e.EmployeeNumber as EmployeeNr,
                        e.Email,
                        t.Name as Team
                        FROM Employee e 
                            INNER JOIN Team t ON t.Id = e.fkTeamId"
                );
            }
        }

        private static async Task<IEnumerable<Employee>> OneToOneFilterListMapping(IEnumerable ids)
        {
            using (var conn = new SqlConnection(CS))
            {
                return await conn.QueryAsync<Employee>(
                    @"SELECT 
                        e.Id,
                        e.Name,
                        e.EmployeeNumber as EmployeeNr,
                        e.Email,
                        t.Name as Team
                        FROM Employee e 
                            INNER JOIN Team t ON t.Id = e.fkTeamId
                        WHERE e.Id in @ids",
                    new {ids});
            }
        }

        private static async Task<IEnumerable<Employee2>> InnerObjectVersion1(IEnumerable ids)
        {
            using (var conn = new SqlConnection(CS))
            {
                return (await conn.QueryAsync<EmployeeSql>(
                    @"SELECT 
                        e.Id,
                        e.Name,
                        e.EmployeeNumber as EmployeeNr,
                        e.Email,
                        t.Id as TeamId,
                        t.Name as TeamName
                        FROM Employee e 
                            INNER JOIN Team t ON t.Id = e.fkTeamId
                        WHERE e.Id in @ids",
                    new {ids}))
                .Select(x=> new Employee2
                {
                    Id = x.Id,
                    Email = x.Email,
                    Name = x.Name,
                    EmployeeNr = x.EmployeeNr,
                    Team = new Team
                    {
                        Id = x.TeamId,
                        Name = x.TeamName
                    }
                });
            }
        }
        
        private static async Task<IEnumerable<Employee2>> MultiMap(IEnumerable ids)
        {
            using (var conn = new SqlConnection(CS))
            {
                return await conn.QueryAsync<Employee2, Team, Employee2>(
                        @"SELECT 
                        e.Id,
                        e.Name,
                        e.EmployeeNumber as EmployeeNr,
                        e.Email,
                        t.Id,
                        t.Name
                        FROM Employee e 
                            INNER JOIN Team t ON t.Id = e.fkTeamId
                        WHERE e.Id in @ids",
                        (emp, team) => { 
                            emp.Team = team;
                            return emp;
                        }
                        ,
                        new {ids})
                    ;
            }
        }

        private static async Task<IEnumerable<Employee3>> MultipleResultsets()
        {
            using (var conn = new SqlConnection(CS))
            {
                using (var grid = await conn.QueryMultipleAsync(
                    @"SELECT 
                        e.Id,
                        e.Name,
                        e.EmployeeNumber as EmployeeNr,
                        e.Email,
                        t.Id as TeamId,
                        t.Name as TeamName
                        FROM Employee e 
                            INNER JOIN Team t ON t.Id = e.fkTeamId

                    SELECT *, fkEmployeeId as EmployeeId FROM ContactInfo"))
                {
                    var emps = (await grid.ReadAsync<EmployeeSql>()).ToList();
                    var contacts = (await grid.ReadAsync<ContactInfo>()).ToList();

                    return emps.Select(x => new Employee3
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name,
                        EmployeeNr = x.EmployeeNr,
                        Team = new Team
                        {
                            Id = x.TeamId,
                            Name = x.TeamName
                        },
                        Contacts = contacts.Where(y => y.EmployeeId == x.Id).ToArray()
                    });
                };
            }
        }

        private static async Task<Counts> StoredProc()
        {
            using (var conn = new SqlConnection(CS))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empCount", dbType:DbType.Int32, direction:ParameterDirection.Output);
                parameters.Add("@contactInfoCount", dbType:DbType.Int32, direction:ParameterDirection.Output);
                
                await conn.ExecuteAsync("sp_Counts", parameters, commandType: CommandType.StoredProcedure);
                
                return new Counts
                {
                    EmployeeCount = parameters.Get<int>("@empCount"),
                    ContactInfoCount =parameters.Get<int>("@contactInfoCount"), 
                };
            }
        }

    }

    public class Counts
    {
        public int EmployeeCount { get; set; }
        public int ContactInfoCount { get; set; }
    }
}