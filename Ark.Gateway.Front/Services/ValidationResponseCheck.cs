using ArkPortal.Gateway.DTO.ViewModels;
using Newtonsoft.Json;

namespace Ark.Gateway.Front.Services
{
    public class ValidationResponseCheck
    {
        public static ErrorCheck IsValidResponse(HttpResponseMessage response)
        {
            bool isError = false;
            string message = "";
            if (response.IsSuccessStatusCode)
            {
                ErrorCheck? content = JsonConvert.DeserializeObject<ErrorCheck>(response.Content.ReadAsStringAsync().Result);
                if (content != null && content.Error)
                {
                    message = content.Message;
                    isError = true;
                }
                else
                {
                    message = "Record has successfully been added.";
                }
            }
            return new ErrorCheck { Error = isError, Message = message };
        }
    }
}
