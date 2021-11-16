using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Users;
using PerfectSound.Database;
using PerfectSound.Helper;
using PerfectSound.Interfaces;
using PerfectSound.Model.Requests;
using PerfectSound.Model.Requests.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using User = PerfectSound.Database.User;

namespace PerfectSound.Services
{
    public class UserService : IUserService
    {
        public PerfectSoundContext _context { get; }
        public IMapper _mapper { get; }
        public UserService(PerfectSoundContext _context, IMapper _mapper)
        {
            this._context = _context;
            this._mapper = _mapper;
        }

        public List<Model.Model.User> Get(UserSearchRequest search)
        {
            var query = _context.Users.Include(x => x.UserType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search?.UserName))
            {
                query = query.Where(x => x.UserName.ToLower().StartsWith(search.UserName.ToLower()));
            }

            if (search?.UserTypeId != null && search?.UserTypeId != 0)
            {
                query = query.Where(x => x.UserTypeId == search.UserTypeId);
            }

            return _mapper.Map<List<Model.Model.User>>(query.ToList());
        }

        public Model.Model.User GetById(int Id)
        {
            var entity = _context.Users.Find(Id);

            if (entity == null)
            {
                return null;
            }

            return _mapper.Map<Model.Model.User>(entity);
        }

        public Model.Model.User Insert(UserUpsertRequest request)
        {
            var entity = _mapper.Map<Database.User>(request);            

            GenerateSaltAndHash(entity, request);

            AddUser(entity);

            _context.SaveChanges();

            return _mapper.Map<Model.Model.User>(entity);
        }

        public void AddUser(Database.User entity)
        {         
            if (string.IsNullOrWhiteSpace(entity.FirstName))
            {
                throw new ArgumentException("Invalid parameter ", "FirstName");
            }
            else
            {
                if (!(entity.FirstName[0]>= 65 && entity.FirstName[0] <= 90))
                {
                    throw new ArgumentException("Invalid parameter ");

                }
            }
            if (string.IsNullOrWhiteSpace(entity.LastName))
            {
                throw new ArgumentException("Invalid parameter ", "LastName");
            }
            else
            {
                if (!(entity.LastName[0] >= 65 && entity.LastName[0] <= 90))
                {
                    throw new ArgumentException("Invalid parameter ");

                }
            }
            if (string.IsNullOrWhiteSpace(entity.UserName))
            {
                throw new ArgumentException("Invalid parameter ", "UserName");
            }
            else
            {
                foreach (var item in _context.Users)
                {
                    if (item.UserName == entity.UserName)
                        throw new ArgumentException("Invalid parameter ");
                }
            }
            if (string.IsNullOrWhiteSpace(entity.Email))
            {
                throw new ArgumentException("Invalid parameter ", "Email");
            }
            else
            {
                foreach (var item in _context.Users)
                {
                    if (item.Email == entity.Email)
                        throw new ArgumentException("Invalid parameter ");
                }
                if (!Regex.IsMatch(entity.Email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"))
                {
                    throw new ArgumentException("Invalid parameter ");
                }
            }
            if (string.IsNullOrWhiteSpace(entity.Phone))
            {
                throw new ArgumentException("Invalid parameter ", "Phone");
            }
            if (entity.UserTypeId == 0)
            {
                throw new ArgumentException("Invalid parameter ", "UserTypeId");
            }
           
            _context.Users.Add(entity);
        }

        private void GenerateSaltAndHash(Database.User entity, UserUpsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Invalid parameter ", "Password");
            }
            else
            {
                if(!Regex.IsMatch(request.Password, "^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$"))
                {
                    throw new ArgumentException("Invalid parameter ");
                }
            }
            if (string.IsNullOrWhiteSpace(request.PasswordConfirm))
            {
                throw new ArgumentException("Invalid parameter ", "PasswordConfirm");
            }
            if (request.Password != request.PasswordConfirm)
            {
                throw new ArgumentException("Password and password confirm not matched ", "PasswordConfirm");
            }

            entity.PasswordSalt = PasswordHash.GenerateSalt();
            entity.PasswordHash = PasswordHash.GenerateHash(entity.PasswordSalt, request.Password);
        }

        public Model.Model.User Update(int Id, UserUpsertRequest request)
        {
            var entity = _context.Users.Find(Id);
            _context.Users.Attach(entity);
            _context.Users.Update(entity);

            if (request.Password != request.PasswordConfirm)
            {
                throw new Exception("Password and password confirm not matched");
            }

            if (request.Password != null)
            {
                entity.PasswordSalt = PasswordHash.GenerateSalt();
                entity.PasswordHash = PasswordHash.GenerateHash(entity.PasswordSalt, request.Password);
            }
            else
            {
                throw new Exception("Password is required field!");
            }
            _mapper.Map(request, entity);

            _context.SaveChanges();

            return _mapper.Map<Model.Model.User>(entity);
        }
        public Model.Model.User Delete(int Id)
        {
            var entity = _context.Users.Find(Id);
            if (entity == null)
                throw new ArgumentNullException();
            var x = entity;
            _context.Users.Remove(entity);
            _context.SaveChanges();
            return _mapper.Map<Model.Model.User>(x);
        }

        public async Task<Model.Model.User> Login(string username, string password)
        {
            var entity = await _context.Users.Include(u=>u.UserType).FirstOrDefaultAsync(x => x.UserName == username);

            if (entity == null)
                throw new ArgumentException("Invalid username or password. User does not exist");

            var hash = PasswordHash.GenerateHash(entity.PasswordSalt, password);

            if (hash != entity.PasswordHash)
                throw new ArgumentException("Invalid username or password.");

            return _mapper.Map<Model.Model.User>(entity);
        }
    }
}
