using UnityEngine;
using Unitilities.Serialization;
using System.IO;

public class Test : MonoBehaviour
{
    public struct MyStruct
    {
        public string Name;
        public string Value;
    }

    private void Awake()
    {

    }

    [ContextMenu("Quick Test")]
    public void QuickTest()
    {
        MyStruct obj = new MyStruct { Name = "name", Value = "value" };
        const string fileName = "Test object";
        PersistentDataManager.SaveObjectAsText(obj, fileName, DataScope.Save);
        var path = PersistentDataManager.GetFullFilePath(fileName, DataScope.Save);

        obj = PersistentDataManager.LoadObjectFromText<MyStruct>(fileName, DataScope.Save);
        Debug.Log($"Directory.Exist: {Directory.Exists(path)} Name: {obj.Name}, Value: {obj.Value}");
    }
}
