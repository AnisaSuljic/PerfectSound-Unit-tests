using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.SongAndPodcast;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestFilterSongAndPodcast
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public TestFilterSongAndPodcast()
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
        [InlineData("Perfect", 1, true, "2021-11-10")]
        [InlineData("Perfect", 1, false, "2021-11-10")]
        [InlineData("Perfect", 0, true, "2021-11-10")]
        [InlineData("Perfect", 0, false, "2021-11-10")]
        [InlineData("", 1, true, "2021-11-10")]
        [InlineData("", 1, false, "2021-11-10")]
        [InlineData("", 0, true, "2021-11-10")]
        [InlineData("", 0, false, "2021-11-10")]
        public void FilterSongAndPodcasts_ShouldWork(string title, int genreid, bool podcast, DateTime release)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "SAPFilterContext1")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.SongAndPodcasts.Count() == 0)
                {
                    _context.SongAndPodcasts.Add(
                        new PerfectSound.Database.SongAndPodcast
                        {
                            SongAndPodcastId=1,
                            Title = "Perfect",
                            Text = "Perfect",
                            RunningTime = "00:00:00",
                            ReleaseDate = new DateTime(2021, 11, 10),
                            Budget = Convert.ToDecimal(2.5),
                            Poster = new byte[1],
                            ProductionCompanyId = 1,
                            IsPodcast = true,
                            SongAndPodcastGenres = { new SongAndPodcastGenre {SongAndPodcastGenreId=1,SongAndPodcastId=1,GenreId=1 }}
                        }
                    );
                    _context.SongAndPodcasts.Add(
                            new PerfectSound.Database.SongAndPodcast
                            {
                                SongAndPodcastId=2,
                                Title = "Perfect",
                                Text = "Perfect",
                                RunningTime = "00:00:00",
                                ReleaseDate = new DateTime(2021, 11, 10),
                                Budget = Convert.ToDecimal(2.5),
                                Poster = new byte[1],
                                ProductionCompanyId = 1,
                                IsPodcast = false,
                                SongAndPodcastGenres = { new SongAndPodcastGenre { SongAndPodcastGenreId = 2, SongAndPodcastId = 2, GenreId = 1 } }

                            });

                    _context.SaveChanges();
                }
            }

            //arange
            SongAndPodcastSearchRequest userreq = new SongAndPodcastSearchRequest
            {
                Title = title,
                GenreId = genreid,
                IsPodcast = podcast,
                ReleaseDate = release
            };

            using (_context = new PerfectSoundContext(options))
            {
                SongAndPodcastService _perService = new SongAndPodcastService(_context, _mapper);
                //assert & act
                var list = _perService.Get(userreq);
                Assert.True(list.Count > 0);
            }
        }
        
        [Theory]
        [InlineData("Perfect123", 0, true, "2021-11-16")]
        [InlineData("Perfect123", 0, false, "2021-11-15")]
        [InlineData("Perfect123", 1, false, "2021-11-17")]
        [InlineData("Perfect123", 1, true, "2021-11-17")]
        public void FilterSongAndPodcasts_ShouldFail(string title, int genreid, bool podcast, DateTime release)
        {
            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "SAPFilterContext2")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (_context = new PerfectSoundContext(options))
            {
                if (_context.SongAndPodcasts.Count() == 0)
                {
                    _context.SongAndPodcasts.Add(
                        new PerfectSound.Database.SongAndPodcast
                        {
                            SongAndPodcastId = 1,
                            Title = "Perfect",
                            Text = "Perfect",
                            RunningTime = "00:00:00",
                            ReleaseDate = new DateTime(2021, 11, 10),
                            Budget = Convert.ToDecimal(2.5),
                            Poster = new byte[1],
                            ProductionCompanyId = 1,
                            IsPodcast = true,
                            SongAndPodcastGenres = { new SongAndPodcastGenre { SongAndPodcastGenreId = 1, SongAndPodcastId = 1, GenreId = 1 } }
                        }
                    );
                    _context.SongAndPodcasts.Add(
                            new PerfectSound.Database.SongAndPodcast
                            {
                                SongAndPodcastId = 2,
                                Title = "Perfect",
                                Text = "Perfect",
                                RunningTime = "00:00:00",
                                ReleaseDate = new DateTime(2021, 11, 10),
                                Budget = Convert.ToDecimal(2.5),
                                Poster = new byte[1],
                                ProductionCompanyId = 1,
                                IsPodcast = false,
                                SongAndPodcastGenres = { new SongAndPodcastGenre { SongAndPodcastGenreId = 2, SongAndPodcastId = 2, GenreId = 1 } }

                            });

                    _context.SaveChanges();
                }
            }

            //arange
            SongAndPodcastSearchRequest userreq = new SongAndPodcastSearchRequest
            {
                Title = title,
                GenreId = genreid,
                IsPodcast = podcast,
                ReleaseDate = release
            };

            using (_context = new PerfectSoundContext(options))
            {
                SongAndPodcastService _perService = new SongAndPodcastService(_context, _mapper);
                //assert & act
                var list = _perService.Get(userreq);
                Assert.True(list.Count == 0);
            }
        }
    }
}
