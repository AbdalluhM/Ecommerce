using AutoMapper;

using FluentAssertions.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyDexef.API;
using MyDexef.BLL.LookUps;
using MyDexef.Core.Entities;
using MyDexef.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Castle.Core.Logging;
using System.ComponentModel.Composition.Primitives;
using System.Data.Entity.Core.EntityClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data.Common;
using MyDexef.BLL.Responses;
using MyDexef.DTO.Files;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace MyDexef.TestBLL
{
    public class UnitTestBase
    {

        protected MyDexefContext context;
        protected IServiceProvider serviceProvider;

        public UnitTestBase( )
        {

            //DbContextOptions<MyDexefContext> dbContextOptions = new DbContextOptionsBuilder<MyDexefContext>()
            //    .UseInMemoryDatabase(databaseName: "MyDexefDB")
            //    .Options;
            //context = new ApplicationDbContext(dbContextOptions);
            var host = Program.CreateHostBuilder(new string [] { "Development" }).Build();
            serviceProvider = host.Services;

            //SeedDb();
        }

        [OneTimeSetUp]
        public void Setup( )
        {
            SeedDb();

        }
        private void SeedDb( )
        {
            #region Countries

            context.Add(new Country
            {
                Id = 1,
                Name = "{\"default\":\"Egypt\",\"ar\":\"مصر\"}",
                IsActive = true,
                IsDeleted = false
            });
            #endregion
            context.SaveChanges();
        }

        #region Helpers
        public FileDto GetFormFile(string filePath)
        {
            var fileDto = new FileDto();
            var FileBaseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string path = Path.Combine(FileBaseDirectory, "Files\\f0509b04-6dc0-4cd2-a30a-8ab40a3819c0.jpg");
            try
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                fileDto.File = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = getFileContentType(Path.GetExtension(stream.Name).Replace(".", ""))
                };
                fileDto.FileBaseDirectory = FileBaseDirectory;
                fileDto.FilePath = $"Files\\{filePath}";
                return fileDto;
            }
            catch (Exception e)
            {

                throw;
            }


        }

        public string getFileContentType(string format) => format switch
        {
            "pdf" => "application/pdf",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "rtf" => "",
            "mht" => "",
            "html" => "text/html",
            "txt" => "text/plain",
            "csv" => "text/plain",
            "png" => "image/png",
            "jpg" => "image/jpg",
        };
       

        #endregion
    }
}

