using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        // подключение к БД
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\ВУЗ\4 семестр\Проектировние\labs\4\Rabotniki.mdf;Integrated Security=True;";

        // создание БД и таблицы, если они не существуют
        CreateDatabaseAndTable(connectionString);

        // добавление сотрудников в БД
        AddEmployeesToDatabase(connectionString);

        // извлечение данных о сотрудниках из БД и расчет ЗП
        CalculateAndDisplaySalaries(connectionString);
    }
    static void CreateDatabaseAndTable(string connectionString)
    {
        string createTableQuery = @"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Employees')
        BEGIN
            CREATE TABLE Employees (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(50) NOT NULL,
                HireDate DATETIME NOT NULL,
                Rate DECIMAL(10,2) NOT NULL,
                HoursWorked INT NOT NULL,
                Tips INT NOT NULL,
                Bonus INT NOT NULL,
                YearsWorked INT NOT NULL,
                Position NVARCHAR(50) NOT NULL
            );
        END
    ";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
    static void AddEmployeesToDatabase(string connectionString)
    {
        // Очищаем таблицу перед добавлением новых записей
        ClearEmployeesTable(connectionString);

        string[] employeeNames = { "Иван", "Евгения", "Алексей", "Елена", "Дмитрий", "Ольга" }; 
        decimal[] rates = { 10m, 12m, 8m, 9m, 20m, 18m };
        int[] hoursWorked = { 160, 180, 150, 170, 850, 1640 };
        int[] tips = { 0, 0, 200, 250, 0, 0 };
        int[] bonuses = { 0, 0, 0, 0, 500, 300 };
        int[] yearsWorked = { 0, 0, 0, 0, 6, 5 };
        string[] positions = { "KitchenStaff", "KitchenStaff", "Waiter", "Waiter", "Manager", "JuniorManager" };
        string insertQuery = @"
        INSERT INTO Employees (Name, HireDate, Rate, HoursWorked, Tips, Bonus, YearsWorked, Position)
        VALUES (@Name, GETDATE(), @Rate, @HoursWorked, @Tips, @Bonus, @YearsWorked, @Position);
    ";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            for (int i = 0; i < employeeNames.Length; i++)
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", employeeNames[i]);
                    command.Parameters.AddWithValue("@Rate", rates[i]);
                    command.Parameters.AddWithValue("@HoursWorked", hoursWorked[i]);
                    command.Parameters.AddWithValue("@Tips", tips[i]);
                    command.Parameters.AddWithValue("@Bonus", bonuses[i]);
                    command.Parameters.AddWithValue("@YearsWorked", yearsWorked[i]);
                    command.Parameters.AddWithValue("@Position", positions[i]);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
    static void ClearEmployeesTable(string connectionString)
    {
        string clearTableQuery = "DELETE FROM Employees";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(clearTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
    static void CalculateAndDisplaySalaries(string connectionString)
    {
        string sqlQuery = "SELECT * FROM Employees";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    decimal rate = (decimal)reader["Rate"];
                    int hoursWorked = (int)reader["HoursWorked"];
                    int tips = (int)reader["Tips"];
                    int bonus = (int)reader["Bonus"];
                    int yearsWorked = (int)reader["YearsWorked"];
                    string position = reader["Position"].ToString();
                    decimal salary = CalculateSalary(rate, hoursWorked, tips, bonus, yearsWorked, position);
                    Console.WriteLine($"Имя: {name}, Заработная плата: {salary}");
                }
            }
        }
    }
    static decimal CalculateSalary(decimal rate, int hoursWorked, int tips, int bonus, int yearsWorked, string position)
    {
        // расчет ЗП для разных должностей
        decimal totalSalary = 0;
        switch (position)
        {
            case "Waiter":
                totalSalary = rate * hoursWorked + tips;
                break;
            case "KitchenStaff":
                totalSalary = rate * hoursWorked;
                break;
            case "Manager":
            case "JuniorManager":
                totalSalary = rate + (bonus * yearsWorked);
                break;
            default:
                // если позиция неизвестна, просто вычисляем зарплату на основе часов работы и ставки
                totalSalary = rate * hoursWorked;
                break;
        }
        return totalSalary;
    }
}
