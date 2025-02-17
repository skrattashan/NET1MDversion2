﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
//using NET1MD;

namespace NET1MDversion2
{
    public class SchoolContext : DbContext
    {
        private string _connectionString; //...migrations migrations migrations migrations migrations migrations...
        public SchoolContext() 
        {
            _connectionString = "";
        }
        public SchoolContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        //public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string cs = ConfigurationManager.ConnectionStrings["SchoolCon"].ConnectionString;

            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
    
    
}
