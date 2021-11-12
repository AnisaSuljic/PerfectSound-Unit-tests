using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using PerfectSound.Controllers;
using PerfectSound.Database;
using PerfectSound.Interfaces;
using PerfectSound.Model.Model;
using PerfectSound.Model.Requests.User;
using PerfectSound.Services;
using PerfectSound.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using User = PerfectSound.Model.Model.User;

namespace PerfectSoundUnitTest
{
    public class UnitTest1
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;


        public UserService _userService;

        public UnitTest1()
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

        [Fact]
        public void Test1()
        {         

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                _context.Users.Add(new PerfectSound.Database.User {
                    UserId = 1,
                    FirstName = "Anisa",
                    LastName = "Suljic",
                    Email = "anisa@gmail.com",
                    Phone = "123456",
                    UserName = "AnisaSuljic",
                    UserTypeId = 1
                });
               
                _context.SaveChanges();

            }
            using (_context = new PerfectSoundContext(options))
            {
                UserService _UUSS = new UserService(_context, _mapper);
                List<User> userssss = _UUSS.Get(null);

                Assert.Equal(1, userssss.Count);
            }
            

            //var result = _UserService.Insert<User>(NewUser);
            //var result = _usercont.Post(NewUser);
            //var result = _userService.Insert(NewUser);

            //assert

            //Assert.NotNull(result);

        }

        [Theory]
        [InlineData("Naida","", "MerzicNaida", "MerzicNaida@gmail.com", "123", "123", "123456",1,"LastName")]
        public void AddUser_ShouldFail(string firstName, string lastName, string userName,
            string email, string pass, string passConf, string phone,int typeid, string param)
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
                UserService _UUSS = new UserService(_context, _mapper);

            //assert & act
            Assert.Throws<ArgumentException>(param, () => _UUSS.Insert(NewUser));
        }
    }
}
