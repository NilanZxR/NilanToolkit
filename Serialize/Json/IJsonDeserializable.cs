using LitJson;

namespace NilanToolkit.Serialize.Json {
    public interface IJsonDeserializable : IJsonFieldNameProvider{
        void FromJson(JsonData json);
    }
}