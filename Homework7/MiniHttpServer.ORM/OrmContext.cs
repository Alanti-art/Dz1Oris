using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MiniHttpServer.ORM
{
    public class OrmContext : IOrmContext
    {
        private readonly string ConnectionString;
        public OrmContext(string connectionString)
        {
            ConnectionString = connectionString;
        }








        
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

     
     
        public async Task<int> DeleteAsync(string tableName, int id, CancellationToken cancellationToken = default)
        {


            string c = $"DELETE FROM {EscapeIdentifier(tableName)} WHERE Id = @id";
            await using SqlConnection connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = new SqlCommand(c, connection);

            command.Parameters.AddWithValue("@id", id);

            int result = await command.ExecuteNonQueryAsync(cancellationToken);
          


            return result;


        }

        public async Task<IReadOnlyList<T>> GetAllAsync<T>(string tableName, CancellationToken cancellationToken = default) where T : class, new()
        {
            var result = new List<T>();
            var properties = typeof(T).GetProperties();

            string c = $"SELECT * FROM {EscapeIdentifier(tableName)}";
            await using SqlConnection connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = new SqlCommand(c, connection);
            await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    T item = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (await reader.IsDBNullAsync(i, cancellationToken))
                            continue;

                        PropertyInfo p = null;

                        foreach (PropertyInfo prop in properties)
                        {
                            if (prop.Name.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase))
                            {
                                p = prop;
                            }

                        }
                        if (p != null)
                        {
                            p.SetValue(item, reader.GetValue(i));
                        }


                    }
                    result.Add(item);


                }
            }

            return result;




        }

        public async Task<int> InsertAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class
        {
            var properties = typeof(T).GetProperties();

            string into = "";
            string values = "";

            foreach (PropertyInfo p in properties)
            {
                if (p.Name != "Id")
                {
                    into += EscapeIdentifier(p.Name) + ",";
                    values += "@" + p.Name + ",";
                }


            }

            into = into.TrimEnd(',');
            values = values.TrimEnd(',');

            string c = $"INSERT INTO {EscapeIdentifier(tableName)} ({into}) VALUES ({values})";



            await using SqlConnection connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = new SqlCommand(c, connection);

            foreach (PropertyInfo p in properties)
            {
                if (p.Name != "Id")
                {
                    object value = p.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.AddWithValue("@" + p.Name, value);
                }


            }


            int result = await command.ExecuteNonQueryAsync(cancellationToken);
           


            return result;



        }

        public async Task<int> UpdateAsync<T>(string tableName, int id, T entity, CancellationToken cancellationToken = default) where T : class
        {


            var properties = typeof(T).GetProperties();

            string into = "";
            string values = "";

            string replaceCom = "";

            foreach (PropertyInfo p in properties)
            {
                if (p.Name != "Id")
                {
                    replaceCom += EscapeIdentifier(p.Name) + "=" + "@" + p.Name + ",";
                }

            }
            replaceCom = replaceCom.TrimEnd(',');

            string c = $"UPDATE {EscapeIdentifier(tableName)} SET {replaceCom} WHERE Id = @id";

            await using SqlConnection connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using SqlCommand command = new SqlCommand(c, connection);

            foreach (PropertyInfo p in properties)
            {
                if (p.Name != "Id")
                {
                    object value = p.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.AddWithValue("@" + p.Name, value);
                }
            }

            command.Parameters.AddWithValue("@id", id);

            int result = await command.ExecuteNonQueryAsync(cancellationToken);
          


            return result;


        }



        private static string EscapeIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier) ||
                !Regex.IsMatch(identifier, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                throw new ArgumentException("Недопустимое имя SQL-идентификатора.");
            }

            return $"[{identifier}]";
        }

       

    }
}