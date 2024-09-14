
namespace SmartEnviMonitoring.API.Test;
class Program{
    static void Main(string[] args){
        NSwagGen.Gen("swagger.json", "ServiceClient", "SmartEnviMonitoring.UI");
        //new MqttTests().SimpleSubPubTest();
    }
}