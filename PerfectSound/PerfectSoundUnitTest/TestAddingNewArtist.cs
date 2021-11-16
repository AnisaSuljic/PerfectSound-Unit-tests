using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.Person;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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


        public void AddArtist_EmptyFields_ShouldFail(string ime, string prezime, byte[]slika, string biografija, DateTime datum1, DateTime datum2,
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
        [Theory]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]

        //[InlineData("Naida", "Merzic", "MerzicNaida", "MerzicNaida@gmail.com", "123", "012", "123456", 1, "PasswordConfirm")]
        public void AddArtist_ExistingFields_ShouldWork(string ime, string prezime, byte[] slika, string biografija, DateTime datum1, DateTime datum2,
            string mjesto, int id)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistListContext5")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.People.Count() == 0)
                {

                    _context.People.Add(new PerfectSound.Database.Person
                    {
                        PersonId = 1,
                        FirstName = "Naida",
                        LastName = "Merzic",
                        Photo = new byte [1],
                        Biography = "biografija" ,
                        DateOfBirth = new DateTime(2021, 11, 01, 1, 1, 1),
                        PlaceOfBirth = "Mostar" ,
                        DateOfDeath = new DateTime(2021, 11, 01, 1, 1, 1),
                        GenderId = 1
                    });

                    _context.SaveChanges();
                }
            }
            //arange
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = ime,
                LastName = prezime,
                Photo = slika,
                Biography = biografija,
                DateOfBirth = datum1 ,
                PlaceOfBirth = mjesto,
                DateOfDeath = datum2,
                GenderId = id
            };
            Thread.Sleep(1);

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _UUSS = new PersonService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewArtist);
                Assert.NotNull(validUser);
            }
        }
        [Theory]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]

        public void AddArtist_ExistingFields_ShouldFail(string ime, string prezime, byte[] slika, string biografija, DateTime datum1, DateTime datum2,
            string mjesto, int id)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistListContext6")
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
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = ime,
                LastName = prezime,
                Photo = slika,
                Biography = biografija,
                DateOfBirth = datum1,
                PlaceOfBirth = mjesto,
                DateOfDeath = datum2,
                GenderId = id
            };

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _UUSS = new PersonService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _UUSS.Insert(NewArtist));
            }
        }

        [Theory]
        [InlineData("Selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]
        public void AddUser_CapitalLetter_ShouldWork(string ime, string prezime, byte[] slika, string biografija, DateTime datum1, DateTime datum2,
            string mjesto, int id)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistListContext7")
            .Options;

            //arange
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = ime,
                LastName = prezime,
                Photo = slika,
                Biography = biografija,
                DateOfBirth = datum1,
                PlaceOfBirth = mjesto,
                DateOfDeath = datum2,
                GenderId = id
            };

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _UUSS = new PersonService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewArtist);
                Assert.NotNull(validUser);
            }
        }
        [Theory]
        [InlineData("selena", "Gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]
        [InlineData("Selena", "gomez", new byte[] { 0 }, "biografija", "2021-11-1", "2021-11-1", "Mostar", 2)]
        public void AddUser_CapitalLetter_ShouldFail(string ime, string prezime, byte[] slika, string biografija, DateTime datum1, DateTime datum2,
            string mjesto, int id)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "ArtistListContext7")
            .Options;

            //arange
            PersonUpsertRequest NewArtist = new PersonUpsertRequest
            {
                FirstName = ime,
                LastName = prezime,
                Photo = slika,
                Biography = biografija,
                DateOfBirth = datum1,
                PlaceOfBirth = mjesto,
                DateOfDeath = datum2,
                GenderId = id
            };

            using (_context = new PerfectSoundContext(options))
            {
                PersonService _UUSS = new PersonService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _UUSS.Insert(NewArtist));
            }
        }
    }
}
