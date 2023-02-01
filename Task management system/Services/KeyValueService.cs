using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_management_system.Areas.Identity;
using Task_management_system.Data;
using Task_management_system.Interfaces;
using Task_management_system.Models;

namespace KeyValue_management_system.Services
{
    public class KeyValueService : Controller, IKeyValueService
    {
        private readonly Context _context;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public KeyValueService(Context context, UserManager<ApplicationUser> userManager,
                IUserStore<ApplicationUser> userStore,
                SignInManager<ApplicationUser> signInManager,
                IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public void CreateKeyValue(KeyValue keyValue, string KeyTypeIntCode)
        {
            KeyType keyType = _context.KeyType.FirstOrDefault((c) => (c.KeyTypeIntCode == KeyTypeIntCode));
            KeyValue keyValue1 = _context.KeyValue.FirstOrDefault((c) => (c.KeyValueIntCode == keyValue.KeyValueIntCode));
            if (keyValue1 == null && keyType != null)
            {
                keyValue.KeyType = keyType;
                _context.KeyValue.Add(keyValue);
                _context.SaveChanges();
            }
        }

        public void CreateKeyType(KeyType keyType)
        {
            KeyType keyType1 = _context.KeyType.FirstOrDefault((c) => (c.KeyTypeIntCode == keyType.KeyTypeIntCode));

            if (keyType == null)
            {
                _context.KeyType.Add(keyType);
                _context.SaveChanges();
            }
        }

        public void DeleteKeyValue(KeyValue keyValue)
        {
            _context.KeyValue.Remove(keyValue);
            _context.SaveChanges();
        }

        public void DeleteKeyValue(int IdKeyValue)
        {
            _context.KeyValue.Remove(new KeyValue { IdKeyValue = IdKeyValue });
            _context.SaveChanges();
        }

        public List<KeyValue> GetAllKeyValues()
        {
            return _context.KeyValue.ToList();
        }

        public List<KeyValue> GetAllKeyValuesByKeyType(string keyTypeIntCode)
        {
            KeyType keyType = GetKeyTypeByKeyTypeIntCode(keyTypeIntCode);

            return _context.KeyValue.Where(x => x.KeyType == keyType).ToList();
        }

        public KeyValue GetKeyValueById(int IdKeyValue)
        {
            return _context.KeyValue.Where(x => x.IdKeyValue == IdKeyValue).FirstOrDefault();
        }

        public KeyValue GetKeyValueByKeyValueIntCode(string KeyValueIntCode)
        {
            return _context.KeyValue.Where(x => x.KeyValueIntCode == KeyValueIntCode).FirstOrDefault();
        }

        public void UpdateKeyValue(KeyValue keyValue)
        {
            KeyValue? local = _context.Set<KeyValue>().Local.FirstOrDefault(entry => entry.IdKeyValue.Equals(keyValue.IdKeyValue));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(keyValue).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteKeyType(KeyType keyType)
        {
            _context.KeyType.Remove(keyType);
            _context.SaveChanges();
        }

        public void DeleteKeyType(int IdKeyType)
        {
            _context.KeyType.Remove(new KeyType { IdKeyType = IdKeyType });
            _context.SaveChanges();
        }

        public List<KeyType> GetAllKeyTypes()
        {
            return _context.KeyType.ToList();
        }

        public KeyType GetKeyTypeById(int IdKeyType)
        {
            return _context.KeyType.Where(x => x.IdKeyType == IdKeyType).FirstOrDefault();
        }

        public KeyType GetKeyTypeByKeyTypeIntCode(string KeyTypeIntCode)
        {
            return _context.KeyType.Where(x => x.KeyTypeIntCode == KeyTypeIntCode).FirstOrDefault();
        }

        public void UpdateKeyType(KeyType keyType)
        {
            KeyType? local = _context.Set<KeyType>().Local.FirstOrDefault(entry => entry.IdKeyType.Equals(keyType.IdKeyType));
            if (local != null)
            {
                // detach
                _context.Entry(local).State = EntityState.Detached;
            }
            _context.Entry(keyType).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}