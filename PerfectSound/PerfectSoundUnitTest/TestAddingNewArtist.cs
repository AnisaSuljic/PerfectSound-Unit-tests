using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.Person;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestAddingNewArtist
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public PersonService _personService;

        public TestAddingNewArtist()
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
        public void AddArtist_EmptyFields_ShouldWork()
        {
            //arange
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = "Selena",
                LastName = "Gomez",
                Photo=new byte[1],
                Biography="biografija",
                DateOfBirth=new DateTime(2021,01,01,1,1,1),
                DateOfDeath=new DateTime(2021,11,01,1,1,1),
                PlaceOfBirth="LA",
                GenderId=1
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "UserListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _personserv = new PersonService(_context, _mapper);
                // _UUSS.Insert(NewUser);/
                //assert & act
                var validArtist = _personserv.Insert(NewArtist);
                Assert.NotNull(validArtist);
            }
           
        }
        [Theory]
        [InlineData("", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1","Mostar",2)]
        [InlineData("Selena", "", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]
        [InlineData("Selena", "Gomez", new byte[] { }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "", "2021-11-1", "2021-11-1", "Mostar", 2)]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "", 2)]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 0)]


        public void AddArtist_ShouldFail(string ime, string prezime, byte[]slika, string biografija, DateTime datum1, DateTime datum2,
            string mjesto, int id)
        {
            //arange
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = ime,
                LastName = prezime,
                Photo = slika,
                Biography = biografija,
                DateOfBirth = datum1,
                DateOfDeath = datum2,
                PlaceOfBirth = mjesto,
                GenderId = id
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _perService = new PersonService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _perService.Insert(NewArtist));
            }

        }
    }
}
