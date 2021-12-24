using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PerfectSound.Controllers;
using PerfectSound.Database;
using PerfectSound.Helper;
using PerfectSound.Interfaces;
using PerfectSound.Model.Model;
using PerfectSound.Model.Requests.User;
using PerfectSound.Services;
using PerfectSound.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;
using User = PerfectSound.Model.Model.User;

namespace PerfectSoundUnitTest
{
    public class TestAddingNewUser
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;


        //public UserService _userService;

        public TestAddingNewUser()
        {

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new PerfectSound.Mapper.AutoMapper());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }


        [Theory]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1)]
        public void AddUser_EmptyFields_ShouldWork(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid)
        {
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext1")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                // _UUSS.Insert(NewUser);/
                //assert & act
                var validUser = _UUSS.Insert(NewUser);
                Assert.NotNull(validUser);
            }

        }
        
        [Theory]
        [InlineData("", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "FirstName")]
        [InlineData("Naida", "", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "LastName")]
        [InlineData("Naida", "Merzic", "", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "UserName")]
        [InlineData("Naida", "Merzic", "MerzicNaida", "", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Email")]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "", "Testing193!", "+91 (123) 456-7890", 1, "Password")]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "", "+91 (123) 456-7890", 1, "PasswordConfirm")]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "", 1, "Phone")]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 0, "UserTypeId")]
        public void AddUser_EmptyFields_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext2")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(param, () => _UUSS.Insert(NewUser));
            }

        }

        [Theory]
        [InlineData("Anisa", "Suljic", "AnisaAnisa", "anisasuljic@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "PasswordConfirm")]
        public void AddUser_MatchPasswords_ShouldWork(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext3")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewUser);
                if (validUser != null)
                {
                    Console.WriteLine("fghjkl");
                }
                Assert.NotNull(validUser);
            }
        }
        
        [Theory]
        [InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "Testing193!", "012", "+91 (123) 456-7890", 1, "PasswordConfirm")]
        public void AddUser_MatchPasswords_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext4")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(param, () => _UUSS.Insert(NewUser));
            }

        }

        [Theory]
        [InlineData("Anisa", "Suljic", "anisasuljic", "anisasuljic@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Username")]
        public void AddUser_ExistingFields_ShouldWork(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext5")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.Users.Count() == 0)
                {
                    var salt = PasswordHash.GenerateSalt();

                    _context.Users.Add(new PerfectSound.Database.User
                    {
                        UserId = 1,
                        FirstName = "Naida",
                        LastName = "Merzic",
                        Email = "MerzicNaida@gmail.com",
                        Phone = "123456",
                        UserName = "MerzicNaida",
                        UserTypeId = 1,
                        PasswordSalt = salt,
                        PasswordHash = PasswordHash.GenerateHash(salt, pass)
                    });

                    _context.SaveChanges();
                }
            }
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            Thread.Sleep(1);

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewUser);
                Assert.NotNull(validUser);
            }
        }
        
        [Theory]
        [InlineData("Naida", "Merzic", "MerzicNaida", "Merzic123@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Username")]
        [InlineData("Naida", "Merzic", "naidanaida", "MerzicNaida@gmail.com", "Testing193!", "012", "+91 (123) 456-7890", 1, "Email")]
        public void AddUser_ExistingFields_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext6")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.Users.Count() == 0)
                {
                    var salt = PasswordHash.GenerateSalt();

                    _context.Users.Add(new PerfectSound.Database.User
                    {
                        UserId = 1,
                        FirstName = "Naida",
                        LastName = "Merzic",
                        Email = "MerzicNaida@gmail.com",
                        Phone = "123456",
                        UserName = "MerzicNaida",
                        UserTypeId = 1,
                        PasswordSalt = salt,
                        PasswordHash = PasswordHash.GenerateHash(salt, pass)
                    });

                    _context.SaveChanges();
                }
            }

            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _UUSS.Insert(NewUser));
            }
        }
        
        
        [Theory]
        [InlineData("Anisa", "Suljic", "anisasuljic", "anisasuljic@gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Username")]
        public void AddUser_Format_ShouldWork(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext9")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.Users.Count() == 0)
                {
                    var salt = PasswordHash.GenerateSalt();

                    _context.Users.Add(new PerfectSound.Database.User
                    {
                        UserId = 1,
                        FirstName = "Naida",
                        LastName = "Merzic",
                        Email = "MerzicNaida@gmail.com",
                        Phone = "123456",
                        UserName = "MerzicNaida1",
                        UserTypeId = 1,
                        PasswordSalt = salt,
                        PasswordHash = PasswordHash.GenerateHash(salt, pass)
                    });

                    _context.SaveChanges();
                }
            }
            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            Thread.Sleep(1);

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewUser);
                Assert.NotNull(validUser);
            }
        }

        [Theory]
        [InlineData("Naida", "Merzic", "MerzicNaida", "Merzic123gmail.com", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Username")]
        [InlineData("Naida", "Merzic", "naidanaida", "MerzicNaida2@gmail", "Testing193!", "Testing193!", "+91 (123) 456-7890", 1, "Email")]
        [InlineData("Naida", "Merzic", "naidanaida", "MerzicNaida2@gmail.com", "123", "123", "+91 (123) 456-7890", 1, "Email")]
        [InlineData("Naida", "Merzic", "naidanaida", "MerzicNaida2@gmail.com", "Testing193!", "Testing193!", "123456", 1, "Email")]
        public void AddUser_Format_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext10")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.Users.Count() == 0)
                {
                    var salt = PasswordHash.GenerateSalt();

                    _context.Users.Add(new PerfectSound.Database.User
                    {
                        UserId = 1,
                        FirstName = "Naida",
                        LastName = "Merzic",
                        Email = "MerzicNaida@gmail.com",
                        Phone = "123456",
                        UserName = "MerzicNaida1",
                        UserTypeId = 1,
                        PasswordSalt = salt,
                        PasswordHash = PasswordHash.GenerateHash(salt, pass)
                    });

                    _context.SaveChanges();
                }
            }

            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _UUSS.Insert(NewUser));
            }
        }
        /*
        [Theory]
        [InlineData("Anisa", "Suljic", "anisasuljic", "anisasuljic@gmail.com", "Testing193!", "Testing193!", "123456", 1)]
        public void AddUser_CapitalLetter_ShouldWork(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext5")
            .Options;

            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };
            Thread.Sleep(1);

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewUser);
                Assert.NotNull(validUser);
            }
        }
        
        [Theory]
        [InlineData("Aaida", "Merzic", "MerzicNaida", "Merzic123@gmail.com", "Testing193!", "Testing193!", "123456", 1, "Username")]
        [InlineData("Maida", "Aerzic", "naidanaida", "MerzicNaida@gmail.com", "Testing193!", "Testing193!", "123456", 1, "LastName")]
        public void AddUser_CapitalLetter_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone, int typeid, string param)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext6")
            .Options;

            //arange
            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = pass,
                PasswordConfirm = passConf,
                Phone = phone,
                UserTypeId = typeid
            };

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _UUSS.Insert(NewUser));
            }
        }*/

    }
}