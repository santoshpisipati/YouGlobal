using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sample.Web.ModalLogin.Classes
{
    public class ReCaptchaClass
    {
        //public static string Validate(string EncodedResponse)
        //{
        //    var client = new System.Net.WebClient();
        //    string PrivateKey = "6Le55gsTAAAAAC1e5EtYFXkkmW5uJH30aJcRysFI";
        //    var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
        //    var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);
        //    return captchaResponse.Success;
        //}

        //[JsonProperty("success")]
        //public string Success
        //{
        //    get { return m_Success; }
        //    set { m_Success = value; }
        //}

        //private string m_Success;

        //[JsonProperty("error-codes")]
        //public List<string> ErrorCodes
        //{
        //    get { return m_ErrorCodes; }
        //    set { m_ErrorCodes = value; }
        //}

        //private List<string> m_ErrorCodes;

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return m_ErrorCodes; }
            set { m_ErrorCodes = value; }
        }

        private List<string> m_ErrorCodes;
    }
}