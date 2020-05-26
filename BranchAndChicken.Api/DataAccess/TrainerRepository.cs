using BranchAndChicken.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace BranchAndChicken.Api.DataAccess
{
    public class TrainerRepository
    {
        //static List<Trainer> _trainers = new List<Trainer>
        //{
        //    new Trainer
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Nathan",
        //        Specialty = Specialty.TaeCluckDo,
        //        YearsOfExperience = 0
        //    },
        //    new Trainer
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Martin",
        //        Specialty = Specialty.Chudo,
        //        YearsOfExperience = 12
        //    },
        //    new Trainer
        //    {                
        //        Id = Guid.NewGuid(),
        //        Name = "Adam",
        //        Specialty = Specialty.Chousting,
        //        YearsOfExperience = 3
        //    }

        string _connectionString = "Server = localhost; Database=BranchAndChicken; Trusted_Connection=True;";

        public Trainer Get(string name)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from Trainer
                            where Trainer.Name = @trainerName";

                // new ({ trainerName = name}) 
                // this creates a new class (anonymous type) with a property that matches the SQL parameter we created above 
                var parameters = new { trainerName = name };

                var trainer = db.QueryFirst<Trainer>(sql, parameters);

                return trainer;
            }
        }

        // PRE-DAPPER
        //public Trainer Get(string name)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = @"select * 
        //                            from Trainer
        //                            where Trainer.Name = @trainerName"; // never use string interpolation for SQL commands, for security reasons

        //        cmd.Parameters.AddWithValue("trainerName", name);  // uses a parameterized query

        //        var reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            return GetTrainerFromDataReader(reader);
        //        }
        //    }

        //    return null;  // returns null if there is no trainer
        //}

        public List<Trainer> GetAll()
        {
            // this using statement is essentially used so that the dispose statements at
            // the end disposes everything from this block
            //using (var connection = new SqlConnection(_connectionString))
            using (var db = new SqlConnection(_connectionString))
            {
                db.Open();

                // db.Query for results
                // db.Execute if you don't need results
                // make sure your SQL results match your class, or the results will be ignored
                var trainers = db.Query<Trainer>("Select Name,Id,YearsOfExperience,Specialty From Trainer");

                /* Pre-Dapper block
                 * 
                //var cmd = connection.CreateCommand();
                ////cmd.CommandText = "Select * From Trainer";
                //cmd.CommandText = @"Select *
                //                From Trainer";

                */

                //cmd.ExecuteNonQuery
                //  returns how many results were changed (used when we don't need to see the results data)

                //cmd.ExecuteScalar
                //  returns value of first cell of first row (used when trying to get the value of a calculation)

                //cmd.ExecuteReader
                //  returns the values (used when we care about the results of our query, so we can iterate through them)

                //var dataReader = cmd.ExecuteReader();

                //var trainers = new List<Trainer>();

                //// we use a while because it reads row by row until there are no rows with data, since 
                //// .Read() returns a bool to determine if there is a record on that row or not
                //while (dataReader.Read())
                //{
                //    // (int) is explicit casting, if int is incorrect it will immediately cause an exception
                //    var id = (int)dataReader["Id"];
                //    // "as string" is implicit casting, if the type is incorrect it'll return a null value
                //    var name = dataReader["Name"] as string;
                //    // Convert, converts your type unless they types can't be converted, like Guids
                //    var yearsOfExperience = Convert.ToInt32(dataReader["YearsOfExperience"]);
                //    // parse
                //    Enum.TryParse<Specialty>(dataReader["Specialty"].ToString(), out var specialty);

                //    var trainer = new Trainer
                //    {
                //        Specialty = specialty,
                //        Id = id,
                //        Name = name,
                //        YearsOfExperience = yearsOfExperience
                //    };

                //    trainers.Add(trainer);

                //}

                //// i believe these aren't actually needed in this situation since we built this block of code within a using statement
                //connection.Dispose(); // very important to dispose of
                //dataReader.Dispose();
                //cmd.Dispose();

                //return trainers;

                // called RESULT MULTI-MAPPING
                foreach (var trainer in trainers)
                {
                    var chickenRepository = new ChickenRepository();
                    var chickens = chickenRepository.GetChickensForTrainer(trainer.Id);

                    trainer.Coop.AddRange(chickens);
                }

                // could also use .AsList() which doesn't loop through everything if it's already a list
                return trainers.ToList();
            }

        }

        public bool Remove(string name)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"delete
                            from Trainer
                            where [name] = @name";

                return db.Execute(sql, new { name }) == 1;
            }
        }

        /// PRE-DAPPER
        //public bool Remove(string name)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = @"delete
        //                            from Trainer
        //                            where [name] = @name";

        //        cmd.Parameters.AddWithValue("name", name);

        //        return cmd.ExecuteNonQuery() == 1; // determine if we're deleting only one item
        //    }
        //}

        public ActionResult<Trainer> GetSpecialty(string specialty)
        {
            throw new NotImplementedException();
        }

        public Trainer Update(Trainer updatedTrainer, int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql  = @"UPDATE [dbo].[Trainer]
                                SET [Name] = @name
                                    ,[YearsOfExperience] = @yearsOfExperience
                                    ,[Specialty] = @specialty
                            output inserted.*
                                WHERE id = @id";

                //var parameters = new
                //{
                //    Id = id,
                //    Name = updatedTrainer.Name,
                //    YearsOfExperience = updatedTrainer.YearsOfExperience,
                //    Specialty = updatedTrainer.Specialty
                //};

                //var trainer = db.QueryFirst<Trainer>(sql, parameters);

                updatedTrainer.Id = id;

                var trainer = db.QueryFirst<Trainer>(sql, updatedTrainer);

                return trainer;
        }
        }

        /// PRE-DAPPER
        //public Trainer Update(Trainer updatedTrainer, int id)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var cmd = connection.CreateCommand();
        //        // In SqlServer:
        //        // Right click the Table and select Script Table As:
        //        // UPDATE To
        //        // New Query Editor Window
        //        // Then in the new query window type "Ctrl + Shift + m" and input our values
        //        cmd.CommandText = @"
        //                UPDATE [dbo].[Trainer]
        //                   SET [Name] = @name
        //                      ,[YearsOfExperience] = @yearsOfExperience
        //                      ,[Specialty] = @specialty
        //                output inserted.*
        //                  WHERE id = @id";

        //        cmd.Parameters.AddWithValue("name", updatedTrainer.Name);
        //        cmd.Parameters.AddWithValue("yearsOfExperience", updatedTrainer.YearsOfExperience);
        //        cmd.Parameters.AddWithValue("specialty", updatedTrainer.Specialty);
        //        cmd.Parameters.AddWithValue("id", id);

        //        var reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            return GetTrainerFromDataReader(reader);
        //        }

        //        return null;
        //    }

        //    // previous value, prior to connecting to SqlServer:
        //    // var trainerToUpdate = _trainers.First(trainer => trainer.Id == id);
        //    // trainerToUpdate.Name = updatedTrainer.Name;
        //    // trainerToUpdate.YearsOfExperience = updatedTrainer.YearsOfExperience;
        //    // trainerToUpdate.Specialty = updatedTrainer.Specialty;

        //}

        public Trainer Add(Trainer newTrainer)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO [dbo].[Trainer]
                                               ([Name]
                                               ,[YearsOfExperience]
                                               ,[Specialty])
                                         output inserted.*
                                         VALUES
                                               (@name
                                               ,@yearsOfExperience
                                               ,@specialty)";

                return db.QueryFirst<Trainer>(sql, newTrainer);
            }
        }

        /// PRE-DAPPER
        //public Trainer Add(Trainer newTrainer)
        //{
        //    // previous values, prior to connecting to SqlServer:
        //    //_trainers.Add(newTrainer);
        //    //return newTrainer;
        //    //return new Trainer();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        var cmd = connection.CreateCommand();
        //        // In SqlServer:
        //        // Right click the Table and select Script Table As:
        //        // INSERT To
        //        // New Query Editor Window
        //        // Then in the new query window type "Ctrl + Shift + m" and input our values
        //        cmd.CommandText = @"
        //                            INSERT INTO [dbo].[Trainer]
        //                                       ([Name]
        //                                       ,[YearsOfExperience]
        //                                       ,[Specialty])
        //                                 output inserted.*
        //                                 VALUES
        //                                       (@name
        //                                       ,@yearsOfExperience
        //                                       ,@specialty)";

        //        cmd.Parameters.AddWithValue("name", newTrainer.Name);
        //        cmd.Parameters.AddWithValue("yearsOfExperience", newTrainer.YearsOfExperience);
        //        cmd.Parameters.AddWithValue("specialty", newTrainer.Specialty);

        //        var reader = cmd.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            return GetTrainerFromDataReader(reader);
        //        }
        //    }

        //    return newTrainer;
        //}

        //Trainer GetTrainerFromDataReader(SqlDataReader reader)
        //{
        //    var id = (int)reader["Id"];
        //    var returnedName = reader["Name"] as string;
        //    var yearsOfExperience = Convert.ToInt32(reader["YearsOfExperience"]);
        //    Enum.TryParse<Specialty>(reader["Specialty"].ToString(), out var specialty);

        //    var trainer = new Trainer
        //    {
        //        Specialty = specialty,
        //        Id = id,
        //        Name = returnedName,
        //        YearsOfExperience = yearsOfExperience
        //    };

        //    return trainer;
        //}
    };
}
