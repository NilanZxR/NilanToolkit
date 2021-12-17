using LitJson;

namespace NilanToolkit.Serialize.Json {
    public static class JsonSerializeExtensions {
        
        public static void AddObject(this JsonData source, IJsonSerializable obj) {
            source[obj.JsonFieldName] = obj.ToJson();
        }
        
        public static void FillJsonData(this JsonData source, IJsonDeserializable obj){
            obj.FromJson(source[obj.JsonFieldName]);
        }
        
    }
}