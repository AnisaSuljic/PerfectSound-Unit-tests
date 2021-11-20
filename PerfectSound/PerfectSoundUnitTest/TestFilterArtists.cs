using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.Person;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestFilterArtists
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public TestFilterArtists()
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
        [InlineData("Selena", "Gomez", 1)]
        [InlineData("Selena", "Gomez", 0)]
        [InlineData("Selena", "", 0)]
        [InlineData("", "Gomez", 0)]
        [InlineData("Selena", "", 1)]
        [InlineData("", "Gomez", 1)]
        [InlineData("", "", 1)]

        public void FilterArtists_ShouldWork(string firstname, string lastname, int genderid)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistsFilterContext1")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.People.Count() == 0)
                {
                    _context.People.Add(new PerfectSound.Database.Person
                    {
                        PersonId = 1,
                        FirstName = "Selena",
                        LastName = "Gomez",
                        Photo = new byte[1],
                        Biography = "biografija",
                        DateOfBirth = new DateTime(2021, 11, 01, 1, 1, 1),
                        PlaceOfBirth = "Mostar",
                        DateOfDeath = new DateTime(2021, 11, 01, 1, 1, 1),
                        GenderId = 1
                    });

                    _context.SaveChanges();
                }
            }

            //arange
            PersonSearchRequest NewNew = new PersonSearchRequest
            {
                FirstName=firstname,
                LastName=lastname,
                GenderId=genderid
            };

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _perService = new PersonService(_context, _mapper);
                //assert & act
                var list = _perService.Get(NewNew);
                Assert.True(list.Count > 0);
            }
        }

        [Theory]
        [InlineData("Selena123", "Gomez123", 1)]
        [InlineData("Selena123", "Gomez123", 0)]
        [InlineData("Selena123", "", 1)]
        [InlineData("Selena123", "", 0)]
        [InlineData("", "Gomez123", 1)]
        [InlineData("", "Gomez123", 0)]

        public void FilterArtists_ShouldFail(string firstname, string lastname, int genderid)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsFilterContext2")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.People.Count() == 0)
                {
                    _context.People.Add(new PerfectSound.Database.Person
                    {
                        PersonId = 1,
                        FirstName = "Selena",
                        LastName = "Gomez",
                        Photo = new byte[1],
                        Biography = "biografija",
                        DateOfBirth = new DateTime(2021, 11, 01, 1, 1, 1),
                        PlaceOfBirth = "Mostar",
                        DateOfDeath = new DateTime(2021, 11, 01, 1, 1, 1),
                        GenderId = 1
                    });

                    _context.SaveChanges();
                }
            }

            //arange
            PersonSearchRequest NewNew = new PersonSearchRequest
            {
                FirstName = firstname,
                LastName = lastname,
                GenderId = genderid
            };

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _perService = new PersonService(_context, _mapper);
                //assert & act
                var list = _perService.Get(NewNew);
                Assert.True(list.Count == 0);
            }
        }
    }
}
