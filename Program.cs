using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\ВУЗ\4 семестр\Проектировние\labs\4\Rabotniki.mdf;Integrated Security=True;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Выполнение запросов к базе данных
            string sqlQuery = "SELECT * FROM Employees";
            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Обработка результатов запроса
                    while (reader.Read())
                    {
                        // Чтение данных о сотруднике из результата запроса
                        string name = reader["Name"].ToString();
                        DateTime hireDate = (DateTime)reader["HireDate"];
                        decimal rate = (decimal)reader["Rate"];
                        int hoursWorked = (int)reader["HoursWorked"];
                        int tips = (int)reader["Tips"];
                        int bonus = (int)reader["Bonus"];
                        int yearsWorked = (int)reader["YearsWorked"];

                        // Вывод информации о сотруднике и расчет заработной платы
                        Console.WriteLine($"Имя: {name}, Дата трудоустройства: {hireDate}, Ставка: {rate}, Отработанные часы: {hoursWorked}, Чаевые: {tips}, Премия: {bonus}, Прошло лет работы: {yearsWorked}");
                        decimal salary = CalculateSalary(rate, hoursWorked, tips, bonus, yearsWorked);
                        Console.WriteLine($"Заработная плата: {salary}");
                    }
                    reader.Close();
                    static decimal CalculateSalary(decimal rate, int hoursWorked, int tips, int bonus, int yearsWorked)
                    {
                        // Расчет заработной платы по вашей логике
                        decimal salary = rate * hoursWorked + tips + bonus;
                        return salary;
                    }
                }
            }
        }
    }
}
