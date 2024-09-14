using System.Text;
using SmartEnviMonitoring.API.Configurations;
using SmartEnviMonitoring.Common.Configurations;

namespace SmartEnviMonitoring.API.Data.Communication;
public class HttpResBuilder{
    public HttpResBuilder(){}

    public string PostResponse(string key, string value){
        return new HttpResMessage(key, value).ToString();
    }
    public string PostResponse(string key, CommandResult res) =>
    PostResponse(key, CommSetting.GetNameInMessage(res));

    public string PostResponseCurrentTime(DateTime time) =>
    PostResponse("time", time.ToString(CommonConfig.TimeFormat));
}