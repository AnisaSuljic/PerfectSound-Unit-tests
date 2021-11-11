using AutoMapper;
using PerfectSound.Controllers;
using PerfectSound.Database;
using PerfectSound.Interfaces;
using PerfectSound.Model.Model;
using PerfectSound.Model.Requests.User;
using PerfectSound.Services;
using PerfectSound.WinForms;
using System;
using System.Text.RegularExpressions;
using Xunit;
using User = PerfectSound.Model.Model.User;

namespace PerfectSoundUnitTest
{
    public class UnitTest1
    {
        public static IUserService _service {get;}
        public static readonly PerfectSoundContext _context;
        private static readonly IMapper _mapper;

        public APIService _UserService = new APIService("User");
        public UserController _usercont = new UserController(_service);
        public UserService _userService = new UserService(_context, _mapper);

        [Fact]
        public void Test1()
        {
            //arrange

            UserUpsertRequest NewUser = new UserUpsertRequest
            {
                FirstName = "Naida",
                LastName = "Merzic",
                UserName = "MerzicNaida",
                Email = "MerzicNaida@gmail.com",
                Password = "123",
                PasswordConfirm = "123",
                Phone = "123456",
                UserTypeId=1
            };
            //act

            //var result = _UserService.Insert<User>(NewUser);
            var result = _usercont.Post(NewUser);
            //var result = _userService.Insert(NewUser);

            //assert

            Assert.NotNull(result);

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
            
            //assert & act
            Assert.ThrowsAsync<ArgumentException>(param, () => _UserService.Insert<User>(NewUser));
        }
    }
}
