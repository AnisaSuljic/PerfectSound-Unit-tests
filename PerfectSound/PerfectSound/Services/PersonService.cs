using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PerfectSound.Model.Requests.Person;
using PerfectSound.Model.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace PerfectSound.Services
{
    public class PersonService : BaseCRUDService<Person, PersonSearchRequest, PersonUpsertRequest, PersonUpsertRequest, Database.Person>
    {
        public PersonService(Database.PerfectSoundContext context, IMapper mapper) : base(context, mapper)
        {
        }
        public override List<Person> Get(PersonSearchRequest search)
        {
           var _searchSet = _context.People.Include(x=>x.Gender).AsQueryable();

            if(!string.IsNullOrWhiteSpace(search.FirstName))
            {
                _searchSet = _searchSet.Where(x => x.FirstName.ToLower().StartsWith(search.FirstName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(search.LastName))
            {
                _searchSet = _searchSet.Where(x => x.LastName.ToLower().StartsWith(search.LastName.ToLower()));
            }
            if(search?.GenderId != null && search?.GenderId != 0)
            {
                _searchSet = _searchSet.Where(x => x.GenderId == search.GenderId);
            }
            return _mapper.Map<List<Person>>(_searchSet.ToList());

        }

        public override Person Insert(PersonUpsertRequest request)
        {
            var entity = _mapper.Map<Database.Person>(request);

            AddingPerson(entity);

            return _mapper.Map<Person>(entity);
        }

        private void AddingPerson( Database.Person entity)
        {
            if (string.IsNullOrWhiteSpace(entity.FirstName))
            {
                throw new ArgumentException("Invalid parameter ");
            }
            else
            {
                if (!(entity.FirstName[0] >= 65 && entity.FirstName[0] <= 90))
                {
                    throw new ArgumentException("Invalid parameter ");

                }
            }
            if (string.IsNullOrWhiteSpace(entity.LastName))
            {
                throw new ArgumentException("Invalid parameter ");
            }
            else
            {
                if (!(entity.LastName[0] >= 65 && entity.LastName[0] <= 90))
                {
                    throw new ArgumentException("Invalid parameter ");

                }
            }
            if (string.IsNullOrWhiteSpace(entity.Biography))
            {
                throw new ArgumentException("Invalid parameter ");
            }
            if (entity.Photo.Length == 0)
            {
                throw new ArgumentException("Invalid parameter ");
            }
            if (string.IsNullOrWhiteSpace(entity.PlaceOfBirth))
            {
                throw new ArgumentException("Invalid parameter ");
            }
            if (entity.GenderId == 0)
            {
                throw new ArgumentException("Invalid parameter ");
            }
            else
            {
                foreach (var item in _context.People)
                {
                    if (item.FirstName == entity.FirstName && item.LastName ==entity.LastName)
                        throw new ArgumentException("Invalid parameter ");
                }
            }
            _context.Set<Database.Person>().Add(entity);
            _context.SaveChanges();

        }
    }
}
