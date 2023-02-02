using Task_management_system.Models;

namespace Task_management_system.Interfaces
{
    public interface IKeyValueService
    {
        void CreateKeyType(KeyType keyType);
        void CreateKeyValue(KeyValue keyValue, string KeyTypeIntCode);
        void DeleteKeyType(KeyType keyType);
        void DeleteKeyType(int IdKeyType);
        void DeleteKeyValue(KeyValue keyValue);
        void DeleteKeyValue(int IdKeyValue);
        List<KeyType> GetAllKeyTypes();
        List<KeyValue> GetAllKeyValues();
        List<KeyValue> GetAllKeyValuesByKeyType(string keyTypeIntCode);
        KeyType GetKeyTypeById(int IdKeyType);
        KeyType GetKeyTypeByKeyTypeIntCode(string KeyTypeIntCode);
        KeyValue GetKeyValueById(int IdKeyValue);
        KeyValue GetKeyValueByKeyValueIntCode(string KeyValueIntCode);
        void UpdateKeyType(KeyType keyType);
        void UpdateKeyValue(KeyValue keyValue);
    }
}