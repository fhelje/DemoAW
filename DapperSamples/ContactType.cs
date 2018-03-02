namespace DapperSamples
{
    public class ContactType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long EmployeeNr { get; set; }
        public string Email { get; set; }
        public string Team { get; set; }

    }
    
    public class Employee2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long EmployeeNr { get; set; }
        public string Email { get; set; }
        public Team Team { get; set; }

    }

    public class Employee3
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long EmployeeNr { get; set; }
        public string Email { get; set; }
        public Team Team { get; set; }
        public ContactInfo[] Contacts { get; set; }
    }

    public class EmployeeSql
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long EmployeeNr { get; set; }
        public string Email { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
        
    public class ContactInfo
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public int EmployeeId { get; set; }
    }

}