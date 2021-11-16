using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.News;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace PerfectSoundUnitTest
{
    public  class TestAddingNews
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public NewsService _newsService;

        public TestAddingNews()
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
            NewsUpsertRequest NewNews = new NewsUpsertRequest
            {
                CoverPhoto = new byte[1],
                Title = "Perfect",
                SubTitle = "Sound",
                Content = "kontent",
                PublicationDate = new DateTime(2021, 11, 01, 1, 1, 1)
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                NewsService _newsServ = new NewsService(_context, _mapper);
                // _UUSS.Insert(NewUser);/
                //assert & act
                var validNew = _newsServ.Insert(NewNews);
                Assert.NotNull(validNew);
            }

        }
        [Theory]
        [InlineData(new byte[] { }, "Perfect", "Sound", "kontent","2021-11-1" )]
        [InlineData(new byte[] { 0 }, "", "Sound", "kontent", "2021-11-1")]
        [InlineData(new byte[] { 0 }, "Perfect", "", "kontent", "2021-11-1")]

        public void AddNew_ShouldFail(byte[] slika, string title, string subtitle, string kontent, DateTime datum)
        {
            //arange
            NewsUpsertRequest NewNew = new NewsUpsertRequest
            {
                CoverPhoto = slika,
                Title = title,
                SubTitle = subtitle,
                Content = kontent,
                PublicationDate = datum
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                NewsService _perService = new NewsService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _perService.Insert(NewNew));
            }
        }
        [Theory]
        [InlineData(new byte[] { 1 }, "Perfect", "Sound", "kontent", "2021-11-1")]
        public void AddNews_ExistingFields_ShouldWork(byte[] slika, string title, string subtitle, string kontent, DateTime datum)
        {

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsListContext5")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.News.Count() == 0)
                {
                    _context.News.Add(new PerfectSound.Database.News
                    {
                        CoverPhoto = new byte[1],
                        Title = "Perfect",
                        SubTitle = "Sound",
                        Content = "kontent",
                        PublicationDate = new DateTime(2021, 11, 01, 1, 1, 1)
                    });

                    _context.SaveChanges();
                }
            }
            //arange
            NewsUpsertRequest NewNew = new NewsUpsertRequest
            {
                CoverPhoto = slika,
                Title = title,
                SubTitle = subtitle,
                Content = kontent,
                PublicationDate = datum
            };
            Thread.Sleep(1);

            using (_context = new PerfectSoundContext(options))
            {
                NewsService _UUSS = new NewsService(_context, _mapper);
                //assert & act
                var validUser = _UUSS.Insert(NewNew);
                Assert.NotNull(validUser);
            }
        }
    }
}

