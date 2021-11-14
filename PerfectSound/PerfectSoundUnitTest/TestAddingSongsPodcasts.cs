using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PerfectSound.Database;
using PerfectSound.Model.Requests.News;
using PerfectSound.Model.Requests.SongAndPodcast;
using PerfectSound.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PerfectSoundUnitTest
{
    public class TestAddingSongsPodcasts
    {
        public static PerfectSoundContext _context;
        private static IMapper _mapper;

        public SongAndPodcast _songAndPodcast;

        public TestAddingSongsPodcasts()
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
        public void AddSongsPodcasts_EmptyFields_ShouldWork()
        {
            //arange
            SongAndPodcastUpsertRequest NewSongPodcast = new SongAndPodcastUpsertRequest
            {
                Title = "Perfect",
                Text = "Perfect",
                RunningTime = "00:00",
                ReleaseDate = new DateTime(2021, 11, 01, 1, 1, 1),
                Budget = Convert.ToDecimal(2.5),
                Poster = new byte[1],
                ProductionCompanyId = 1,
                IsPodcast = true,
                GenreIDList = new List<int> { 1, 2, 3 }
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "SongPodcastListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                SongAndPodcastService _newSongPodc = new SongAndPodcastService(_context, _mapper);
                // _UUSS.Insert(NewUser);/
                //assert & act
                var validSongsPodcast = _newSongPodc.Insert(NewSongPodcast);
                Assert.NotNull(validSongsPodcast);
            }
        }
        [Theory]
        [InlineData("", "Sound", "00:00", "2021-11-1", 2.5, new byte[] { 0 }, 1, true)]
        [InlineData("Perfect", "", "00:00", "2021-11-1", 2.5, new byte[] { 0 }, 1, true)]
        [InlineData("Perfect", "Sound", "", "2021-11-1", 2.5, new byte[] { 0 }, 1, true)]
        public void AddNewSongsPodcast_ShouldFail(string title, string text, string time, DateTime datum, decimal budget,
            byte[] slika, int id, bool Is)
        {
            //arange
            SongAndPodcastUpsertRequest NewSongAndPodcast = new SongAndPodcastUpsertRequest
            {
                Title = title,
                Text = text,
                RunningTime = time,
                ReleaseDate = datum,
                Budget = budget,
                Poster = slika,
                ProductionCompanyId = id,
                IsPodcast = Is,
            };

            var options = new DbContextOptionsBuilder<PerfectSoundContext>()
            .UseInMemoryDatabase(databaseName: "SongsAndPodcastListContext")
            .Options;

            using (_context = new PerfectSoundContext(options))
            {
                SongAndPodcastService _songsPodc = new SongAndPodcastService(_context, _mapper);
                //assert & act
                Assert.Throws<ArgumentException>(() => _songsPodc.Insert(NewSongAndPodcast));
            }
        }
    }
}
