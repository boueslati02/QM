using Newtonsoft.Json;
using System;

namespace Ponant.Medical.WebServices.Converters
{
    /// <summary>
    /// Permet de convertir une valeur vide JSON en null du côté code
    /// </summary>
    public class EmptyToNullConverter : JsonConverter
    {
        private JsonSerializer cobj_JsonSerializer = new JsonSerializer();

        public override bool CanConvert(Type aobj_Type)
        {
            return aobj_Type == typeof(string);
        }

        public override object ReadJson(JsonReader aobj_JsonReader, Type aobj_Type, object aobj_Valeur, JsonSerializer aobj_JsonSerializer)
        {
            string value = cobj_JsonSerializer.Deserialize<string>(aobj_JsonReader);

            if (string.IsNullOrEmpty(value))
            {
                value = null;
            }

            return value;
        }

        public override void WriteJson(JsonWriter aobj_JsonWriter, object aobj_Valeur, JsonSerializer aobj_JsonSerializer)
        {
            cobj_JsonSerializer.Serialize(aobj_JsonWriter, aobj_Valeur);
        }
    }
}