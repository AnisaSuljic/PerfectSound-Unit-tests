using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.News;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestFilterNews
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public TestFilterNews()
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
        [InlineData("Perfect","2021-11-16")]
        [InlineData("","2021-11-16")]

        public void FilterNews_ShouldWork(string title,DateTime datum)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsFilterContext1")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.News.Count() == 0)
                {
                    _context.News.Add(new PerfectSound.Database.News
                    {
                        NewsId = 1,
                        Title = "Perfect",
                        SubTitle = "Podnaslov",
                        Content = "Sadrzaj novosti",
                        PublicationDate = new DateTime(2021, 11, 16)
                    });

                    _context.SaveChanges();
                }
            }
            
            //arange
            NewsSearchRequest NewNew = new NewsSearchRequest
            {
                Title = title,
                PublicationDate = datum
            };

            using (_context = new PerfectSoundContext(options))
            {
                NewsService _perService = new NewsService(_context, _mapper);
                //assert & act
                var list = _perService.Get(NewNew);
                Assert.True(list.Count > 0);
            }
        }

        [Theory]
        [InlineData("Perfect123", "2021-11-16")]
        [InlineData("", "2021-11-10")]

        public void FilterNews_ShouldFail(string title, DateTime datum)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "NewsFilterContext2")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.News.Count() == 0)
                {
                    _context.News.Add(new PerfectSound.Database.News
                    {
                        NewsId = 1,
                        Title = "Perfect",
                        SubTitle = "Podnaslov",
                        Content = "Sadrzaj novosti",
                        PublicationDate = new DateTime(2021, 11, 16)
                    });

                    _context.SaveChanges();
                }
            }

            //arange
            NewsSearchRequest NewNew = new NewsSearchRequest
            {
                Title = title,
                PublicationDate = datum
            };

            using (_context = new PerfectSoundContext(options))
            {
                NewsService _perService = new NewsService(_context, _mapper);
                //assert & act
                var list = _perService.Get(NewNew);
                Assert.True(list.Count == 0);
            }
        }
    }
}
