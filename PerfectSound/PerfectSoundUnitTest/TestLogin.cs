using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Helper;
using PerfectSound.Model.Requests.User;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestLogin
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;


        //public UserService _userService;

        public TestLogin()
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
        [InlineData("MerzicNaida", "Merzic")]
        public void Login_Should_Work(string username, string password)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "LoginContext1")
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
                        PasswordHash = PasswordHash.GenerateHash(salt, "Merzic")
                    });

                    _context.SaveChanges();
                }
            }

            using (_context = new PerfectSoundContext(options))
            {
                UserService _userv = new UserService(_context, _mapper);
                //assert & act
                var validUser = _userv.Login(username, password);
                Assert.NotNull(validUser);
            }

        }

        [Theory]
        [InlineData("MerzicNaida123", "Merzic")]
        [InlineData("MerzicNaida", "Merzic123")]
        public void Login_Should_Fail(string username, string password)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "LoginContext1")
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
                        PasswordHash = PasswordHash.GenerateHash(salt, "Merzic")
                    });

                    _context.SaveChanges();
                }
            }

            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                //assert & act
                Assert.ThrowsAsync<ArgumentException>(() => _UUSS.Login(username, password));
            }

        }
    }
}
