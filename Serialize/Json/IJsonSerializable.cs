using LitJson;

namespace NilanToolkit.Serialize.Json {
    public interface IJsonSerializable : IJsonFieldNameProvider {
        JsonData ToJson();
        
    }
}