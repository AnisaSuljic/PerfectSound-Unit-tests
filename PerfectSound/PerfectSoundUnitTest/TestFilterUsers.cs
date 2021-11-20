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
    public class TestFilterUsers
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public TestFilterUsers()
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
        [InlineData("MerzicNaida", 1)]
        [InlineData("MerzicNaida", 0)]
        [InlineData("", 1)]

        public void FilterArtists_ShouldWork(string username, int typeid)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UsersFilterContext1")
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

            //arange
            UserSearchRequest userreq = new UserSearchRequest
            {
                UserName = username,
                UserTypeId = typeid
            };

            using (_context = new PerfectSoundContext(options))
            {
                UserService _perService = new UserService(_context, _mapper);
                //assert & act
                var list = _perService.Get(userreq);
                Assert.True(list.Count > 0);
            }
        }

        [Theory]
        [InlineData("MerzicNaida123", 1)]
        [InlineData("MerzicNaida123", 0)]

        public void FilterArtists_ShouldFail(string username, int typeid)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UsersFilterContext2")
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

            //arange
            UserSearchRequest userreq = new UserSearchRequest
            {
                UserName = username,
                UserTypeId = typeid
            };

            using (_context = new PerfectSoundContext(options))
            {
                UserService _perService = new UserService(_context, _mapper);
                //assert & act
                var list = _perService.Get(userreq);
                Assert.True(list.Count == 0);
            }
        }
    }
}
